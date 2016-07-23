using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessManagement.BL
{
    internal class UserSettingsManager
    {
        private const string APP_FOLDER_NAME = "Gamoholic";
        private const string APP_USERS_SETTINGS_FILE_NAME = "UsersSettings.icr";

        GamoholicSettings _currentSettings = null;
        private static UserSettingsManager _instance;

        public static UserSettingsManager Instance {
            get
            {
                if (_instance == null)
                    _instance = new UserSettingsManager();

                return _instance;
            }
        }

        private UserSettingsManager()
        {
            // this row must be set first:
            MyCurrentUserName = "defaultGamoHolicUser";

            _currentSettings = GetSettings();
        }

        public string MyCurrentUserName { get; private set; }

       public bool? GetProcessStatus(string processName)
        {
            var currentUserSettings = GetCurrentUserSettings();

            var process= currentUserSettings.MyHazardouesProcessesList.SingleOrDefault(x => x.MyProcessName.Equals(processName));

            if (process != null)
                return process.MyIsHazardous;

            return null;
        }

        public IEnumerable<ProcessSettingsItem> GetAllHazardousProcessesForCurrentUser()
        {
            var currentUserSettings = GetCurrentUserSettings();
            return currentUserSettings.MyHazardouesProcessesList.Where(x => x.MyIsHazardous != null && x.MyIsHazardous == true);
        }

        public void UpdateProcess(ProcessSettingsItem process)
        {
            var currentUserSettings = GetCurrentUserSettings();

            var existingPrc = currentUserSettings.MyHazardouesProcessesList.SingleOrDefault(x => x.MyProcessName.Equals(process.MyProcessName));

            if (existingPrc != null)
            {
                if (process.MyIsHazardous == null)
                    currentUserSettings.MyHazardouesProcessesList.Remove(existingPrc);
                else
                    existingPrc.MyIsHazardous = process.MyIsHazardous;
            }
            else if (process.MyIsHazardous!=null)
                currentUserSettings.MyHazardouesProcessesList.Add(process);

            SaveSettings(_currentSettings);
        }


        /// <summary>
        /// may return null if nothing 
        /// </summary>
        /// <returns></returns>
        private UserSettings GetCurrentUserSettings()
        {
            var currentUserSetting = _currentSettings.MyUsersSettings.SingleOrDefault(x => x.MyUserName.Equals(MyCurrentUserName));

            if (currentUserSetting == null)
            {
                currentUserSetting = new UserSettings() { MyUserName = MyCurrentUserName };
                _currentSettings.MyUsersSettings.Add(currentUserSetting);
            };

            return currentUserSetting;
        }
        /// <summary>
        /// returns a safe for use folder path (folder will be there)
        /// </summary>
        /// <returns></returns>
        public string GetSafeFolderPath()
        {
            var appdataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var SaveFolder = Path.Combine(appdataFolder, APP_FOLDER_NAME);

            if (!Directory.Exists(SaveFolder))
            {
                Directory.CreateDirectory(SaveFolder);
            }

            return SaveFolder;
        }

        /// <summary>
        /// returns a safe for use filePath (folder will be there, fill might not)
        /// </summary>
        /// <returns></returns>
        private string GetSaveFilePath()
        {
            return Path.Combine(GetSafeFolderPath(), APP_USERS_SETTINGS_FILE_NAME);
        }

        public List<ProcessSettingsItem> GetSavedHazardousList()
        {
            return GetCurrentUserSettings().MyHazardouesProcessesList;
        }

        private void SaveSettings(GamoholicSettings settings)
        {
            var jsonToSave = JsonConvert.SerializeObject(settings);
            File.WriteAllText(GetSaveFilePath(), jsonToSave);
        }

        private GamoholicSettings GetSettings()
        {
            try
            {
                var jsonToDes = File.ReadAllText(GetSaveFilePath());
                var settings = (GamoholicSettings)JsonConvert.DeserializeObject<GamoholicSettings>(jsonToDes);
                return settings;
            }
            catch
            {

            }

            GamoholicSettings defaultSettings = new GamoholicSettings();
            defaultSettings.MyUsersSettings.Add(new UserSettings()
            {
                MyUserName = MyCurrentUserName
            });

            return defaultSettings;
        }
        
    }
}
