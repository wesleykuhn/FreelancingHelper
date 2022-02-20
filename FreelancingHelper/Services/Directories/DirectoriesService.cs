using FreelancingHelper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FreelancingHelper.Services.Directories
{
    public class DirectoriesService : IDirectoriesService
    {
        public string AppConfigurationsDir => _appPath + $"\\{nameof(AppConfiguration)}.json";
        public string HirersDir => _appPath + $"{nameof(Hirer)}s";
        public string DaysWorkDir => _appPath + "History";

        private readonly string _appPath;

        public DirectoriesService()
        {
            _appPath =  AppDomain.CurrentDomain.BaseDirectory;
        }

        public string GetHirerDir(Hirer hirer) =>
            HirersDir + $"\\{hirer.Id}.json";

        //id_ano_mes_dia_hirerId
        public string GetDayWorkDir(DayWork dayWork) =>
            DaysWorkDir + $"\\{dayWork.Id}_{dayWork.Started.Year}_{dayWork.Started.Month}_{dayWork.Started.Day}_{dayWork.HirerId}.json";

        public string TryRecoveryLastDayWorkDir(long currentHirerId)
        {
            var now = DateTime.Now;

            var possibleFileName = $"{now.Year}_{now.Month}_{now.Day}_{currentHirerId}.json";

            var filesList = Directory.GetFiles(DaysWorkDir);

            foreach (var fileName in filesList)
            {
                if (fileName.Contains(possibleFileName))
                    return fileName;
            }

            return null;
        }

        public IEnumerable<string> GetAllDaysWorkPathAsync()
        {
            var dirs = Directory.EnumerateFiles(DaysWorkDir);

            if (dirs == null || dirs.Count() == 0)
                return null;
            else
                return dirs;
        }
    }
}
