using Common.Web;
using PI.Baktun.Controllers;
using PI.Common;
using PI.Core;
using PI.Core.Abstract;
using PI.Models;
using PI.Models.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PI.Baktun.Areas.Admin.Controllers
{
    public class UsuariosController : CoreController
    {
        private IUserManager _userManager;

        public UsuariosController(IUserManager userManager, ISessionManager sessionManager)
            : base(sessionManager)
        {
            _userManager = userManager;
            result.Succeeded = false;
        }

        // GET: /Test/1 >userid=1
        public JsonResult Index(int id = 0)
        {
            if (IsAuth)
            {
                if (id == 0)
                {
                    result.Succeeded = true;
                    result.Result = new Usuario() { Id = -1 };
                }
                else
                {
                    result = _userManager.Get(id);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadMyPerfil()
        {
            if (IsAuth)
            {
                result = _userManager.Get(sessionToken.UsuarioId);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private static string CAMBIAR_CLAVE_MSG = @"
        Estimado usuario,
        Si usted ha olvidado o solicitado cambiar su contraseña, siga el siguiente link:
        
        {0}
        
        Gracias,
        ";

        public JsonResult cambiarClave(Usuario model)
        {
            string locPath = System.Web.HttpContext.Current.Request.Headers["locPath"];

            if (string.IsNullOrEmpty(locPath) || !locPath.Contains("olvide-clave"))
            {
                result.Result = null;
                result.Succeeded = false;
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!string.IsNullOrEmpty(model.Email))
            {
                result = _userManager.Get(u => u.Email == model.Email);
                var usr = (Usuario)result.Result;
                string fullkey = setSpk(usr);
                locPath = locPath.Replace("olvide-clave", "cambiarPW") + "/" + fullkey + "/" + usr.Id;
                result = _userManager.Update(usr);
                EmailSender.send(model.Nombre, model.Email, "Cambiar clave", string.Format(CAMBIAR_CLAVE_MSG, locPath));
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        // GET: /Test/1 >userid=1
        public JsonResult GetWithSpk(ModelToken modelToken)
        {
            if (modelToken.Id > 0)
            {
                result = _userManager.Get(modelToken.Id);
                var usr = (Usuario)result.Result;
                if (usr.Spk != modelToken.Spk || usr.SpkExpiration < DateTime.Now)
                {
                    result.Result = null;
                    result.Succeeded = false;
                    result.Errors = "Link invalido, contacte al administrador del sistema.";
                }
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public ActionResult ResetPW(Usuario model)
        {
            if (model.Id > 0)
            {
                result = _userManager.SetPassword(model);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        private string setSpk(Usuario model)
        {
            string key1 = Util.RandomString(4);
            string key2 = Util.RandomString(4);
            string fullkey = key1 + "-" + key2;
            model.Spk = fullkey;
            model.SpkExpiration = DateTime.Now.AddHours(24);
            return fullkey;
        }

        public ActionResult Save(Usuario model)
        {
            if (IsAuth)
            {
                string fullkey = setSpk(model);
                if (model.Id == -1)
                {
                    model.Salt = model.Password = fullkey; //invalid pw when creating a user                     
                    result = _userManager.Create(model);
                }
                else
                {
                    var dbUser = _userManager.Get(model.Id);
                    var usr = (Usuario)dbUser.Result;
                    model.Password = usr.Password;

                    result = _userManager.Update(model);
                }
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public JsonResult GetPage(int page = 1, int pageSize = 10)
        {
            if (IsAuth)
                result = _userManager.GetPage(page, pageSize, null, order => order.Nombre);

            return Json(result, JsonRequestBehavior.AllowGet); 
        }

        public JsonResult GetPageFilter(string filter, int page = 1, int pageSize = 0)
        {
            if (IsAuth)
                result = _userManager.GetPage(page, pageSize, f => f.Nombre.Contains(filter), order => order.Nombre);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Login(string userName, string password)
        {
            ResultInfo result = _userManager.Find(userName, password);
            var login = result.Result;
            var sessionResult = sessionManager.SetTokenForUserId(((UserSettings)result.Result).Usuario.Id, 1 );
            result.Result = new { Usuario = login, token = ((Session)sessionResult.Result).Token };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult CreateMasterUser()
        //{
        //    var user = new Usuario()
        //    {
        //        Nombre = "MasterUser",
        //        Email = "email@gmail.com",
        //        Password = "123",
        //        Salt = "sinsal"
        //    };

        //    var resultX = _userManager.SignIn(user);
        //    return Json(resultX, JsonRequestBehavior.AllowGet);
        //}

    }
}
