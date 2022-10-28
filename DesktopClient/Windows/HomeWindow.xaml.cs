using Common.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DataFormat = RestSharp.DataFormat;

namespace DesktopClient.Windows
{
    /// <summary>
    /// Interaction logic for HomeWindow.xaml
    /// </summary>
    public partial class HomeWindow : Window
    {
        private bool IsDirectToLogin = false;
        private static Thread networkThread, serverThread;

        public HomeWindow()
        {
            InitializeComponent();
            lblCurrentUser.Content = MainWindow.mainWindow.currentClient.FirstName + " " + MainWindow.mainWindow.currentClient.LastName;
            StartNetworkThread();
        }

        private void StartNetworkThread()
        {
            if(networkThread == null)
            {
                networkThread = new Thread(() =>
                {
                    while(true)
                    {
                        try
                        {
                            var client = new RestClient("http://localhost:51709");
                            var request = new RestRequest("api/Client/RetrieveOnlineClients/");
                            request.RequestFormat = DataFormat.Json;
                            request.AddBody(new ClientModel
                            {
                                ClientID = MainWindow.mainWindow.currentClient.ClientID,
                                Token = MainWindow.mainWindow.currentClient.Token,
                            });

                            var response = client.Post(request);
                            ClientsModel clientsResult = JsonConvert.DeserializeObject<ClientsModel>(response.Content.ToString());
                            if(clientsResult.Response.IsSuccess)
                            {
                                foreach(var client1 in clientsResult.ClientsList)
                                {
                                    try
                                    {
                                        var request2 = new RestRequest("api/Job/RetrieveClientJobs/");
                                        request2.RequestFormat = DataFormat.Json;
                                        request2.AddBody(new ClientModel
                                        {
                                            ClientID = MainWindow.mainWindow.currentClient.ClientID,
                                            Token = MainWindow.mainWindow.currentClient.Token,
                                        });

                                        var response2 = client.Post(request2);
                                        JobsModel jobsResult = JsonConvert.DeserializeObject<JobsModel>(response.Content.ToString());
                                        /*if (jobsResult.Response.IsSuccess)
                                        {
                                            foreach (var job in jobsResult.JobsList)
                                            {

                                            }
                                        }*/
                                    }
                                    catch(Exception ex)
                                    {
                                        MessageBox.Show("Failed to retrieve job information.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                            }

                            Thread.Sleep(10000);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Failed to retrieve clients information.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                });
            }

            networkThread.Start();
        }

        private void HideAllUserControls()
        {
            userControlDashboard.Visibility = Visibility.Hidden;
            userControlHost.Visibility = Visibility.Hidden;
            userControlUpload.Visibility = Visibility.Hidden;
        }

        private void btnDashboardControl_Click(object sender, RoutedEventArgs e)
        {
            HideAllUserControls();
            userControlDashboard.Visibility = Visibility.Visible;
        }

        private void btnHostControl_Click(object sender, RoutedEventArgs e)
        {
            HideAllUserControls();
            userControlHost.Visibility = Visibility.Visible;
        }

        private void btnUploadControl_Click(object sender, RoutedEventArgs e)
        {
            HideAllUserControls();
            userControlUpload.Visibility = Visibility.Visible;
        }

        private void btnCloseHome_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure want to exit?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure want to logout?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                IsDirectToLogin = true;
                this.Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(MainWindow.mainWindow.currentClient != null)
            {
                var logoutResult = MainWindow.mainWindow.authInterface.Logout((int)MainWindow.mainWindow.currentClient.ClientID, MainWindow.mainWindow.currentClient.Token);
                if (logoutResult.Response.IsSuccess)
                {
                    if(IsDirectToLogin)
                    {
                        MainWindow.mainWindow.currentClient = null;
                        MainWindow.mainWindow.Show();
                    }
                    else
                    {
                        Environment.Exit(0);
                    }
                }
                else
                {
                    MessageBox.Show(logoutResult.Response.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (Exception)
            {

            }
        }
    }
}
