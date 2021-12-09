using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatusApp.Domain.Model.Local
{
    public class GraphData
    {
        public List<DataPoint> DataPoints { get; set; }
        public string Unit { get; set; }

        public GraphData(List<DataPoint> dataPoints, string unit)
        {
            this.DataPoints = dataPoints;
            this.Unit = unit;
        }
    }
}
