using Microsoft.AspNetCore.Mvc;
using WebSwarm.Models;
using Common;
using Common.Other;
using System.ServiceModel;
using Common.Models;

namespace WebSwarm.Controllers
{
    public class AccountController : Controller
    {
        public static IAuthenticationService? authInterface;
        public static Functions functions = new Functions();
        public static Common.Models.ClientModel? currentUser;

        [HttpGet]
        public IActionResult Login()
        {
            if (authInterface == null)
            {
                authInterface = functions.InitAuthentication();
            }

            return View();
        }

        public JsonResult ProcessLogin(Common.Models.ClientModel model)
        {
            if(string.IsNullOrEmpty(model.Email))
            {
                return Json(new ResponseModel() { IsSuccess = false, Message = "Email Address is required" });
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                return Json(new ResponseModel() { IsSuccess = false, Message = "Password is required" });
            }

            try
            {
                var loginResult = authInterface.Login(model.Email, model.Password);
                if (loginResult.Response.IsSuccess)
                {
                    currentUser = loginResult;
                    return Json(new ResponseModel() { IsSuccess = true });
                }
                else
                {
                    return Json(new ResponseModel() { IsSuccess = false, Message = loginResult.Response.Message });
                }
            }
            catch (Exception)
            {
                return Json(new ResponseModel() { IsSuccess = false, Message = "Failed to login." });
            }
        }
    }
}
