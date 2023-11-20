using PI.Common;
using PI.Core;
using PI.Core.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace PI.Baktun.Controllers
{
    public class AvisosController : CoreController
    {
        private IAvisosManager _avisosManager;

        public AvisosController(IAvisosManager avisosManager, ISessionManager sessionManager)
            : base(sessionManager)
        {
            _avisosManager = avisosManager;
        }



        public JsonNetResult GetPage(int page = 1, int pageSize = 0)
        {
            result = _avisosManager.GetPage(page, pageSize, a => a.enabled == true, order => order.Fecha, descending: true);

            return new JsonNetResult() { Data = result };
        }
    }
}
