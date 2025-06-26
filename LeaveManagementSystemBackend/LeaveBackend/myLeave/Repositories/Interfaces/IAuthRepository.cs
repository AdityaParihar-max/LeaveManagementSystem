
using myLeave.Dtos;
using myLeave.Models;

namespace myLeave.Interfaces
{
    public interface IAuthRepository
    {

        Task<Employee?> LoginAsync(string email, string password);
        Task<List<Employee>> GetAllAsync();
        Task<bool> RegisterAsync(EmployeeRegisterDto dto);
        Task<Employee?> GetByEmailAsync(string email);
    }
}
