namespace Transactions.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        // Must be virtual if proxy tracking is enabled
        public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    }
}