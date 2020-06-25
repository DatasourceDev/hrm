using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SBSModel.Common;
using System.Data.Entity;
using SBSModel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using System.Data;


namespace SBSModel.Models
{
    public class InvoiceService
    {
        public List<Invoice> LstInvoice(int pCompany_ID, int pProfileID)
        {
            using (var db = new SBS2DBContext())
            {
                var iagroups = db.IAs.Where(w => w.Profile_ID == pProfileID & w.IG.Type == InventoryType.Quotation).Select(s => s.IG_ID);
                if (iagroups.Count() > 0)
                {
                    // have approval role
                    var ir = db.IRs.Where(w => iagroups.Contains(w.IG_ID)).Select(s => s.Profile_ID);
                    return db.Invoices
                        .Include(i => i.Invoice1)
                        .Include(i => i.Invoice2)
                        .Include(i => i.Invoice_has_Quotation)
                        .Include(i => i.Invoice_has_Quotation.Select(s => s.Quotation))
                        .Include(i => i.Company)
                        .Include(i => i.Customer_Company)
                        .Include(i => i.User_Profile)
                        .Include(i => i.Inventory_Prefix_Config)
                        .Where(w => w.Company_ID == pCompany_ID && ir.Contains(w.Profile_ID))
                        .OrderByDescending(o => o.Invoice_Date)
                        .ThenByDescending(o => o.Invoice_Ref_No)
                        .ToList();
                }
                else
                {
                    return db.Invoices
                    .Include(i => i.Invoice_has_Quotation)
                    .Include(i => i.Invoice_has_Quotation.Select(s => s.Quotation))
                    .Include(i => i.Company)
                    .Include(i => i.Customer_Company)
                    .Include(i => i.User_Profile)
                    .Include(i => i.Inventory_Prefix_Config)
                    .Where(w => w.Company_ID == pCompany_ID && w.Profile_ID == pProfileID)
                    .OrderByDescending(o => o.Invoice_Date)
                    .ThenByDescending(o => o.Invoice_Ref_No)
                    .ToList();
                }

            }
        }

        public List<Invoice> LstRelateInvoice(int[] pQuotationID, Nullable<int> pCurrentInvoice = null)
        {
            using (var db = new SBS2DBContext())
            {
                Nullable<int>[] invoice;

                if (pCurrentInvoice.HasValue)
                {
                    invoice = db.Invoice_has_Quotation.Where(w => pQuotationID.Contains(w.Quotation_ID.Value) && w.Invoice_ID != pCurrentInvoice.Value).Select(s => s.Invoice_ID).ToArray();
                }
                else
                {
                    invoice = db.Invoice_has_Quotation.Where(w => pQuotationID.Contains(w.Quotation_ID.Value)).Select(s => s.Invoice_ID).ToArray();
                }

                var otherinvoices = db.Invoices
                     .Include(i => i.Invoice_has_Quotation)
                     .Include(i => i.Invoice_has_Quotation.Select(s => s.Quotation))
                     .Include(i => i.Invoice_has_Quotation.Select(s => s.Quotation.Quotation_has_Product))
                     .Include(i => i.Invoice_has_Quotation.Select(s => s.Quotation.Customer_Company))
                     .Include(i => i.Invoice_has_Product)
                     .Include(i => i.Invoice_has_Product.Select(s => s.Product))
                     .Include(i => i.Company)
                     .Include(i => i.Customer_Company)
                     .Include(i => i.User_Profile)
                     .Include(i => i.Inventory_Prefix_Config)
                    .Where(w => invoice.Contains(w.Invoice_ID));

                return otherinvoices.ToList();
            }
        }

