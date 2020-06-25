using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using SBSModel.Common;
using SBSModel.Models;

namespace SBSModel.Models
{
    public class SaleOrderService
    {
        public List<Sale_Order> LstSaleOrder(int pCompanyID, int pProfileID)
        {
            using (var db = new SBS2DBContext())
            {
                var iagroups = db.IAs.Where(w => w.Profile_ID == pProfileID & w.IG.Type == InventoryType.SalesOrder).Select(s => s.IG_ID);

                var saleorderlist = db.Sale_Order
                    .Include(i => i.Customer_Company)
                    .Include(i => i.Sale_Order_has_Product)
                    //.Include(i => i.Sale_Order_Approval)
                    .Include(i => i.User_Profile)
                    .Include(i => i.Inventory_Prefix_Config)
                    .Where(w => w.Company_ID == pCompanyID);

                if (iagroups.Count() > 0)
                {
                    var ir = db.IRs.Where(w => iagroups.Contains(w.IG_ID)).Select(s => s.Profile_ID);
                    saleorderlist = saleorderlist.Where(w => ir.Contains(w.Profile_ID));
                }
                else
                {
                    saleorderlist = saleorderlist.Where(w => w.Profile_ID == pProfileID);
                }
                return saleorderlist.OrderByDescending(o => o.Sale_Order_Date).ThenByDescending(o => o.Sale_Order_Ref_No).ToList();

            }
        }

        public Sale_Order GetSaleOrder(int pSaleOrderID, int pProfileID, ref bool hasApproval1, ref bool hasApproval2)
        {
            using (var db = new SBS2DBContext())
            {
                var iagroups = db.IAs.Where(w => w.Profile_ID == pProfileID & w.IG.Type == InventoryType.SalesOrder);

                var saleorderlist = db.Sale_Order
                    .Include(i => i.Customer_Company)
                    .Include(i => i.Sale_Order_has_Product)
                    .Include(i => i.Sale_Order_has_Product.Select(s => s.Product))
                    .Include(i => i.User_Profile)
                    .Include(i => i.Inventory_Prefix_Config)
                    //.Include(i => i.Sale_Order_Approval)
                    //.Include(i => i.Sale_Order_Approval.Select(s => s.User_Profile))
                    //.Include(i => i.Sale_Order_Approval.Select(s => s.User_Profile1))
                    .Include(i => i.SO_Return)
                    .Include(i => i.SO_Return.Select(s => s.Inventory_Transaction))
                    .Where(w => w.Sale_Order_ID == pSaleOrderID);

                if (iagroups.Select(s => s.IG_ID).Count() > 0)
                {
                    var ir = db.IRs.Where(w => iagroups.Select(s => s.IG_ID).Contains(w.IG_ID)).Select(s => s.Profile_ID);

                    if (iagroups.Where(w => w.Is_Approval1 == true).FirstOrDefault() != null)
                        hasApproval1 = true;
                    if (iagroups.Where(w => w.Is_Approval2 == true).FirstOrDefault() != null)
                        hasApproval2 = true;

                    saleorderlist = saleorderlist.Where(w => ir.Contains(w.Profile_ID));
                }
                else
                {
                    saleorderlist = saleorderlist.Where(w => w.Profile_ID == pProfileID);
                }
                return saleorderlist.FirstOrDefault();

            }
        }

        public Sale_Order getSaleOrder(int Company_ID, string Sale_Order_Ref_No)
        {
            Sale_Order so = null;
            using (var db = new SBS2DBContext())
            {
                so = db.Sale_Order
                    .Include(i => i.Customer_Company)
                    .Include(i => i.Sale_Order_has_Product)
                    .Include(i => i.Sale_Order_has_Product.Select(s => s.Product))
                    .Include(i => i.User_Profile)
                    .Include(i => i.Withdraws)
                    .Include(i => i.Withdraws.Select(s => s.Inventory_Transaction))
                    .Include(i => i.SO_Return)
                    .Include(i => i.SO_Return.Select(s => s.Inventory_Transaction))
                    .Where(w => w.Company_ID == Company_ID && w.Sale_Order_Ref_No == Sale_Order_Ref_No).FirstOrDefault();
            }
            return so;
        }

