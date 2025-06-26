namespace myLeave.Interfaces
{
    public interface ILeaveRepository
    {
        Task<bool> ApplyAsync(Leave leave);
        Task<List<Leave>> GetByEmployeeIdAsync(int employeeId);
        Task<List<Leave>> GetPendingAsync();
        Task<List<Leave>> GetAllWithEmployeeAsync();
        Task<Leave?> GetByIdAsync(int id);
        Task<bool> UpdateStatusAsync(Leave leave, string status);
        Task<List<Leave>> GetLeavesByEmailAsync(string email);

    }
}
