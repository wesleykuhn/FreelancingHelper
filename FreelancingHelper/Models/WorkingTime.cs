using System;

namespace FreelancingHelper.Models
{
    public class WorkingTime
    {
        public DateTime StartedAt { get; set; }
        public DateTime FinishedAt { get; set; }

        public WorkingTime() { }

        public WorkingTime(DateTime startedAt)
        {
            StartedAt = startedAt;
        }
    }
}
