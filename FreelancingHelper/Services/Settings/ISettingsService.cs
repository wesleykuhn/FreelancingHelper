using FreelancingHelper.Models;
using System.Threading.Tasks;

namespace FreelancingHelper.Services.Settings
{
    public interface ISettingsService
    {
        AppConfiguration AppConfiguration { get; }

        Task LoadAppConfigurationAsync();
        Task SaveAppConfigurationAsync();
        Task GenerateDefaultAppConfiguration();
    }
}