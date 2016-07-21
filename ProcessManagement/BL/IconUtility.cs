using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessManagement.BL
{
    public static class IconUtility
    {
        private const string saveExtension = ".ico";

        private static Dictionary<string, string> knownPaths = new Dictionary<string, string>();

        public static string GetProcessIconPath(string processPath)
        {
            try
            {
                if (knownPaths.ContainsKey(processPath)) return knownPaths[processPath];

                var saveFolder = UserSettingsManager.Instance.GetSafeFolderPath();

                var icon = Icon.ExtractAssociatedIcon(processPath);

                if (icon != null)
                {
                    var saveName = Path.GetFileName(processPath) + saveExtension;
                    var savePath = Path.Combine(saveFolder, saveName);

                    if (!File.Exists(savePath))
                    {
                        var s = icon.ToBitmap();
                        s.Save(savePath);
                    }

                    if (!knownPaths.ContainsKey(processPath))
                    {
                        knownPaths.Add(processPath, savePath);
                    }

                    return savePath;
                }

            }
            catch
            { }

            return null;
        }
    }
}
