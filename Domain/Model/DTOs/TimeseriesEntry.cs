using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StatusApp.Domain.Model.DTOs
{
    public class TimeseriesEntry
    {
        public double ResponseTime { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}
