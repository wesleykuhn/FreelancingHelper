using FreelancingHelper.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreelancingHelper.Services.Serializator
{
    public interface ISerializatorService
    {
        Task<AppConfiguration> DesserializeAppConfigurationAsync();
        Task SerializeAppConfigurationAsync(AppConfiguration appConfiguration);
        Task SerializeHirer(Hirer hirer);
        Task<List<Hirer>> DesserializeAllHirers();
        Task SerializeDayWork(DayWork dayWork);
        Task<DayWork> DesserializeDayWorkAsync(string path);
    }
}