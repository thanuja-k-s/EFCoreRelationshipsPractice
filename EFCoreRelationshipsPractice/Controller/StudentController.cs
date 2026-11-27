
using System.Data;
using Dapper;
using EFCorePractice.DTOs;
using EFCorePractice.Models;
using EFCoreRelationshipsPractice.Data;
using EFCoreRelationshipsPractice.DTO;
using EFCoreRelationshipsPractice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCoreRelationshipsPractice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("add-orders")]
        public async Task<IActionResult> AddOrders()
        {
            var students = await _context.Students.ToListAsync();

            if (students.Count < 3)
            {
                return BadRequest("Add at least 3 students first.");
            }

            var orders = new List<Order>
    {
        new Order
        {
            StudentId = students[0].Id,
            Product = "Laptop"
        },
        new Order
        {
            StudentId = students[0].Id,
            Product = "Mouse"
        },
        new Order
        {
            StudentId = students[1].Id,
            Product = "Keyboard"
        },
        new Order
        {
            StudentId = students[2].Id,
            Product = "Monitor"
        }
    };

            _context.Orders.AddRange(orders);

            await _context.SaveChangesAsync();

            return Ok("Sample orders added successfully");
        }

        [HttpGet("n-plus-one")]
        public async Task<IActionResult> GetStudentsWithNPlusOne()
        {
            var students = await _context.Students.ToListAsync();

            foreach (var student in students)
            {
                var orders = await _context.Orders
                    .Where(o => o.StudentId == student.Id)
                    .ToListAsync();

                student.Orders = orders;
            }

            return Ok(students.Select(student => new
            {
                student.Id,
                student.Name,
                student.Age,
                Orders = student.Orders.Select(order => new
                {
                    order.OrderId,
                    order.Product,
                    order.StudentId
                })
            }));
        }

        [HttpGet("with-include")]
        public async Task<IActionResult> GetStudentsWithInclude()
        {
            var students = await _context.Students
                .Include(s => s.Orders)
                .ToListAsync();

            return Ok(students.Select(student => new
            {
                student.Id,
                student.Name,
                student.Age,
                Orders = student.Orders.Select(order => new
                {
                    order.OrderId,
                    order.Product,
                    order.StudentId
                })
            }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudent(Student student)
        {
            if (student is null)
            {
                return BadRequest("Student data is required.");
            }

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return Ok(student);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Student ID must be a positive integer.");
            }

            var student = await _context.Students
                .AsNoTracking()
                
               .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                return NotFound($"Student with ID {id} was not found.");
            }

            return Ok(student);
        }

            [HttpGet]
            public async Task<IActionResult> GetStudents()
            {
                var students = await _context.Students
                    .ToListAsync();

                return Ok(students);
            }

            [HttpGet("eager")]
            public async Task<IActionResult> GetStudentsWithDepartment()
            {
                var students = await _context.Students
                    .Include(s => s.Department)
                    .ToListAsync();

                return Ok(students.Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Age,
                    DepartmentName = s.Department.Name
                }));

            }


            [HttpGet("explicit/{id}")]
            public async Task<IActionResult> GetStudentExplicit(int id)
            {
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (student == null)
                {
                    return NotFound();
                }

                await _context.Entry(student)
                    .Reference(s => s.Department)
                    .LoadAsync();

                return Ok(new
                {
                    student.Id,
                    student.Name,
                    student.Age,
                    DepartmentName = student.Department.Name
                });
            }

            // Exercise 2C: Lazy Loading Test Endpoint
            [HttpGet("lazy/{id}")]
            public async Task<IActionResult> GetStudentLazy(int id)
            {
                // Notice: NO .Include() or .LoadAsync() is used here!
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (student == null)
                {
                    return NotFound();
                }

                // Accessing the 'Department' navigation property here 
                // will automatically trigger a second database query behind the scenes.
                var departmentName = student.Department?.Name;

                return Ok(new
                {
                    student.Id,
                    student.Name,
                    student.Age,
                    DepartmentName = departmentName
                });
            }

            [HttpPost("tracking/add")]
            public async Task<IActionResult> AddWithTracking(Student student)
            {
                _context.Students.Add(student);

                var stateBeforeSave =
                    _context.Entry(student).State.ToString();

                await _context.SaveChangesAsync();

                var stateAfterSave =
                    _context.Entry(student).State.ToString();

                return Ok(new
                {
                    Student = student,
                    StateBeforeSave = stateBeforeSave,
                    StateAfterSave = stateAfterSave
                });
            }


            // Exercise 3: Change Tracking & Entity States
            [HttpPut("tracking/update/{id}")]
            public async Task<IActionResult> UpdateStudentWithTracking(int id)
            {
                // 1. Retrieve the student (EF Core starts tracking this entity)
                var student = await _context.Students
                    .Include(s => s.Department)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (student == null)
                {
                    return NotFound();
                }

                // Check entry state right after fetching (should be Unchanged)
                var stateBeforeChange = _context.Entry(student).State.ToString();

                // 2. Modify a property
                student.Age = 22;

                // Check entry state after modifying (should be Modified)
                var stateBeforeSave = _context.Entry(student).State.ToString();

                // 3. Save changes to the database
                await _context.SaveChangesAsync();

                // Check entry state after saving (should return to Unchanged)
                var stateAfterSave = _context.Entry(student).State.ToString();

                // Return safe anonymous object to prevent JSON circular reference loops
                return Ok(new
                {
                    Message = "Change tracking demonstration successful!",
                    StudentId = student.Id,
                    StudentName = student.Name,
                    UpdatedAge = student.Age,
                    States = new
                    {
                        AfterFetching = stateBeforeChange,
                        AfterModifying = stateBeforeSave,
                        AfterSaveChanges = stateAfterSave
                    },
                    DepartmentName = student.Department?.Name
                });
            }

            [HttpDelete("tracking/delete/{id}")]
            public async Task<IActionResult> DeleteWithTracking(int id)
            {
                var student = await _context.Students.FindAsync(id);

                if (student == null)
                {
                    return NotFound();
                }

                _context.Students.Remove(student);

                var stateBeforeSave =
                    _context.Entry(student).State.ToString();

                await _context.SaveChangesAsync();

                var stateAfterSave =
                   _context.Entry(student).State.ToString();

                return Ok(new
                {
                    Message = "Student deleted",

                    StateBeforeSave = stateBeforeSave,
                    StateAfterSave = stateAfterSave
                });
            }

            [HttpGet("tracking/read-only")]
            public async Task<IActionResult> GetReadOnlyStudents()
            {
                // Project the data into a clean anonymous object to prevent circular reference loops
                var students = await _context.Students
                    .AsNoTracking()
                    .Include(s => s.Department)
                    .Select(s => new
                    {
                        s.Id,
                        s.Name,
                        s.Age,
                        DepartmentName = s.Department != null ? s.Department.Name : null
                    })
                    .ToListAsync();

                return Ok(students);
            }

            // Challenge 1: Create a profile for Student ID 1
            [HttpPost("profile/create/{studentId}")]
            public async Task<IActionResult> CreateStudentProfile(int studentId, [FromBody] StudentProfileDto dto)
            {
                var student = await _context.Students.FindAsync(studentId);
                if (student == null)
                {
                    return NotFound("Student not found.");
                }

                var profile = new StudentProfile
                {
                    Address = dto.Address,
                    PhoneNumber = dto.PhoneNumber,
                    StudentId = studentId
                };

                _context.StudentProfiles.Add(profile);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Profile created successfully!", studentId, dto.Address, dto.PhoneNumber });
            }

            // Challenge 1: Retrieve student with profile using Include()
            [HttpGet("profile/get/{id}")]
            public async Task<IActionResult> GetStudentWithProfile(int id)
            {
                var student = await _context.Students
                    .Include(s => s.Profile) // Eagerly load the related profile
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (student == null)
                {
                    return NotFound();
                }

                // Return safe flat object with null-conditional operators to prevent warnings
                return Ok(new
                {
                    student.Id,
                    student.Name,
                    student.Age,
                    Address = student.Profile?.Address,
                    PhoneNumber = student.Profile?.PhoneNumber
                });
            }


            [HttpPost("transaction")]
            public async Task<IActionResult> CreateStudentTransaction(Student student)
            {
                await using var transaction =
                    await _context.Database.BeginTransactionAsync();

                try
                {
                    _context.Students.Add(student);

                    await _context.SaveChangesAsync();

                    var payment = new Payment
                    {
                        StudentId = student.Id,
                        Amount = 500
                    };

                    _context.Payments.Add(payment);

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Ok(new
                    {
                        Message = "Student and payment saved successfully",
                        Student = new
                        {
                            student.Id,
                            student.Name,
                            student.Age,
                            DepartmentName = student.Department?.Name
                        },
                        Payment = new
                        {
                            payment.Id,
                            payment.StudentId,
                            payment.Amount
                        }
                    });
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }


            [HttpPut("concurrency/{id}")]
            public async Task<IActionResult> UpdateWithConcurrency(
                int id,
                UpdateStudentRequest request)
            {
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (student == null)
                {
                    return NotFound();
                }

                // Use the version sent by the client
                // as the original version for the database check.
                _context.Entry(student)
                    .Property(s => s.Version)
                    .OriginalValue = request.Version;

                student.Age = request.Age;

                // Increase the version for the next update.
                student.Version++;

                try
                {
                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        Message = "Student updated successfully",
                        Student = student
                    });
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Conflict(new
                    {
                        Message = "This student was updated by another user. Please refresh and try again."
                    });
                }
            }


            [HttpGet("dapper")]
            public async Task<IActionResult> GetStudentsUsingDapper()
            {
                var connection = _context.Database.GetDbConnection();

                bool shouldClose = connection.State != ConnectionState.Open;

                if (shouldClose)
                {
                    await connection.OpenAsync();
                }

                try
                {
                    var sql = """
            SELECT Id, Name
            FROM Students
            """;

                    var students = await connection.QueryAsync<StudentDto>(sql);

                    return Ok(students);
                }
                finally
                {
                    if (shouldClose)
                    {
                        await connection.CloseAsync();
                    }
                }
            }



        }
    }
