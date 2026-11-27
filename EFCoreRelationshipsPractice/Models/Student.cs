
using EFCorePractice.Models;

namespace EFCoreRelationshipsPractice.Models
{
    public class Student
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public int Age { get; set; }

        public int DepartmentId { get; set; }

        public Department? Department { get; set; } = null!;

        public StudentProfile? Profile { get; set; }

        public int Version { get; set; } = 1;

        public ICollection<Order> Orders { get; set; }
    = new List<Order>();
    }
}
