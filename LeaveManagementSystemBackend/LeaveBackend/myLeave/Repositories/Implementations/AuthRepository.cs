using Microsoft.EntityFrameworkCore;
using myLeave.Data;
using myLeave.Dtos;
using myLeave.Interfaces;
using myLeave.Models;

namespace myLeave.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Employee?> LoginAsync(string email, string password)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.Email == email && e.Password == password);
        }

        public async Task<List<Employee>> GetAllAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<bool> RegisterAsync(EmployeeRegisterDto dto)
        {
            if (await _context.Employees.AnyAsync(e => e.Email == dto.Email))
                return false;

            _context.Employees.Add(new Employee { Email = dto.Email, Password = dto.Password });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Employee?> GetByEmailAsync(string email)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);
        }
    }
}
