using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;

namespace YourAV
{
    internal static class ManagementHelper
    {
        private static readonly string wmipathstr = @"\\" + Environment.MachineName + @"\root\SecurityCenter:AntiVirusProduct";
        //private static readonly string wmipathstr2 = @"\\" + Environment.MachineName + @"\root\SecurityCenter2:AntiVirusProduct";
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
        //public static bool RemoveAllAntivirus2()
        //{
        //    try
        //    {
        //        ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmipathstr2, "SELECT * FROM AntivirusProduct");
        //        ManagementObjectCollection instances = searcher.Get();
        //        foreach (ManagementObject obj in instances)
        //            if (obj.GetPropertyValue("displayName").ToString() != "Windows Defender")
        //                obj.Delete();
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
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

                status.Put();
                return true;
            }
            catch
            {
                return false;
            }
        }
        //public static bool AddAntivirus2(string displayName, string instanceGuid)
        //{
        //    try
        //    {

        //        ManagementClass avp = new ManagementClass(wmipathstr2);
        //        ManagementObject status = avp.CreateInstance();
        //        status.SetPropertyValue("displayName", displayName);
        //        status.SetPropertyValue("instanceGuid", $"{{{instanceGuid}}}");
        //        status.SetPropertyValue("productState", new Random().Next());
        //        status.SetPropertyValue("timestamp", DateTime.UtcNow.ToString());
        //        status.SetPropertyValue("pathToSignedProductExe", $"{AppDomain.CurrentDomain.BaseDirectory}YourAV.exe");
        //        status.SetPropertyValue("pathToSignedReportingExe", $"{AppDomain.CurrentDomain.BaseDirectory}YourAV.exe");

        //        status.Put();
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
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
        public static bool RestartService(string serviceName)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                List<ServiceController> dependencies = new List<ServiceController>();
                if ((service.Status.Equals(ServiceControllerStatus.Running)) || (service.Status.Equals(ServiceControllerStatus.StartPending)))
                {
                    FillDependencyTreeLeaves(service, dependencies);
                    service.Stop();
                }
                service.WaitForStatus(ServiceControllerStatus.Stopped);

                foreach (ServiceController dependency in dependencies)
                {
                    dependency.Start();
                    dependency.WaitForStatus(ServiceControllerStatus.Running);
                }


                return true;
            }
            catch
            {
                return false;
            }
        }

        //public static bool RestartService(string serviceName, int timeoutMilliseconds)
        //{
        //    ServiceController service = new ServiceController(serviceName);
        //    try
        //    {
        //        int millisec1 = Environment.TickCount;
        //        TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
        //        List<ServiceController> dependencies = new List<ServiceController>();
        //        if ((service.Status.Equals(ServiceControllerStatus.Running)) || (service.Status.Equals(ServiceControllerStatus.StartPending)))
        //        {
        //            FillDependencyTreeLeaves(service, dependencies);
        //            service.Stop();
        //        }
        //        service.WaitForStatus(ServiceControllerStatus.Stopped);
        //        service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

        //        foreach (ServiceController dependency in dependencies)
        //        {
        //            int millisec2 = Environment.TickCount; dependency.Start();
        //            timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));
        //            dependency.Start();
        //            service.WaitForStatus(ServiceControllerStatus.Running, timeout);
        //        }

        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
        private static void FillDependencyTreeLeaves(ServiceController controller, List<ServiceController> controllers)
        {
            bool dependencyAdded = false;
            foreach (ServiceController dependency in controller.DependentServices)
            {
                ServiceControllerStatus status = dependency.Status;
                // add only those that are actually running
                if (status != ServiceControllerStatus.Stopped && status != ServiceControllerStatus.StopPending)
                {
                    dependencyAdded = true;
                    FillDependencyTreeLeaves(dependency, controllers);
                }
            }
            // if no dependency has been added, the service is dependency tree's leaf
            if (!dependencyAdded && !controllers.Contains(controller))
            {
                controllers.Add(controller);
            }
        }
    }
}
