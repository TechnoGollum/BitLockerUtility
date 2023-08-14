using System;
using System.Diagnostics;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace CmdBitlockerService
{

    public class BitlockerService : IBitlockerService
    {
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int nCmdShow);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool IsIconic(IntPtr handle);
        public string[]? ListVolume()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                Verb = "Administartor",
                FileName = "cmd.exe",
                Arguments = "/C manage-bde.exe -status"
            };
            process.StartInfo = startInfo;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.Start();
            string q = "";
            while (!process.HasExited)
            {
                q += process.StandardOutput.ReadToEnd();
            }

            var volumesList = q.Split("\nVolume ");
            return volumesList;
        }

        public string? UnlockVolume(string password, string selectedVolume)
        {
            string? messageResponse = null;
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                Verb = "Administartor",
                FileName = "cmd.exe",
                Arguments = $"/C manage-bde.exe -unlock {selectedVolume}: -password"
            };
            process.StartInfo = startInfo;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = false;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            int processId = process.Id;
            Process processesSelected = Process.GetProcessById(processId);
            SetForegroundWindow(processesSelected.MainWindowHandle);
            Thread.Sleep(40);
            SendKeys.SendWait(password);
            SendKeys.SendWait("{ENTER}");
            SendKeys.SendWait("exit");
            SendKeys.SendWait("{ENTER}");
            while (!process.HasExited)
            {
                messageResponse += process.StandardOutput.ReadToEnd();
            }

            process.Dispose();

            return messageResponse;
        }
        public string? LockVolume(string selectedVolume)
        {
            string? messageResponse = null;
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                Verb = "Administartor",
                FileName = "cmd.exe",
                Arguments = $"/C manage-bde.exe -lock {selectedVolume}:"
            };
            process.StartInfo = startInfo;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.Start();
            while (!process.HasExited)
            {
                messageResponse += process.StandardOutput.ReadToEnd();
            }
            return messageResponse;
        }
        public string ChangeBitlockerPassword(string password, string confirmPassword, string selectedVolume)
        {

            if (selectedVolume == null)
            {
                return "No volume selected!";
            }
            if (password != confirmPassword)
            {
                return "Password and confirm password does not match!";
            }

            string? messageResponse = null;
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Maximized,
                Verb = "Administartor",
                FileName = "cmd.exe",
                Arguments = $"/C manage-bde.exe -changepassword {selectedVolume}:"
            };

            process.StartInfo = startInfo;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = false;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            int processId = process.Id;
            // Run the manage-bde command to change the password
            // process.StandardInput.WriteLine($"manage-bde -changepassword {driveLetter}");
            Process processesSelected = Process.GetProcessById(processId);
            SetForegroundWindow(processesSelected.MainWindowHandle);
            //WORKAROUD Since Write line it is not working probablly due to bitlocker security reasons. 
            Thread.Sleep(40);
            SendKeys.SendWait(password);
            SendKeys.SendWait("{ENTER}");
            Thread.Sleep(40);
            SendKeys.SendWait(confirmPassword);
            Thread.Sleep(40);
            SendKeys.SendWait("{ENTER}");
            SendKeys.SendWait("{ENTER}");
            Thread.Sleep(40);
            var response = process.StandardOutput.ReadToEnd();
            process.Dispose();

            if (Regex.IsMatch(response, "Protector|ID"))
            {
                messageResponse = "Password changed succesfully!";
            }
            if (Regex.IsMatch(response, "ERROR:"))
            {
                messageResponse = response.Split("ERROR:")[1];
            }
            return messageResponse;
        }

        public string ChangeBitlockerPin(string pin, string confirmPin, string selectedVolume)
        {

            if (selectedVolume == null)
            {
                return "No volume selected!";
            }
            if (pin != confirmPin)
            {
                return "PIN and confirm PIN does not match!";
            }

            string? messageResponse = null;
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Maximized,
                Verb = "Administartor",
                FileName = "cmd.exe",
                Arguments = $"/C manage-bde.exe -changepin {selectedVolume}:"
            };

            process.StartInfo = startInfo;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = false;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            int processId = process.Id;
            // Run the manage-bde command to change the password
            // process.StandardInput.WriteLine($"manage-bde -changepassword {driveLetter}");
            Process processesSelected = Process.GetProcessById(processId);
            SetForegroundWindow(processesSelected.MainWindowHandle);
            //WORKAROUD Since Write line it is not working probablly due to bitlocker security reasons. 
            Thread.Sleep(40);
            SendKeys.SendWait(pin);
            SendKeys.SendWait("{ENTER}");
            Thread.Sleep(40);
            SendKeys.SendWait(confirmPin);
            Thread.Sleep(40);
            SendKeys.SendWait("{ENTER}");
            SendKeys.SendWait("{ENTER}");
            Thread.Sleep(40);
            var response = process.StandardOutput.ReadToEnd();
            process.Dispose();

            if (Regex.IsMatch(response, "Protector|ID"))
            {
                messageResponse = "PIN changed succesfully!";
            }
            if (Regex.IsMatch(response, "ERROR:"))
            {
                messageResponse = response.Split("ERROR:")[1];
            }
            return messageResponse;
        }

        //Experimental
        public object UnlockWithPassphrase(string driveLetter, string passphrase)
        {
            object result = null;
            const string wmiNamespace = @"\\.\root\cimv2\Security\MicrosoftVolumeEncryption";
            var scope = new ManagementScope(string.Format(wmiNamespace));

            var query = new ObjectQuery("SELECT * FROM Win32_EncryptableVolume");
            var searcher = new ManagementObjectSearcher(scope, query);

            var allVolumes = searcher.Get();
            foreach (var o in allVolumes)
            {
                var volume = (ManagementObject)o;
                if (volume.Properties["DriveLetter"].Value.ToString() != driveLetter) continue;
                object[] methodArgs = { passphrase };
                result = volume.InvokeMethod("UnlockWithPassphrase", methodArgs);
            }
            return result;
        }

    }

}