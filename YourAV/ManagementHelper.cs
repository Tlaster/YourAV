using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace YourAV
{
    internal static class ManagementHelper
    {
        private static readonly string wmipathstr = @"\\" + Environment.MachineName + @"\root\SecurityCenter:AntiVirusProduct";
        public static bool RemoveAllAntivirus()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmipathstr, "SELECT * FROM AntivirusProduct");
                ManagementObjectCollection instances = searcher.Get();
                foreach (ManagementObject obj in instances)
                    obj.Delete();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool AddAntivirus(string displayName, string instanceGuid, bool productUptoDate = true, bool onAccessScanningEnabled = true)
        {
            try
            {
                ManagementClass avp = new ManagementClass(wmipathstr);
                ManagementObject status = avp.CreateInstance();
                status.SetPropertyValue("displayName", displayName);
                status.SetPropertyValue("instanceGuid", instanceGuid);
                status.SetPropertyValue("productUptoDate", productUptoDate);
                status.SetPropertyValue("onAccessScanningEnabled", onAccessScanningEnabled);
                status.Put();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool IsAntivirusInstalled()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmipathstr, "SELECT * FROM AntivirusProduct");
                ManagementObjectCollection instances = searcher.Get();
                return instances.Count > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
