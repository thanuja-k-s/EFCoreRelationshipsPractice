using Transactions.Models;

namespace Transactions.Models
{
    public class StudentProfile
    {
        public int Id { get; set; }
        public string Address { get; set; } = "";
        public string PhoneNumber { get; set; } = "";

        public int StudentId { get; set; }
        public virtual Student Student { get; set; } = null!;
    }
}