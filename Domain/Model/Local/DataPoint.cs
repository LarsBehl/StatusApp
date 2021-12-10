using System;
using System.Net;

namespace StatusApp.Domain.Model.Local
{
    public class DataPoint : AbstractDataPoint
    {
        public override int Value => (int) this.RawValue;
        public override double RawValue { get; set; }
        public DateTime RequestedAt { get; set; }
        public HttpStatusCode ResponseCode { get; set; }

        public DataPoint(double rawValue, DateTime requestedAt, HttpStatusCode responseCode)
        {
            this.RawValue = rawValue;
            this.RequestedAt = requestedAt;
            this.ResponseCode = responseCode;
        }

        public override string[] GetDescription() => new string[] { $"Response time: {this.RawValue.ToString("F2")}ms", $"Requested at: {this.RequestedAt.ToLocalTime()}", $"Response: {this.ResponseCode}" };

        public override string GetXLabel() => this.RequestedAt.ToString("HH:mm");

        public override string GetYLabel(int scale = 1) => $"{this.Value / scale}ms";

    }
}
