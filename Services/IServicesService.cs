using StatusApp.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StatusApp.Services
{
    public interface IServicesService
    {
        /// <summary>
        /// Get List of Services from backend
        /// </summary>
        /// <returns>List of services if response was received from server, null otherwise</returns>
        Task<List<ServiceInformation>> GetServiceInformation();
    }
}
