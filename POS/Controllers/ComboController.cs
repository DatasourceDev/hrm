using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using POS.Models;
using SBSModel.Models;


namespace POS.Controllers
{
    public class ComboController : ControllerBase
    {
        //
        // GET: /Combo/
        public ActionResult Reload(ComboTypeEnum type, string param)
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
                case ComboTypeEnum.Proposal_Item:
                   
                case ComboTypeEnum.Customer_Company:
                   
                default:
                    break;
            }

            return Json(combolist, JsonRequestBehavior.AllowGet);
        }
    }
}