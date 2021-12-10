using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace StatusApp.Components
{
	public partial class MenuEntryComponent : ContentView
	{
		private string _menuEntryText;
		private string _menuEntryIcon;
		public event EventHandler<EventArgs> Clicked;

		public static readonly BindableProperty MenuEntryTextProperty = BindableProperty.Create(
			propertyName: nameof(MenuEntryText),
			returnType: typeof(string),
			declaringType: typeof(MenuEntryComponent),
			defaultBindingMode: BindingMode.OneWay,
			defaultValue: string.Empty,
			propertyChanged: MenuEntryTextChanged
		);

		public string MenuEntryText
		{
			get => this._menuEntryText;
			set
			{
				this._menuEntryText = value;
				this.OnPropertyChanged(nameof(this.MenuEntryText));
			}
		}

		public static readonly BindableProperty MenuEntryIconProperty = BindableProperty.Create(
			propertyName: nameof(MenuEntryIcon),
			returnType: typeof(string),
			declaringType: typeof(MenuEntryComponent),
			defaultBindingMode: BindingMode.OneWay,
			defaultValue: string.Empty,
			propertyChanged: MenuEntryIconChanged
		);

		public string MenuEntryIcon
        {
			get => this._menuEntryIcon;
			set
            {
				this._menuEntryIcon = value;
				this.OnPropertyChanged(nameof(this.MenuEntryIcon));
            }
        }

		public MenuEntryComponent()
		{
			InitializeComponent();
			this.BindingContext = this;
		}

		void OnEntryTapped(object sender, EventArgs e)
        {
			this.Clicked?.Invoke(this, e);
        }

		private static void MenuEntryTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
			MenuEntryComponent menuEntryComponent = bindable as MenuEntryComponent;
			menuEntryComponent.MenuEntryText = newValue as string;
        }

		private static void MenuEntryIconChanged(BindableObject bindable, object oldValue, object newValue)
        {
			MenuEntryComponent menuEntryComponent = bindable as MenuEntryComponent;
			menuEntryComponent.MenuEntryIcon = newValue as string;
        }
	}
}