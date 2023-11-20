using PI.Common;
using PI.Core;
using PI.Core.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PI.Baktun.Controllers
{
    //
    // Gaceta de edictos de marcas
    //
    public class GacetaPatentesEdictosController : CoreController
    {

        private IGacetaManager _gacetaManager;
        IFormatProvider culture = new System.Globalization.CultureInfo("es-GT", true);

        public GacetaPatentesEdictosController(IGacetaManager gacetaManager, ISessionManager sessionManager)
            : base(sessionManager)
        {
            _gacetaManager = gacetaManager;
        }

        public JsonNetResult GetPage(int page = 1, int pageSize = 100)
        {
            if (IsAuth)
                result = _gacetaManager.GetPagePublicacionEdictoPatente(page, pageSize);

            return new JsonNetResult() { Data = result };
        }

        public JsonNetResult GetPageFilter(string filter, int page = 1, int pageSize = 100)
        {
            DateTime date = new DateTime();
            Boolean filterByDate = false;

            if (!string.IsNullOrEmpty(filter))
                filterByDate = DateTime.TryParse(filter, culture, System.Globalization.DateTimeStyles.AssumeLocal, out date);


            if (filterByDate)
                result = _gacetaManager.GetPagePublicacionEdictoPatenteFilterByDate(page, pageSize, date.Date);
            else
                result = _gacetaManager.GetPagePublicacionEdictoPatente(page, pageSize, filter);

            return new JsonNetResult() { Data = result };
        }

        public JsonNetResult GetPageFilterByDate(string fechaPublicacion, int page = 1, int pageSize = 100)
        {
            DateTime date = new DateTime();

            IFormatProvider culture = new System.Globalization.CultureInfo("es-GT", true);
            if (!string.IsNullOrEmpty(fechaPublicacion))
                date = DateTime.Parse(fechaPublicacion, culture, System.Globalization.DateTimeStyles.AssumeLocal);

            result = _gacetaManager.GetPagePublicacionEdictoPatenteFilterByDate(page, pageSize, date);

            return new JsonNetResult() { Data = result };
        }

        public ActionResult Edicto(int Id)
        {
            result = _gacetaManager.GetPublicacionEdictoPatenteByExpedienteId(Id);
            ViewBag.Many = false;
            return View(result.Result);
        }


        public ActionResult EdictoAsPdf(int Id)
        {
            ViewBag.Many = false;
            result = _gacetaManager.GetPublicacionEdictoPatenteByExpedienteId(Id);     
            var rotativaOptions = new Rotativa.Core.DriverOptions()
            {
                CustomSwitches = "--print-media-type",
                PageOrientation = Rotativa.Core.Options.Orientation.Portrait,
                PageMargins = new Rotativa.Core.Options.Margins(1,0,1,0),
                PageWidth = 215.9,
                PageHeight = 279.4
            };

            return new Rotativa.MVC.ViewAsPdf("Edicto", result.Result)
            {
                FileName = "Edicto"+Id.ToString()+".Pdf",
                RotativaOptions = rotativaOptions
            };
        }

        public ActionResult Semanal(string fecha, string format)
        {
            DateTime? date = null;

            IFormatProvider culture = new System.Globalization.CultureInfo("es-GT", true);
            if (!string.IsNullOrEmpty(fecha))
                date = DateTime.Parse(fecha, culture, System.Globalization.DateTimeStyles.AssumeLocal);
            else
                date = DateTime.Today;

            var startDate = date.Value.AddDays(1 - (int)(date.Value.DayOfWeek));
            var endDate = startDate.AddDays(4);
            result.Result = new { startDate = startDate, endDate = endDate };

            result = _gacetaManager.GetPublicacionEdictoPatenteSemanal(startDate, endDate);

            ViewBag.Many = true;
            var week = startDate.ToLongDateString() + " - " + endDate.ToLongDateString();
            ViewBag.Week = week;
            
            if (!String.IsNullOrEmpty(format) && format=="pdf")
            {
                var rotativaOptions = new Rotativa.Core.DriverOptions()
                {
                    CustomSwitches = "--print-media-type",
                    PageOrientation = Rotativa.Core.Options.Orientation.Portrait,
                    PageMargins = new Rotativa.Core.Options.Margins(1, 0, 1, 0),
                    PageWidth = 215.9,
                    PageHeight = 279.4
                };

                return new Rotativa.MVC.ViewAsPdf("Semanal", ((PagedList)result.Result).DataSet)
                {
                    FileName = "Gaceta_" + week + ".pdf",
                    RotativaOptions = rotativaOptions
                };
            }
            return View(((PagedList)result.Result).DataSet);
        }
    }
}
