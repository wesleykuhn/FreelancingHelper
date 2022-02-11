using System;
using System.Collections.Generic;

namespace FreelancingHelper.Models
{
    public class DayWork
    {
        public int Id { get; set; }
        public DateTime Started { get; set; }
        public DateTime Finished { get; set; }
        public List<WorkingTime>? DayWorkingTimes { get; set; }
    }
}
