using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatusApp.Domain.Model.DTOs
{
    public class ServiceInformationTimeseriesResponse
    {
        public string ServiceName { get; set; }
        public int ServiceId { get; set; }
        public List<TimeseriesEntry> Data { get; set; }
    }
}
