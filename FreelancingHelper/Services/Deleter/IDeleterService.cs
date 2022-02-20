using FreelancingHelper.Models;

namespace FreelancingHelper.Services.Deleter
{
    public interface IDeleterService
    {
        void DeleteHirer(Hirer hirer);
        void DeleteDayWork(DayWork dayWork);
    }
}