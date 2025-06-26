using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myLeave.Data;
using Microsoft.AspNetCore.Authorization;
namespace myLeave.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("byEmail/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);
            if (employee == null)
                return NotFound(new { message = "Employee not found" });

            return Ok(new { id = employee.Id, email = employee.Email });
        }
    }
}
