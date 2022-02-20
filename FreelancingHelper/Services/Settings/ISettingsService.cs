using FreelancingHelper.Models;
using FreelancingHelper.Models.Interfaces;
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
        Task<long> GetAndIncrementObjectTypeIdCounter<TModel>() where TModel : IAutoId;
        Color TrySetAppsPrimaryColorFromHexa(string newColorHexa);
        void SetAppsPrimaryColor(Color newColor);
        bool CheckIfEmailSettingsAreSet();
    }
}