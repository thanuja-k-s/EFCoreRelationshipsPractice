using EFCoreRelationshipsPractice.Models;

namespace EFCoreRelationshipsPractice.Repositories;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(int id);
}
