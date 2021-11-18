using StatusApp.Domain.Model;
using StatusApp.Domain.Model.DTOs;

namespace StatusApp.Components
{
    public partial class ServiceInformationComponent
    {
        // properties
        private ServiceInformation _service;
        public ServiceInformation Service
        {
            get => _service;
            set
            {
                this._service = value;
                OnPropertyChanged(nameof(Service));
            }
        }

        public ServiceInformationComponent()
        {
            InitializeComponent();
            this.BindingContext = this;
        }

        public ServiceInformationComponent(ServiceInformation service) : this()
        {
            this.Service = service;
        }
    }
}