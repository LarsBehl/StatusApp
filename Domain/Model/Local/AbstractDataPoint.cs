using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatusApp.Domain.Model.Local
{
    public abstract class AbstractDataPoint
    {
        public abstract int Value { get; }
        public abstract double RawValue { get; set; }

        public abstract string GetXLabel();
        public abstract string GetYLabel(int scale = 1);
        public abstract string[] GetDescription();
    }
}
