using FreelancingHelper.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreelancingHelper.Services.Objects
{
    public interface IHirerService
    {
        List<Hirer> Hirers { get; }
        Task<Hirer> AddHirer(string name, string email, float salaryPerHour);
        Task LoadAllHirers();
        Task UpdateHirer(Hirer hirer);
        void DeleteHirer(Hirer hirer);
    }
}