using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Dto;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace MediaBrowser.Windows8.Controls
{
    [TemplateVisualState(Name="PasswordHidden", GroupName="PasswordVisibility")]
    [TemplateVisualState(Name = "PasswordShowing", GroupName = "PasswordVisibility")]
    [TemplateVisualState(Name="ErrorHidden", GroupName="ErrorVisibility")]
    [TemplateVisualState(Name = "ErrorShowing", GroupName = "ErrorVisibility")]
    public sealed class ProfileBox : Control
    {
        private PasswordBox passwordBox;
        private Button loginButton;
        private CheckBox checkBox;
        private bool checkBoxTapped;
        public ProfileBox()
        {
            DefaultStyleKey = typeof(ProfileBox);
            Tapped += ProfileBox_Tapped;
            LostFocus += ProfileBox_LostFocus;

            Messenger.Default.Register<NotificationMessage>(this, async m=>
            {
                if(m.Notification.Equals(Constants.ErrorLoggingInMsg))
                {
                    Guid userId = (Guid)m.Sender;
                    if(userId == Profile.Id)
                    {
                        // Display error
                        VisualStateManager.GoToState(this, "ErrorShowing", true);
                    }
                }
            });

            if(ViewModelBase.IsInDesignModeStatic)
            {
                Profile = new UserDto
                {
                    Name = "Scott Lovegrove",
                    LastLoginDate = DateTime.Now.AddHours(-1)
                };
            }
        }

        void ProfileBox_LostFocus(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PasswordHidden", true);
        }

        async void ProfileBox_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (checkBoxTapped)
            {
                checkBoxTapped = false;
                return;
            }
            if (SuppressLogin) return;
            if (Profile == default(UserDto)) return;
            if (Profile.HasPassword)
            {
                // Show password box
                VisualStateManager.GoToState(this, "PasswordShowing", true);
                if(passwordBox != null)
                {
                    passwordBox.Focus(FocusState.Keyboard);
                }
            }
            else
            {
                DoLogin();
            }
        }

        public string Password
        {
            get { return GetValue(PasswordProperty) as string; }
            set { SetValue(PasswordProperty, value); }
        }

        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(
            "Password",
            typeof(string),
            typeof(ProfileBox),
            new PropertyMetadata(string.Empty));
        
        public UserDto Profile
        {
            get { return (UserDto)GetValue(ProfileProperty); }
            set { SetValue(ProfileProperty, value); }
        }

        public static readonly DependencyProperty ProfileProperty = DependencyProperty.Register(
            "Profile",
            typeof(UserDto),
            typeof(ProfileBox),
            new PropertyMetadata(null));

        public static readonly DependencyProperty SuppressLoginProperty = DependencyProperty.Register(
            "SuppressLogin", 
            typeof (bool), 
            typeof (ProfileBox), 
            new PropertyMetadata(false));

        public bool SuppressLogin
        {
            get { return (bool) GetValue(SuppressLoginProperty); }
            set { SetValue(SuppressLoginProperty, value); }
        }

        public static readonly DependencyProperty TapSignsOutProperty = DependencyProperty.Register(
            "TapSignsOut", 
            typeof (bool), 
            typeof (ProfileBox), 
            new PropertyMetadata(default(bool)));

        public bool TapSignsOut
        {
            get { return (bool) GetValue(TapSignsOutProperty); }
            set { SetValue(TapSignsOutProperty, value); }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            passwordBox = GetTemplateChild("passwordBox") as PasswordBox;

            if(passwordBox != null)
            {
                passwordBox.KeyUp += (sender, args) =>
                {
                    if (args.Key == VirtualKey.Enter)
                    {
                        DoLogin();
                    }
                };
            }

            loginButton = GetTemplateChild("loginButton") as Button;
            if (loginButton != null)
                loginButton.Tapped +=
                    (sender, args) =>
                    DoLogin();

            checkBox = GetTemplateChild("chbxSaveUser") as CheckBox;
            if(checkBox != null)
            {
                checkBox.Tapped += (sender, args) =>
                {
                    checkBoxTapped = true;
                };
            }
        }

        private void DoLogin()
        {
            VisualStateManager.GoToState(this, "ErrorHidden", true);
            var loginObjects = new object[]
                                       {
                                           Profile,
                                           passwordBox.Password,
                                           checkBox.IsChecked
                                       };
            Messenger.Default.Send(new NotificationMessage(loginObjects, Constants.DoLoginMsg));
        }
    }
}
