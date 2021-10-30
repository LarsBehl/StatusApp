using StatusApp.Domain.Model;

namespace StatusApp.Components
{
    public partial class ServiceComponent
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

        public ServiceComponent()
        {
            InitializeComponent();
            this.BindingContext = this;
        }

        public ServiceComponent(ServiceInformation service) : this()
        {
            this.Service = service;
        }
    }
}