using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using EmployeeService.Models;

namespace EmployeeService.App_Code
{
    [ServiceContract]
    public interface IEmployeeService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetEmployeeById?id={id}", ResponseFormat = WebMessageFormat.Json)]
        Task<Employee> GetEmployeeByIdAsync(int id);

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "EnableEmployee?id={id}&enable={enable}", ResponseFormat = WebMessageFormat.Json)]
        Task EnableEmployeeAsync(int id, bool enable);

    }
}
