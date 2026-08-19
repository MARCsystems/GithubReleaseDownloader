using System;
using System.Collections.Generic;
using System.Text;

namespace GithubReleaseDownloader
{
    internal class UnitCollapser
    {
        internal static string CollapseBytes(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            double cacheSize = Convert.ToDouble(bytes);
            int chosenUnit = units.Length - 1;
            for (int i = 0; i < units.Length; i++)
            {
                if (cacheSize < 1024.0)
                {
                    chosenUnit = i;
                    break;
                }
                cacheSize /= 1024.0;
            }

            return $"{(chosenUnit > 0 ? Math.Round(cacheSize, 2).ToString("0.00") : Math.Round(cacheSize, 2).ToString("0"))} {units[chosenUnit]}";
        }
    }
}
