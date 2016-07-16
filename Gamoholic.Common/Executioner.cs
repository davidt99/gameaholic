using Gamoholic.Common.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gamoholic.Common
{
    public class Executioner
    {
        private ProcessManagment _processManagment;
        public Executioner(ProcessManagment processManagment)
        {
            _processManagment = processManagment;
        }

        public void Start()
        {
            while (true)
            {
                CheckProcess();
                Thread.Sleep(1000 * 5);
            }
        }

        private void CheckProcess()
        {
            var hazaroudesProcesses = this._processManagment.getHazardouesProcess();
            var systemProcesses = Process.GetProcesses();
            foreach (var hazardousProcess in hazaroudesProcesses)
            {
                var systemProcess = systemProcesses.FirstOrDefault(p => p.ProcessName == hazardousProcess.Name);
                if (systemProcess != null)
                {
                    if (IsProcessShouldBeKilled(hazardousProcess))
                    {
                        KillProcessTree(systemProcess);
                    }
                }
            }
        }

        private void KillProcessTree(Process systemProcess)
        {
            ManagementObjectSearcher searcher = 
                new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + systemProcess.Id);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                KillProcessTree(Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])));
            }

            KillProcess(systemProcess);
        }

        private void KillProcess(Process systemProcess)
        {
            try
            {
                var forceKill = !systemProcess.CloseMainWindow();
                if (!forceKill)
                {
                    forceKill = !systemProcess.WaitForExit(1000);
                }

                if (forceKill)
                {
                    systemProcess.Refresh();
                    systemProcess.Kill();
                    var hasBeenKilled = systemProcess.WaitForExit(1000);
                    if (hasBeenKilled)
                    {
                        systemProcess.Close();
                    }
                }
            }
            catch (Win32Exception)
            {
                // TODO: take extreme measures when couldn't kill. Try win32 api or taskkill /f
            }
            catch (InvalidOperationException)
            {
                // Process already dead, do nothing
            }
        }

        private bool IsProcessShouldBeKilled(HazardousProcess processes)
        {
            return true;
        }

        
    }

    public class ProcessManagment
    {
        public List<HazardousProcess> getHazardouesProcess()
        {
            return new List<HazardousProcess>() { new HazardousProcess() { Name = "notepad" } };
        }
    }

}
