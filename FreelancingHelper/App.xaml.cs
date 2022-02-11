using FreelancingHelper.Services.Directories;
using FreelancingHelper.Services.Serializator;
using FreelancingHelper.Services.Settings;
using FreelancingHelper.Services.Startup;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Linq;
using System;

namespace FreelancingHelper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ServiceProvider ServiceProvider;

        public App()
        {
            //Custom Microsoft Dependency Injection
            ServiceCollection services = new();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<IStartupService, StartupService>();
            services.AddSingleton<IDirectoriesService, DirectoriesService>();
            services.AddSingleton<ISerializatorService, SerializatorService>();
            services.AddSingleton<ISettingsService, SettingsService>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            ServiceProvider.GetService<IStartupService>()?.InitialChecks();

            base.OnStartup(e);
        }
    }
}
