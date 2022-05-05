using FreelancingHelper.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace FreelancingHelper.Models
{
    public class DayWork : IAutoId
    {
        public long Id { get; set; }
        public long HirerId { get; set; }
        public DateTime Started { get; set; }
        public DateTime Finished { get; set; }
        public TimeSpan TotalWorkingTime { get; set; }
        public double TotalMoneyGained { get; set; }
        public List<WorkingTime> DayWorkingTimes { get; set; }
    }
}
