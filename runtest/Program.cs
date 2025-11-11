using System.Diagnostics;

namespace runtest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            string exePath = @"d:\RecycleAppPool\RecycleAppPool.exe";

            // Create a new process start info
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = "", // Add any arguments here if needed
                UseShellExecute = false, // Set to false if you want to redirect output
                RedirectStandardOutput = false, // Redirect output if UseShellExecute is false
                RedirectStandardError = false, // Redirect error if UseShellExecute is false
                CreateNoWindow = false // Set to true if you don't want a new window
            };

            // Start the process
            Process process = Process.Start(startInfo);



        }
    }
}
