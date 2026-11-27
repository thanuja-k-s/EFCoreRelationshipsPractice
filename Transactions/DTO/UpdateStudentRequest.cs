namespace Transactions.DTOs
{
    public class UpdateStudentRequest
    {
        public int Age { get; set; }
        public int Version { get; set; }
    }

    public class PaymentRequestDto
    {
        public int StudentId { get; set; }
        public decimal Amount { get; set; }
    }
}