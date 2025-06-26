using myLeave.Models;
using myLeave.Dtos;

namespace myLeave.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Employee?> LoginAsync(string email, string password);
        Task<List<Employee>> GetAllUsersAsync();
        Task<bool> RegisterAsync(EmployeeRegisterDto dto);
        Employee? Authenticate(string email, string password);


    }
}
