using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using POS.Models;
using SBSModel.Models;
using SBSModel.Common;

namespace POS.Controllers
{
    public class HomeController : ControllerBase
    {
        public ActionResult Index()
        {
            //-------cashier rights------------
            RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A, "/POS/POS");
            if (rightResult.action == null) return RedirectToAction("POS", "POS");

                
            //-------supervisor rights------------
            RightResult supRightResult = base.validatePageRight(UserSession.RIGHT_A, "/POSConfig/ConfigurationAmin");
            if (supRightResult.action == null) return RedirectToAction("Report", "POS");


            return RedirectToAction("POS","POS");
        }
    }
}