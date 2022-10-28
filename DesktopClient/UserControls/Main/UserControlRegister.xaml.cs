using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DesktopClient.UserControls.Main
{
    /// <summary>
    /// Interaction logic for UserControlRegister.xaml
    /// </summary>
    public partial class UserControlRegister : UserControl
    {
        public UserControlRegister()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            #region -Validate_Registration_Data
            string mFirstName = txtFirstName.Text;
            if (string.IsNullOrEmpty(mFirstName) || string.Equals(mFirstName = mFirstName.Trim(), ""))
            {
                MessageBox.Show("First Name is required.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (mFirstName.Length <= 1 || MainWindow.functions.IsLettersSpacesOnly(mFirstName) == false)
            {
                MessageBox.Show("First Name is invalid.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                mFirstName = MainWindow.functions.EncodeString(mFirstName);
            }

            string mLastName = txtLastName.Text;
            if (string.IsNullOrEmpty(mLastName) || string.Equals(mLastName = mLastName.Trim(), ""))
            {
                MessageBox.Show("Last Name is required.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (mLastName.Length <= 1 || MainWindow.functions.IsLettersSpacesOnly(mLastName) == false)
            {
                MessageBox.Show("Last Name is invalid.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                mLastName = MainWindow.functions.EncodeString(mLastName);
            }

            string mUsername = txtUsername.Text;
            if (string.IsNullOrEmpty(mUsername) || string.Equals(mUsername = mUsername.Trim(), ""))
            {
                MessageBox.Show("Username is required.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (mUsername.Length < 5)
            {
                MessageBox.Show("Atleast 5 characters required for username.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                mUsername = MainWindow.functions.EncodeString(mUsername);
            }

            string mEmail = txtEmailAddress.Text;
            if (string.IsNullOrEmpty(mEmail) || string.Equals(mEmail = mEmail.Trim(), ""))
            {
                MessageBox.Show("Email Address is required.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (MainWindow.functions.IsValidEmail(mEmail) == false)
            {
                MessageBox.Show("Email Address is invalid.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                mEmail = MainWindow.functions.EncodeString(mEmail);
            }

            string mPassword = txtPassword.Text;
            if (string.IsNullOrEmpty(mPassword) || string.Equals(mPassword = mPassword.Trim(), ""))
            {
                MessageBox.Show("Password is required.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (mPassword.Length < 10)
            {
                MessageBox.Show("Atleast 10 characters required for password.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                mPassword = MainWindow.functions.EncodeString(mPassword);
            }
            #endregion

            try
            {
                var registrationResult = MainWindow.mainWindow.authInterface.Register(mUsername, mEmail, mPassword, mFirstName, mLastName);
                if (registrationResult.Response.IsSuccess)
                {
                    MessageBox.Show(registrationResult.Response.Message, "", MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow.mainWindow.userControlRegister.Visibility = Visibility.Hidden;
                    MainWindow.mainWindow.userControlLogin.Visibility = Visibility.Visible;
                    MainWindow.mainWindow.userControlLogin.txtEmailAddress.Text = registrationResult.Email;
                }
                else
                {
                    MessageBox.Show(registrationResult.Response.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch(Exception)
            {
                MessageBox.Show("Failed to register the client.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lblLogin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.mainWindow.userControlRegister.Visibility = Visibility.Hidden;
            MainWindow.mainWindow.userControlLogin.Visibility = Visibility.Visible;
        }

        public void ClearRegistrationForm()
        {
            txtFirstName.Clear();
            txtLastName.Clear();
            txtUsername.Clear();
            txtEmailAddress.Clear();
            txtPassword.Clear();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Are you sure want to clear?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ClearRegistrationForm();
            }
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                ClearRegistrationForm();
            }
        }
    }
}
