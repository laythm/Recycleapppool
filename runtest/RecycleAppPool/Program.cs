using System.Diagnostics;
using System.Security.Principal;
using System.ServiceProcess;

namespace RecycleAppPool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (!IsRunAsAdministrator())
            {
                RestartAsAdministrator();
                return;
            }
            Console.WriteLine("Running with administrative privileges.");
            RestartService("W3SVC");
            EnsureW3svcRunning(20, 3);
            Console.WriteLine("Running with administrative done.");
            Console.ReadLine();
        }



        public static void EnsureW3svcRunning(int maxAttempts = 10, int delaySeconds = 5)
        {
            int attempt = 1;
            bool useMaxAttempts = maxAttempts > 0;

            using (ServiceController service = new ServiceController("W3SVC"))
            {
                while (service.Status != ServiceControllerStatus.Running)
                {
                    try
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Attempt {attempt}: W3SVC status is {service.Status}");

                        if (service.Status == ServiceControllerStatus.Stopped ||
                            service.Status == ServiceControllerStatus.Paused)
                        {
                            Console.WriteLine("Trying to start W3SVC...");
                            service.Start();
                        }

                        // Wait for it to be running (timeout after 30s)
                        service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(5));

                        if (service.Status == ServiceControllerStatus.Running)
                        {
                            Console.WriteLine("✅ W3SVC started successfully.");
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Error starting W3SVC: {ex.Message}");
                    }

                    if (useMaxAttempts && attempt >= maxAttempts)
                    {
                        Console.WriteLine("❗ Max retry attempts reached. Stopping retry loop.");
                        break;
                    }

                    attempt++;
                    Thread.Sleep(delaySeconds * 1000);
                    service.Refresh(); // Refresh to get latest status
                }
            }
        }

        private static void RestartService(string serviceName)
        {
            //IL_0001: Unknown result type (might be due to invalid IL or missing references)
            //IL_0007: Expected O, but got Unknown
            //IL_0008: Unknown result type (might be due to invalid IL or missing references)
            //IL_000e: Invalid comparison between Unknown and I4
            ServiceController val = new ServiceController(serviceName);
            if ((int)val.Status != 1)
            {
                val.Stop();
                val.WaitForStatus((ServiceControllerStatus)1, TimeSpan.FromSeconds(10.0));
            }
            val.Start();
            val.WaitForStatus((ServiceControllerStatus)4, TimeSpan.FromSeconds(10.0));
        }

        private static bool IsRunAsAdministrator()
        {
            try
            {
                return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        private static void RestartAsAdministrator()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = Process.GetCurrentProcess().MainModule.FileName,
                Arguments = Environment.CommandLine.Substring(Environment.CommandLine.IndexOf(' ') + 1),
                UseShellExecute = true,
                Verb = "runas"
            };
            try
            {
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }
}