        public List<Invoice> LstRelateInvoice(int pRelateInvoiceID, Nullable<int> pCurrentInvoice = null)
        {
            using (var db = new SBS2DBContext())
            {
                int[] invoice;

                if (pCurrentInvoice.HasValue)
                {
                    invoice = db.Invoices
                        .Where(w => (w.Invoice_ID == pRelateInvoiceID || w.Relate_Invoice_ID == pRelateInvoiceID) && w.Invoice_ID != pCurrentInvoice)
                        .Select(s => s.Invoice_ID).ToArray();
                }
                else
                {
                    invoice = db.Invoices
                       .Where(w => w.Invoice_ID == pRelateInvoiceID || w.Relate_Invoice_ID == pRelateInvoiceID)
                       .Select(s => s.Invoice_ID).ToArray();
                }

                var otherinvoices = db.Invoices
                     .Include(i => i.Invoice_has_Quotation)
                     .Include(i => i.Invoice_has_Quotation.Select(s => s.Quotation))
                     .Include(i => i.Invoice_has_Quotation.Select(s => s.Quotation.Quotation_has_Product))
                     .Include(i => i.Invoice_has_Quotation.Select(s => s.Quotation.Customer_Company))
                     .Include(i => i.Invoice_has_Product)
                     .Include(i => i.Invoice_has_Product.Select(s => s.Product))
                     .Include(i => i.Company)
                     .Include(i => i.Customer_Company)
                     .Include(i => i.User_Profile)
                     .Include(i => i.Inventory_Prefix_Config)
                    .Where(w => invoice.Contains(w.Invoice_ID));

                return otherinvoices.ToList();
            }
        }

        public Invoice GetInvoice(Nullable<int> pInvoiceID)
        {
            using (var db = new SBS2DBContext())
            {
                var invoices = db.Invoices
                     .Include(i => i.Invoice_has_Quotation)
                     .Include(i => i.Invoice_has_Quotation.Select(s => s.Quotation))
                     .Include(i => i.Invoice_has_Quotation.Select(s => s.Quotation.Quotation_has_Product))
                     .Include(i => i.Invoice_has_Quotation.Select(s => s.Quotation.Customer_Company))
                     .Include(i => i.Invoice_has_Product)
                     .Include(i => i.Invoice_has_Product.Select(s => s.Product))
                     .Include(i => i.Invoice_Payment)
                     .Include(i => i.Company)
                     .Include(i => i.Customer_Company)
                     .Include(i => i.User_Profile)
                     .Include(i => i.Inventory_Prefix_Config)
                     .Where(w => w.Invoice_ID == pInvoiceID);

                return invoices.FirstOrDefault();
            }
        }

        public Invoice_Payment GetInvoicePayment(Nullable<int> pPaymentID)
        {
            using (var db = new SBS2DBContext())
            {
                var invPayment = db.Invoice_Payment
                     .Include(i => i.Invoice)
                     .Include(i => i.Invoice.Company)
                     .Where(w => w.Invoice_Payment_ID == pPaymentID);

                return invPayment.FirstOrDefault();
            }
        }


        public Invoice GetInvoice(int pProfileID, Nullable<int> pInvoiceID = null, string pInvoiceNo = "", bool chkRelateInvoice = false)
        {
            using (var db = new SBS2DBContext())
            {
                var invoices = db.Invoices
                     .Include(i => i.Invoice_has_Quotation)
                     .Include(i => i.Invoice_has_Quotation.Select(s => s.Quotation))
                     .Include(i => i.Invoice_has_Quotation.Select(s => s.Quotation.Quotation_has_Product))
                     .Include(i => i.Invoice_has_Quotation.Select(s => s.Quotation.Customer_Company))
                     .Include(i => i.Invoice_has_Product)
                     .Include(i => i.Invoice_has_Product.Select(s => s.Product))
                     .Include(i => i.Invoice_Payment)
                     .Include(i => i.Company)
                     .Include(i => i.Customer_Company)
                     .Include(i => i.User_Profile)
                     .Include(i => i.Inventory_Prefix_Config);

                if (pInvoiceID.HasValue)
                {
                    invoices = invoices.Where(w => w.Invoice_ID == pInvoiceID);
                }
                if (!string.IsNullOrEmpty(pInvoiceNo))
                {
                    invoices = invoices.Where(w => w.Invoice_Ref_No == pInvoiceNo);
                }

                if (chkRelateInvoice)
                {
                    // get main invoice only
                    invoices = invoices.Where(w => w.Relate_Invoice_ID == null && w.Manual_Quotation == true);
                    var inv = invoices.FirstOrDefault();

                    if (inv != null)
                    {
                        var payamount = db.Invoices
                            .Where(w => w.Relate_Invoice_ID == inv.Invoice_ID || w.Invoice_ID == inv.Invoice_ID)
                            .Sum(s => (s.Payment_Amount.HasValue ? s.Payment_Amount.Value : 0));

                        if (payamount >= inv.Total_Amount)
                        {
                            return null;
                        }
                    }

                }

                var iagroups = db.IAs.Where(w => w.Profile_ID == pProfileID & w.IG.Type == InventoryType.Quotation).Select(s => s.IG_ID);
                if (iagroups.Count() > 0)
                {
                    // have approval role
                    var ir = db.IRs.Where(w => iagroups.Contains(w.IG_ID)).Select(s => s.Profile_ID);
                    invoices = invoices.Where(w => ir.Contains(w.Profile_ID));
                }
                else
                {
                    invoices = invoices.Where(w => w.Profile_ID == pProfileID);
                }



                return invoices.FirstOrDefault();
            }
        }

