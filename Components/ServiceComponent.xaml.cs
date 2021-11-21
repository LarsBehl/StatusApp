using Microsoft.Maui.Controls;
using StatusApp.Domain.Model.DTOs;
using System;

namespace StatusApp.Components
{
    public partial class ServiceComponent : ContentView
	{
		private Service _service;

		public event EventHandler<int> OnMore;

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

		// TODO implement
		public void OnMoreClicked(object sender, EventArgs e)
        {
			this.OnMore?.Invoke(this, this.Service.Id);
        }
	}
}