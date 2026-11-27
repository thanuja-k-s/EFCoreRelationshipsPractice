namespace EFCoreRelationshipsPractice.Services
{
    public class StudentNameValidator
    {
        public bool IsValid(string? name)
        {

            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            if (name.Length < 3)
            {
                return false;
            }

            return true;
        }
    }
}
