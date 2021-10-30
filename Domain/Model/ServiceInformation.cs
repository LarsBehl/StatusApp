using System.Net;

namespace StatusApp.Domain.Model
{
    public class ServiceInformation
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public double ResponseTime { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
