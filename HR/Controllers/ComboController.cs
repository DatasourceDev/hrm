using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Models;
using HR.Common;
using SBSModel.Models;
using SBSModel.Common;
using SBSResourceAPI;


namespace HR.Controllers
{
    public class ComboController : ControllerBase
    {
        //
        // GET: /Combo/
        public ActionResult Reload(ComboTypeEnum type, String param, string param2)
        {
            var cbService = new ComboService();

            List<ComboViewModel> combolist = new List<ComboViewModel>();
            switch (type)
            {
                case ComboTypeEnum.Country:
                    combolist = cbService.LstCountry();
                    break;
                case ComboTypeEnum.State:
                    combolist = cbService.LstState(param);
                    break;
                case ComboTypeEnum.PRC:
                    combolist = cbService.LstPRC(NumUtil.ParseInteger(param), NumUtil.ParseInteger(param2));
                    break;              
                case ComboTypeEnum.Supervisor:
                    combolist = cbService.LstSupervisor(NumUtil.ParseInteger(param));
                    break;
                case ComboTypeEnum.Residential_Status:
                    var isSG = Convert.ToBoolean(param);
                    combolist = cbService.LstResidentialStatus(isSG);
                    break;
                default:
                    break;
            }

            return Json(combolist, JsonRequestBehavior.AllowGet);
        }

     
    }


}