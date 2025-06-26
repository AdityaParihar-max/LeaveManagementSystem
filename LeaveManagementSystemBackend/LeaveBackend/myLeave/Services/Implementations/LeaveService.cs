using Microsoft.EntityFrameworkCore;
using myLeave.Data;
using myLeave.Dtos;
using myLeave.Services.Interfaces;


namespace myLeave.Services.Implementations
{
    public class LeaveService : ILeaveService
    {
        private readonly AppDbContext _context;

        public LeaveService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<LeaveDto>> GetLeavesByEmailAsync(string email)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);
            if (employee == null) return new List<LeaveDto>();

            var leaves = await _context.Leaves
                .Where(l => l.EmployeeId == employee.Id)
                .ToListAsync();

            return leaves.Select(l => new LeaveDto
            {
                LeaveType = l.LeaveType,
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                StartDateFormatted = l.StartDate.ToString("dd-MM-yyyy"),
                EndDateFormatted = l.EndDate.ToString("dd-MM-yyyy"),
                Reason = l.Reason,
                Status = l.Status
            }).ToList();
        }



        public async Task<bool> ApplyLeaveAsync(LeaveCreateDto dto)
        {
            if (!await _context.Employees.AnyAsync(e => e.Id == dto.EmployeeId))
                return false;

            var requestedDate = dto.StartDate.Date;

            var existingLeaves = await _context.Leaves
                .Where(l => l.EmployeeId == dto.EmployeeId && l.StartDate.Date == requestedDate && l.Status != "Rejected")
                .ToListAsync();

            if (existingLeaves.Any())
            {

                return false;
            }

            var leave = new Leave
            {
                EmployeeId = dto.EmployeeId,
                LeaveType = dto.LeaveType,
                StartDate = DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(dto.EndDate, DateTimeKind.Utc),
                Reason = dto.Reason,
                Status = "Pending"
            };

            _context.Leaves.Add(leave);
            await _context.SaveChangesAsync();
            return true;
        }



        public async Task<List<object>> GetAllLeavesAsync()
        {
            return await _context.Leaves
                .Include(l => l.Employee)
                .Select(l => new
                {
                    l.Id,
                    l.EmployeeId,
                    EmployeeEmail = l.Employee != null ? l.Employee.Email : null,
                    l.LeaveType,
                    StartDate = l.StartDate.ToString("dd-MM-yyyy"),
                    EndDate = l.EndDate.ToString("dd-MM-yyyy"),
                    l.Reason,
                    l.Status
                }).ToListAsync<object>();
        }


        public async Task<List<LeaveDto>> GetPendingLeavesAsync()
        {
            return await _context.Leaves
                .Include(l => l.Employee)
                .Where(l => l.Status == "Pending")
                .Select(l => new LeaveDto
                {
                    Id = l.Id,
                    Email = l.Employee!.Email!,
                    LeaveType = l.LeaveType!,
                    StartDateFormatted = l.StartDate.ToString("dd-MM-yyyy"),
                    EndDateFormatted = l.EndDate.ToString("dd-MM-yyyy"),
                    Reason = l.Reason!,
                    Status = l.Status!
                })
                .ToListAsync();
        }



        public async Task<bool> UpdateLeaveStatusAsync(int leaveId, string status)
        {
            var leave = await _context.Leaves.FindAsync(leaveId);
            if (leave == null) return false;

            leave.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
