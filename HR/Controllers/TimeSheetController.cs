using HR.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SBSModel.Common;
using SBSModel.Models;
using SBSResourceAPI;
using SBSTimeModel.Common;
using SBSTimeModel.Models;
using SBSWorkFlowAPI.Constants;
using SBSWorkFlowAPI.Models;
using SBSWorkFlowAPI.ModelsAndService;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;


namespace HR.Controllers
{
   [Authorize]
   public class TimeSheetController : ControllerBase
   {

      #region Time Sheet
      [HttpGet]
      [AllowAuthorized]
      public ActionResult TimeSheetManagement(ServiceResult result, TimeSheetViewModel model, int pno = 1, int plen = 10)
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         var userlogin = UserUtil.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         var rightResult = base.validatePageRight(Operation.A);
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         var hService = new EmploymentHistoryService();
         var hist = hService.GetCurrentEmploymentHistoryByProfile(userlogin.Profile_ID);
         if (hist == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Employment_History);

         var tsexService = new TsExService();
         var cbService = new ComboService();
         var uService = new UserService();

         //filter
         var cri = new UserCriteria();
         cri.Company_ID = userlogin.Company_ID;
         model.EmployeeList = uService.LstUserProfile(cri);

         if (!model.Search_Pending_Year.HasValue)
            model.Search_Pending_Year = currentdate.Year;
         if (!model.Search_Process_Year.HasValue)
            model.Search_Process_Year = currentdate.Year;

         //----------------------- Pending
         var criteriaPending = new TsExCriteria()
         {
            Company_ID = userlogin.Company_ID,
            Profile_ID = userlogin.Profile_ID,
            Employee_Profile_ID = hist.Employee_Profile_ID,
            Department_ID = hist.Department_ID,
            Include_Extra = true,
            Tab_Pending = true,

            //Page_Size = plen,
            //Page_No = pno,

            Request_Profile_ID = model.Search_Pending_Emp,
            Month = model.Search_Pending_Month,
            Year = model.Search_Pending_Year,

         };
         var presultPending = tsexService.LstTsEx(criteriaPending);
         if (presultPending.Object != null)
            model.TsEXPendingList = (List<TsEX>)presultPending.Object;

         //----------------------- Processed
         var criteriaProcessed = new TsExCriteria()
         {
            Company_ID = userlogin.Company_ID,
            Profile_ID = userlogin.Profile_ID,
            Employee_Profile_ID = hist.Employee_Profile_ID,
            Department_ID = hist.Department_ID,

            Include_Extra = true,
            Tab_Processed = true,

            Page_Size = plen,
            Page_No = pno,

            Request_Profile_ID = model.Search_Process_Emp,
            Month = model.Search_Process_Month,
            Year = model.Search_Process_Year,
         };
         var presultProcessed = tsexService.LstTsEx(criteriaProcessed);
         if (presultProcessed.Object != null)
            model.TsEXProcessedList = (List<TsEX>)presultProcessed.Object;

         model.Record_Count = presultProcessed.Record_Count;
         model.Page_No = pno;
         model.Page_Length = plen;

         return View(model);
      }

      [HttpGet]
      [AllowAuthorized]
      public ActionResult TimeSheetTransactionReport(ServiceResult result, TimeSheetTransactionViewModel model, int pno = 1, int plen = 15)
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         var userlogin = UserUtil.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         var rightResult = base.validatePageRight(Operation.A);
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         var empService = new EmployeeService();
         var cbService = new ComboService();
         var tsexService = new TsExService();

         //filter
         var emp = new List<Employee_Profile>();
         var criteria2 = new EmployeeCriteria() { Company_ID = userlogin.Company_ID };
         var pResult2 = empService.LstEmployeeProfile(criteria2);
         if (pResult2.Object != null) emp = (List<Employee_Profile>)pResult2.Object;
         model.EmployeeList = emp;

         model.Yearlst = new List<int>();
         for (int i = currentdate.Year - 2; i <= currentdate.Year; i++)
            model.Yearlst.Add(i);

       
         model.Monthlst = cbService.LstMonth(true);

