using Common.Models;
using Common.Other;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Windows;
using System.Windows.Controls;
using DataFormat = RestSharp.DataFormat;

namespace DesktopClient.UserControls.Home
{
    /// <summary>
    /// Interaction logic for UserControlHost.xaml
    /// </summary>
    public partial class UserControlHost : UserControl
    {
        private static Functions functions = new Functions();

        public UserControlHost()
        {
            InitializeComponent();
            ShowMyRemoteServiceInfo();
        }

        public void ShowMyRemoteServiceInfo()
        {
            try
            {
                var client = new RestClient("http://localhost:51709");
                var request = new RestRequest("api/RemoteServices/ListRemoteServices/");
                request.RequestFormat = DataFormat.Json;
                request.AddBody(new ClientModel
                {
                    ClientID = MainWindow.mainWindow.currentClient.ClientID,
                    Token = MainWindow.mainWindow.currentClient.Token,
                });

                var response = client.Post(request);
                RemoteServicesModel remoteServicesResult = JsonConvert.DeserializeObject<RemoteServicesModel>(response.Content.ToString());
                dataGridMyRemoteServices.Items.Clear();
                if(remoteServicesResult != null)
                {
                    if(remoteServicesResult.RemoteServices.Count > 0)
                    {
                        foreach(var remoteService in remoteServicesResult.RemoteServices)
                        {
                            dataGridMyRemoteServices.Items.Add(remoteService);
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
            ShowMyRemoteServiceInfo();
        }

        private bool ValidateRemoteServiceData(out RemoteServiceModel remoteService)
        {
            remoteService = new RemoteServiceModel() { IPAddress = txtIPAddress.Text };
            
            if (string.IsNullOrEmpty(remoteService.IPAddress) || string.Equals(remoteService.IPAddress = remoteService.IPAddress.Trim(), ""))
            {
                MessageBox.Show("IP Address is required.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (!functions.IsValidIPAddress(remoteService.IPAddress))
            {
                MessageBox.Show("IP Address is invalid.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
            {
                remoteService.IPAddress = functions.EncodeString(remoteService.IPAddress);
            }

            if (string.IsNullOrEmpty(txtPort.Text))
            {
                MessageBox.Show("Port is required.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
            {
                try
                {
                    remoteService.Port = int.Parse(txtPort.Text);
                    if(remoteService.Port < 0)
                    {
                        MessageBox.Show("Port is invalid.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
                catch(FormatException)
                {
                    MessageBox.Show("Port is invalid.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            return true;
        }

        private void btnPublish_Click(object sender, RoutedEventArgs e)
        {
            RemoteServiceModel remoteService = new RemoteServiceModel();
            bool isValidRemoteService = ValidateRemoteServiceData(out remoteService);
            if (isValidRemoteService)
            {
                try
                {
                    var client = new RestClient("http://localhost:51709/api/RemoteServices/PublishRemoteService/");
                    var request = new RestRequest();
                    request.RequestFormat = DataFormat.Json;
                    remoteService.Client = MainWindow.mainWindow.currentClient;
                    remoteService.Token = MainWindow.mainWindow.currentClient.Token;
                    request.AddBody(remoteService);
                    var response = client.Post(request);

                    RemoteServiceModel remoteServiceResult = JsonConvert.DeserializeObject<RemoteServiceModel>(response.Content.ToString());
                    if (remoteServiceResult.Response.IsSuccess)
                    {
                        MessageBox.Show(remoteServiceResult.Response.Message, "", MessageBoxButton.OK, MessageBoxImage.Information);
                        ShowMyRemoteServiceInfo();
                    }
                    else
                    {
                        MessageBox.Show(remoteServiceResult.Response.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to publish the remote service.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnUnpublish_Click(object sender, RoutedEventArgs e)
        {
            RemoteServiceModel remoteService = new RemoteServiceModel();
            bool isValidRemoteService = ValidateRemoteServiceData(out remoteService);
            if (isValidRemoteService)
            {
                try
                {
                    var client = new RestClient("http://localhost:51709/api/RemoteServices/UnpublishRemoteService/");
                    var request = new RestRequest();
                    request.RequestFormat = DataFormat.Json;
                    remoteService.Client = MainWindow.mainWindow.currentClient;
                    remoteService.Token = MainWindow.mainWindow.currentClient.Token;
                    request.AddBody(remoteService);
                    var response = client.Post(request);

                    RemoteServiceModel remoteServiceResult = JsonConvert.DeserializeObject<RemoteServiceModel>(response.Content.ToString());
                    if (remoteServiceResult.Response.IsSuccess)
                    {
                        MessageBox.Show(remoteServiceResult.Response.Message, "", MessageBoxButton.OK, MessageBoxImage.Information);
                        ShowMyRemoteServiceInfo();
                    }
                    else
                    {
                        MessageBox.Show(remoteServiceResult.Response.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to unpublish the remote service.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
