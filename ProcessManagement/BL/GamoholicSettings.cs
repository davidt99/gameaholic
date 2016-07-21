using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace ProcessManagement.BL
{
    internal class ProcessSettingsItem
    {
        public string MyProcessName { get; set; }
        public bool? MyIsHazardous { get; set; }
    }

    internal class UserSettings
    {
        public UserSettings()
        {
            MyHazardouesProcessesList = new List<ProcessSettingsItem>();
        }

        public string MyUserName { get; set; }
        public List<ProcessSettingsItem> MyHazardouesProcessesList { get; set; }
    }

    internal class GamoholicSettings
    {
        public GamoholicSettings()
        {
            MyUsersSettings = new List<UserSettings>();
        }

        public List<UserSettings> MyUsersSettings { get; set; }
    }
}
