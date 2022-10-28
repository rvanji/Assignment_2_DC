using DesktopClient.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopClient.UserControls.Main
{
    /// <summary>
    /// Interaction logic for UserControlLogin.xaml
    /// </summary>
    public partial class UserControlLogin : UserControl
    {
        public UserControlLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            #region -Validate_Login_Data
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
                var loginResult = MainWindow.mainWindow.authInterface.Login(mEmail, mPassword);
                if (loginResult.Response.IsSuccess)
                {
                    MainWindow.mainWindow.currentClient = loginResult;
                    MainWindow.mainWindow.Hide();
                    new HomeWindow().ShowDialog();
                }
                else
                {
                    MessageBox.Show(loginResult.Response.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to login.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lblRegister_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.mainWindow.userControlLogin.Visibility = Visibility.Hidden;
            MainWindow.mainWindow.userControlRegister.Visibility = Visibility.Visible;
        }

        private void ClearLoginForm()
        {
            txtEmailAddress.Clear();
            txtPassword.Clear();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearLoginForm();
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(Visibility == Visibility.Visible)
            {
                ClearLoginForm();
            }
        }
    }
}
