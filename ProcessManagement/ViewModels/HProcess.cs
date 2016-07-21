using ProcessManagement.BL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessManagement.ViewModels
{
    internal class HProcess : BindableBase
    {
        private ProcessSettingsItem _savedSettings;
        private const string UNKNOWN_ICON_PATH = @"\Resources\UnknownIcon.png";
        private const string HAZARD_ICON_PATH = @"\Resources\Hazardous.png";
        private const string SAFE_ICON_PATH = @"\Resources\Safe.png";
        private Process _p;


        public HProcess(ProcessSettingsItem settings,Process p)
        {
            _savedSettings = settings;
            _p = p;
        }

        public string MyProcessName
        {
            get
            {
                return _savedSettings.MyProcessName;
            }
        }

        public string IconPath
        {
            get
            {
                try
                {
                    return IconUtility.GetProcessIconPath(_p.MainModule.FileName);
                }
                catch
                {
                    return UNKNOWN_ICON_PATH;
                }

            }
        }

        public bool? MyIsHazardous
        {
            get
            {
                return _savedSettings.MyIsHazardous;
            }
            set
            {
                _savedSettings.MyIsHazardous = value;
                UserSettingsManager.Instance.UpdateProcess(_savedSettings);
                RaisePropertyChanged(() => MyIsHazardous);
                RaisePropertyChanged(() => StatusIconPath);
            }
        }

        public string StatusIconPath
        {
            get
            {
                if (_savedSettings.MyIsHazardous == null)
                    return UNKNOWN_ICON_PATH;
                else
                {
                    if (_savedSettings.MyIsHazardous == true)
                        return HAZARD_ICON_PATH;
                    if (_savedSettings.MyIsHazardous == false)
                        return SAFE_ICON_PATH;
                }

                return UNKNOWN_ICON_PATH;
            }
        }
    }

}
