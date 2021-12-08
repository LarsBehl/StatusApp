using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatusApp.Domain.Model.Local
{
    public class DataPoint
    {
        public int Value { get; set; }
        public string Unit { get; set; }
        public string Label { get; set; }

        public DataPoint(int value, string unit, string label)
        {
            this.Value = value;
            this.Unit = unit;
            this.Label = label;
        }
    }
}