        public List<Sale_Order> LstSaleOrdersManagement(int pCompany_ID, int pApprovalProfileID, ref bool hasApproval1, ref bool hasApproval2)
        {
            using (var db = new SBS2DBContext())
            {
                var slist = new List<Sale_Order>();
                var iagroups = db.IAs.Where(w => w.Profile_ID == pApprovalProfileID & w.IG.Type == InventoryType.SalesOrder & (w.Is_Approval1 == true | w.Is_Approval2 == true));

                if (iagroups.Count() > 0)
                {
                    // have approval role
                    var ir = db.IRs.Where(w => iagroups.Select(s => s.IG_ID).Contains(w.IG_ID)).Select(s => s.Profile_ID);

                    var sales = db.Sale_Order
                        .Include(i => i.Customer_Company)
                        //.Include(i => i.Sale_Order_Approval)
                        //.Include(i => i.Sale_Order_Approval.Select(s => s.User_Profile))
                        //.Include(i => i.Sale_Order_Approval.Select(s => s.User_Profile1))
                        .Where(w => w.Company_ID == pCompany_ID && ir.Contains(w.Profile_ID));
                    //.OrderByDescending(o => o.Sale_Order_ID);

                    if (iagroups.Where(w => w.Is_Approval1 == true).FirstOrDefault() != null)
                        hasApproval1 = true;
                    if (iagroups.Where(w => w.Is_Approval2 == true).FirstOrDefault() != null)
                        hasApproval2 = true;

                    foreach (var q in sales)
                    {
                        if (q.Overall_Status == LeaveStatus.Pending || string.IsNullOrEmpty(q.Overall_Status))
                        {
                            //var approval = q.Sale_Order_Approval.OrderByDescending(o => o.Sale_Order_Approval_ID).FirstOrDefault();// get last approval
                            //if (approval != null)
                            //{
                            //    // have approval do
                            //    if (approval.Approval_1.HasValue)
                            //    {
                            //        // goto apprval2 approve
                            //        if (hasApproval2 == true | hasApproval1 == true)
                            //        {
                            //            slist.Add(q);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    // approval 2 can't see this row.
                            //    if (hasApproval1 == true)
                            //    {
                            //        slist.Add(q);
                            //    }
                            //}

                            slist.Add(q);
                        }
                        else
                        {
                            // all approval can see this row.
                            slist.Add(q);
                        }
                    }
                }

                slist = slist.OrderByDescending(o => o.Sale_Order_Date).ToList();

                return slist;
            }
        }

        public ServiceResult InsertSaleOrder(Sale_Order pSaleOrder, Customer_Company pCompany, Sale_Order_has_Product[] pSaleOrderProductItems, string[] pRowsType, string domain)
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

