using System.Linq;
using System.Threading.Tasks;
using Data.Repositories;
using Models.DTOs;

namespace Business.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IProjectRepository _projectRepository;

        public DashboardService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<DashboardSummaryDTO> GetDashboardSummaryAsync()
        {
            var projects = await _projectRepository.GetAllAsync();

            int totalProject = projects.Count();
            int onProgress = projects.Count(p => p.Status == "On Progress");
            int completed = projects.Count(p => p.Status == "Completed");
            int overdue = projects.Count(p => p.Status == "Overdue");

            double progressPercentage = 0;
            if (totalProject > 0)
            {
                progressPercentage = projects.Average(p => p.Progress);
            }

            return new DashboardSummaryDTO
            {
                TotalProject = totalProject,
                OnProgress = onProgress,
                Completed = completed,
                Overdue = overdue,
                ProgressPercentage = System.Math.Round(progressPercentage, 2)
            };
        }
    }
}
