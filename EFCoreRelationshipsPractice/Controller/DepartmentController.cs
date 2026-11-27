
using EFCoreRelationshipsPractice.Data;
using EFCoreRelationshipsPractice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCoreRelationshipsPractice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DepartmentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment(Department department)
        {
            _context.Departments.Add(department);

            await _context.SaveChangesAsync();

            return Ok(department);
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartments()
        {
            var departments = await _context.Departments
                .ToListAsync();

            return Ok(departments);
        }
    }
}
