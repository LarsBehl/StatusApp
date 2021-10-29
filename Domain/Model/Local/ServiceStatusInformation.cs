using System.Net;

namespace StatusApp.Domain.Model.Local
{
    public class ServiceStatusInformation
    {
        public double ResponseTime { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
