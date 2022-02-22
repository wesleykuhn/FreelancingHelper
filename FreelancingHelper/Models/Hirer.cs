using FreelancingHelper.Models.Interfaces;

namespace FreelancingHelper.Models
{
    public class Hirer : IAutoId
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public float SalaryPerHour { get; set; }

        public Hirer() { }
        public Hirer(string name, string email, float salaryPerHour)
        {
            Name = name;
            Email = email;
            SalaryPerHour = salaryPerHour;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is Hirer))
                return false;

            return
                Id == ((Hirer)obj).Id;
        }

        public override string ToString() =>
            Name + " - " + Email;
    }
}
