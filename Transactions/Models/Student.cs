using Transactions.Models;

namespace Transactions.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Age { get; set; }
        public decimal PendingFees { get; set; } = 10000;

        // Concurrency token version
        public int Version { get; set; } = 1;

        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; } = null!;

        public virtual StudentProfile? Profile { get; set; }
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}