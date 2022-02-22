using FreelancingHelper.Models;
using FreelancingHelper.Services.Deleter;
using FreelancingHelper.Services.Serializator;
using FreelancingHelper.Services.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreelancingHelper.Services.Objects
{
    public class HirerService : IHirerService
    {
        private List<Hirer> _hirers;
        public List<Hirer> Hirers
        {
            get => _hirers;
            private set => _hirers = value;
        }

        private readonly ISerializatorService _serializatorService;
        private ISettingsService _settingsService;
        private readonly IDeleterService _deleterService;
        public HirerService(ISerializatorService serializatorService, ISettingsService settingsService, IDeleterService deleterService)
        {
            _serializatorService = serializatorService;
            _settingsService = settingsService;
            _deleterService = deleterService;
        }

        public async Task<Hirer> AddHirer(string name, string email, float salaryPerHour)
        {
            Hirer newHirer = new(name, email, salaryPerHour);

            newHirer.Id = await _settingsService.GetAndIncrementObjectTypeIdCounter<Hirer>();

            await _serializatorService.SerializeHirer(newHirer);

            Hirers.Add(newHirer);

            return newHirer;
        }

        public async Task LoadAllHirers()
        {
            Hirers = await _serializatorService.DesserializeAllHirers();
        }

        public Task UpdateHirer(Hirer hirer)
        {
            var index = Hirers.FindIndex(fi => fi.Id == hirer.Id);
            Hirers[index] = hirer;

            return _serializatorService.SerializeHirer(hirer);
        }

        public void DeleteHirer(Hirer hirer)
        {
            _deleterService.DeleteHirer(hirer);

            var index = Hirers.FindIndex(fi => fi.Id == hirer.Id);
            Hirers.RemoveAt(index);
        }
    }
}
