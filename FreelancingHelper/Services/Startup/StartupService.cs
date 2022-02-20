using FreelancingHelper.Services.Directories;
using FreelancingHelper.Services.Settings;
using System.IO;

namespace FreelancingHelper.Services.Startup
{
    public class StartupService : IStartupService
    {
        private readonly IDirectoriesService _directoryService;
        private readonly ISettingsService _settingsService;
        public StartupService(IDirectoriesService directoryService, ISettingsService settingsService)
        {
            _directoryService = directoryService;
            _settingsService = settingsService;
        }

        public void InitialChecks()
        {
            CheckDirectories();

            if (!File.Exists(_directoryService.AppConfigurationsDir))
                _settingsService.GenerateDefaultAppConfiguration();
            else
                _settingsService.LoadAppConfigurationAsync();

        }

        private void CheckDirectories()
        {
            CheckAndCreateDirectory(_directoryService.DaysWorkDir);
            CheckAndCreateDirectory(_directoryService.HirersDir);
        }

        private void CheckAndCreateDirectory(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
    }
}
