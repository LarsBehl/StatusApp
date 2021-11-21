namespace StatusApp.Domain.Model.DTOs
{
    public class ServiceConfigurationRequest
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public ServiceConfigurationRequest(string name, string url)
        {
            this.Name = name;
            this.Url = url;
        }
    }
}
