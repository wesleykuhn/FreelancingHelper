using FreelancingHelper.Models;
using FreelancingHelper.Services.Deleter;
using FreelancingHelper.Services.Directories;
using FreelancingHelper.Services.Serializator;
using FreelancingHelper.Services.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FreelancingHelper.Services.Objects
{
    public class DayWorkService : IDayWorkService
    {
        private readonly IDirectoriesService _directoriesService;
        private readonly IDeleterService _deleterService;
        private ISettingsService _settingsService;
        private ISerializatorService _serializatorService;
        public DayWorkService(IDirectoriesService directoriesService,
            ISettingsService settingsService,
            ISerializatorService serializatorService,
            IDeleterService deleterService)
        {
            _directoriesService = directoriesService;
            _settingsService = settingsService;
            _serializatorService= serializatorService;
            _deleterService = deleterService;
        }

        public async Task<DayWork> CreateDayWork(long hirerId, DateTime startedAt, List<WorkingTime> dayWorkingTimes)
        {
            DayWork newDayWork = new()
            {
                HirerId = hirerId,
                Started = startedAt,
                DayWorkingTimes = dayWorkingTimes
            };

            newDayWork.Id = await _settingsService.GetAndIncrementObjectTypeIdCounter<DayWork>();

            await _serializatorService.SerializeDayWork(newDayWork);

            return newDayWork;
        }

        public async Task<IEnumerable<DayWork>> GetAllDayWorks()
        {
            var files = _directoriesService.GetAllDaysWorkPathAsync();

            if (files == null || files.Count() == 0)
                return null;

            List<DayWork> dayWorks = new List<DayWork>();

            foreach (var file in files)
            {
                dayWorks.Add(await _serializatorService.DesserializeDayWorkAsync(file));
            }

            return dayWorks;
        }

        public async Task<DayWork> SeekForTodaysDayWork()
        {
            var dayWorkDir = _directoriesService.TryRecoveryLastDayWorkDir(_settingsService.AppConfiguration.CurrentSelectedHirerId);

            if (!string.IsNullOrEmpty(dayWorkDir))
            {
                var dayWork = await _serializatorService.DesserializeDayWorkAsync(dayWorkDir);

                if (dayWork == null || dayWork == default(DayWork) || dayWork.Finished != DateTime.MinValue)
                    return null;
                else
                    return dayWork;
            }
            else return null;
        }

        public Task UpdateDayWork(DayWork dayWork) =>
            _serializatorService.SerializeDayWork(dayWork);

        public void DeleteDayWork(DayWork dayWork) =>
            _deleterService.DeleteDayWork(dayWork);
    }
}
