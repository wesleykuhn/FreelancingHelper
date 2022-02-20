using FreelancingHelper.Models;
using System.Collections.Generic;

namespace FreelancingHelper.Services.Directories
{
    public interface IDirectoriesService
    {
        string AppConfigurationsDir { get; }
        string HirersDir { get; }
        string DaysWorkDir { get; }
        string GetHirerDir(Hirer hirer);
        string GetDayWorkDir(DayWork dayWork);
        string TryRecoveryLastDayWorkDir(long currentHirerId);
        IEnumerable<string> GetAllDaysWorkPathAsync();
    }
}