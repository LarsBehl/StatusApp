using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace StatusApp.Components
{
    public partial class InputComponent : ContentView
    {
        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
            propertyName: nameof(Placeholder),
            returnType: typeof(string),
            declaringType: typeof(InputComponent),
            defaultBindingMode: BindingMode.TwoWay,
            defaultValue: string.Empty,
            propertyChanged: PlaceholderChanged
        );

        public string Placeholder { get; set; }

        public static readonly BindableProperty InputNameProperty = BindableProperty.Create(
            propertyName: nameof(InputName),
            returnType: typeof(string),
            declaringType: typeof(InputComponent),
            defaultBindingMode: BindingMode.TwoWay,
            defaultValue: string.Empty,
            propertyChanged: InputNameChanged
        );

        public string InputName { get; set; }

        public static readonly BindableProperty InputTextProperty = BindableProperty.Create(
            propertyName: nameof(InputText),
            returnType: typeof(string),
            declaringType: typeof(InputComponent),
            defaultBindingMode: BindingMode.TwoWay,
            defaultValue: string.Empty,
            propertyChanged: InputTextChanged
        );

        public string InputText { get; set; }

        public static readonly BindableProperty InputErrorProperty = BindableProperty.Create(
            propertyName: nameof(InputError),
            returnType: typeof(string),
            declaringType: typeof(InputComponent),
            defaultBindingMode: BindingMode.TwoWay,
            defaultValue: string.Empty,
            propertyChanged: InputErrorChanged
        );

        public string InputError { get; set; }

        public static readonly BindableProperty InputValidationProperty = BindableProperty.Create(
            propertyName: nameof(InputValidation),
            returnType: typeof(Func<string, bool>),
            declaringType: typeof(InputComponent),
            defaultBindingMode: BindingMode.OneWay,
            defaultValue: null,
            propertyChanged: InputValidationChanged
        );

        public Func<string, bool> InputValidation { get; set; }

        public static readonly BindableProperty InputReturnProperty = BindableProperty.Create(
            propertyName: nameof(InputReturn),
            returnType: typeof(Func<Task>),
            declaringType: typeof(InputComponent),
            defaultBindingMode: BindingMode.OneWay,
            defaultValue: null,
            propertyChanged: InputReturnChanged
        );

        public Func<Task> InputReturn { get; set; }

        public static readonly BindableProperty InputIsPasswordProperty = BindableProperty.Create(
            propertyName: nameof(InputIsPassword),
            returnType: typeof(bool),
            declaringType: typeof(InputComponent),
            defaultValue: false,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: InputIsPasswordChanged
        );

        public bool InputIsPassword { get; set; }

        public static readonly BindableProperty InputHasErrorProperty = BindableProperty.Create(
            propertyName: nameof(InputHasError),
            returnType: typeof(bool),
            declaringType: typeof(InputComponent),
            defaultValue: false,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: InputHasErrorChanged
        );

        public bool InputHasError { get; set; }
        public string InputContent { get => this.InputEntry.Text; }

        public InputComponent()
        {
            InitializeComponent();
            this.InputEntry.Unfocused += this.OnFocusOut;
            this.InputEntry.Completed += this.OnReturn;
            this.InputErrorLabel.IsVisible = false;
        }

        private static void PlaceholderChanged(BindableObject bindable, object oldValue, object newValue)
        {
            InputComponent inputComponent = (InputComponent)bindable;
            inputComponent.Placeholder = newValue.ToString();
            inputComponent.InputEntry.Placeholder = newValue.ToString();
        }

        private static void InputNameChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            InputComponent inputComponent = (InputComponent)bindableObject;
            inputComponent.InputName = newValue.ToString();
            inputComponent.InputLabel.Text = newValue.ToString();
        }

        private static void InputTextChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            InputComponent inputComponent = (InputComponent)bindableObject;
            inputComponent.InputText = newValue.ToString();
            inputComponent.InputEntry.Text = newValue.ToString();
        }

        private static void InputErrorChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            InputComponent inputComponent = (InputComponent)bindableObject;
            inputComponent.InputError = newValue.ToString();
            inputComponent.InputErrorLabel.Text = newValue.ToString();
        }

        public static void InputValidationChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            InputComponent inputComponent = (InputComponent)bindableObject;
            inputComponent.InputValidation = (Func<string, bool>)newValue;
        }

        public static void InputReturnChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            InputComponent inputComponent = (InputComponent)bindableObject;
            inputComponent.InputReturn = (Func<Task>)newValue;
        }

        public static void InputIsPasswordChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            InputComponent inputComponent = (InputComponent)bindableObject;
            inputComponent.InputIsPassword = (bool)newValue;
            inputComponent.InputEntry.IsPassword = (bool)newValue;
        }

        public static void InputHasErrorChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            InputComponent inputComponent = (InputComponent)bindableObject;
            inputComponent.InputHasError = (bool)newValue;

            if (inputComponent.InputHasError)
                ShowError(inputComponent);
            else
                HideError(inputComponent);
        }

        public void Clear()
        {
            this.InputEntry.Text = string.Empty;
            this.InputHasError = false;
        }

        void OnFocusOut(object sender, EventArgs e)
        {
            if (this.InputValidation is not null && (this.InputValidation?.Invoke(this.InputEntry.Text) ?? false))
            {
                ShowError(this);
                return;
            }

            HideError(this);
        }

        async void OnReturn(object sender, EventArgs e)
        {
            if (this.InputValidation is not null && (this.InputValidation?.Invoke(this.InputEntry.Text) ?? false))
            {
                ShowError(this);
                return;
            }

            HideError(this);

            if (this.InputReturn is not null)
                await this.InputReturn?.Invoke();
        }

        private static void ShowError(InputComponent inputComp)
        {
            inputComp.InputHasError = true;
            inputComp.InputErrorLabel.IsVisible = inputComp.InputHasError;
        }

        private static void HideError(InputComponent inputComp)
        {
            inputComp.InputHasError = false;
            inputComp.InputErrorLabel.IsVisible = inputComp.InputHasError;
        }
    }
}