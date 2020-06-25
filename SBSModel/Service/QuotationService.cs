using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SBSModel.Common;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;
using SBSModel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;



namespace SBSModel.Models
{
    public class QuotationService
    {

        public Country GetCountry(Nullable<int> pCountryID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.Countries.Where(w => w.Country_ID == pCountryID).FirstOrDefault();
            }
        }
        public State GetState(Nullable<int> pStateID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.States.Where(w => w.State_ID == pStateID).FirstOrDefault();
            }
        }

        public List<User_Profile> LstAssignTo(int pCompanyID)
        {
            using (var db = new SBS2DBContext())
            {

                return db.User_Profile
                .Where(w => w.Company_ID == pCompanyID)
                .ToList();
            }
        }

        public CRM_Details GetOpp(int pCrmID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.CRM_Details
                    .Include(i => i.User_Profile)
                    .Include(i => i.Customer_Company)
                    .Where(w => w.CRM_ID == pCrmID).FirstOrDefault();
            }
        }

        public Customer_Company GetCustomerCompany(int pCustomerCompanyID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.Customer_Company.Where(w => w.Customer_Company_ID == pCustomerCompanyID).FirstOrDefault();
            }
        }

        public Product GetProduct(int pProductID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.Products.Include(w => w.Unit_Of_Measurement).Where(w => w.Product_ID == pProductID).FirstOrDefault();
            }
        }

        public List<Quotation_Revision> LstQuotationsRevision(int pQuotationID, int pProfileID)
        {
            var revisionList = new List<Quotation_Revision>();
            using (var db = new SBS2DBContext())
            {
                var quotation = new Quotation();
                var iagroups = db.IAs.Where(w => w.Profile_ID == pProfileID & w.IG.Type == InventoryType.Quotation).Select(s => s.IG_ID);
                if (iagroups.Count() > 0)
                {
                    // have approval role
                    var ir = db.IRs.Where(w => iagroups.Contains(w.IG_ID)).Select(s => s.Profile_ID);
                    quotation = db.Quotations.Where(w => w.Quotation_ID == pQuotationID && ir.Contains(w.Profile_ID)).FirstOrDefault();
                }
                else
                {
                    quotation = db.Quotations.Where(w => w.Quotation_ID == pQuotationID && w.Profile_ID == pProfileID).FirstOrDefault();
                }


                if (quotation != null)
                {
                    var quotationItem = db.Quotation_has_Product.Where(w => w.Quotation_ID == pQuotationID).OrderBy(o => o.Rev_ID);

                    var revisiongroup = quotationItem.GroupBy(g => g.Rev_ID)
                        .Select(g => new
                        {
                            Amount = g.Sum(s => s.Amount),
                            Create_On = g.Max(m => m.Create_On),
                            Create_By = g.Max(m => m.Create_By),
                            Rev_ID = g.Max(m => m.Rev_ID)
                        });

                    foreach (var row in revisiongroup)
                    {
                        var revision = new Quotation_Revision()
                        {
                            Company_Name = quotation.Customer_Company != null ? quotation.Customer_Company.Company_Name : "",
                            Quotation_ID = quotation.Quotation_ID,
                            Quotation_Ref_No = quotation.Quotation_Ref_No,
                            Amount = row.Amount.HasValue ? row.Amount.Value : 0,
                            Create_On = row.Create_On,
                            Create_By = row.Create_By,
                            Rev_ID = row.Rev_ID.HasValue ? row.Rev_ID.Value : 0,
                            Has_Invoices = quotation.Invoice_has_Quotation.Count() > 0 ? true : false,
                            Overall_Status = quotation.Overall_Status
                        };

                        revisionList.Add(revision);
                    }

                    return revisionList;
                }
            }
            return revisionList;
        }

        public List<Quotation> LstQuotations(int[] pQuotation_IDs)
        {
            using (var db = new SBS2DBContext())
            {

                return db.Quotations
                .Include(i => i.Customer_Company)
                .Include(i => i.Invoice_has_Quotation)
                .Include(i => i.Quotation_has_Product)
                .Include(i => i.Quotation_has_Product.Select(s => s.Product))
                .Include(i => i.User_Profile)
                .Include(i => i.Inventory_Prefix_Config)
                .Include(i => i.Invoice_has_Quotation)
                .Where(w => pQuotation_IDs.Contains(w.Quotation_ID))
                .OrderByDescending(o => o.Rev_ID).ToList();


            }
        }

        public List<Quotation> LstRelatedQuotations(int pQuotation_ID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.Quotations
                .Include(i => i.Invoice_has_Quotation)
                .Include(i => i.Quotation_has_Product)
                .Include(i => i.Quotation_has_Product.Select(s => s.Product))
                .Where(w => w.Related_Quotation_ID == pQuotation_ID)
                .OrderByDescending(o => o.Quotation_ID).ToList();
            }
        }

        public List<Quotation> LstQuotationsManagement(int pCompany_ID, int pApprovalProfileID, ref bool hasApproval1, ref bool hasApproval2)
        {
            using (var db = new SBS2DBContext())
            {
                var qlist = new List<Quotation>();
                var iagroups = db.IAs.Where(w => w.Profile_ID == pApprovalProfileID & w.IG.Type == InventoryType.Quotation & (w.Is_Approval1 == true | w.Is_Approval2 == true));

                if (iagroups.Count() > 0)
                {
                    // have approval role
                    var ir = db.IRs.Where(w => iagroups.Select(s => s.IG_ID).Contains(w.IG_ID)).Select(s => s.Profile_ID);

                    var quotations = db.Quotations
                        .Include(i => i.Customer_Company)
                        .Include(i => i.Quotation_Approval)
                        .Include(i => i.Quotation_Approval.Select(s => s.User_Profile))
                        .Include(i => i.Quotation_Approval.Select(s => s.User_Profile1))
                        .Where(w => w.Company_ID == pCompany_ID && ir.Contains(w.Profile_ID))
                        .OrderByDescending(o => o.To_Date)
                        .ThenByDescending(o => o.Quotation_Ref_No);

                    if (iagroups.Where(w => w.Is_Approval1 == true).FirstOrDefault() != null)
                        hasApproval1 = true;
                    if (iagroups.Where(w => w.Is_Approval2 == true).FirstOrDefault() != null)
                        hasApproval2 = true;

                    foreach (var q in quotations)
                    {
                        if (q.Overall_Status == LeaveStatus.Pending || string.IsNullOrEmpty(q.Overall_Status))
                        {
                            var approval = q.Quotation_Approval.OrderByDescending(o => o.Quotation_Approval_ID).FirstOrDefault();// get last approval
                            if (approval != null)
                            {
                                // have approval do
                                if (approval.Approval_1.HasValue)
                                {
                                    // goto apprval2 approve
                                    if (hasApproval2 == true | hasApproval1 == true)
                                    {
                                        qlist.Add(q);
                                    }
                                }
                            }
                            else
                            {
                                // approval 2 can't see this row.
                                if (hasApproval1 == true)
                                {
                                    qlist.Add(q);
                                }
                            }
                        }
                        else
                        {
                            // all approval can see this row.
                            qlist.Add(q);
                        }
                    }
                }


                return qlist;
            }
        }

        public ServiceResult UpdateQuotationStatus(int pQuotationID, int pApprovalProfileID, string pStatus, string pRemark, string pSig, string domain)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var iagroups = db.IAs.Where(w => w.Profile_ID == pApprovalProfileID & w.IG.Type == InventoryType.Quotation & (w.Is_Approval1 == true | w.Is_Approval2 == true));
                    if (iagroups.Count() > 0)
                    {
                        var ir = db.IRs.Where(w => iagroups.Select(s => s.IG_ID).Contains(w.IG_ID)).Select(s => s.Profile_ID);

                        var hasApproval1 = false;
                        var hasApproval2 = false;

                        if (iagroups.Where(w => w.Is_Approval1 == true).FirstOrDefault() != null)
                            hasApproval1 = true;
                        if (iagroups.Where(w => w.Is_Approval2 == true).FirstOrDefault() != null)
                            hasApproval2 = true;

                        var approval = db.User_Profile.Where(w => w.Profile_ID == pApprovalProfileID).FirstOrDefault();

                        var quotation = db.Quotations.Where(w => w.Quotation_ID == pQuotationID).FirstOrDefault();
                        if (quotation != null && approval != null)
                        {
                            //quotation.Signature = pSig;
                            if (hasApproval1 == true && hasApproval2 == true)
                            {
                                var quotationApproval = new Quotation_Approval()
                                {
                                    Approval_1 = pApprovalProfileID,
                                    Approval_2 = pApprovalProfileID,
                                    Quotation_ID = pQuotationID,
                                    Remarks = pRemark,
                                    Status = pStatus
                                };
                                db.Quotation_Approval.Add(quotationApproval);

                                quotation.Overall_Status = pStatus;
                            }
                            else if (hasApproval1 == true)
                            {
                                var quotationApproval = new Quotation_Approval()
                                {
                                    Approval_1 = pApprovalProfileID,
                                    Quotation_ID = pQuotationID,
                                    Remarks = pRemark,
                                    Status = pStatus
                                };
                                db.Quotation_Approval.Add(quotationApproval);

                                if (pStatus == LeaveStatus.Rejected)
                                {
                                    quotation.Overall_Status = LeaveStatus.Rejected;
                                }
                            }
                            else if (hasApproval2 == true)
                            {
                                var oldquotationApproval = db.Quotation_Approval.Where(w => w.Quotation_ID == pQuotationID).OrderByDescending(o => o.Quotation_Approval_ID).FirstOrDefault();

                                var quotationApproval = new Quotation_Approval()
                                {
                                    Approval_1 = oldquotationApproval.Approval_1,
                                    Approval_2 = pApprovalProfileID,
                                    Quotation_ID = pQuotationID,
                                    Remarks = pRemark,
                                    Status = pStatus
                                };
                                db.Quotation_Approval.Add(quotationApproval);

                                quotation.Overall_Status = pStatus;
                            }
                            db.SaveChanges();

                            // send mail

                            //if (pStatus == LeaveStatus.Approved) {
                            //    if (hasApproval2 == true) {
                            //        var result = EmailTemplete.sendApproveEmail(quotation.User_Profile.User_Authentication.Email_Address, quotation.User_Profile.Name, "Quotation", approval.Name, quotation.Quotation_ID, domain);
                            //        if (result == false) {
                            //            return new ServiceResult() { Code = ERROR_CODE.ERROR_501_CANT_SEND_EMAIL, Msg = new Error().getError(ERROR_CODE.ERROR_501_CANT_SEND_EMAIL) };
                            //        }
                            //    } else {
                            //        // send request to approval 2
                            //        var groups = db.IRs.Where(w => w.Profile_ID == quotation.Profile_ID & w.IG.Type == InventoryType.Quotation).Select(s => s.IG_ID);
                            //        if (groups != null) {
                            //            var approval2s = db.IAs.Where(w => groups.Contains(w.IG_ID) & w.Is_Approval2 == true);
                            //            foreach (var ia in approval2s) {
                            //                var result = EmailTemplete.sendRequestEmail(ia.User_Profile.User_Authentication.Email_Address, quotation.User_Profile.Name, "Quotation", ia.User_Profile.Name, quotation.Quotation_ID, domain);
                            //                if (result == false) {
                            //                    return new ServiceResult() { Code = ERROR_CODE.ERROR_501_CANT_SEND_EMAIL, Msg = new Error().getError(ERROR_CODE.ERROR_501_CANT_SEND_EMAIL) };
                            //                }
                            //            }
                            //        }

                            //    }
                            //} else if (pStatus == LeaveStatus.Rejected) {
                            //    var result = EmailTemplete.sendRejectEmail(quotation.User_Profile.User_Authentication.Email_Address, quotation.User_Profile.Name, "Quotation", approval.Name, quotation.Quotation_ID, domain);
                            //    if (result == false) {
                            //        return new ServiceResult() { Code = ERROR_CODE.ERROR_501_CANT_SEND_EMAIL, Msg = new Error().getError(ERROR_CODE.ERROR_501_CANT_SEND_EMAIL) };
                            //    }
                            //}



                        }

                    }


                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Quotation Management" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = "Quotation Management" };
            }
        }

        public ServiceResult UpdateQuotationStatus(Quotation pQuotation)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {

                    var quotation = db.Quotations.Where(w => w.Quotation_ID == pQuotation.Quotation_ID).FirstOrDefault();

                    if (quotation != null)
                    {
                        quotation.Quotation_Stage = pQuotation.Quotation_Stage;
                        quotation.Overall_Status = pQuotation.Overall_Status;
                    }

                    db.SaveChanges();

                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Quotation" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = "Quotation" };
            }
        }

        public ServiceResult UpdateQuotationRequestID(Quotation pQuotation)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {

                    var quotation = db.Quotations.Where(w => w.Quotation_ID == pQuotation.Quotation_ID).FirstOrDefault();

                    if (quotation != null)
                    {
                        quotation.Request_ID = pQuotation.Request_ID;
                    }

                    db.SaveChanges();

                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Quotation" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = "Quotation" };
            }
        }

        public List<Quotation> LstQuotations(int pCompany_ID, int pProfileID)
        {
            using (var db = new SBS2DBContext())
            {
                var iagroups = db.IAs.Where(w => w.Profile_ID == pProfileID & w.IG.Type == InventoryType.Quotation).Select(s => s.IG_ID);
                if (iagroups.Count() > 0)
                {
                    // have approval role
                    var ir = db.IRs.Where(w => iagroups.Contains(w.IG_ID)).Select(s => s.Profile_ID);

                    return db.Quotations
                        .Include(i => i.Customer_Company)
                        .Include(i => i.Invoice_has_Quotation)
                        .Include(i => i.Invoice_has_Quotation.Select(s => s.Invoice))
                        .Include(i => i.Invoice_has_Quotation.Select(s => s.Invoice).Select(s => s.Invoice_Payment))
                        .Include(i => i.Quotation_has_Product)
                        .Include(i => i.Quotation_has_Product.Select(s => s.Product))
                        .Include(i => i.Quotation_Payment_Term)
                        .Include(i => i.User_Profile)
                        .Include(i => i.Inventory_Prefix_Config)
                        .Where(w => w.Company_ID == pCompany_ID && ir.Contains(w.Profile_ID))
                        .OrderByDescending(o => o.Quotation_Ref_No)
                        .ThenByDescending(o => o.To_Date)
                        .ToList();
                }
                else
                {
                    return db.Quotations
                       .Include(i => i.Customer_Company)
                       .Include(i => i.Invoice_has_Quotation)
                        .Include(i => i.Invoice_has_Quotation.Select(s => s.Invoice))
                        .Include(i => i.Invoice_has_Quotation.Select(s => s.Invoice).Select(s => s.Invoice_Payment))
                       .Include(i => i.Quotation_has_Product)
                       .Include(i => i.Quotation_has_Product.Select(s => s.Product))
                       .Include(i => i.Quotation_Payment_Term)
                       .Include(i => i.User_Profile)
                       .Include(i => i.Inventory_Prefix_Config)
                       .Where(w => w.Company_ID == pCompany_ID && w.Profile_ID == pProfileID)
                       .OrderByDescending(o => o.Quotation_Ref_No)
                       .ThenByDescending(o => o.To_Date)
                       .ToList();
                }
            }
        }

        public Quotation GetQuotation(int pQuotationID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.Quotations
               .Include(i => i.Customer_Company)
               .Include(i => i.Invoice_has_Quotation)
               .Include(i => i.Quotation_has_Product)
               .Include(i => i.Quotation_has_Product.Select(s => s.Product))
               .Include(i => i.Quotation_Payment_Term)
               .Include(i => i.User_Profile)
               .Include(i => i.User_Profile.Employee_Profile)
               .Include(i => i.User_Profile.User_Authentication)
               .Include(i => i.Inventory_Prefix_Config)
               .Include(i => i.Invoice_has_Quotation)
               .Include(i => i.CRM_Details)
               .Where(w => w.Quotation_ID == pQuotationID)
               .FirstOrDefault();
            }
        }

        public Quotation GetQuotation(int pQuotationID, int pProfileID, ref bool hasApproval1, ref bool hasApproval2)
        {
            using (var db = new SBS2DBContext())
            {
                var iagroups = db.IAs.Where(w => w.Profile_ID == pProfileID & w.IG.Type == InventoryType.Quotation);
                if (iagroups.Count() > 0)
                {
                    // have approval role
                    var ir = db.IRs.Where(w => iagroups.Select(s => s.IG_ID).Contains(w.IG_ID)).Select(s => s.Profile_ID);

                    if (iagroups.Where(w => w.Is_Approval1 == true).FirstOrDefault() != null)
                        hasApproval1 = true;
                    if (iagroups.Where(w => w.Is_Approval2 == true).FirstOrDefault() != null)
                        hasApproval2 = true;

                    return db.Quotations
                    .Include(i => i.Customer_Company)
                    .Include(i => i.Invoice_has_Quotation)
                    .Include(i => i.Quotation_has_Product)
                    .Include(i => i.Quotation_has_Product.Select(s => s.Product))
                    .Include(i => i.Quotation_Payment_Term)
                    .Include(i => i.User_Profile)
                    .Include(i => i.User_Profile.Employee_Profile)
                    .Include(i => i.User_Profile.User_Authentication)
                    .Include(i => i.Inventory_Prefix_Config)
                    .Include(i => i.Invoice_has_Quotation)
                    .Include(i => i.Quotation_Approval)
                    .Include(i => i.Quotation_Approval.Select(s => s.User_Profile))
                    .Include(i => i.Quotation_Approval.Select(s => s.User_Profile1))
                    .Include(i => i.CRM_Details)
                    .Where(w => w.Quotation_ID == pQuotationID && ir.Contains(w.Profile_ID))
                    .FirstOrDefault();
                }
                else
                {
                    return db.Quotations
                   .Include(i => i.Customer_Company)
                   .Include(i => i.Invoice_has_Quotation)
                   .Include(i => i.Quotation_has_Product)
                   .Include(i => i.Quotation_has_Product.Select(s => s.Product))
                   .Include(i => i.Quotation_Payment_Term)
                   .Include(i => i.User_Profile)
                   .Include(i => i.User_Profile.Employee_Profile)
                   .Include(i => i.User_Profile.User_Authentication)
                   .Include(i => i.Inventory_Prefix_Config)
                   .Include(i => i.Quotation_Approval)
                   .Include(i => i.Quotation_Approval.Select(s => s.User_Profile))
                   .Include(i => i.Quotation_Approval.Select(s => s.User_Profile1))
                   .Include(i => i.CRM_Details)
                   .Where(w => w.Quotation_ID == pQuotationID && w.Profile_ID == pProfileID)
                   .FirstOrDefault();
                }

            }
        }

        public Quotation GetQuotation(string pQuotationRefNo, int pProfileID, string status = "", Nullable<bool> pQuoationManual = null)
        {
            using (var db = new SBS2DBContext())
            {
                var iagroups = db.IAs.Where(w => w.Profile_ID == pProfileID & w.IG.Type == InventoryType.Quotation).Select(s => s.IG_ID);

                var quotations = db.Quotations
                    .Include(i => i.Customer_Company)
                    .Include(i => i.Invoice_has_Quotation)
                    .Include(i => i.Quotation_has_Product)
                    .Include(i => i.Quotation_has_Product.Select(s => s.Product))
                    .Include(i => i.Quotation_Payment_Term)
                    .Include(i => i.User_Profile)
                    .Include(i => i.Inventory_Prefix_Config)
                    .Include(i => i.CRM_Details)
                    .Where(w => w.Quotation_Ref_No == pQuotationRefNo);

                if (!string.IsNullOrEmpty(status))
                {
                    quotations = quotations.Where(w => w.Overall_Status == status);
                }
                if (pQuoationManual.HasValue)
                {
                    quotations = quotations.Where(w => w.Quotation_Manual == pQuoationManual);
                }
                if (iagroups.Count() > 0)
                {
                    // have approval role
                    var ir = db.IRs.Where(w => iagroups.Contains(w.IG_ID)).Select(s => s.Profile_ID);

                    return quotations.Where(w => ir.Contains(w.Profile_ID)).FirstOrDefault();
                }
                else
                {
                    return quotations.Where(w => w.Profile_ID == pProfileID).FirstOrDefault();
                }

            }
        }

        public Quotation GetQuotation(string pQuotationRefNo, string status = "", Nullable<bool> pQuoationManual = null)
        {
            using (var db = new SBS2DBContext())
            {

                var quotations = db.Quotations
                    .Include(i => i.Customer_Company)
                    .Include(i => i.Invoice_has_Quotation)
                    .Include(i => i.Quotation_has_Product)
                    .Include(i => i.Quotation_has_Product.Select(s => s.Product))
                    .Include(i => i.User_Profile)
                    .Include(i => i.Inventory_Prefix_Config)
                    .Include(i => i.CRM_Details)
                    .Where(w => w.Quotation_Ref_No == pQuotationRefNo);

                if (!string.IsNullOrEmpty(status))
                {
                    quotations = quotations.Where(w => w.Overall_Status == status);
                }
                if (pQuoationManual.HasValue)
                {
                    quotations = quotations.Where(w => w.Quotation_Manual == pQuoationManual);
                }
                return quotations.FirstOrDefault();

            }
        }



        public List<Quotation> GetCustomerQuotations(int pCustomerCompanyID, int pCompanyID, string status = "")
        {
            using (var db = new SBS2DBContext())
            {

                var quotations = db.Quotations
                    .Include(i => i.Customer_Company)
                    .Include(i => i.Invoice_has_Quotation)
                    .Include(i => i.Quotation_has_Product)
                    .Include(i => i.Quotation_has_Product.Select(s => s.Product))
                    .Include(i => i.User_Profile)
                    .Include(i => i.Inventory_Prefix_Config)
                    .Include(i => i.CRM_Details).Where(w => w.Customer_Company_ID == pCustomerCompanyID && w.Company_ID == pCompanyID);

                if (!string.IsNullOrEmpty(status))
                {
                    quotations = quotations.Where(w => w.Overall_Status == status);
                }

                return quotations.ToList();
            }
        }

        public List<Quotation> GetRelationQuotation(string pQuotationRefNo)
        {
            using (var db = new SBS2DBContext())
            {
                var quotations = db.Quotations.Where(w => w.Quotation_Ref_No == pQuotationRefNo).FirstOrDefault();
                var allinvoice = db.Invoice_has_Quotation.Where(w => w.Quotation_ID == quotations.Quotation_ID).Select(s => s.Invoice_ID);
                var allquotation = db.Invoice_has_Quotation.Where(w => allinvoice.Contains(w.Invoice_ID)).Select(s => s.Quotation_ID);

                if (allquotation != null && allquotation.Count() > 0)
                {
                    return db.Quotations
                      .Include(i => i.Customer_Company)
                      .Include(i => i.Invoice_has_Quotation)
                      .Include(i => i.Quotation_has_Product)
                      .Include(i => i.Quotation_has_Product.Select(s => s.Product))
                      .Include(i => i.User_Profile)
                      .Include(i => i.Inventory_Prefix_Config)
                      .Where(w => allquotation.Contains(w.Quotation_ID)).ToList();
                }
                else
                {
                    return db.Quotations
                     .Include(i => i.Customer_Company)
                     .Include(i => i.Quotation_has_Product)
                     .Include(i => i.Quotation_has_Product.Select(s => s.Product))
                     .Include(i => i.User_Profile)
                     .Include(i => i.Inventory_Prefix_Config)
                     .Where(w => w.Quotation_ID == quotations.Quotation_ID).ToList();

                }

            }
        }

        public ServiceResult InsertQuotation(Quotation pQuotation, Customer_Company pCompany,
            List<Quotation_has_Product> pQuotationItems, List<string> pItemRowsType,
            List<Quotation_Payment_Term> pQuotationPaymentTerms, List<string> pPaymentRowsType,
            int pUserlogin)
        {

            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    // Edit Company Detail
                    if (pCompany.Customer_Company_ID > 0)
                    {
                        // update customer company
                        var curentcompany = db.Customer_Company.Where(w => w.Customer_Company_ID == pCompany.Customer_Company_ID).FirstOrDefault();
                        if (curentcompany == null)
                            return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Company" };

                        db.Entry(curentcompany).CurrentValues.SetValues(pCompany);
                    }
                    else
                    {
                        // insert
                        var curentcompany = db.Customer_Company.Where(w => w.Company_Name == pCompany.Company_Name).FirstOrDefault();
                        if (curentcompany != null)
                        {

                            curentcompany.Email = pCompany.Email;
                            curentcompany.Person_In_Charge = pCompany.Person_In_Charge;
                            curentcompany.Office_Phone = pCompany.Office_Phone;
                            curentcompany.Billing_Address = pCompany.Billing_Address;
                            curentcompany.Billing_Street = pCompany.Billing_Street;
                            curentcompany.Billing_Country_ID = pCompany.Billing_Country_ID;
                            curentcompany.Billing_State_ID = pCompany.Billing_State_ID;
                            curentcompany.Billing_City = pCompany.Billing_City;
                            curentcompany.Billing_Postal_Code = pCompany.Billing_Postal_Code;
                            pQuotation.Customer_Company_ID = curentcompany.Customer_Company_ID;
                        }
                        else
                        {
                            pQuotation.Customer_Company = pCompany;
                        }

                    }

                    // Insert Quotation and Get Quotation Configuretion
                    if (!pQuotation.Quotation_Config_ID.HasValue)
                        return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Quotation Configuration" };

                    var config = db.Inventory_Prefix_Config.Where(w => w.Prefix_Config_ID == pQuotation.Quotation_Config_ID).FirstOrDefault();

                    if (config == null)
                        return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Quotation Configuration" };

                    var nextNumber = config.Ref_Count.HasValue ? config.Ref_Count.Value : 1;
                    var quotation_ref = config.Prefix_Ref_No + "-" + currentdate.Year.ToString().Substring(2, 2) + nextNumber.ToString().PadLeft(config.Number_Of_Digit.HasValue ? config.Number_Of_Digit.Value : 5, '0');

                    config.Ref_Count = config.Ref_Count + 1;

                    pQuotation.Quotation_Ref_No = quotation_ref;
                    pQuotation.Rev_ID = 0;
                    pQuotation.Create_On = currentdate;

                    //---------quotation item----------
                    if (pQuotationItems != null && pItemRowsType != null)
                    {
                        var i = 0;
                        foreach (var row in pQuotationItems)
                        {
                            row.Rev_ID = pQuotation.Rev_ID.Value;
                            row.Create_By = pQuotation.Create_By;
                            row.Create_On = pQuotation.Create_On;

                            if (pItemRowsType[i] == RowType.ADD | pItemRowsType[i] == RowType.EDIT)
                            {
                                pQuotation.Quotation_has_Product.Add(row);
                            }
                            i++;
                        }
                    }

                    //---------quotation item----------
                    if (pQuotationPaymentTerms != null && pPaymentRowsType != null)
                    {
                        var i = 0;
                        foreach (var row in pQuotationPaymentTerms)
                        {
                            row.Rev_ID = pQuotation.Rev_ID.Value;
                            row.Create_By = pQuotation.Create_By;
                            row.Create_On = pQuotation.Create_On;

                            if (pPaymentRowsType[i] == RowType.ADD | pPaymentRowsType[i] == RowType.EDIT)
                            {
                                pQuotation.Quotation_Payment_Term.Add(row);
                            }
                            i++;
                        }
                    }


                    db.Quotations.Add(pQuotation);
                    db.SaveChanges();
                    db.Entry(pQuotation).GetDatabaseValues();

                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = "Quotation" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = "Quotation" };
            }
        }

        public ServiceResult UpdateQuotation(Quotation pQuotation, Customer_Company pCompany, List<Quotation_has_Product> pQuotationItems, List<string> pItemRowsType,
            List<Quotation_Payment_Term> pQuotationPaymentTerms, List<string> pPaymentRowsType,
            int pUserlogin)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    // Edit Company Detail
                    if (pCompany.Customer_Company_ID > 0)
                    {
                        // update customer company
                        var cuurentcompany = db.Customer_Company.Where(w => w.Customer_Company_ID == pCompany.Customer_Company_ID).FirstOrDefault();
                        if (cuurentcompany == null)
                            return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Company" };

                        db.Entry(cuurentcompany).CurrentValues.SetValues(pCompany);
                    }
                    else
                    {
                        // insert
                        var curentcompany = db.Customer_Company.Where(w => w.Company_Name == pCompany.Company_Name).FirstOrDefault();
                        if (curentcompany != null)
                        {
                            curentcompany.Billing_Address = pCompany.Billing_Address;
                            curentcompany.Billing_Street = pCompany.Billing_Street;
                            curentcompany.Billing_Country_ID = pCompany.Billing_Country_ID;
                            curentcompany.Billing_State_ID = pCompany.Billing_State_ID;
                            curentcompany.Billing_City = pCompany.Billing_City;
                            curentcompany.Billing_Postal_Code = pCompany.Billing_Postal_Code;

                            curentcompany.Email = pCompany.Email;
                            curentcompany.Person_In_Charge = pCompany.Person_In_Charge;
                            curentcompany.Office_Phone = pCompany.Office_Phone;
                            pQuotation.Customer_Company_ID = curentcompany.Customer_Company_ID;
                        }
                        else
                        {
                            db.Customer_Company.Add(pCompany);
                            db.SaveChanges();
                            db.Entry(pCompany).GetDatabaseValues();
                            pQuotation.Customer_Company_ID = pCompany.Customer_Company_ID;
                        }

                    }

                    // Insert Quotation and
                    var current = (from a in db.Quotations where a.Quotation_ID == pQuotation.Quotation_ID select a).FirstOrDefault();
                    if (current != null)
                    {
                        pQuotation.Create_On = current.Create_On;
                        pQuotation.Create_By = current.Create_By;
                        pQuotation.Update_On = currentdate;

                        if (pQuotationItems != null && pItemRowsType != null)
                        {
                            var i = 0;
                            foreach (var row in pQuotationItems)
                            {
                                if (row.Quotation_Product_ID > 0)
                                {
                                    if (pItemRowsType[i] == RowType.DELETE)
                                    {
                                        db.Entry(row).State = EntityState.Deleted;
                                    }
                                    else
                                    {
                                        var curItem = (from a in db.Quotation_has_Product
                                                       where a.Quotation_Product_ID == row.Quotation_Product_ID
                                                       select a).FirstOrDefault();

                                        if (curItem != null)
                                        {
                                            if (curItem.Rev_ID != pQuotation.Rev_ID)
                                            {
                                                row.Rev_ID = pQuotation.Rev_ID;
                                                row.Quotation_ID = pQuotation.Quotation_ID;
                                                row.Create_By = pQuotation.Update_By;
                                                row.Create_On = pQuotation.Update_On;

                                                db.Entry(row).State = EntityState.Added;
                                            }
                                            else
                                            {
                                                curItem.Rev_ID = pQuotation.Rev_ID;
                                                curItem.Amount = row.Amount;
                                                curItem.Discount = row.Discount;
                                                curItem.Discount_Type = row.Discount_Type;
                                                curItem.Product_ID = row.Product_ID;
                                                curItem.Unit = row.Unit;
                                                curItem.Unit_Price = row.Unit_Price;
                                                curItem.Quotation_Item_Name = row.Quotation_Item_Name;
                                                curItem.Quotation_Item_Description = row.Quotation_Item_Description;

                                                db.Entry(curItem).State = EntityState.Modified;
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    if (pItemRowsType[i] == RowType.ADD | pItemRowsType[i] == RowType.EDIT)
                                    {
                                        row.Rev_ID = pQuotation.Rev_ID;
                                        row.Quotation_ID = pQuotation.Quotation_ID;
                                        row.Create_By = pQuotation.Update_By;
                                        row.Create_On = pQuotation.Update_On;
                                        //pQuotation.Quotation_has_Product.Add(row);
                                        db.Entry(row).State = EntityState.Added;
                                    }
                                }
                                i++;
                            }
                        }

                        //---------quotation payment----------
                        if (pQuotationPaymentTerms != null && pPaymentRowsType != null)
                        {
                            var i = 0;
                            foreach (var row in pQuotationPaymentTerms)
                            {
                                if (row.Quotation_Payment_ID > 0)
                                {
                                    if (pPaymentRowsType[i] == RowType.DELETE)
                                    {
                                        db.Entry(row).State = EntityState.Deleted;
                                    }
                                    else
                                    {
                                        var curItem = (from a in db.Quotation_Payment_Term
                                                       where a.Quotation_Payment_ID == row.Quotation_Payment_ID
                                                       select a).FirstOrDefault();

                                        if (curItem != null)
                                        {

                                            if (curItem.Rev_ID != pQuotation.Rev_ID)
                                            {
                                                row.Rev_ID = pQuotation.Rev_ID.Value;
                                                row.Quotation_ID = pQuotation.Quotation_ID;
                                                row.Create_By = pQuotation.Create_By;
                                                row.Create_On = pQuotation.Create_On;

                                                db.Entry(row).State = EntityState.Added;
                                            }
                                            else
                                            {

                                                curItem.Rev_ID = pQuotation.Rev_ID.Value;
                                                curItem.Amount = row.Amount;
                                                curItem.Amount_Type = row.Amount_Type;
                                                curItem.Payment_Method = row.Payment_Method;
                                                curItem.Payment_Method_ID = row.Payment_Method_ID;
                                                curItem.Payment_Term = row.Payment_Term;
                                                curItem.Payment_Term_ID = row.Payment_Term_ID;

                                                db.Entry(curItem).State = EntityState.Modified;
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    if (pPaymentRowsType[i] == RowType.ADD | pPaymentRowsType[i] == RowType.EDIT)
                                    {
                                        row.Rev_ID = pQuotation.Rev_ID.Value;
                                        row.Quotation_ID = pQuotation.Quotation_ID;
                                        row.Create_By = pQuotation.Create_By;
                                        row.Create_On = pQuotation.Create_On;

                                        db.Entry(row).State = EntityState.Added;
                                    }
                                }
                                i++;
                            }
                        }

                        db.Entry(current).CurrentValues.SetValues(pQuotation);
                        db.SaveChanges();
                    }

                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Quotation" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = "Quotation" };
            }
        }

        public ServiceResult UpdateQuotationSignature(int pQuotationID, string pSig)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var current = (from a in db.Quotations where a.Quotation_ID == pQuotationID select a).FirstOrDefault();
                    if (current != null)
                    {
                        current.Signature = pSig;
                    }
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Quotation" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = "Quotation" };
            }
        }

        //Added by Nay on 19-Jun-2015
        public List<Quotation> LstQuotationDetails(int pProfileID, string pStartDate, string pEndDate, int pClient, string pStatus = "", bool pOrderDesc = false, List<Nullable<int>> requestIDs = null)
        {
            using (var db = new SBS2DBContext())
            {
                var quoList = (from a in db.Quotations
                               .Include(i => i.Customer_Company)
                                .Include(i => i.Company)
                                .Include(i => i.Company.Company_Details)
                                   .Include(i => i.Invoice_has_Quotation)
                                   .Include(i => i.Quotation_has_Product)
                                   .Include(i => i.Quotation_has_Product.Select(s => s.Product))
                                   .Include(i => i.Quotation_Payment_Term)
                                   .Include(i => i.User_Profile)
                                   .Include(i => i.Inventory_Prefix_Config)
                               select a);

                if (requestIDs != null)
                {
                    quoList = quoList.Where(w => w.Profile_ID == pProfileID || (requestIDs.Contains(w.Request_ID.Value)));
                }
                else
                {
                    quoList = quoList.Where(w => (w.Profile_ID == pProfileID && w.Related_Quotation_ID == null));
                }

                if (pStartDate != null || pEndDate != null)
                {
                    var date1 = DateTime.ParseExact(pStartDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    var date2 = DateTime.ParseExact(pEndDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                    quoList = quoList.Where(w => w.To_Date >= date1 && w.To_Date <= date2);
                }

                if (pClient > 0)
                {
                    quoList = quoList.Where(w => w.Company_ID == pClient);
                }

                if (!string.IsNullOrEmpty(pStatus))
                {
                    quoList = quoList.Where(w => w.Overall_Status == pStatus);
                }

                if (pOrderDesc)
                {
                    quoList = quoList.OrderByDescending(w => w.Quotation_ID);
                }
                else
                {
                    quoList = quoList.OrderBy(w => w.Quotation_ID);
                }

                return quoList.ToList();
            }
        }
        public Quotation GetQuotationByRequestID(int pRequestID)
        {
            using (var db = new SBS2DBContext())
            {
                var quotations = db.Quotations
                    .Include(i => i.Customer_Company)
                    .Include(i => i.Invoice_has_Quotation)
                    .Include(i => i.Quotation_has_Product)
                    .Include(i => i.Quotation_has_Product.Select(s => s.Product))
                    .Include(i => i.User_Profile)
                    .Include(i => i.Inventory_Prefix_Config)
                    .Include(i => i.CRM_Details)
                    .Where(w => w.Request_ID == pRequestID);

                return quotations.FirstOrDefault();
            }
        }

        //Added by Nay on 19-Jun-2015
        public List<Customer_Company> getCustomerCompanies(Nullable<int> pCompanyID)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.Customer_Company where a.Company_ID == pCompanyID select a).ToList();
            }
        }

        //Added by Nay on 19-Jun-2015
        public List<string> getApprovalStatge()
        {
            using (var db = new SBS2DBContext())
            {
                var q = (from a in db.Quotations select a.Quotation_Stage).Distinct();

                return q.ToList();
            }
        }

        //Added by Nay on 20-Jun-2015
        public string getUserName(int user_ID)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.User_Profile
                        where a.Profile_ID == user_ID
                        select a.Name).SingleOrDefault();
            }
        }

        //Added by Nay on 20-Jun-2015
        public string getCompanyName(int pCompany_ID)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.Company_Details select a.Name).FirstOrDefault();
            }
        }

        //Added by Nay on 20-Jun-2015
        public List<Quotation> LstQuotationsDetail(int pProfileID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.Quotations
                    .Include(i => i.Quotation_Approval)
                    .Include(i => i.Quotation_has_Product)
                    .Include(i => i.Quotation_has_Product.Select(s => s.Product))
                    .Where(w => w.Profile_ID == pProfileID)
                    .OrderByDescending(o => o.To_Date)
                    .ThenByDescending(o => o.Quotation_Ref_No)
                    .ToList();
            }
        }

        //Added by Nay on 20-Jun-2015
        public List<Quotation> LstQuotationPendingDetails(int pProfileID, string startDate, string endDate, int Client, string PreparedBy)
        {
            SBSWorkFlowAPI.Service svc = new SBSWorkFlowAPI.Service();

            using (var db = new SBS2DBContext())
            {
                var quoList = (from a in db.Quotations
                               .Include(i => i.Customer_Company)
                                .Include(i => i.Company)
                                .Include(i => i.Company.Company_Details)
                                   .Include(i => i.Invoice_has_Quotation)
                                   .Include(i => i.Quotation_has_Product)
                                   .Include(i => i.Quotation_has_Product.Select(s => s.Product))
                                   .Include(i => i.User_Profile)
                                   .Include(i => i.Inventory_Prefix_Config)
                               .Where(w => w.Profile_ID == pProfileID && w.Overall_Status == "P")
                               select a);

                //if (startDate != null || endDate != null) {
                //    var date1 = DateTime.ParseExact(startDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                //    var date2 = DateTime.ParseExact(endDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                //    quoList = quoList.Where(w => w.To_Date >= date1 && w.To_Date <= date2);
                //}

                //if (Client > 0) {
                //    quoList = quoList.Where(w => w.Company_ID == Client);
                //}

                //if (PreparedBy != null) {
                //    quoList = quoList.Where(w => w.Create_By == PreparedBy);
                //}

                return quoList.ToList();
            }
        }

        //Added by Nay on 20-Jun-2015
        public string getPreparedName(string email)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.User_Profile where a.Email == email select a.Name).SingleOrDefault();
            }
        }

        //Added by Nay on 20-Jun-2015
        public List<Quotation> LstQuotationRejApprDetails(int pProfileID, string startDate, string endDate, int Client, string PreparedBy)
        {
            using (var db = new SBS2DBContext())
            {
                var quoList = (from a in db.Quotations
                               .Include(i => i.Customer_Company)
                                .Include(i => i.Company)
                                .Include(i => i.Company.Company_Details)
                                .Include(i => i.Invoice_has_Quotation)
                                .Include(i => i.Quotation_has_Product)
                                .Include(i => i.Quotation_has_Product.Select(s => s.Product))
                                .Include(i => i.User_Profile)
                                .Include(i => i.Inventory_Prefix_Config)
                               .Where(w => w.Profile_ID == pProfileID && (w.Overall_Status == "R" || w.Overall_Status == "A"))
                               select a);

                if (startDate != null || endDate != null)
                {
                    var date1 = DateTime.ParseExact(startDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    var date2 = DateTime.ParseExact(endDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    quoList = quoList.Where(w => w.To_Date >= date1 && w.To_Date <= date2);
                }

                if (Client > 0)
                {
                    quoList = quoList.Where(w => w.Company_ID == Client);
                }

                if (PreparedBy != null)
                {
                    quoList = quoList.Where(w => w.Create_By == PreparedBy);
                }

                return quoList.ToList();
            }
        }

        //Added by Nay on 21-Jun-2015
        public string getUserName(string email)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.User_Profile where a.Email == email select a.Name).SingleOrDefault();
            }
        }

        //Added by Nay on 21-Jun-2015
        public string getEmailAddr(int pProfile_ID)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.User_Profile where a.Profile_ID == pProfile_ID select a.Email).SingleOrDefault();
            }
        }

        //Added by Nay on 22-Jun-2015
        public List<Quotation_Delegation> LstQuotationDelegated(int pProfile_ID)
        {
            using (var db = new SBS2DBContext())
            {
                var q = (from a in db.Quotation_Delegation
                         where a.Curr_Profile_Id == pProfile_ID
                         select a);

                return q.ToList();
            }
        }

        //Added by Nay on 22-Jun-2015
        public bool chkValidDelegation(int pProfile, string startDate, string endDate)
        {
            Boolean chkValue = false;
            using (var db = new SBS2DBContext())
            {
                var date1 = DateTime.ParseExact(startDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                var date2 = DateTime.ParseExact(endDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                var q = (from a in db.Quotation_Delegation
                         where (date1 >= a.From_Date && date1 <= a.To_Date) || (date2 >= a.From_Date && date2 <= a.To_Date)
                         select a).ToList();

                if (q.Count > 0)
                {
                    chkValue = true;
                }
                return chkValue;
            }
        }

        //Added by Nay on 22-Jun-2015
        public int SaveDelegation(Quotation_Delegation quoDel)
        {
            int value = 0;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Quotation_Delegation.Add(quoDel);
                    db.SaveChanges();
                }
                return value;
            }
            catch
            {
                return -500;
            }
        }
        //Added by Nay on 06-Jul-2015
        public int SaveTemplate(QuotationTemplate model)
        {
            int value = 0;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var current = (from quo in db.QuotationTemplates where quo.Template_ID == model.Template_ID select quo).FirstOrDefault();
                    if (current != null)
                    {
                        current.Template_Name = model.Template_Name;
                        current.Header_1 = model.Header_1;
                        current.Header_2 = model.Header_2;
                        current.Header_3 = model.Header_3;
                        current.Header_4 = model.Header_4;
                        current.Body_1 = model.Body_1;
                        current.Footer_1 = model.Footer_1;
                        current.Footer_2 = model.Footer_2;
                        current.Footer1_Remark = model.Footer1_Remark;
                        current.Footer2_Remark = model.Footer2_Remark;
                        current.Footer_3 = model.Footer_3;
                        current.Footer_4 = model.Footer_4;
                        current.FontName = model.FontName;
                        current.FontSize = model.FontSize;
                        current.Profile_ID = model.Profile_ID;
                        current.Company_ID = model.Company_ID;
                        current.Update_By = model.Update_By;
                        current.Update_On = model.Update_On;

                        db.Entry(current).CurrentValues.SetValues(current);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.QuotationTemplates.Add(model);
                        db.SaveChanges();
                    }
                }
                return value;
            }
            catch
            {
                return -500;
            }
        }
        //Added by Nay on 06-Jul-2015 
        public QuotationTemplate GetQuotationTemplate(int TemplateID)
        {
            var db = new SBS2DBContext();
            var q = (from temp in db.QuotationTemplates
                     where temp.Template_ID == TemplateID
                     select temp).SingleOrDefault();
            return q;
        }

        //Added by Nay on 10-Jul-2015 
        public List<QuotationTemplate> GetQuotationTemplateList(int pCompanyID)
        {
            var db = new SBS2DBContext();
            var q = (from temp in db.QuotationTemplates
                        .Include(i => i.User_Profile)
                        .Include(i => i.Company)
                        .Where(i => i.Company_ID == pCompanyID)
                     select temp);
            return q.ToList();
        }

        public bool HideQuotationPayment(int pPaymentID)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var qPayment = (from a in db.Quotation_Payment_Term
                                    where (a.Quotation_Payment_ID == pPaymentID)
                                    select a).FirstOrDefault();

                    qPayment.Is_Override = true;

                    db.Entry(qPayment).State = EntityState.Modified;
                    db.SaveChanges();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }

    public class Quotation_Revision
    {
        public int Quotation_ID { get; set; }
        public string Quotation_Ref_No { get; set; }
        public int Rev_ID { get; set; }
        public decimal Amount { get; set; }
        public string Company_Name { get; set; }
        public string Create_By { get; set; }
        public Nullable<DateTime> Create_On { get; set; }
        public bool Has_Invoices { get; set; }

        public string Overall_Status { get; set; }
    }

}
