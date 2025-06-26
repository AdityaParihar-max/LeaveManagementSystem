using myLeave.Dtos;

namespace myLeave.Services.Interfaces
{
    public interface ILeaveService
    {
        Task<List<LeaveDto>> GetLeavesByEmailAsync(string email);
        Task<List<LeaveDto>> GetPendingLeavesAsync();

        Task<List<object>> GetAllLeavesAsync();

        Task<bool> UpdateLeaveStatusAsync(int leaveid, string status);
        Task<bool> ApplyLeaveAsync(LeaveCreateDto dto);

    }
}
