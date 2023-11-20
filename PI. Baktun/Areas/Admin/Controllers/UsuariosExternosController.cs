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
    public class UsuariosPublicosController : CoreController
    {
        private IUsuarioPublicoManager _usuarioPublicoManager;

        public UsuariosPublicosController(IUsuarioPublicoManager usuarioPublicoManager, ISessionManager sessionManager)
            : base(sessionManager)
        {
            _usuarioPublicoManager = usuarioPublicoManager;
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
                    result.Result = new UsuarioPublico() { Id = -1 };
                }
                else
                {
                    result = _usuarioPublicoManager.Get(id);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadMyPerfil()
        {
            if (IsAuth)
            {
                result = _usuarioPublicoManager.Get(sessionToken.UsuarioId);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private static string CAMBIAR_CLAVE_MSG = @"
        Estimado usuario,
        Si usted ha olvidado o solicitado cambiar su contraseña, siga el siguiente link:
        
        {0}
        
        Gracias,
        ";

        public JsonResult cambiarClave(UsuarioPublico model)
        {   
            string locPath = System.Web.HttpContext.Current.Request.Headers["locPath"];

            if (string.IsNullOrEmpty(locPath) || !locPath.Contains("olvide-clave")) {
                result.Result = null;
                result.Succeeded = false;
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            
            if (!string.IsNullOrEmpty(model.Cuenta))
            {
                result = _usuarioPublicoManager.Get(u => u.Cuenta == model.Cuenta);
                var usr = (UsuarioPublico)result.Result;
                string fullkey = setSpk(usr);
                locPath = locPath.Replace("olvide-clave", "cambiarPW") + "/" + fullkey + "/" + usr.Id;
                result = _usuarioPublicoManager.Update(usr);
                EmailSender.send(model.Nombre, model.Cuenta, "Cambiar clave", string.Format(CAMBIAR_CLAVE_MSG, locPath));
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        // GET: /Test/1 >userid=1
        public JsonResult GetWithSpk(ModelToken modelToken)
        {
            if (modelToken.Id > 0)
            {
                result = _usuarioPublicoManager.Get(modelToken.Id);
                var usr = (UsuarioPublico)result.Result;
                if (usr.Spk != modelToken.Spk || usr.SpkExpiration < DateTime.Now)
                {
                    result.Result = null;
                    result.Succeeded = false;
                    result.Errors = "Link invalido, contacte al administrador del sistema.";
                }
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public ActionResult ResetPW(UsuarioPublico model)
        {
            if (model.Id > 0)
            {
                result = _usuarioPublicoManager.SetPassword(model);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        private string setSpk(UsuarioPublico model) {
            string key1 = Util.RandomString(4);
            string key2 = Util.RandomString(4);
            string fullkey = key1 + "-" + key2;
            model.Spk = fullkey;
            model.SpkExpiration = DateTime.Now.AddHours(24);
            return fullkey;
        }

        public ActionResult Save(UsuarioPublico model)
        {
            string fullkey = setSpk(model);

            model.Telefonos = string.IsNullOrEmpty(model.Telefonos) ? string.Empty : model.Telefonos;
            if (IsAuth)
            {                
                if (model.Id == -1)
                {
                    model.Pwd = fullkey; //invalid pw when creating a user
                    result = _usuarioPublicoManager.Create(model);
                }
                else
                {
                    var dbUser = _usuarioPublicoManager.Get(model.Id);
                    var usr = (UsuarioPublico)dbUser.Result;
                    model.Pwd = usr.Pwd;

                    result = _usuarioPublicoManager.Update(model);
                }
            }
            else if (model.Id == -1) {

                var dbUser = _usuarioPublicoManager.Get(u => u.Cuenta == model.Cuenta);
                if (dbUser.Result == null)
                {
                    model.Suscripcion = false;
                    result = _usuarioPublicoManager.Create(model);
                    if (result.Succeeded)
                        ResetPW((UsuarioPublico)result.Result);
                }
                else {
                    result.Succeeded = false;
                    result.Errors = "Cuenta ya existe!";
                }
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public JsonResult GetPage(int page = 1, int pageSize = 10)
        {
            if (IsAuth)
                result = _usuarioPublicoManager.GetPage(page, pageSize, null, order => order.Nombre);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPageFilter(string filter, int page = 1, int pageSize = 0)
        {
            if (IsAuth)
                result = _usuarioPublicoManager.GetPage(page, pageSize, f => f.Nombre.Contains(filter), order => order.Nombre);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult Login(string userName, string password)
        {
            ResultInfo result = _usuarioPublicoManager.Find(userName, password);
            var login = result.Result;
            var sessionResult = sessionManager.SetTokenForUserId(((UsuarioPublico)result.Result).Id, 2); // Usuario Publico

            result.Result = new { Usuario = login, token = ((Session)sessionResult.Result).Token };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //create first user /dev mode only
        //public JsonResult CreateMasterUser()
        //{
        //    var user = new UsuarioPublico()
        //    {
        //        Nombre = "MasterUser",
        //        Cuenta = "email@gmail.com",
        //        Pwd = "123"
        //    };

        //    var resultX = _usuarioPublicoManager.SignIn(user);
        //    return Json(resultX, JsonRequestBehavior.AllowGet);
        //}

    }
}
