using FreelancingHelper.Models;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FreelancingHelper.Services.Settings
{
    public interface ISettingsService
    {
        AppConfiguration AppConfiguration { get; }

        Task LoadAppConfigurationAsync();
        Task SaveAppConfigurationAsync();
        Task GenerateDefaultAppConfiguration();
        Color TrySetAppsPrimaryColorFromHexa(string newColorHexa);
        Task SaveAppsPrimaryColor(Color newColor);
    }
}