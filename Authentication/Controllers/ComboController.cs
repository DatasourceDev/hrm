using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Authentication.Models;
using SBSModel.Models;
using SBSModel.Common;


namespace Authentication.Controllers
{
    public class ComboController : ControllerBase
    {
        //
        // GET: /Combo/
        public ActionResult Reload(ComboTypeEnum type, String param)
        {
            var cbService = new ComboService();
            
            List<ComboViewModel> combolist  = new List<ComboViewModel>();
            switch (type)
            {
                case ComboTypeEnum.Country :
                    combolist = cbService.LstCountry();
                    break;
                case ComboTypeEnum.State:
                    combolist = cbService.LstState(param);
                    break;
                default:
                    break;
            }
           
            return Json(combolist, JsonRequestBehavior.AllowGet);
        }
	}
}