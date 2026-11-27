using Transactions.DTOs;
using Transactions.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[Route("api/[controller]")]
[ApiController]
public class StudentController : ControllerBase
{
    private readonly AppDbContext _context;

    public StudentController(AppDbContext context)
    {
        _context = context;
    }

    // 1. Fee Payment with Explicit Transaction
    [HttpPost("pay-fees")]
    public async Task<IActionResult> PayFees([FromBody] PaymentRequestDto request)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == request.StudentId);
            if (student == null)
            {
                return NotFound("Student not found.");
            }

            if (request.Amount <= 0)
            {
                return BadRequest("Payment amount must be greater than zero.");
            }

            if (request.Amount > student.PendingFees)
            {
                return BadRequest(new { Message = "Reject payment: Payment amount exceeds pending fees.", CurrentPendingFees = student.PendingFees });
            }

            var payment = new Payment
            {
                StudentId = request.StudentId,
                Amount = request.Amount,
                PaymentDate = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            // Reduce pending fees
            student.PendingFees -= request.Amount;
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return Ok(new
            {
                Message = "Payment successful!",
                student.Id,
                student.Name,
                PaidAmount = request.Amount,
                RemainingFees = student.PendingFees
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, new { Message = "Transaction rolled back due to an error.", Error = ex.Message });
        }
    }

    // 2. Optimistic Concurrency Update
    [HttpPut("concurrency/{id}")]
    public async Task<IActionResult> UpdateWithConcurrency(int id, [FromBody] UpdateStudentRequest request)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
        if (student == null)
        {
            return NotFound("Student not found.");
        }

        // Set the original value for concurrency checking
        _context.Entry(student).Property(s => s.Version).OriginalValue = request.Version;

        student.Age = request.Age;
        student.Version++; // Increment version for the next save

        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Student updated successfully!", Student = student });
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new { Message = "This student was updated by another user. Please refresh and try again." });
        }
    }
}