         var criteriaProcessed = new TsExCriteria()
         {
            Company_ID = userlogin.Company_ID,
            Include_Draft = true,

            Page_Size = plen,
            Page_No = pno,

            Employee_Profile_ID = model.Search_Employee_Profile_ID,
            Month = model.Search_Month,
            Year = model.Search_Year,
         };
         var presult = tsexService.LstTsEx(criteriaProcessed);
         if (presult.Object != null)
         {
            model.TsExList = (List<TsEX>)presult.Object;
            model.Record_Count = presult.Record_Count;
         }
         model.Page_No = pno;
         model.Page_Length = plen;
         return View(model);
      }
      #endregion


      # region Send Email
      public void sendProceedEmail(Time_Sheet ts, Company_Details com, User_Profile sentto, User_Profile receivedfrom, Employment_History receivedhist, string Overall_Status, List<Reviewer> Reviewers, string approverName = "")
      {
         var ecode = "[RE" + ts.Time_Sheet_ID + "_";
         if (ts.Request_Cancel_ID.HasValue)
            ecode += "RQC" + ts.Request_Cancel_ID;
         else if (ts.Request_ID.HasValue)
            ecode += "RQ" + ts.Request_ID;

         ecode += "S" + receivedfrom.User_Authentication.User_Authentication_ID + "R" + sentto.User_Authentication.User_Authentication_ID + "]";


         if (string.IsNullOrEmpty(approverName))
         {
            var userlogin = UserUtil.getUser(HttpContext);
            if (userlogin != null)
               approverName = AppConst.GetUserName(userlogin);
         }

         var eitem = new EmailTimeItem()
         {
            LogoLink = GenerateLogoLink(),
            Company = com,
            Send_To_Email = sentto.User_Authentication.Email_Address,
            Send_To_Name = AppConst.GetUserName(sentto),
            Received_From_Email = receivedfrom.User_Authentication.Email_Address,
            Received_From_Name = AppConst.GetUserName(receivedfrom),
            Received_From_Department = receivedhist.Department != null ? receivedhist.Department.Name : "",
            Module = ModuleCode.Time,
            Approval_Type = ApprovalType.TimeSheet,
            TimeSheet = ts,
            Status = Overall_Status,
            Reviewer = Reviewers,
            Url = Request.Url.AbsoluteUri,
            ECode = ecode,
            Approver_Name = approverName
         };
         EmailTimeTemplete.sendProceedEmail(eitem);
      }

      public void sendRequestEmail(Time_Sheet ts, Company_Details com, User_Profile sentto, User_Profile receivedfrom, Employment_History receivedhist, string Overall_Status, List<Reviewer> Reviewers, string linkApp, string linkRej)
      {
         var ecode = "[PE" + ts.Time_Sheet_ID + "_";
         if (ts.Request_Cancel_ID.HasValue)
            ecode += "RQC" + ts.Request_Cancel_ID;
         else if (ts.Request_ID.HasValue)
            ecode += "RQ" + ts.Request_ID;

         ecode += "S" + receivedfrom.User_Authentication.User_Authentication_ID + "R" + sentto.User_Authentication.User_Authentication_ID + "]";

         var eitem = new EmailTimeItem()
         {
            LogoLink = GenerateLogoLink(),
            Company = com,
            Send_To_Email = sentto.User_Authentication.Email_Address,
            Send_To_Name = AppConst.GetUserName(sentto),
            Received_From_Email = receivedfrom.User_Authentication.Email_Address,
            Received_From_Name = AppConst.GetUserName(receivedfrom),
            Received_From_Department = receivedhist.Department != null ? receivedhist.Department.Name : "",
            Module = ModuleCode.Time,
            Approval_Type = ApprovalType.TimeSheet,
            TimeSheet = ts,
            Status = Overall_Status,
            Reviewer = Reviewers,
            Link = linkApp,
            Link2 = linkRej,
            Url = Request.Url.AbsoluteUri,
            ECode = ecode
         };
         EmailTimeTemplete.sendRequestEmail(eitem);
      }
      #endregion
   }
}