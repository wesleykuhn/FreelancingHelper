using FreelancingHelper.Models;
using FreelancingHelper.Services.Directories;
using System.IO;

namespace FreelancingHelper.Services.Deleter
{
    public class DeleterService : IDeleterService
    {
        private readonly IDirectoriesService _directoriesService;
        public DeleterService(IDirectoriesService directoriesService)
        {
            _directoriesService = directoriesService;
        }

        public void DeleteHirer(Hirer hirer)
        {
            var hirerDir = _directoriesService.GetHirerDir(hirer);

            DeleteFile(hirerDir);
        }

        public void DeleteDayWork(DayWork dayWork)
        {
            var dayWorkDir = _directoriesService.GetDayWorkDir(dayWork);

            DeleteFile(dayWorkDir);
        }

        private void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
