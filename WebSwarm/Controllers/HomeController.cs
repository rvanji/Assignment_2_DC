using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using WebSwarm.Models;

namespace WebSwarm.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if(AccountController.currentUser != null)
            {
                ViewBag.Title = "Swarm";
                RemoteServicesModel remoteServicesModel = new RemoteServicesModel();

                try
                {
                    var client = new RestClient("http://localhost:51709");
                    var request = new RestRequest("api/RemoteServices/ListRemoteServices/");
                    request.RequestFormat = DataFormat.Json;
                    request.AddBody(new ClientModel
                    {
                        Token = AccountController.currentUser.Token,
                    });

                    var response = client.Post(request);
                    RemoteServicesModel? remoteServicesResult = JsonConvert.DeserializeObject<RemoteServicesModel>(response.Content.ToString());
                    if (remoteServicesResult != null && remoteServicesResult.RemoteServices != null)
                    {
                        if (remoteServicesResult.RemoteServices.Count > 0)
                        {
                            foreach(var remoteService in remoteServicesResult.RemoteServices)
                            {
                                remoteServicesModel.RemoteServices.Add(new RemoteServiceModel()
                                {
                                    IPAddress = remoteService.IPAddress,
                                    Port = remoteService.Port,
                                    NoOfJobs = remoteService.NoOfJobs,
                                    IsAllocated = remoteService.IsAllocated,
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    remoteServicesModel.ErrorMessage = "An error occured while retrieving remote services";
                }

                return View(remoteServicesModel);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}
