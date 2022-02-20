using FreelancingHelper.Services.Deleter;
using FreelancingHelper.Services.Directories;
using FreelancingHelper.Services.Email;
using FreelancingHelper.Services.Objects;
using FreelancingHelper.Services.Serializator;
using FreelancingHelper.Services.Settings;
using FreelancingHelper.Services.Startup;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

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
            services.AddScoped<IDeleterService, DeleterService>();
            services.AddScoped<IStartupService, StartupService>();
            services.AddScoped<ISerializatorService, SerializatorService>();
            services.AddScoped<IDirectoriesService, DirectoriesService>();
            services.AddScoped<IEmailTemplatesService, EmailTemplatesService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IHirerService, HirerService>();
            services.AddSingleton<IDayWorkService, DayWorkService>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            ServiceProvider.GetService<IStartupService>()?.InitialChecks();

            base.OnStartup(e);
        }
    }
}
