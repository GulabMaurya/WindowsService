using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WindowsService.Models
{
    public class Service : IEnumerable<Service>
    {
        public string ServiceName { get; set; }
        public string DisplayName { get; set; }
        public string StartName { get; set; }
        public string Status { get; set; }
        public string Actions { get; set; }

        public int timeoutMilliseconds { get; set; }



        private List<Service> _relatedServices;

        public Service()
        {
            _relatedServices = new List<Service>();
        }

        public IEnumerator<Service> GetEnumerator()
        {
            return _relatedServices.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddRelatedService(Service Service)
        {
            _relatedServices.Add(Service);
        }
    }

}
