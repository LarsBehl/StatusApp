using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using StatusApp.Domain.Model.DTOs;
using StatusApp.Extensions;
using StatusApp.Services;

namespace StatusApp.Views
{
	public partial class ServiceDetailView : ContentPage
	{
		private static readonly string EMPTY_MSG = "No information have been querried yet";
		private static readonly string GENERIC_ERROR_MSG = "There was an issue retrieving the data";

		private readonly IServiceInformationService _serviceInformationService;
		
		private int _serviceId;
		private ServiceInformationTimeseriesResponse _timeseries;
		private bool _isLoading;
		private string _message;
		private bool _isEmpty;
		private double _avgRespnseTime;
		private string _serviceName;
		private HttpStatusCode _currentStatus;

		public ServiceInformationTimeseriesResponse Timeseries
        {
			get => this._timeseries;
			set
            {
				this._timeseries = value;
				this.OnPropertyChanged(nameof(this.Timeseries));
            }
        }

		public bool IsLoading
        {
			get => this._isLoading;
			set
            {
				this._isLoading = value;
				this.OnPropertyChanged(nameof(this.IsLoading));
            }
        }

		public string Message
        {
			get => this._message;
			set
            {
				this._message = value;
				this.OnPropertyChanged(nameof(this.Message));
            }
        }

		public bool IsEmpty
        {
			get => this._isEmpty;
			set
            {
				this._isEmpty = value;
				this.OnPropertyChanged(nameof(this.IsEmpty));
            }
        }

		public double AvgResponseTime
        {
			get => this._avgRespnseTime;
			set
            {
				this._avgRespnseTime = value;
				this.OnPropertyChanged(nameof(this.AvgResponseTime));
            }
        }

		public string ServiceName
        {
			get => this._serviceName;
			set
            {
				this._serviceName = value;
				this.OnPropertyChanged(nameof(this.ServiceName));
            }
        }

		public HttpStatusCode CurrentStatus
        {
			get => this._currentStatus;
			set
            {
				this._currentStatus = value;
				this.OnPropertyChanged(nameof(this.CurrentStatus));
            }
        }

		public ServiceDetailView(int serviceId)
		{
			InitializeComponent();
			this.BindingContext = this;
			this._serviceId = serviceId;
			this._serviceInformationService = MauiProgram.App.Services.GetRequiredService<IServiceInformationService>();
			this.IsEmpty = true;

			this.LoadTimeseries().GetAwaiter().OnCompleted(() => { });
		}

		private async Task LoadTimeseries()
        {
			this.IsLoading = true;
			this.Timeseries = await this._serviceInformationService.GetServiceTimeseriesAsync(this._serviceId);

			if (this.Timeseries is null)
			{
				this.IsEmpty = true;
				this.Message = GENERIC_ERROR_MSG;
				this.ServiceName = string.Empty;
			}
			else if (this.Timeseries.Data.IsEmtpy())
			{
				this.IsEmpty = true;
				this.Message = EMPTY_MSG;
			}
			else
				this.IsEmpty = false;

			this.ServiceName = this.Timeseries?.ServiceName;

			this.AvgResponseTime = Math.Round(this._timeseries?.Data.Average(d => d.ResponseTime) ?? 0, 2);
			this.CurrentStatus = this._timeseries?.Data.Last().StatusCode ?? HttpStatusCode.Unused;
			this.IsLoading = false;
        }
	}
}