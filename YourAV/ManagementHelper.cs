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
        //private static readonly string wmipathstr = @"\\" + Environment.MachineName + @"\root\SecurityCenter2:AntiVirusProduct";
        public static bool RemoveAllAntivirus()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmipathstr, "SELECT * FROM AntivirusProduct");
                ManagementObjectCollection instances = searcher.Get();
                foreach (ManagementObject obj in instances)
                    if (obj.GetPropertyValue("displayName").ToString() != "Windows Defender")
                        obj.Delete();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool AddAntivirus(string displayName, string instanceGuid)
        {
            try
            {

                ManagementClass avp = new ManagementClass(wmipathstr);
                ManagementObject status = avp.CreateInstance();
                status.SetPropertyValue("displayName", displayName);
                status.SetPropertyValue("instanceGuid", $"{{{instanceGuid}}}");
                status.SetPropertyValue("productUptoDate", true);
                status.SetPropertyValue("onAccessScanningEnabled", true);

                //ManagementClass avp = new ManagementClass(wmipathstr);
                //ManagementObject status = avp.CreateInstance();
                //status.SetPropertyValue("displayName", displayName);
                //status.SetPropertyValue("instanceGuid", $"{{{instanceGuid}}}");
                //status.SetPropertyValue("productState", new Random().Next());
                //status.SetPropertyValue("timestamp", DateTime.UtcNow.ToString());
                //status.SetPropertyValue("pathToSignedProductExe", $"{AppDomain.CurrentDomain.BaseDirectory}\\YourAV.exe");
                //status.SetPropertyValue("pathToSignedReportingExe", $"{AppDomain.CurrentDomain.BaseDirectory}\\YourAV.exe");
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
                return instances.Count > 0 && instances.Cast<ManagementObject>().Any(item => item.GetPropertyValue("displayName").ToString() != "Windows Defender");
            }
            catch
            {
                return false;
            }
        }
    }
}
