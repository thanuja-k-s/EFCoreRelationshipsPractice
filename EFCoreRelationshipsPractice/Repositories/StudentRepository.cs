using EFCoreRelationshipsPractice.Data;
using EFCoreRelationshipsPractice.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCoreRelationshipsPractice.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _context;

    public StudentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await _context.Students
            .FirstOrDefaultAsync(s => s.Id == id);
    }
}