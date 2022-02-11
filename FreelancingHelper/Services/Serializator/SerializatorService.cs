using FreelancingHelper.Models;
using FreelancingHelper.Services.Directories;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace FreelancingHelper.Services.Serializator
{
    public class SerializatorService : ISerializatorService
    {
        private readonly IDirectoriesService _directoryService;
        public SerializatorService(IDirectoriesService directoryService)
        {
            _directoryService = directoryService;
        }

        private T SerializationToObject<T>(string serializedObject) => JsonConvert.DeserializeObject<T>(serializedObject);

        private string ObjectToSerialization<T>(T obj) => JsonConvert.SerializeObject(obj);

        public async Task SerializeAppConfigurationAsync(AppConfiguration appConfiguration)
        {
            var serialized = ObjectToSerialization(appConfiguration);

            await File.WriteAllTextAsync(_directoryService.AppConfigurationsDir, serialized);
        }

        public async Task<AppConfiguration> DesserializeAppConfigurationAsync()
        {
            var readed = await File.ReadAllTextAsync(_directoryService.AppConfigurationsDir);

            try
            {
                return SerializationToObject<AppConfiguration>(readed);
            }
            catch (System.Exception)
            {
                return null;
            }
        }
    }
}
