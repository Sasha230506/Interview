using System;
using System.Net;
using System.ServiceModel.Web;
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
                    WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
                    WebOperationContext.Current.OutgoingResponse.StatusDescription = $"Employee with ID={id} not found.";

                    return null;
                }

                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.OK;
                WebOperationContext.Current.OutgoingResponse.StatusDescription = $"Employee with ID={id} retrieved successfully.";
                return employee;
            }
            catch (ArgumentException ex)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                WebOperationContext.Current.OutgoingResponse.StatusDescription = ex.Message;
                return null;
            }
            catch (Exception ex)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                WebOperationContext.Current.OutgoingResponse.StatusDescription = ex.Message;
                Console.WriteLine($"Error in GetEmployeeByIdAsync: {ex}");
                return null;
            }
        }

        public async Task EnableEmployeeAsync(int id, bool enable)
        {
            try
            {
                var affected = await _repository.EnableEmployeeAsync(id, enable);

                if (affected == 0)
                {
                    WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
                    WebOperationContext.Current.OutgoingResponse.StatusDescription = $"Employee with ID={id} not found.";
                    return;
                }

                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.OK;
                WebOperationContext.Current.OutgoingResponse.StatusDescription = $"Employee {id} updated successfully (Enable={enable}).";
            }
            catch (Exception ex)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                WebOperationContext.Current.OutgoingResponse.StatusDescription = ex.Message;
                Console.WriteLine($"Error in EnableEmployeeAsync: {ex}");
            }
        }
    }
}
