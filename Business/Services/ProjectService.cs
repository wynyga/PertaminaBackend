using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Repositories;
using Models.DTOs;
using Models.Entities;

namespace Business.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IEncryptionService _encryptionService;

        public ProjectService(IProjectRepository projectRepository, IEncryptionService encryptionService)
        {
            _projectRepository = projectRepository;
            _encryptionService = encryptionService;
        }

        private ProjectResponseDTO MapToDTO(Project project)
        {
            return new ProjectResponseDTO
            {
                Id = _encryptionService.EncryptId(project.Id),
                ProjectName = project.ProjectName,
                Description = project.Description,
                Status = project.Status,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Progress = project.Progress
            };
        }

        public async Task<IEnumerable<ProjectResponseDTO>> GetAllProjectsAsync()
        {
            var projects = await _projectRepository.GetAllAsync();
            return projects.Select(MapToDTO);
        }

        public async Task<ProjectResponseDTO?> GetProjectByIdAsync(string encryptedId)
        {
            try
            {
                int id = _encryptionService.DecryptId(encryptedId);
                var project = await _projectRepository.GetByIdAsync(id);
                if (project == null) return null;

                return MapToDTO(project);
            }
            catch (ArgumentException)
            {
                return null; // Invalid encrypted ID
            }
        }

        public async Task<ProjectResponseDTO?> CreateProjectAsync(ProjectRequestDTO request)
        {
            // Check for duplicate name
            var existingProject = await _projectRepository.GetByNameAsync(request.ProjectName);
            if (existingProject != null)
            {
                throw new InvalidOperationException("Nama project sudah digunakan.");
            }

            var project = new Project
            {
                ProjectName = request.ProjectName,
                Description = request.Description,
                Status = request.Status,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Progress = request.Progress
            };

            var createdProject = await _projectRepository.AddAsync(project);
            return MapToDTO(createdProject);
        }

        public async Task<ProjectResponseDTO?> UpdateProjectAsync(string encryptedId, ProjectRequestDTO request)
        {
            try
            {
                int id = _encryptionService.DecryptId(encryptedId);
                var project = await _projectRepository.GetByIdAsync(id);
                if (project == null) return null;

                // Check for duplicate name if the name is changed
                if (project.ProjectName != request.ProjectName)
                {
                    var existingProject = await _projectRepository.GetByNameAsync(request.ProjectName);
                    if (existingProject != null)
                    {
                        throw new InvalidOperationException("Nama project sudah digunakan.");
                    }
                }

                project.ProjectName = request.ProjectName;
                project.Description = request.Description;
                project.Status = request.Status;
                project.StartDate = request.StartDate;
                project.EndDate = request.EndDate;
                project.Progress = request.Progress;

                await _projectRepository.UpdateAsync(project);
                return MapToDTO(project);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        public async Task<bool> DeleteProjectAsync(string encryptedId)
        {
            try
            {
                int id = _encryptionService.DecryptId(encryptedId);
                var project = await _projectRepository.GetByIdAsync(id);
                if (project == null) return false;

                await _projectRepository.DeleteAsync(project);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}
