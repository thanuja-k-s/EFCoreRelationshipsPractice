using EFCoreRelationshipsPractice.Models;

namespace EFCorePractice.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public string Product { get; set; } = "";

        public int StudentId { get; set; }

        public Student Student { get; set; } = null!;
    }
}