using Common.Models;
using Common.Other;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using DataFormat = RestSharp.DataFormat;

namespace DesktopClient.UserControls.Home
{
    /// <summary>
    /// Interaction logic for UserControlDashboard.xaml
    /// </summary>
    public partial class UserControlDashboard : UserControl
    {
        private static Functions functions = new Functions();

        public UserControlDashboard()
        {
            InitializeComponent();
            ShowRemoteServiceInfo();
        }

        public void ShowRemoteServiceInfo()
        {
            try
            {
                var client = new RestClient("http://localhost:51709");
                var request = new RestRequest("api/RemoteServices/ListRemoteServices/");
                request.RequestFormat = DataFormat.Json;
                request.AddBody(new ClientModel
                {
                    Token = MainWindow.mainWindow.currentClient.Token,
                });

                var response = client.Post(request);
                RemoteServicesModel remoteServicesResult = JsonConvert.DeserializeObject<RemoteServicesModel>(response.Content.ToString());
                dataGridRemoteServices.Items.Clear();
                if (remoteServicesResult != null)
                {
                    if (remoteServicesResult.RemoteServices.Count > 0)
                    {
                        foreach (var remoteService in remoteServicesResult.RemoteServices)
                        {
                            dataGridRemoteServices.Items.Add(remoteService);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to retrieve remote services info.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnReloadRemoteServices_Click(object sender, RoutedEventArgs e)
        {
            ShowRemoteServiceInfo();
        }
    }
}
