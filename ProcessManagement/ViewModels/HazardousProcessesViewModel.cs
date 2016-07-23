using ProcessManagement.BL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ProcessManagement.ViewModels
{
    internal class HazardousProcessesViewModel : BindableBase
    {
        Timer _timer;

        private bool _showUnknown = true;
        private bool _showHazardous = true;
        private bool _showSafe = true;

        public HazardousProcessesViewModel()
        {
            _timer = new Timer(ProcessManagmentSettings.Default.ProcessFetchIntervalInSeconds*1000);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Enabled = true;
            RefreshList();
        }

        private void RefreshList()
        {
            var currentlyRunning = Process.GetProcesses().ToList();
            List<HProcess> result = new List<HProcess>();


            foreach (var prc in currentlyRunning)
            {
                string processName = prc.ProcessName;
                
                if (!result.Any(x=>x.MyProcessName.Equals(prc.ProcessName)))
                {
                    bool? processStatus = UserSettingsManager.Instance.GetProcessStatus(processName);

                    if (
                            (processStatus == null && _showUnknown) ||
                            (processStatus!=null && (
                                                        (processStatus.Value && _showHazardous) ||
                                                        (!processStatus.Value && _showSafe)
                                                    )
                            )
                        )
                    {
                        var newHPrc = new HProcess(new ProcessSettingsItem()
                        {
                            MyProcessName = processName,
                            MyIsHazardous = processStatus
                        },
                        prc);
                        result.Add(newHPrc);
                    }
                }
            }

            MyProcessList = result;
        }

        

        public void FilterList(bool showUnknown, bool showHazardous, bool showSafe)
        {
            _showUnknown = showUnknown;
            _showHazardous = showHazardous;
            _showSafe = showSafe;
            RefreshList();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RefreshList();
        }

        List<HProcess> _processes;
        public List<HProcess> MyProcessList
        {
            get
            {
                return _processes;
            }

            set
            {
                _processes = value;
                RaisePropertyChanged(() => MyProcessList);
            }
        }
    }
}
