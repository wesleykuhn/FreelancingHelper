using FreelancingHelper.Models;
using FreelancingHelper.Services.Directories;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FreelancingHelper.Services.Serializator
{
    public class SerializatorService : ISerializatorService
    {
        private readonly IDirectoriesService _directoriesService;
        public SerializatorService(IDirectoriesService directoriesService)
        {
            _directoriesService = directoriesService;
        }

        private T SerializationToObject<T>(string serializedObject) => JsonConvert.DeserializeObject<T>(serializedObject);

        private string ObjectToSerialization<T>(T obj) => JsonConvert.SerializeObject(obj);

        #region [ APPs COFIG ]

        public Task SerializeAppConfigurationAsync(AppConfiguration appConfiguration)
        {
            var serialized = ObjectToSerialization(appConfiguration);

            return File.WriteAllTextAsync(_directoriesService.AppConfigurationsDir, serialized);
        }

        public async Task<AppConfiguration> DesserializeAppConfigurationAsync()
        {
            var readed = await File.ReadAllTextAsync(_directoriesService.AppConfigurationsDir);

            try
            {
                return SerializationToObject<AppConfiguration>(readed);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        #endregion

        #region [ HIRER ]

        public Task SerializeHirer(Hirer hirer)
        {
            var serialized = ObjectToSerialization(hirer);

            return File.WriteAllTextAsync(_directoriesService.GetHirerDir(hirer), serialized);
        }

        public async Task<List<Hirer>> DesserializeAllHirers()
        {
            var hirers = new List<Hirer>();

            var hirersFiles = Directory.EnumerateFiles(_directoriesService.HirersDir);

            foreach (var file in hirersFiles)
            {
                hirers.Add(await DesserializeHirer(file));
            }

            return hirers;
        }

        private async Task<Hirer> DesserializeHirer(string path)
        {
            var readed = await File.ReadAllTextAsync(path);

            try
            {
                return SerializationToObject<Hirer>(readed);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        #endregion

        public Task SerializeDayWork(DayWork dayWork)
        {
            var serialized = ObjectToSerialization(dayWork);

            return File.WriteAllTextAsync(_directoriesService.GetDayWorkDir(dayWork), serialized);
        }

        public async Task<DayWork> DesserializeDayWorkAsync(string path)
        {
            var readed = await File.ReadAllTextAsync(path);

            try
            {
                return SerializationToObject<DayWork>(readed);
            }
            catch (System.Exception)
            {
                return null;
            }
        }
    }
}
