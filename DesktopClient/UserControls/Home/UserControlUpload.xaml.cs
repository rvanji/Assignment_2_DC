using System;
using System.Windows;
using System.Windows.Controls;

namespace DesktopClient.UserControls.Home
{
    /// <summary>
    /// Interaction logic for UserControlUpload.xaml
    /// </summary>
    public partial class UserControlUpload : UserControl
    {
        public UserControlUpload()
        {
            InitializeComponent();
        }

        private void btnBrowseFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "Text documents (.txt)|*.txt";
            openFileDialog.InitialDirectory = @"C:\";
            Nullable<bool> result = openFileDialog.ShowDialog();
            if (result == true)
            {
                txtPythonCode.Text = System.IO.File.ReadAllText(openFileDialog.FileName);
            }
        }
    }
}
