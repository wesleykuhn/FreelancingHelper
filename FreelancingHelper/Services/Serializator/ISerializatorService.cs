using FreelancingHelper.Models;
using System.Threading.Tasks;

namespace FreelancingHelper.Services.Serializator
{
    public interface ISerializatorService
    {
        Task<AppConfiguration> DesserializeAppConfigurationAsync();
        Task SerializeAppConfigurationAsync(AppConfiguration appConfiguration);
    }
}