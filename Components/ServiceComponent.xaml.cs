using Microsoft.Maui.Controls;
using StatusApp.Domain.Model.DTOs;

namespace StatusApp.Components
{
    public partial class ServiceComponent : ContentView
	{
		private Service _service;

		public Service Service
        {
			get => this._service;
			set
            {
				this._service = value;
				this.OnPropertyChanged(nameof(this.Service));
            }
        }

		public ServiceComponent()
		{
			InitializeComponent();
			BindingContext = this;
		}

		public ServiceComponent(Service service) : this()
        {
			this.Service = service;
        }
	}
}