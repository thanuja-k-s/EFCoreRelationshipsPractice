using Transactions.Models;

namespace Transactions.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        public virtual Student Student { get; set; } = null!;
    }
}