        public ServiceResult InsertManualInvoice(Invoice pInvoice, Customer_Company pCompany, Invoice_has_Product[] pProductItems, string[] pRowsType)
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

                            pInvoice.Customer_Company_ID = curentcompany.Customer_Company_ID;
                        }
                        else
                        {
                            pInvoice.Customer_Company = pCompany;
                        }

                    }

                    // Insert Quotation and Get Quotation Configuretion
                    if (!pInvoice.Invoice_Config_ID.HasValue)
                        return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Invoice Configuration" };

                    var config = db.Inventory_Prefix_Config.Where(w => w.Prefix_Config_ID == pInvoice.Invoice_Config_ID).FirstOrDefault();

                    if (config == null)
                        return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Invoice Configuration" };

                    var nextNumber = config.Ref_Count.HasValue ? config.Ref_Count.Value : 1;
                    var nextref = config.Prefix_Ref_No + "-" + currentdate.Year.ToString().Substring(2, 2) + nextNumber.ToString().PadLeft(config.Number_Of_Digit.HasValue ? config.Number_Of_Digit.Value : 5, '0');

                    config.Ref_Count = config.Ref_Count + 1;

                    pInvoice.Invoice_Ref_No = nextref;
                    pInvoice.Create_On = currentdate;

                    if (!pInvoice.Relate_Invoice_ID.HasValue)
                    {
                        //---------invoice has product----------

                        // Insert if not relate invoice
                        if (pProductItems != null && pRowsType != null)
                        {
                            var i = 0;
                            foreach (var row in pProductItems)
                            {
                                if (pRowsType[i] == RowType.ADD)
                                {
                                    row.Product = null;
                                    pInvoice.Invoice_has_Product.Add(row);
                                }
                                i++;
                            }
                        }
                    }


                    db.Invoices.Add(pInvoice);
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = "Invoice" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = "Invoice" };
            }
        }

        public ServiceResult UpdateInvoiceManual(Invoice pInvoice, Customer_Company pCompany, Invoice_has_Product[] pProductItems, string[] pRowsType)
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

                            pInvoice.Customer_Company_ID = curentcompany.Customer_Company_ID;
                        }
                        else
                        {
                            db.Customer_Company.Add(pCompany);
                            db.SaveChanges();
                            db.Entry(pCompany).GetDatabaseValues();
                            pInvoice.Customer_Company_ID = pCompany.Customer_Company_ID;
                        }

                    }

                    var current = (from a in db.Invoices where a.Invoice_ID == pInvoice.Invoice_ID select a).FirstOrDefault();
                    if (current != null)
                    {
                        pInvoice.Create_On = current.Create_On;
                        pInvoice.Create_By = current.Create_By;
                        pInvoice.Update_On = currentdate;
                        pInvoice.Invoice_Ref_No = current.Invoice_Ref_No;

                        if (!pInvoice.Relate_Invoice_ID.HasValue)
                        {
                            //if (pProductItems != null && pRowsType != null)
                            //{
                            //    var i = 0;
                            //    foreach (var row in pProductItems)
                            //    {
                            //        row.Product = null;
                            //        if (pRowsType[i] == RowType.ADD)
                            //        {
                            //            row.Invoice_Product_ID = 0;
                            //            row.Invoice_ID = pInvoice.Invoice_ID;
                            //            db.Invoice_has_Product.Add(row);

                            //        }
                            //        else if (pRowsType[i] == RowType.EDIT)
                            //        {
                            //            var currentitem = (from a in db.Invoice_has_Product where a.Invoice_Product_ID == row.Invoice_Product_ID select a).FirstOrDefault();
                            //            if (currentitem != null)
                            //            {
                            //                currentitem.Amount = row.Amount;
                            //                currentitem.Discount = row.Discount;
                            //                currentitem.Product_ID = row.Product_ID;
                            //                currentitem.Quantity = row.Quantity;
                            //                currentitem.Unit_Price = row.Unit_Price;

                            //            }
                            //        }
                            //        else if (pRowsType[i] == RowType.DELETE)
                            //        {
                            //            var currentitem = (from a in db.Invoice_has_Product where a.Invoice_Product_ID == row.Invoice_Product_ID select a).FirstOrDefault();
                            //            if (currentitem != null)
                            //            {
                            //                db.Invoice_has_Product.Remove(currentitem);
                            //            }
                            //        }
                            //        i++;
                            //    }

                            //}
                        }
                        db.Entry(current).CurrentValues.SetValues(pInvoice);
                        db.SaveChanges();
                    }



                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Invoice" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = "Invoice" };
            }
        }

        public ServiceResult InsertInvoice(Invoice pInvoice, int[] pQuotationIDs)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    // Edit Company Detail
                    if (pInvoice.Customer_Company_ID > 0)
                    {
                        // update customer company
                        var curentcompany = db.Customer_Company.Where(w => w.Customer_Company_ID == pInvoice.Customer_Company.Customer_Company_ID).FirstOrDefault();
                        if (curentcompany == null)
                            return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Company" };

                        db.Entry(curentcompany).CurrentValues.SetValues(pInvoice.Customer_Company);
                    }
                    else
                    {
                        // insert
                        var curentcompany = db.Customer_Company.Where(w => w.Company_Name == pInvoice.Customer_Company.Company_Name).FirstOrDefault();
                        if (curentcompany != null)
                        {

                            curentcompany.Email = pInvoice.Customer_Company.Email;
                            curentcompany.Person_In_Charge = pInvoice.Customer_Company.Person_In_Charge;
                            curentcompany.Office_Phone = pInvoice.Customer_Company.Office_Phone;
                            curentcompany.Billing_Address = pInvoice.Customer_Company.Billing_Address;
                            curentcompany.Billing_Street = pInvoice.Customer_Company.Billing_Street;
                            curentcompany.Billing_Country_ID = pInvoice.Customer_Company.Billing_Country_ID;
                            curentcompany.Billing_State_ID = pInvoice.Customer_Company.Billing_State_ID;
                            curentcompany.Billing_City = pInvoice.Customer_Company.Billing_City;
                            curentcompany.Billing_Postal_Code = pInvoice.Customer_Company.Billing_Postal_Code;
                            pInvoice.Customer_Company.Customer_Company_ID = curentcompany.Customer_Company_ID;
                        }
                    }

                    // Insert Quotation and Get Quotation Configuretion
                    if (!pInvoice.Invoice_Config_ID.HasValue)
                        return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Invoice Configuration" };

                    var config = db.Inventory_Prefix_Config.Where(w => w.Prefix_Config_ID == pInvoice.Invoice_Config_ID).FirstOrDefault();

                    if (config == null)
                        return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Invoice Configuration" };

                    var nextNumber = config.Ref_Count.HasValue ? config.Ref_Count.Value : 1;
                    var nextref = config.Prefix_Ref_No + "-" + currentdate.Year.ToString().Substring(2, 2) + nextNumber.ToString().PadLeft(config.Number_Of_Digit.HasValue ? config.Number_Of_Digit.Value : 5, '0');

                    config.Ref_Count = config.Ref_Count + 1;

                    pInvoice.Invoice_Ref_No = nextref;
                    pInvoice.Create_On = currentdate;

                    if (pQuotationIDs != null)
                    {
                        foreach (var quotationid in pQuotationIDs)
                        {
                            pInvoice.Invoice_has_Quotation.Add(new Invoice_has_Quotation() { Quotation_ID = quotationid });
                        }
                    }


                    //---------quotation item----------
                    if (pInvoice.Invoice_Payment != null)
                    {
                        var i = 0;
                        foreach (var row in pInvoice.Invoice_Payment)
                        {
                            row.Create_By = pInvoice.Create_By;
                            row.Create_On = pInvoice.Create_On;

                            pInvoice.Invoice_Payment.Add(row);
                            i++;
                        }
                    }

                    db.Invoices.Add(pInvoice);
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = "Invoice" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = "Invoice" };
            }
        }

        public ServiceResult InsertInvoice(Invoice pInvoice, int[] pQuotationIDs,
            List<Invoice_has_Product> pInvoiceItems, List<string> pItemRowsType,
            List<Invoice_Payment> pInvoicePayment, List<string> pPaymentRowsType,
            int pUserlogin, Nullable<int> pQuotationPaymentTermID = null)
        {

            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {

                    // Insert Quotation and Get Quotation Configuretion
                    if (!pInvoice.Invoice_Config_ID.HasValue)
                        return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Invoice Configuration" };

                    var config = db.Inventory_Prefix_Config.Where(w => w.Prefix_Config_ID == pInvoice.Invoice_Config_ID).FirstOrDefault();

                    if (config == null)
                        return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Invoice Configuration" };

                    var nextNumber = config.Ref_Count.HasValue ? config.Ref_Count.Value : 1;
                    var nextref = config.Prefix_Ref_No + "-" + currentdate.Year.ToString().Substring(2, 2) + nextNumber.ToString().PadLeft(config.Number_Of_Digit.HasValue ? config.Number_Of_Digit.Value : 5, '0');

                    config.Ref_Count = config.Ref_Count + 1;

                    pInvoice.Invoice_Ref_No = nextref;
                    pInvoice.Create_On = currentdate;


                    if (pQuotationIDs != null)
                    {

                        foreach (var quotationid in pQuotationIDs)
                        {
                            pInvoice.Invoice_has_Quotation.Add(new Invoice_has_Quotation() { Quotation_ID = quotationid });

                            var quotation = (from a in db.Quotations where a.Quotation_ID == quotationid select a).FirstOrDefault();

                            quotation.Overall_Status = QuotationStatus.INVOICED;

                            db.Entry(quotation).CurrentValues.SetValues(quotation);

                        }

                    }

                    //---------quotation item----------
                    if (pInvoiceItems != null && pItemRowsType != null)
                    {
                        var i = 0;
                        foreach (var row in pInvoiceItems)
                        {
                            row.Create_By = pInvoice.Create_By;
                            row.Create_On = pInvoice.Create_On;

                            if (pItemRowsType[i] == RowType.ADD | pItemRowsType[i] == RowType.EDIT)
                            {
                                pInvoice.Invoice_has_Product.Add(row);
                            }
                            i++;
                        }
                    }

                    //---------quotation item----------
                    if (pInvoicePayment != null && pPaymentRowsType != null)
                    {
                        var i = 0;
                        foreach (var row in pInvoicePayment)
                        {
                            row.Create_By = pInvoice.Create_By;
                            row.Create_On = pInvoice.Create_On;

                            if (pPaymentRowsType[i] == RowType.ADD | pPaymentRowsType[i] == RowType.EDIT)
                            {
                                pInvoice.Invoice_Payment.Add(row);
                            }
                            i++;
                        }
                    }

                    db.Invoices.Add(pInvoice);

                    if (pQuotationPaymentTermID.HasValue)
                    {
                        var qPayment = (from a in db.Quotation_Payment_Term where a.Quotation_Payment_ID == pQuotationPaymentTermID.Value select a).FirstOrDefault();

                        if (qPayment != null)
                        {
                            qPayment.Is_Invoiced = true;
                            db.Entry(qPayment).State = EntityState.Modified;
                        }
                    }

                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = "Invoice" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = "Invoice" };
            }
        }

        public ServiceResult UpdateInvoice(Invoice pInvoice, int[] pQuotationIDs,
            List<Invoice_has_Product> pInvoiceItems, List<string> pItemRowsType,
            List<Invoice_Payment> pInvoicePayment, List<string> pPaymentRowsType,
            int pUserlogin)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    // Insert Quotation and
                    var current = (from a in db.Invoices where a.Invoice_ID == pInvoice.Invoice_ID select a).FirstOrDefault();
                    if (current != null)
                    {
                        pInvoice.Create_On = current.Create_On;
                        pInvoice.Create_By = current.Create_By;
                        pInvoice.Update_On = currentdate;
                        pInvoice.Invoice_Ref_No = current.Invoice_Ref_No;
                        //---------INVOICE ITEMS----------
                        if (pInvoiceItems != null && pItemRowsType != null)
                        {
                            var i = 0;
                            foreach (var row in pInvoiceItems)
                            {

                                if (row.Invoice_Product_ID > 0)
                                {
                                    if (pItemRowsType[i] == RowType.DELETE)
                                    {
                                        db.Entry(row).State = EntityState.Deleted;
                                    }
                                    else
                                    {
                                        var curItem = (from a in db.Invoice_has_Product
                                                       where a.Invoice_Product_ID == row.Invoice_Product_ID
                                                       select a).FirstOrDefault();

                                        if (curItem != null)
                                        {
                                            curItem.Product_ID = row.Product_ID;
                                            curItem.Product_Name = row.Product_Name;
                                            curItem.Unit = row.Unit;
                                            curItem.Unit_Price = row.Unit_Price;
                                            curItem.Quantity = row.Quantity;
                                            curItem.Discount = row.Discount;
                                            curItem.Discount_Type = row.Discount_Type;
                                            curItem.Amount = row.Amount;
                                            curItem.Create_By = current.Create_By;
                                            curItem.Create_On = currentdate;

                                            db.Entry(curItem).State = EntityState.Modified;
                                        }
                                    }
                                }
                                else
                                {
                                    if (pItemRowsType[i] == RowType.ADD | pItemRowsType[i] == RowType.EDIT)
                                    {
                                        row.Invoice_ID = pInvoice.Invoice_ID;
                                        row.Create_By = pInvoice.Update_By;
                                        row.Create_On = pInvoice.Update_On;
                                    }
                                }
                                i++;
                            }
                        }
                        //---------INVOICE PAYMENT----------
                        if (pInvoicePayment != null && pPaymentRowsType != null)
                        {
                            var i = 0;
                            foreach (var row in pInvoicePayment)
                            {
                                if (row.Invoice_Payment_ID > 0)
                                {
                                    if (pPaymentRowsType[i] == RowType.DELETE)
                                    {
                                        db.Entry(row).State = EntityState.Deleted;
                                    }
                                    else
                                    {
                                        var curItem = (from a in db.Invoice_Payment
                                                       where a.Invoice_Payment_ID == row.Invoice_Payment_ID
                                                       select a).FirstOrDefault();

                                        if (curItem != null)
                                        {
                                            curItem.Amount = row.Amount;
                                            curItem.Amount_Type = row.Amount_Type;
                                            curItem.Payment_Method = row.Payment_Method;
                                            curItem.Payment_Method_ID = row.Payment_Method_ID;
                                            curItem.Payment_Term = row.Payment_Term;
                                            curItem.Payment_Term_ID = row.Payment_Term_ID;
                                            curItem.Amount = row.Amount;
                                            curItem.Create_By = current.Create_By;
                                            curItem.Create_On = currentdate;

                                            db.Entry(curItem).State = EntityState.Modified;
                                        }
                                    }
                                }
                                else
                                {
                                    if (pPaymentRowsType[i] == RowType.ADD | pPaymentRowsType[i] == RowType.EDIT)
                                    {
                                        row.Invoice_ID = pInvoice.Invoice_ID;
                                        row.Create_By = pInvoice.Update_By;
                                        row.Create_On = pInvoice.Update_On;
                                    }
                                }
                                i++;
                            }
                        }

                        db.Entry(current).CurrentValues.SetValues(pInvoice);
                        db.Entry(current).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Invoice" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = "Invoice" };
            }
        }

        public ServiceResult ConfirmPaid(User_Profile userlogin, int pInvoiceID)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    // Insert Quotation and
                    var current = (from a in db.Invoices where a.Invoice_ID == pInvoiceID select a).FirstOrDefault();
                    if (current != null)
                    {
                        current.Update_On = currentdate;
                        current.Update_By = userlogin.User_Authentication.Email_Address;
                        current.Is_Paid = true;

                        db.SaveChanges();
                    }



                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Invoice" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = "Invoice" };
            }
        }

        public ServiceResult DeleteInvoice(int pInvoiceID)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    // Insert Quotation and
                    var current = (from a in db.Invoices where a.Invoice_ID == pInvoiceID select a).FirstOrDefault();
                    if (current != null)
                    {

                        var invoicehasproduct = db.Invoice_has_Product.Where(w => w.Invoice_ID == pInvoiceID);
                        var invoicehasquotation = db.Invoice_has_Quotation.Where(w => w.Invoice_ID == pInvoiceID);
                        var invoice = db.Invoices.Where(w => w.Invoice_ID == pInvoiceID);

                        db.Invoice_has_Product.RemoveRange(invoicehasproduct);
                        db.Invoice_has_Quotation.RemoveRange(invoicehasquotation);
                        db.Invoices.RemoveRange(invoice);
                        db.SaveChanges();
                    }
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = "Invoice" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = "Invoice" };
            }
        }

        //Added by Nay on 20-Jun-2015
        public List<Invoice> LstInvoiceDetails(int pProfile_ID, string startDate, string endDate, int Client, string FullyPaid)
        {
            using (var db = new SBS2DBContext())
            {
                var InvList = (from a in db.Invoices
                               .Include(w => w.Customer_Company)
                              .Where(w => w.Profile_ID == pProfile_ID)
                               select a);

                if (startDate != null || endDate != null)
                {
                    var date1 = DateTime.ParseExact(startDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    var date2 = DateTime.ParseExact(endDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    InvList = InvList.Where(w => w.Invoice_Date >= date1 && w.Invoice_Date <= date2);
                }

                if (Client > 0)
                {
                    InvList = InvList.Where(w => w.Customer_Company_ID == Client);
                }

                if (FullyPaid == "Paid")
                {
                    InvList = InvList.Where(w => (w.Total_Amount - w.Payment_Amount) >= 0);
                }
                else if (FullyPaid == "Unpaid")
                {
                    InvList = InvList.Where(w => (w.Total_Amount - w.Payment_Amount) > 0);
                }

                return InvList
                    .OrderByDescending(o => o.Invoice_Date)
                        .ThenByDescending(o => o.Invoice_Ref_No).ToList();
            }
        }
        public List<Invoice> LstInvoiceDetails(int pProfile_ID, Nullable<int> pMode=null)
        {
            using (var db = new SBS2DBContext())
            {
                var InvList = (from a in db.Invoices
                               .Include(w => w.Customer_Company)
                               .Include(w => w.Invoice_Payment)
                              .Where(w => w.Profile_ID == pProfile_ID)
                               select a);

                if (pMode.HasValue)
                {
                    InvList = InvList.Where(w => w.Mode == pMode.Value);
                }

                return InvList
                    .OrderByDescending(o => o.Invoice_Date)
                        .ThenByDescending(o => o.Invoice_Ref_No).ToList();
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

        public List<Customer_Company> getCustomerCompanies(Nullable<int> pCompanyID)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.Customer_Company
                        where a.Company_ID == pCompanyID
                        select a).ToList();
            }
        }


        //Added by Nay on 20-Jun-2015
        public List<Quotation> calcMonthlySF(DateTime startDate, DateTime endDate, int user, int compID)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.Quotations
                        .Include(i => i.Quotation_has_Product)
                        where a.Profile_ID == user && a.Company_ID == compID && (a.To_Date >= startDate && a.To_Date <= endDate)
                        select a).ToList();
            }
        }

        //Added by Nay on 20-Jun-2015
        public Nullable<decimal> calcMonthlySR(DateTime startDate, DateTime endDate, int user)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.Invoices
                        where a.Profile_ID == user && a.Invoice_Date >= startDate && a.Invoice_Date <= endDate
                        select (a.Total_Amount - a.Payment_Amount)).ToList().Sum();
            }
        }

        //Added by Nay on 20-Jun-2015
        public Nullable<decimal> calcMonthlyAS(DateTime startDate, DateTime endDate, int user, int compID)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.Invoices
                        join b in db.Invoice_has_Quotation
                            on a.Invoice_ID equals b.Invoice_ID
                        where a.Profile_ID == user && a.Company_ID == compID && (a.Invoice_Date >= startDate && a.Invoice_Date <= endDate)
                        select a.Total_Amount).ToList().Sum();
            }
        }

        public List<Invoice> LstInvoiceDetail(int pProfileID, int pCompanyID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.Invoices
                .Include(i => i.Inventory_Prefix_Config)
                .Where(w => w.Profile_ID == pProfileID && w.Company_ID == pCompanyID)
                .OrderByDescending(o => o.Invoice_Date)
                .ThenByDescending(o => o.Invoice_Ref_No)
                .ToList();
            }
        }

        public ServiceResult PayInvoice(int pPaymentID, decimal pAmount, int pPaymentMethod, string pChequeNo = "")
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var invPayment = (from a in db.Invoice_Payment where a.Invoice_Payment_ID == pPaymentID select a).FirstOrDefault();
                    if (invPayment != null)
                    {
                        var paymentAmount = pAmount + (invPayment.Payment_Amount.HasValue ? invPayment.Payment_Amount : 0);
                        var amtDiff = invPayment.Total_Amount - paymentAmount;

                        invPayment.Payment_Amount = paymentAmount;
                        invPayment.Remaining_Amount = amtDiff;

                        db.Entry(invPayment).State = EntityState.Modified;

                        var invPaymentDetail = new Invoice_Payment_Detail()
                        {
                            Invoice_ID = invPayment.Invoice_ID.Value,
                            Invoice_Payment_ID = invPayment.Invoice_Payment_ID,
                            Payment_Mode_ID = pPaymentMethod,
                            Amount = paymentAmount,
                            Cheque_No = pChequeNo,
                            Payment_Date = currentdate,
                        };

                        db.Entry(invPaymentDetail).State = EntityState.Added;

                        db.SaveChanges();
                    }

                }

                return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = "Invoice" };
            }
            catch 
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = "Invoice" };
            }

        }

        public List<Invoice_Payment_Detail> GetPaymentHistory(int pInvoicePaymentId, bool pOrderByDesc = true)
        {
            using (var db = new SBS2DBContext())
            {
                var invPaymentHistory = db.Invoice_Payment_Detail
                    .Include(x => x.Invoice_Payment)
                    .Include(x => x.Invoice_Payment.Invoice)
                    .Where(x => x.Invoice_Payment_ID == pInvoicePaymentId);

                if (pOrderByDesc)
                {
                    invPaymentHistory = invPaymentHistory.OrderByDescending(x => x.Payment_Date);
                }
                else
                {
                    invPaymentHistory = invPaymentHistory.OrderBy(x => x.Payment_Date);
                }

                return invPaymentHistory.ToList();
            }
        }

        public bool DeleteInvoicePayment(int pPaymentID)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var qPayment = (from a in db.Invoice_Payment
                                    where (a.Invoice_Payment_ID == pPaymentID)
                                    select a).FirstOrDefault();

                    db.Entry(qPayment).State = EntityState.Deleted;
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
}
