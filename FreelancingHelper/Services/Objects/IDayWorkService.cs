using FreelancingHelper.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreelancingHelper.Services.Objects
{
    public interface IDayWorkService
    {
        Task<DayWork> CreateDayWork(long hirerId, DateTime startedAt, List<WorkingTime> dayWorkingTimes);
        Task<IEnumerable<DayWork>> GetAllDayWorks();
        Task<IEnumerable<DayWork>> GetOnlyDayWorksAsync(IEnumerable<long> ids);
        Task<DayWork> SeekForTodaysDayWork();
        Task UpdateDayWork(DayWork dayWork);
        void DeleteDayWork(DayWork dayWork);
    }
}