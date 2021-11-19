using StatusApp.Domain.Model.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StatusApp.Services
{
    public interface IServicesService
    {
        Task<List<Service>> GetServicesAsync();
        Task<Service> CreateServiceAsync(string name, string url);
    }
}
