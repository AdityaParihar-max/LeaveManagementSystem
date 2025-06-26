using Microsoft.EntityFrameworkCore;
using myLeave.Data;
using myLeave.Interfaces;

namespace myLeave.Repositories
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly AppDbContext _context;

        public LeaveRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Leave>> GetLeavesByEmailAsync(string email)
        {
            return await _context.Leaves
                .Where(l => l.EmployeeEmail == email)
                .OrderByDescending(l => l.StartDate)
                .ToListAsync();
        }



        public async Task<bool> ApplyAsync(Leave leave)
        {
            _context.Leaves.Add(leave);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Leave>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.Leaves.Where(l => l.EmployeeId == employeeId).ToListAsync();
        }

        public async Task<List<Leave>> GetPendingAsync()
        {
            return await _context.Leaves.Include(l => l.Employee).Where(l => l.Status == "Pending").ToListAsync();
        }

        public async Task<List<Leave>> GetAllWithEmployeeAsync()
        {
            return await _context.Leaves.Include(l => l.Employee).ToListAsync();
        }

        public async Task<Leave?> GetByIdAsync(int id)
        {
            return await _context.Leaves.FindAsync(id);
        }

        public async Task<bool> UpdateStatusAsync(Leave leave, string status)
        {
            leave.Status = status;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}