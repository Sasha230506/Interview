using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using EmployeeService.Models;
using EmployeeService.Utils;

namespace EmployeeService.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["EmployeeDb"]?.ConnectionString;

            if (_connectionString == null)
                throw new InvalidOperationException("Connection string 'EmployeeDb' not found.");
        }

        public async Task<Employee> GetEmployeeTreeByIdAsync(int id)
        {
            Console.WriteLine($"[INFO] GetEmployeeTreeByIdAsync called with ID={id}");

            if (id <= 0) throw new ArgumentException("Employee ID must be positive.", nameof(id));

            var employees = new Dictionary<int, Employee>();
            Employee rootEmployee = null;

            await RetryHelper.ExecuteWithRetryAsync(async () =>
            {
                try
                {
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        Console.WriteLine("[INFO] SQL connection opened.");

                        string query = @"
                        WITH EmployeeCTE AS
                        (
                            SELECT ID, Name, ManagerID, Enable
                            FROM Employee
                            WHERE ID = @RootID
                            UNION ALL
                            SELECT e.ID, e.Name, e.ManagerID, e.Enable
                            FROM Employee e
                            INNER JOIN EmployeeCTE cte ON e.ManagerID = cte.ID
                        )
                        SELECT * FROM EmployeeCTE
                    ";

                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.Add("@RootID", SqlDbType.Int).Value = id;

                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var emp = new Employee
                                    {
                                        ID = reader.GetInt32(reader.GetOrdinal("ID")),
                                        Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? string.Empty : reader.GetString(reader.GetOrdinal("Name")),
                                        Enable = reader.GetBoolean(reader.GetOrdinal("Enable"))
                                    };

                                    int? managerId = reader.IsDBNull(reader.GetOrdinal("ManagerID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ManagerID"));

                                    employees[emp.ID] = emp;

                                    if (managerId.HasValue)
                                    {
                                        if (!employees.ContainsKey(managerId.Value))
                                        {
                                            employees[managerId.Value] = new Employee
                                            {
                                                ID = managerId.Value,
                                                Subordinates = new List<Employee>()
                                            };
                                        }
                                        employees[managerId.Value].Subordinates.Add(emp);
                                    }
                                    else
                                    {
                                        rootEmployee = emp;
                                    }
                                }

                                Console.WriteLine($"[INFO] Loaded {employees.Count} employees from database.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Exception in GetEmployeeTreeByIdAsync: {ex.Message}");
                    throw;
                }
            });

            if (rootEmployee == null && employees.ContainsKey(id))
                rootEmployee = employees[id];

            if (rootEmployee == null)
                Console.WriteLine($"[WARN] Employee with ID={id} not found.");

            return rootEmployee;
        }

        public async Task EnableEmployeeAsync(int id, bool enable)
        {
            Console.WriteLine($"[INFO] EnableEmployeeAsync called with ID={id}, enable={enable}");

            if (id <= 0) throw new ArgumentException("Employee ID must be positive.", nameof(id));

            await RetryHelper.ExecuteWithRetryAsync(async () =>
            {
                try
                {
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        using (var command = new SqlCommand("UPDATE Employee SET Enable=@Enable WHERE ID=@ID", connection))
                        {
                            command.Parameters.Add("@Enable", SqlDbType.Bit).Value = enable;
                            command.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                            int affected = await command.ExecuteNonQueryAsync();
                            Console.WriteLine($"[INFO] Updated Enable for Employee ID={id}, rows affected={affected}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Exception in EnableEmployeeAsync: {ex.Message}");
                    throw;
                }
            });
        }
    }
}
