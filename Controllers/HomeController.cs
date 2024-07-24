using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Web;
using System.Web.Mvc;
using WindowsService.Models;

namespace WindowsService.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var services = GetServicesData();
            return View(services);
          
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        #region Windows Services Tool

        public ActionResult WindowsServices()
        {

            var services = GetServicesData();
            return View(services);
        }

        //public List<Service> GetServicesData()
        //{
        //    ServiceController[] services = ServiceController.GetServices();
        //    List<Service> servicesList = new List<Service>();

        //    foreach (ServiceController service in services)
        //    {
        //        if (service.ServiceName.ToLower().Contains("sn5"))
        //        {
        //            servicesList.Add(new Service { ServiceName = service.ServiceName, DisplayName = service.DisplayName, StartName = service.MachineName, Status = service.Status.ToString(), Actions = "" });
        //            Console.WriteLine("Service Name :" + service.ServiceName);
        //            Console.WriteLine("Service Status :" + service.Status);
        //        }
        //        else
        //        {
        //            Console.WriteLine("Service Name :" + service.DisplayName);
        //          //  Console.WriteLine("Service Name :" + service.ServiceName);
        //            Console.WriteLine("Service Status :" + service.Status);
        //        }
        //    }

        //    return servicesList;
        //}

        public List<Service> GetServicesData()
        {
            ServiceController[] services = ServiceController.GetServices();
            List<Service> servicesList = new List<Service>();

            foreach (ServiceController service in services)
            {
                // Log the service name and status for debugging
                Console.WriteLine("Service Name :" + service.ServiceName);
                Console.WriteLine("Service Display Name :" + service.DisplayName);
                Console.WriteLine("Service Status :" + service.Status);

                // Adding all services to the list regardless of the name filter
                servicesList.Add(new Service
                {
                    ServiceName = service.ServiceName,
                    DisplayName = service.DisplayName,
                    StartName = service.MachineName,
                    Status = service.Status.ToString(),
                    Actions = ""
                });
            }

            // Log the final count of services added to the list
            Console.WriteLine("Total Services Count: " + servicesList.Count);

            return servicesList;
        }


        [HttpPost]
        public ActionResult StartService(string serviceName)
        {
            try
            {
                if (string.IsNullOrEmpty(serviceName))
                {
                    ModelState.AddModelError("ServiceName", "Service name is required.");
                    return Content("Service name is required.", "text/plain");
                }

                try
                {
                    ServiceController service = new ServiceController(serviceName);
                    if (service.Status == ServiceControllerStatus.Stopped || service.Status == ServiceControllerStatus.StopPending)
                    {
                        service.Start();
                        service.WaitForStatus(ServiceControllerStatus.Running);
                    }
                    return Content($"Service '{serviceName}' started successfully.", "text/plain");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred while starting service '{serviceName}': {ex.Message}");
                    return Content($"An error occurred while starting service '{serviceName}': {ex.Message}", "text/plain");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelState.AddModelError("", $"An error occurred while starting service '{serviceName}': {ex.Message}");
                return Content($"An error occurred while starting service '{serviceName}': {ex.Message}", "text/plain");
            }
        }

        [HttpPost]
        public ActionResult StopService(string serviceName)
        {
            try
            {
                //return View(serviceName);
                if (string.IsNullOrEmpty(serviceName))
                {
                    ModelState.AddModelError("ServiceName", "Service name is required.");
                    return Content("Service name is required.", "text/plain");
                }

                try
                {
                    ServiceController service = new ServiceController(serviceName);
                    if (service.Status == ServiceControllerStatus.Running || service.Status == ServiceControllerStatus.StartPending)
                    {
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped);
                    }
                    TempData["Error"] = $"Service '{serviceName}' stopped successfully.";
                    return Content($"Service '{serviceName}' stopped successfully.", "text/plain");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred while stopping service '{serviceName}': {ex.Message}");
                    TempData["Error"] = $"An error occurred while stopping service '{serviceName}': {ex.Message}";
                    return Content($"An error occurred while stopping service '{serviceName}': {ex.Message}", "text/plain");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelState.AddModelError("", $"An error occurred while stopping service '{serviceName}': {ex.Message}");
                return Content($"An error occurred while stopping service '{serviceName}': {ex.Message}", "text/plain");

            }

        }


        [HttpPost]
        public ActionResult RestartService(string serviceName, int timeoutMilliseconds)
        {
            try
            {
                //return View(serviceName);

                if (string.IsNullOrEmpty(serviceName))
                {
                    ModelState.AddModelError("ServiceName", "Service name is required.");
                    return Content("Service name is required.", "text/plain");
                }

                try
                {
                    ServiceController service = new ServiceController(serviceName);
                    int millisec1 = Environment.TickCount;
                    TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                    // Stop the service
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                    // Calculate remaining timeout
                    int millisec2 = Environment.TickCount;
                    timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                    // Start the service
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                    return Content($"Service '{serviceName}' Restarted successfully.", "text/plain");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred while starting service '{serviceName}': {ex.Message}");
                    return Content($"An error occurred while Restarting service '{serviceName}': {ex.Message}", "text/plain");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelState.AddModelError("", $"An error occurred while starting service '{serviceName}': {ex.Message}");
                return Content($"An error occurred while Restarting service '{serviceName}': {ex.Message}", "text/plain");
            }
        }

        #endregion

    }
}