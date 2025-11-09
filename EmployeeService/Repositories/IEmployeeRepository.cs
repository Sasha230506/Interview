using System.Threading.Tasks;
using EmployeeService.Models;

namespace EmployeeService.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetEmployeeTreeByIdAsync(int id);
        Task<int> EnableEmployeeAsync(int id, bool enable);
    }
}
