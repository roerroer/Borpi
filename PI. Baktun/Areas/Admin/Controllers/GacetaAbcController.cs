using Newtonsoft.Json;
using PI.Baktun.Controllers;
using PI.Common;
using PI.Core;
using PI.Core.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PI.Baktun.Areas.Admin.Controllers
{
    public class GacetaAbcController : CoreController
    {

        private IGacetaAbcManager _gacetaAbcManager;

        public GacetaAbcController(IGacetaAbcManager gacetaAbcManager, ISessionManager sessionManager)
            : base(sessionManager)
        {
            _gacetaAbcManager = gacetaAbcManager;
        }

        // GET: /Test/1 >userid=1
        public JsonResult Index(int id = 0)
        {
            if (IsAuth)
            {
                if (id == 0)
                {
                    result.Succeeded = true;
                    result.Result = new Agente() { Id = -1 };
                }
                else
                {
                    result = _gacetaAbcManager.Get(id);
                }
            }

            return new JsonNetResult() { Data = result };
        }

        public ActionResult Save(Gaceta model)
        {
            if (IsAuth)
            {
                if (model.Id == -1)
                {
                    result = _gacetaAbcManager.Create(model);
                }
                else
                {
                    result = _gacetaAbcManager.Update(model);
                }
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public JsonResult GetPage(int page = 1, int pageSize = 10)
        {
            if (IsAuth)
                result = _gacetaAbcManager.GetPage(page, pageSize, null, order => order.FechaPublicacion);

            return new JsonNetResult() { Data = result };
        }

        public JsonResult GetPageBySeccion(int seccionId, int page = 1, int pageSize = 0)
        {
            if (IsAuth)
                result = _gacetaAbcManager.GetPage(page, pageSize, f => f.GacetaSeccionId == seccionId, order => order.FechaPublicacion);

            return new JsonNetResult() { Data = result };
        }
    }
}
