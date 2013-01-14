using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Coding4Fun.Toolkit.Controls;
using MediaBrowser.Model.DTO;

namespace MediaBrowser.WindowsPhone.Controls
{
    [TemplateVisualState(Name = "PasswordHidden", GroupName = "PasswordVisibility")]
    [TemplateVisualState(Name = "PasswordShowing", GroupName = "PasswordVisibility")]
    [TemplateVisualState(Name = "ErrorHidden", GroupName = "ErrorVisibility")]
    [TemplateVisualState(Name = "ErrorShowing", GroupName = "ErrorVisibility")]
    public class ProfileBox : Control
    {
        private PasswordBox passwordBox;
        private RoundButton loginButton;
        private CheckBox checkBox;
        private bool passwordShowing;
        private bool checkboxTapped;

        public ProfileBox()
        {
            this.DefaultStyleKey = typeof (ProfileBox);
            this.Tap += ProfileBox_Tap;
            //this.LostFocus += OnLostFocus;
            if(DesignerProperties.IsInDesignTool)
            {
                Profile = new DtoUser
                {
                    Name = "Scott Lovegrove",
                    LastLoginDate = DateTime.Now.AddHours(-1)
                };
            }
        }

        private void OnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            if (VisualStateManager.GoToState(this, "PasswordHidden", true))
                passwordShowing = false;
        }

        void ProfileBox_Tap(object sender, GestureEventArgs e)
        {
            if (checkboxTapped)
            {
                checkboxTapped = false;
                return;
            }
            if (Profile.HasPassword)
            {
                if (!passwordShowing)
                {
                    if (VisualStateManager.GoToState(this, "PasswordShowing", true))
                    {
                        if (passwordBox != null)
                            passwordBox.Focus();
                        passwordShowing = true;
                    }
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
        
        public DtoUser Profile
        {
            get { return (DtoUser)GetValue(ProfileProperty); }
            set { SetValue(ProfileProperty, value); }
        }

        public static readonly DependencyProperty ProfileProperty = DependencyProperty.Register(
            "Profile",
            typeof(DtoUser),
            typeof(ProfileBox),
            new PropertyMetadata(null));

        public static readonly DependencyProperty LoginCommandProperty = DependencyProperty.Register(
            "LoginCommand", 
            typeof (ICommand), 
            typeof (ProfileBox), 
            new PropertyMetadata(default(ICommand)));

        public ICommand LoginCommand
        {
            get { return (ICommand) GetValue(LoginCommandProperty); }
            set { SetValue(LoginCommandProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            passwordBox = GetTemplateChild("passwordBox") as PasswordBox;
            if(passwordBox != null)
            {
                passwordBox.KeyUp += (sender, args) =>
                {
                    if(args.Key == Key.Enter)
                    {
                        DoLogin();
                    }
                };
            }

            loginButton = GetTemplateChild("loginButton") as RoundButton;
            if(loginButton != null)
            {
                loginButton.Click += (sender, args) => DoLogin();
            }

            checkBox = GetTemplateChild("chbxSaveUser") as CheckBox;
            if(checkBox != null)
            {
                checkBox.Tap += (sender, args) =>
                {
                    checkboxTapped = true;
                };
            }
        }

        private void DoLogin()
        {
            if (LoginCommand != null)
            {
                var loginDetails = new object[]
                                       {
                                           Profile,
                                           passwordBox.Password,
                                           checkBox.IsChecked
                                       };
                LoginCommand.Execute(loginDetails);
            }
        }
    }
}
