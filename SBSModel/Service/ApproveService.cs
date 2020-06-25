using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using SBSModel.Models;
using SBSModel.Common;
using System.Data.Entity.Validation;
using SBSResourceAPI;

namespace SBSModel.Models
{
    public  class ApproveService
    {
        public ServiceResult UpdateAppStatus(Nullable<int> pDocID, string pAppType,string pStatus)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (pAppType == SBSWorkFlowAPI.Constants.ApprovalType.Leave)
                    {
                        var doc = db.Leave_Application_Document.Where(w => w.Leave_Application_Document_ID == pDocID).FirstOrDefault();
                        db.SaveChanges();
                        return new ServiceResult() { 
                            Code = ERROR_CODE.SUCCESS,
                            Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), 
                            Field = pAppType,
                            Object = doc
                        };
                    }
                    else if (pAppType == SBSWorkFlowAPI.Constants.ApprovalType.Expense)
                    {
                        var doc = db.Expenses_Application.Where(w => w.Expenses_Application_ID == pDocID).FirstOrDefault();
                        doc.Overall_Status = pStatus;
                        db.SaveChanges();
                        return new ServiceResult() { 
                            Code = ERROR_CODE.SUCCESS, 
                            Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), 
                            Field = pAppType ,
                            Object = doc
                        };
                    }
                    else if (pAppType == SBSWorkFlowAPI.Constants.ApprovalType.PurchaseOrder)
                    {
                        //var doc = db.Purchase_Order
                        //    .Include(i=>i.User_Profile)
                        //    .Include(i => i.User_Profile.User_Authentication)
                        //    .Include(i => i.Purchase_Order_has_Vendor_Product)
                        //    .Include(i => i.Purchase_Order_has_Vendor_Product.Select(s=>s.Product))
                        //    .Where(w => w.Purchase_Order_ID == pDocID).FirstOrDefault();

                        //doc.Overall_Status = pStatus;
                        //db.SaveChanges();
                       
                        //return new ServiceResult()
                        //{
                        //    Code = ERROR_CODE.SUCCESS,
                        //    Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT),
                        //    Field = pAppType,
                        //    Object = doc
                        //};
                    }

                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = pAppType };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_506_SAVE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_506_SAVE_ERROR), Field = pAppType };
            }
        }
       
    }
}
