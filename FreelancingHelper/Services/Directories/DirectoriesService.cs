using FreelancingHelper.Models;
using System;

namespace FreelancingHelper.Services.Directories
{
    public class DirectoriesService : IDirectoriesService
    {
        public string HistoryFilesDir => _appPath + "History";

        public string AppConfigurationsDir => _appPath + $"\\{nameof(AppConfiguration)}.json";

        private readonly string _appPath;

        public DirectoriesService()
        {
            _appPath =  AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}
