using Microsoft.EntityFrameworkCore;
using myLeave.Data;
using myLeave.Dtos;
using myLeave.Models;
using myLeave.Services.Interfaces;

namespace myLeave.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Employee?> LoginAsync(string email, string password)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.Email == email && e.Password == password);
        }

        public async Task<List<Employee>> GetAllUsersAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<bool> RegisterAsync(EmployeeRegisterDto dto)
        {
            var existing = await _context.Employees.FirstOrDefaultAsync(e => e.Email == dto.Email);
            if (existing != null)
            {
                throw new Exception("User already exists");
            }

            var employee = new Employee
            {
                Email = dto.Email,
                Password = dto.Password,
                Role = dto.Role
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return true;
        }


        public Employee? Authenticate(string email, string password)
        {
            return _context.Employees.FirstOrDefault(e => e.Email == email && e.Password == password);
        }
    }
}
