using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace EmployeeService.Utils
{
    public static class RetryHelper
    {
        public static async Task ExecuteWithRetryAsync(Func<Task> action, int maxRetry = 3, int delayMs = 200)
        {
            int attempt = 0;
            while (true)
            {
                try
                {
                    attempt++;
                    await action();
                    return;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"[WARN] SqlException on attempt {attempt}: {ex.Message}");
                    if (attempt >= maxRetry)
                    {
                        Console.WriteLine("[ERROR] Max retries reached, throwing exception.");
                        throw;
                    }
                    await Task.Delay(delayMs);
                }
            }
        }
    }
}