                            pSaleOrder.Customer_Company_ID = curentcompany.Customer_Company_ID;
                        }
                        else
                        {
                            pSaleOrder.Customer_Company = pCompany;
                        }

                    }

                    // Insert Quotation and Get Quotation Configuretion
                    if (!pSaleOrder.Sale_Order_Config_ID.HasValue)
                        return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Sale Order Prefix" };

                    var config = db.Inventory_Prefix_Config.Where(w => w.Prefix_Config_ID == pSaleOrder.Sale_Order_Config_ID).FirstOrDefault();

                    if (config == null)
                        return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Sale Order Prefix" };

                    var nextNumber = config.Ref_Count.HasValue ? config.Ref_Count.Value : 1;
                    var saleorder_ref = config.Prefix_Ref_No + "-" + currentdate.Year.ToString().Substring(2, 2) + nextNumber.ToString().PadLeft(config.Number_Of_Digit.HasValue ? config.Number_Of_Digit.Value : 5, '0');

                    config.Ref_Count = config.Ref_Count + 1;

                    pSaleOrder.Sale_Order_Ref_No = saleorder_ref;
                    pSaleOrder.Create_On = currentdate;

                    decimal discount = 0;
                    decimal totalprice = 0;

                    //---------sale order product----------
                    if (pSaleOrderProductItems != null && pRowsType != null)
                    {
                        var i = 0;
                        foreach (var row in pSaleOrderProductItems)
                        {
                            if (pRowsType[i] == RowType.ADD)
                            {
                                if (row.UOM == 0)
                                    row.UOM = null;

                                row.Product = null;
                                pSaleOrder.Sale_Order_has_Product.Add(row);

                                if (row.Discount_Type == "P")
                                {
                                    discount = discount + ((row.Amount.HasValue ? row.Amount.Value : 0) * ((row.Discount.HasValue ? row.Discount.Value : 0) / 100));
                                }
                                else
                                {
                                    discount = discount + (row.Discount.HasValue ? row.Discount.Value : 0);
                                }
                                totalprice = totalprice + (row.Amount.HasValue ? row.Amount.Value : 0);
                            }
                            i++;
                        }
                    }
                    pSaleOrder.Total_Price = totalprice;
                    pSaleOrder.Discount = discount;
                    db.Sale_Order.Add(pSaleOrder);
                    db.SaveChanges();


                    //var irs = db.IRs.Where(w => w.Profile_ID == pSaleOrder.Profile_ID & w.IG.Type == InventoryType.SalesOrder);
                    //if (irs != null)
                    //{
                    //    IA hasallapproval = null;
                    //    foreach (var ir in irs)
                    //    {
                    //        hasallapproval = db.IAs.Where(w => w.IG_ID == ir.IG_ID & w.Is_Approval1 == true & w.Is_Approval2 == true & w.Profile_ID == pSaleOrder.Profile_ID).FirstOrDefault();
                    //        if (hasallapproval != null)
                    //        {
                    //            break;
                    //        }
                    //    }
                    //    if (hasallapproval != null)
                    //    {
                    //        var result = UpdateSaleOrderStatus(pSaleOrder.Sale_Order_ID, pSaleOrder.Profile_ID.Value, LeaveStatus.Approved, "", domain);
                    //        if (result.Code != ERROR_CODE.SUCCESS)
                    //        {
                    //            return new ServiceResult() { Code = result.Code, Msg = result.Msg };
                    //        }
                    //    }
                    //    else
                    //    {
                    //        foreach (var ir in irs)
                    //        {
                    //            var ias = db.IAs.Where(w => w.IG_ID == ir.IG_ID & w.Is_Approval1 == true);
                    //            foreach (var ia in ias)
                    //            {
                    //                var result = EmailTemplete.sendRequestEmail(ia.User_Profile.User_Authentication.Email_Address, ir.User_Profile.Name, "SalesOrder", ia.User_Profile.Name, pSaleOrder.Sale_Order_ID, domain);
                    //                if (result == false)
                    //                {
                    //                    return new ServiceResult() { Code = ERROR_CODE.ERROR_501_CANT_SEND_EMAIL, Msg = new Error().getError(ERROR_CODE.ERROR_501_CANT_SEND_EMAIL) };
                    //                }
                    //            }
                    //        }
                    //    }


                    //}

                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = "Sale Order" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = "Sale Order" };
            }
        }

        public ServiceResult UpdateSaleOrder(Sale_Order pSaleOrder, Customer_Company pCompany, Sale_Order_has_Product[] pSaleOrderProductItems, string[] pRowsType, List<Inventory_Transaction> pReturnTransaction, Nullable<DateTime> pReturnDate = null)
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
                            curentcompany.Email = pCompany.Email;
                            curentcompany.Person_In_Charge = pCompany.Person_In_Charge;

                            curentcompany.Office_Phone = pCompany.Office_Phone;
                            curentcompany.Billing_Address = pCompany.Billing_Address;
                            curentcompany.Billing_Street = pCompany.Billing_Street;
                            curentcompany.Billing_Country_ID = pCompany.Billing_Country_ID;
                            curentcompany.Billing_State_ID = pCompany.Billing_State_ID;
                            curentcompany.Billing_City = pCompany.Billing_City;
                            curentcompany.Billing_Postal_Code = pCompany.Billing_Postal_Code;

                            pSaleOrder.Customer_Company_ID = curentcompany.Customer_Company_ID;
                        }
                        else
                        {
                            db.Customer_Company.Add(pCompany);
                            db.SaveChanges();
                            db.Entry(pCompany).GetDatabaseValues();
                            pSaleOrder.Customer_Company_ID = pCompany.Customer_Company_ID;
                        }

                    }

                    // Insert Sale Order and
                    var current = (from a in db.Sale_Order where a.Sale_Order_ID == pSaleOrder.Sale_Order_ID select a).FirstOrDefault();
                    if (current != null)
                    {
                        pSaleOrder.Create_On = current.Create_On;
                        pSaleOrder.Create_By = current.Create_By;
                        pSaleOrder.Update_On = currentdate;


                        var currentreturn = db.SO_Return.Where(w => w.Sale_Order_ID == current.Sale_Order_ID).FirstOrDefault();
                        if (currentreturn == null)
                        {
                            //insert return
                            if (pReturnTransaction != null && pReturnTransaction.Count() > 0)
                            {
                                var soreturn = new SO_Return()
                                {
                                    Sale_Order_ID = pSaleOrder.Sale_Order_ID,
                                    Return_By = pSaleOrder.Profile_ID,
                                    Remarks = pSaleOrder.Remarks,
                                    Return_Date = pReturnDate
                                };
                                foreach (var transaction in pReturnTransaction)
                                {
                                    soreturn.Inventory_Transaction.Add(transaction);
                                }
                                db.SO_Return.Add(soreturn);
                            }

                        }
                        else
                        {
                            //update return
                            var transactions = db.Inventory_Transaction.Where(w => w.SO_Return_ID == currentreturn.SO_Return_ID);
                            db.Inventory_Transaction.RemoveRange(transactions);

                            currentreturn.Return_Date = pReturnDate;
                            foreach (var transaction in pReturnTransaction)
                            {
                                transaction.SO_Return_ID = currentreturn.SO_Return_ID;
                                db.Inventory_Transaction.Add(transaction);
                            }
                        }

                        decimal discount = 0;
                        decimal totalprice = 0;
                        if (pSaleOrderProductItems != null && pRowsType != null)
                        {
                            var i = 0;
                            foreach (var row in pSaleOrderProductItems)
                            {
                                row.Product = null;
                                if (row.UOM == 0)
                                    row.UOM = null;

                                if (pRowsType[i] == RowType.ADD)
                                {
                                    row.Sale_Order_has_Product_ID = 0;
                                    row.Sale_Order_ID = pSaleOrder.Sale_Order_ID;
                                    db.Sale_Order_has_Product.Add(row);

                                    if (row.Discount_Type == "P")
                                    {
                                        discount = discount + ((row.Amount.HasValue ? row.Amount.Value : 0) * ((row.Discount.HasValue ? row.Discount.Value : 0) / 100));
                                    }
                                    else
                                    {
                                        discount = discount + (row.Discount.HasValue ? row.Discount.Value : 0);
                                    }
                                    totalprice = totalprice + (row.Amount.HasValue ? row.Amount.Value : 0);
                                }
                                else if (pRowsType[i] == RowType.EDIT)
                                {
                                    var currentitem = (from a in db.Sale_Order_has_Product where a.Sale_Order_has_Product_ID == row.Sale_Order_has_Product_ID select a).FirstOrDefault();
                                    if (currentitem != null)
                                    {
                                        currentitem.Amount = row.Amount;
                                        currentitem.Batch_No = row.Batch_No;
                                        currentitem.Discount = row.Discount;
                                        currentitem.Product_ID = row.Product_ID;
                                        currentitem.Quantity = row.Quantity;
                                        currentitem.Serial_No = row.Serial_No;
                                        currentitem.Unit_Price = row.Unit_Price;
                                        currentitem.Discount_Type = row.Discount_Type;
                                        currentitem.Sale_Order_Item_Description = row.Sale_Order_Item_Description;

                                        if (row.Discount_Type == "P")
                                        {
                                            discount = discount + ((row.Amount.HasValue ? row.Amount.Value : 0) * ((row.Discount.HasValue ? row.Discount.Value : 0) / 100));
                                        }
                                        else
                                        {
                                            discount = discount + (row.Discount.HasValue ? row.Discount.Value : 0);
                                        }

                                        totalprice = totalprice + (row.Amount.HasValue ? row.Amount.Value : 0);
                                    }
                                }
                                else if (pRowsType[i] == RowType.DELETE)
                                {
                                    var currentitem = (from a in db.Sale_Order_has_Product where a.Sale_Order_has_Product_ID == row.Sale_Order_has_Product_ID select a).FirstOrDefault();
                                    if (currentitem != null)
                                    {
                                        db.Sale_Order_has_Product.Remove(currentitem);
                                    }
                                }
                                i++;
                            }

                        }
                        pSaleOrder.Total_Price = totalprice;
                        pSaleOrder.Discount = discount;
                        db.Entry(current).CurrentValues.SetValues(pSaleOrder);
                        db.SaveChanges();
                    }

                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Sale Order" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = "Sale Order" };
            }
        }

        public ServiceResult DeleteSaleOrder(int pSaleOrderID)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var current = (from a in db.Sale_Order where a.Sale_Order_ID == pSaleOrderID select a).FirstOrDefault();
                    if (current != null)
                    {
                        var currentSproduct = (from a in db.Sale_Order_has_Product where a.Sale_Order_ID == pSaleOrderID select a);
                        var currentSreturn = (from a in db.SO_Return where a.Sale_Order_ID == pSaleOrderID select a).FirstOrDefault();
                        if (currentSreturn != null)
                        {
                            var currentStransaction = (from a in db.Inventory_Transaction where a.SO_Return_ID == currentSreturn.SO_Return_ID select a);
                            db.Inventory_Transaction.RemoveRange(currentStransaction);
                            db.SO_Return.Remove(currentSreturn);
                        }

                        db.Sale_Order_has_Product.RemoveRange(currentSproduct);
                        db.Sale_Order.Remove(current);
                        db.SaveChanges();
                    }
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = "Sale Order" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = "Sale Order" };
            }
        }

        public ServiceResult UpdateSaleOrderStatus(int pSaleOrderID, int pApprovalProfileID, string pStatus, string pRemark, string domain)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var iagroups = db.IAs.Where(w => w.Profile_ID == pApprovalProfileID & w.IG.Type == InventoryType.SalesOrder & (w.Is_Approval1 == true | w.Is_Approval2 == true));
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

                        var saleorder = db.Sale_Order.Where(w => w.Sale_Order_ID == pSaleOrderID).FirstOrDefault();
                        if (saleorder != null && approval != null)
                        {

                            if (hasApproval1 == true && hasApproval2 == true)
                            {
                                var saleorderApproval = new Sale_Order_Approval()
                                {
                                    Approval_1 = pApprovalProfileID,
                                    Approval_2 = pApprovalProfileID,
                                    Sale_Order_ID = pSaleOrderID,
                                    Remarks = pRemark,
                                    Status = pStatus
                                };
                                db.Sale_Order_Approval.Add(saleorderApproval);

                                saleorder.Overall_Status = pStatus;
                            }
                            else if (hasApproval1 == true)
                            {
                                var quotationApproval = new Sale_Order_Approval()
                                {
                                    Approval_1 = pApprovalProfileID,
                                    Sale_Order_ID = pSaleOrderID,
                                    Remarks = pRemark,
                                    Status = pStatus
                                };
                                db.Sale_Order_Approval.Add(quotationApproval);

                                if (pStatus == LeaveStatus.Rejected)
                                {
                                    saleorder.Overall_Status = LeaveStatus.Rejected;
                                }
                            }
                            else if (hasApproval2 == true)
                            {
                                var oldquotationApproval = db.Sale_Order_Approval.Where(w => w.Sale_Order_ID == pSaleOrderID).OrderByDescending(o => o.Sale_Order_Approval_ID).FirstOrDefault();

                                var quotationApproval = new Sale_Order_Approval()
                                {
                                    Approval_1 = oldquotationApproval.Approval_1,
                                    Approval_2 = pApprovalProfileID,
                                    Sale_Order_ID = pSaleOrderID,
                                    Remarks = pRemark,
                                    Status = pStatus
                                };
                                db.Sale_Order_Approval.Add(quotationApproval);

                                saleorder.Overall_Status = pStatus;
                            }
                            db.SaveChanges();

                            //// send mail
                            //if (pStatus == LeaveStatus.Approved)
                            //{
                            //    if (hasApproval2 == true)
                            //    {
                            //        var result = EmailTemplete.sendApproveEmail(saleorder.User_Profile.User_Authentication.Email_Address, saleorder.User_Profile.Name, "SaleOrder", approval.Name, saleorder.Sale_Order_ID, domain);
                            //        if (result == false)
                            //        {
                            //            return new ServiceResult() { Code = ERROR_CODE.ERROR_501_CANT_SEND_EMAIL, Msg = new Error().getError(ERROR_CODE.ERROR_501_CANT_SEND_EMAIL) };
                            //        }
                            //    }
                            //    else
                            //    {
                            //        // send request to approval 2
                            //        var groups = db.IRs.Where(w => w.Profile_ID == saleorder.Profile_ID & w.IG.Type == InventoryType.Quotation).Select(s => s.IG_ID);
                            //        if (groups != null)
                            //        {
                            //            var approval2s = db.IAs.Where(w => groups.Contains(w.IG_ID) & w.Is_Approval2 == true);
                            //            foreach (var ia in approval2s)
                            //            {
                            //                var result = EmailTemplete.sendRequestEmail(ia.User_Profile.User_Authentication.Email_Address, saleorder.User_Profile.Name, "SaleOrder", ia.User_Profile.Name, saleorder.Sale_Order_ID, domain);
                            //                if (result == false)
                            //                {
                            //                    return new ServiceResult() { Code = ERROR_CODE.ERROR_501_CANT_SEND_EMAIL, Msg = new Error().getError(ERROR_CODE.ERROR_501_CANT_SEND_EMAIL) };
                            //                }
                            //            }
                            //        }

                            //    }
                            //}
                            //else if (pStatus == LeaveStatus.Rejected)
                            //{
                            //    var result = EmailTemplete.sendRejectEmail(saleorder.User_Profile.User_Authentication.Email_Address, saleorder.User_Profile.Name, "SaleOrder", approval.Name, saleorder.Sale_Order_ID, domain);
                            //    if (result == false)
                            //    {
                            //        return new ServiceResult() { Code = ERROR_CODE.ERROR_501_CANT_SEND_EMAIL, Msg = new Error().getError(ERROR_CODE.ERROR_501_CANT_SEND_EMAIL) };
                            //    }
                            //}



                        }

                    }


                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Sale Order Management" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = "Sale Order Management" };
            }
        }

    }

}