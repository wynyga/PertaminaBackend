using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTOs;

namespace Business.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectResponseDTO>> GetAllProjectsAsync();
        Task<ProjectResponseDTO?> GetProjectByIdAsync(string encryptedId);
        Task<ProjectResponseDTO?> CreateProjectAsync(ProjectRequestDTO request);
        Task<ProjectResponseDTO?> UpdateProjectAsync(string encryptedId, ProjectRequestDTO request);
        Task<bool> DeleteProjectAsync(string encryptedId);
    }
}
