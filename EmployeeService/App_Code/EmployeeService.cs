using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeService.Models;
using EmployeeService.Repositories;

namespace EmployeeService.App_Code
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeService()
        {
            _repository = new EmployeeRepository();
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            try
            {
                var employee = await _repository.GetEmployeeTreeByIdAsync(id);

                if (employee == null)
                {
                    return new Employee
                    {
                        ID = id,
                        Name = "Not Found",
                        Enable = false,
                        Subordinates = new List<Employee>()
                    };
                }

                return employee;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetEmployeeByIdAsync: {ex}");
                throw;
            }
        }

        public async Task EnableEmployeeAsync(int id, bool enable)
        {
            try
            {
                await _repository.EnableEmployeeAsync(id, enable);
                Console.WriteLine($"Employee {id} enable set to {enable}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in EnableEmployeeAsync: {ex}");
                throw;
            }
        }
    }
}
