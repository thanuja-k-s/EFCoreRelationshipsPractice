using EFCoreRelationshipsPractice.Models;
using EFCoreRelationshipsPractice.Repositories;

namespace EFCoreRelationshipsPractice.Services;

public class StudentService
{
    private readonly IStudentRepository _repository;

    public StudentService(IStudentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Student?> GetStudentByIdAsync(int id)
    {
        var student = await _repository.GetByIdAsync(id);

        return student;
    }
}