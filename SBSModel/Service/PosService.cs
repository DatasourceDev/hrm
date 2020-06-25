using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using SBSModel.Models;
using SBSModel.Common;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Data;
using SBSModel.Offline;

namespace SBSModel.Models
{
    public class POSReciptCriteria : CriteriaBase
    {
        public string Receipt_No { get; set; }
        public Nullable<int> Receipt_ID { get; set; }
        public Nullable<int> Cashier_ID { get; set; }
        public Nullable<int> Shift_ID { get; set; }
        public Nullable<DateTime> Start_Date { get; set; }
        public Nullable<DateTime> End_Date { get; set; }
        public string Status { get; set; }
        public bool isByCashier { get; set; }
        public bool orderByAsc { get; set; }
        public bool includeTime { get; set; }
        public Nullable<int> Branch_ID { get; set; }
        public Nullable<int> User_Authentication_ID { get; set; }

    }

    public class POSService
    {
        public POS_Shift GetShift(Nullable<int> pShiftID)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    return db.POS_Shift
                        .Where(w => w.Shift_ID == pShiftID).FirstOrDefault();
                }
            }
            catch
            {
                return null;
            }
        }

        public POS_Shift GetCurrentOpenShift(Nullable<int> pCompanyID, Nullable<int> pTerminalID)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    return db.POS_Shift
                        .Where(w => w.Company_ID == pCompanyID &&
                            w.Terminal_ID == pTerminalID &&
                            w.Status == ShiftStatus.Open)
                            .Include(i => i.Branch)
                        .OrderByDescending(o => o.Effective_Date).FirstOrDefault();
                }
            }
            catch
            {
                return null;
            }
        }

        public List<POS_Shift> LstPOSShift(Nullable<int> pCompanyID, Nullable<int> pBranchID, Nullable<int> pTerminalID)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var shifts = db.POS_Shift
                        .Include(i => i.Branch)
                        .Where(w => w.Company_ID == pCompanyID);

                    if (pBranchID.HasValue)
                        shifts = shifts.Where(w => w.Branch_ID == pBranchID);

                    if (pTerminalID.HasValue)
                        shifts = shifts.Where(w => w.Terminal_ID == pTerminalID);

                    return shifts.OrderByDescending(o => o.Effective_Date).ToList();
                }
            }
            catch
            {
                return new List<POS_Shift>();
            }
        }

        public decimal GetPOSReceiptTotalAmount(Nullable<int> pShiftID)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    return db.POS_Receipt
                        .Where(w => w.Shift_ID == pShiftID & w.Status == ReceiptStatus.Paid)
                        .Sum(s => (s.Net_Amount.HasValue ? s.Net_Amount.Value : 0));

                }
            }
            catch
            {
                return 0M;
            }
        }

        public Product_Table GetProductTable(Nullable<int> pCompanyID)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    return db.Product_Table.Where(w => w.Company_ID == pCompanyID).FirstOrDefault();
                }
            }
            catch
            {
                return null;
            }
        }

        public POS_Terminal GetTerminal(Nullable<int> pCashierID)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    return db.POS_Terminal.Include(i => i.Branch).Include(i => i.User_Profile).Where(w => w.Cashier_ID == pCashierID).FirstOrDefault();
                }
            }
            catch
            {
                return null;
            }
        }

        public POS_Terminal GetTerminalByTerminalID(Nullable<int> pTerminalID)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    return db.POS_Terminal.Include(i => i.Branch).Include(i => i.User_Profile).Include(i => i.User_Profile.User_Authentication).Where(w => w.Terminal_ID == pTerminalID).FirstOrDefault();
                }
            }
            catch
            {
                return null;
            }
        }

        public POS_Receipt GetPOSReceipt(Nullable<int> pReceiptID = null, string pReceiptLocalID = "")
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var rcp = db.POS_Receipt
                        .Include(i => i.User_Profile)
                        .Include(i => i.POS_Shift)
                        .Include(i => i.POS_Shift.POS_Terminal)
                        .Include(i => i.POS_Shift.POS_Terminal.Branch)
                        .Include(i => i.POS_Products_Rcp)
                        .Include(i => i.POS_Products_Rcp.Select(s => s.Product))
                        .Include(i => i.POS_Products_Rcp.Select(s => s.Product.Product_Color))
                        .Include(i => i.POS_Products_Rcp.Select(s => s.Product.Product_Size))
                        .Include(i => i.POS_Products_Rcp.Select(s => s.Product_Color))
                        .Include(i => i.POS_Products_Rcp.Select(s => s.Product_Size))
                        .Include(i => i.POS_Receipt_Payment)
                        .Include(i => i.Member);

                    if (pReceiptID.HasValue)
                        rcp = rcp.Where(w => w.Receipt_ID == pReceiptID);
                    if (!string.IsNullOrEmpty(pReceiptLocalID))
                        rcp = rcp.Where(w => w.Receipt_Local_ID == pReceiptLocalID);

                    return rcp.FirstOrDefault();
                }
            }
            catch
            {
            }
            return new POS_Receipt();
        }

        //public List<POS_Receipt> LstPOSReceipt(Nullable<int> Company_ID, Nullable<int> Cashier_ID,
        //   Nullable<DateTime> Start_Date = null, Nullable<DateTime> End_Date = null,
        //   string Status = "", string text = "",
        //   bool orderByAsc = false, bool includeTime = false, bool isByCashier = false,
        //   Nullable<int> pBranchID = null, Nullable<int> pUserAuthenticationID = null)

        public List<POS_Receipt> LstPOSReceipt(POSReciptCriteria cri) {
            List<POS_Receipt> p = new List<POS_Receipt>();
            try {
                using (var db = new SBS2DBContext()) {
                    var q = db.POS_Receipt
                        .Include(i => i.POS_Shift)
                        .Include(i => i.POS_Shift.POS_Terminal.Branch)
                        .Include(i => i.POS_Products_Rcp)
                        .Include(i => i.POS_Products_Rcp.Select(s => s.Product))
                        .Include(i => i.POS_Products_Rcp.Select(s => s.Product.Product_Category))
                        .Include(i => i.POS_Products_Rcp.Select(s => s.Product.Product_Color))
                        .Include(i => i.POS_Products_Rcp.Select(s => s.Product.Product_Size))
                        .Include(i => i.User_Profile)
                        .Include(i => i.POS_Receipt_Payment)
                        .Include(i => i.Member)
                        .Where(w => w.Company_ID == cri.Company_ID);


                    var isAdmin = false;
                    var roles = db.User_Assign_Role.Where(w => w.User_Authentication_ID == cri.User_Authentication_ID).Select(s => s.User_Role_ID);
                    if (roles != null) {
                        var hasPageRole = db.Page_Role.Where(w => w.Page.Page_Url == "/POSConfig/ConfigurationAdmin" && roles.Contains(w.User_Role_ID)).FirstOrDefault();
                        if (hasPageRole != null) {
                            isAdmin = true;
                        }
                    }

                    if (!isAdmin) {
                        if (cri.Branch_ID.HasValue) {
                            q = q.Where(w => w.POS_Shift.POS_Terminal.Branch_ID == cri.Branch_ID);
                        }
                    }

                    if (!cri.isByCashier) {
                        q = q.Where(w => w.Status != "Hold" ? true : w.Cashier == cri.Cashier_ID);
                    } else {
                        q = q.Where(w => w.Cashier == cri.Cashier_ID);
                    }

                    if (cri.Start_Date.HasValue) {
                        if (!cri.includeTime) {
                            q = q.Where(w => EntityFunctions.CreateDateTime(w.Receipt_Date.Value.Year, w.Receipt_Date.Value.Month, w.Receipt_Date.Value.Day, 0, 0, 0) >= cri.Start_Date);
                        } else {
                            q = q.Where(w => EntityFunctions.CreateDateTime(w.Receipt_Date.Value.Year, w.Receipt_Date.Value.Month, w.Receipt_Date.Value.Day,
                                w.Receipt_Date.Value.Hour, w.Receipt_Date.Value.Minute, w.Receipt_Date.Value.Second) >= cri.Start_Date);
                        }

                    }
                    if (cri.End_Date.HasValue) {
                        if (!cri.includeTime) {
                            q = q.Where(w => EntityFunctions.CreateDateTime(w.Receipt_Date.Value.Year, w.Receipt_Date.Value.Month, w.Receipt_Date.Value.Day, 0, 0, 0) <= cri.End_Date);
                        } else {
                            q = q.Where(w => EntityFunctions.CreateDateTime(w.Receipt_Date.Value.Year, w.Receipt_Date.Value.Month, w.Receipt_Date.Value.Day,
                                w.Receipt_Date.Value.Hour, w.Receipt_Date.Value.Minute, w.Receipt_Date.Value.Second) <= cri.End_Date);
                        }
                    }
                    if (!string.IsNullOrEmpty(cri.Status)) {
                        if (cri.Status == ReceiptStatus.Paid) {
                            q = q.Where(w => w.Status == ReceiptStatus.Paid);
                        } else if (cri.Status == ReceiptStatus.Hold) {
                            q = q.Where(w => w.Status == ReceiptStatus.Hold | w.Status == ReceiptStatus.BackOrder);
                        } else if (cri.Status == ReceiptStatus.Void) {
                            q = q.Where(w => w.Status == ReceiptStatus.Void | w.Status == ReceiptStatus.Paid);
                        }
                    }
                    if (!string.IsNullOrEmpty(cri.Text_Search)) {
                        var textdecimal = NumUtil.ParseDecimal(cri.Text_Search);
                        q = q.Where(w => w.Receipt_No.Contains(cri.Text_Search) | w.POS_Shift.POS_Terminal.Terminal_Name.Contains(cri.Text_Search) | w.Total_Amount == textdecimal);
                    }
                    if (cri.orderByAsc) {
                        p = q.OrderBy(o => o.Receipt_Date)
                            .ThenBy(o => o.Receipt_No)
                            .ToList();
                    } else {
                        p = q.OrderByDescending(o => o.Receipt_Date)
                               .ThenByDescending(o => o.Receipt_No)
                               .ToList();
                    }

                }
            } catch {
            }
            return p;
        }
        
        public List<POS_Receipt> LstPOSReceipt(CustomerPurchaseCriteria cri)
        {
            List<POS_Receipt> p = new List<POS_Receipt>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var q = db.POS_Receipt
                        .Include(i => i.POS_Shift)
                        .Include(i => i.POS_Shift.POS_Terminal.Branch)
                        .Include(i => i.POS_Products_Rcp)
                        .Include(i => i.POS_Products_Rcp.Select(s => s.Product))
                        .Include(i => i.POS_Products_Rcp.Select(s => s.Product.Product_Category))
                        .Include(i => i.Member)
                        .Where(w => w.Company_ID == cri.Company_ID && w.Status == "Paid");

                    var isAdmin = false;
                    var roles = db.User_Assign_Role.Where(w => w.User_Authentication_ID == cri.User_Authentication_ID).Select(s => s.User_Role_ID);
                    if (roles != null)
                    {
                        var hasPageRole = db.Page_Role.Where(w => w.Page.Page_Url == "/POSConfig/ConfigurationAdmin" && roles.Contains(w.User_Role_ID)).FirstOrDefault();
                        if (hasPageRole != null)
                        {
                            isAdmin = true;
                        }
                    }

                    if (!isAdmin)
                    {
                        if (cri.Branch_ID.HasValue) {
                            q = q.Where(w => w.POS_Shift.POS_Terminal.Branch_ID == cri.Branch_ID);
                        }
                    }


                    if (!string.IsNullOrEmpty(cri.Text_Search)) {
                        var textdecimal = NumUtil.ParseDecimal(cri.Text_Search);
                        q = q.Where(w => w.Receipt_No.Contains(cri.Text_Search) | w.POS_Shift.POS_Terminal.Terminal_Name.Contains(cri.Text_Search) | w.Total_Amount == textdecimal);
                    }

                    if (cri.Product_Category_ID.HasValue) {
                        //var productList = q.Select(prd => prd.POS_Products_Rcp.Where(x => x.Product.Product_Category.Product_Category_ID == cri.Product_Category_ID.Value).SelectMany(j => (int)j.Receipt_ID).ToList());
                        var prodList = q.SelectMany(posPrd => posPrd.POS_Products_Rcp.Where(x => x.Product.Product_Category.Product_Category_ID == cri.Product_Category_ID.Value)).Select(j => (int)j.Receipt_ID);

                        q = q.Where(rcp => prodList.Any(id => id == rcp.Receipt_ID));
                    }

                    p = q.ToList();

                }
            }
            catch
            {
            }
            return p;
        }

        public ServiceResult InsertPOSReceipt(POS_Receipt rcp)
        {
            var currentdate = StoredProcedure.GetCurrentDate();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    POS_Terminal terminal = null;
                    if (AppSetting.POS_OFFLINE_CLIENT)
                    {
                        var device = db.Device_Configuration.Where(w => w.Field_Name == "MacAddress").FirstOrDefault();
                        if (device != null)
                            terminal = db.POS_Terminal.Where(w => w.Mac_Address == device.Field_Value).FirstOrDefault();
                    }
                    else
                    {
                        terminal = db.POS_Terminal.Where(w => w.Cashier_ID == rcp.Cashier).FirstOrDefault();
                    }

                    if (terminal == null)
                        return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Terminal" };


                    var shift = GetCurrentOpenShift(rcp.Company_ID, terminal.Terminal_ID);
                    if (shift == null)
                        return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Shift" };

                    rcp.Shift_ID = shift.Shift_ID;
                    rcp.Shift_Local_ID = shift.Shift_Local_ID;

                    if (AppSetting.POS_OFFLINE_CLIENT)
                    {

                        var rcplocalno = GenerateLocalID(db, rcp.Company_ID, Pattern_Type.Receipt, Pattern_Prefix.Receipt);
                        rcp.Receipt_Local_ID = rcplocalno;
                        rcp.Receipt_Local_No = rcplocalno;
                        rcp.Receipt_No = rcplocalno;
                    }
                    else
                    {
                        var rcpConfig = (from a in db.POS_Receipt_Configuration
                                         where a.Company_ID == rcp.Company_ID
                                         select a).FirstOrDefault();
                        if (rcpConfig == null)
                            return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Receipt Configuration" };

                        string rcpNo = "";
                        int no = rcpConfig.Ref_Count.HasValue ? rcpConfig.Ref_Count.Value : 0;
                        no++;
                        rcpNo = rcpConfig.Prefix + currentdate.Date.ToString(rcpConfig.Date_Format) + no.ToString().PadLeft(rcpConfig.Num_Lenght.Value, '0') + rcpConfig.Suffix;

                        rcpConfig.Ref_Count = no;
                        rcp.Receipt_No = rcpNo;
                    }

                    var result = InsertPOSReceipt(db, rcp);
                    if (result.Code != ERROR_CODE.SUCCESS)
                        return result;

                    db.SaveChanges();
                    db.Entry(rcp).GetDatabaseValues();
                    return result;
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = "Receipt" };
            }
        }

        private string GenerateLocalID(SBS2DBContext db, Nullable<int> pCompanyID, string pType, string pPrefix)
        {
            var runnig = db.SBS_No_Pattern.Where(w => w.Company_ID == pCompanyID && w.Pattern_Type == pType).FirstOrDefault();
            var localrunnig = db.SBS_No_Pattern.Local.Where(w => w.Company_ID == pCompanyID && w.Pattern_Type == pType).FirstOrDefault();
            if (runnig == null && localrunnig == null)
            {
                runnig = new SBS_No_Pattern() { Company_ID = pCompanyID, Pattern_Type = pType, Ref_Count = 1 };
                var no = pPrefix + "LC" + pCompanyID + "-" + (runnig.Ref_Count.ToString().PadLeft(6, '0'));
                runnig.Ref_Count = runnig.Ref_Count + 1;
                db.SBS_No_Pattern.Add(runnig);
                return no;
            }
            else
            {
                if (runnig == null && localrunnig != null)
                {
                    var no = pPrefix + "LC" + pCompanyID + "-" + (localrunnig.Ref_Count.ToString().PadLeft(6, '0'));
                    localrunnig.Ref_Count = localrunnig.Ref_Count + 1;
                    return no;
                }
                else
                {
                    var no = pPrefix + "LC" + pCompanyID + "-" + (runnig.Ref_Count.ToString().PadLeft(6, '0'));
                    runnig.Ref_Count = runnig.Ref_Count + 1;
                    return no;
                }
            }
        }

        public ServiceResult InsertPOSReceipt(SBS2DBContext db, POS_Receipt rcp)
        {
            if (rcp.Status == ReceiptStatus.Paid)
            {
                if (rcp.POS_Products_Rcp != null)
                {
                    foreach (var product in rcp.POS_Products_Rcp)
                    {
                        if (product.Product_ID.HasValue)
                        {
                            product.Receipt_Local_ID = rcp.Receipt_Local_ID;

                            var tran = new Inventory_Transaction();
                            tran.Company_ID = rcp.Company_ID.Value;
                            tran.Product_ID = product.Product_ID.Value;
                            tran.Qty = product.Qty.Value;
                            tran.Selling_Price = product.Price;
                            tran.Transaction_Type = InventoryType.Sale;
                            tran.Create_By = rcp.Create_By;
                            tran.Update_By = rcp.Update_By;
                            tran.Create_On = rcp.Create_On;
                            tran.Update_On = rcp.Update_On;
                            tran.Is_Uploaded = rcp.Is_Uploaded;
                            tran.Receipt_Local_ID = rcp.Receipt_Local_ID;
                            tran.Do_Not_Upload = rcp.Do_Not_Upload;
                            tran.Transaction_Local_ID = GenerateLocalID(db, rcp.Company_ID, Pattern_Type.Transaction, Pattern_Prefix.Transaction);
                            rcp.Inventory_Transaction.Add(tran);

                            product.Do_Not_Upload = rcp.Do_Not_Upload;
                            product.Receipt_Product_Local_ID = GenerateLocalID(db, rcp.Company_ID, Pattern_Type.Receipt_Product, Pattern_Prefix.Receipt_Product);




                        }

                    }
                }
                if (rcp.POS_Receipt_Payment != null)
                {

                    foreach (var payment in rcp.POS_Receipt_Payment)
                    {
                        payment.Receipt_Payment_Local_ID = GenerateLocalID(db, rcp.Company_ID, Pattern_Type.Receipt_Payment, Pattern_Prefix.Receipt_Payment);
                        payment.Receipt_Local_ID = rcp.Receipt_Local_ID;
                        payment.Do_Not_Upload = rcp.Do_Not_Upload;
                    }
                }
            }

            if (rcp.Member_ID.HasValue)
            {
                var redeem = rcp.POS_Receipt_Payment.Where(w => w.Payment_Type == PaymentType.Redeem).FirstOrDefault();

                var mem = db.Members.Where(w => w.Member_ID == rcp.Member_ID).FirstOrDefault();
                if (mem != null && redeem != null)
                {
                    mem.Credit = mem.Credit - redeem.Payment_Amount;
                }
            }
            db.POS_Receipt.Add(rcp);
            return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = "Receipt" };
        }

        public ServiceResult VoidPOSReceipt(Nullable<int> pReceiptID)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var current = db.POS_Receipt.Where(w => w.Receipt_ID == pReceiptID).FirstOrDefault();
                    if (current != null)
                    {
                        current.Status = ReceiptStatus.Void;
                        var trans = db.Inventory_Transaction.Where(w => w.Receipt_ID == pReceiptID);
                        db.Inventory_Transaction.RemoveRange(trans);
                    }
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Receipt" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = "Receipt" };
            }
        }

        public ServiceResult UpdatePOSReceipt(POS_Receipt rcp)
        {
            var currentdate = StoredProcedure.GetCurrentDate();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (rcp.Status != ReceiptStatus.Hold)
                    {
                        if (string.IsNullOrEmpty(rcp.Receipt_No))
                        {
                            if (AppSetting.POS_OFFLINE_CLIENT)
                            {
                                rcp.Receipt_No = rcp.Receipt_Local_No;
                            }
                            else
                            {
                                var rcpConfig = (from a in db.POS_Receipt_Configuration
                                                 where a.Company_ID == rcp.Company_ID
                                                 select a).FirstOrDefault();
                                if (rcpConfig == null)
                                {
                                    return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Receipt Configuration" };
                                }
                                string rcpNo = "";
                                int no = rcpConfig.Ref_Count.HasValue ? rcpConfig.Ref_Count.Value : 0;
                                no++;
                                rcpNo = rcpConfig.Prefix + currentdate.Date.ToString(rcpConfig.Date_Format) + no.ToString().PadLeft(rcpConfig.Num_Lenght.Value, '0') + rcpConfig.Suffix;


                                rcpConfig.Ref_Count = no;
                                rcp.Receipt_No = rcpNo;
                            }

                            rcp.Receipt_Date = currentdate;

                        }
                    }

                    var result = UpdatePOSReceipt(db, rcp);
                    if (result.Code != ERROR_CODE.SUCCESS)
                        return result;

                    db.SaveChanges();
                    return result;
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = "Receipt" };
            }
        }

        public ServiceResult UpdatePOSReceipt(SBS2DBContext db, POS_Receipt rcp)
        {
            var producttemp = rcp.POS_Products_Rcp;

            var cbService = new ComboService();
            decimal currGST = 0;
            var tax = new TaxServie().GetTax(rcp.Company_ID);
            if (tax != null && tax.Include_GST.HasValue && tax.Include_GST.Value)
            {
                var gst = tax.Tax_GST.Where(w => w.Tax_Type == TaxType.Exclusive && w.Record_Status == RecordStatus.Active && w.Is_Default == true).FirstOrDefault();
                if (gst != null)
                    currGST = gst.Tax.HasValue ? gst.Tax.Value : 0;
            }

            var current = db.POS_Receipt.Where(w => w.Receipt_ID == rcp.Receipt_ID).FirstOrDefault();
            if (current != null)
            {
                //Added by Nay on 02-Sept-2015 
                //To skip generating Receipt_No when receipt status is "Hold Bill" 


                if (rcp.POS_Receipt_Payment != null && rcp.POS_Receipt_Payment.Count > 0)
                {
                    var payments = db.POS_Receipt_Payment.Where(w => w.Receipt_ID == rcp.Receipt_ID);
                    db.POS_Receipt_Payment.RemoveRange(payments);

                    foreach (var row in rcp.POS_Receipt_Payment)
                    {
                        var pay = new POS_Receipt_Payment()
                        {
                            Approval_Code = row.Approval_Code,
                            Card_Branch = row.Card_Branch,
                            Card_Type = row.Card_Type,
                            Payment_Amount = row.Payment_Amount,
                            Payment_Type = row.Payment_Type,
                            Receipt_ID = rcp.Receipt_ID,
                            Receipt_Local_ID = rcp.Receipt_Local_ID,
                            Create_By = rcp.Create_By,
                            Create_On = rcp.Create_On,
                            Update_By = rcp.Update_By,
                            Update_On = rcp.Update_On,
                            Do_Not_Upload = rcp.Do_Not_Upload,
                        };

                        if (string.IsNullOrEmpty(row.Receipt_Payment_Local_ID))
                            pay.Receipt_Payment_Local_ID = GenerateLocalID(db, rcp.Company_ID, Pattern_Type.Receipt_Payment, Pattern_Prefix.Receipt_Payment);

                        db.POS_Receipt_Payment.Add(pay);
                    }
                }

                if (rcp.POS_Products_Rcp == null || rcp.POS_Products_Rcp.Count == 0)
                {
                    // Delete
                    db.POS_Products_Rcp.RemoveRange(current.POS_Products_Rcp);
                }
                else
                {
                    // Delete
                    foreach (var crow in current.POS_Products_Rcp)
                    {
                        if (!rcp.POS_Products_Rcp.Select(s => s.ID).Contains(crow.ID))
                        {
                            db.POS_Products_Rcp.Remove(crow);
                        }
                    }

                    foreach (var row in rcp.POS_Products_Rcp)
                    {

                        if (row.Product_ID < 0) row.Product_ID = null;

                        var doTran = false;
                        Inventory_Transaction tran = null;

                        if (row.Product_ID.HasValue && row.Product_ID > 0 && rcp.Status == ReceiptStatus.Paid)
                        {
                            doTran = true;
                            tran = db.Inventory_Transaction.Where(w => w.Receipt_ID == rcp.Receipt_ID & w.Product_ID == row.Product_ID).FirstOrDefault();
                        }
                        if (row.ID == 0)
                        {
                            //Insert product
                            var product = new POS_Products_Rcp()
                            {
                                Price = row.Price,
                                Product_Color_ID = row.Product_Color_ID,
                                Product_ID = row.Product_ID,
                                Product_Size_ID = row.Product_Size_ID,
                                Qty = row.Qty,
                                Product_Name = row.Product_Name,
                                Receipt_ID = rcp.Receipt_ID,
                                Discount = row.Discount,
                                Discount_Type = row.Discount_Type,
                                GST = row.GST,
                                Receipt_Local_ID = rcp.Receipt_Local_ID,
                                Create_By = rcp.Create_By,
                                Create_On = rcp.Create_On,
                                Update_By = rcp.Update_By,
                                Update_On = rcp.Update_On,
                                Do_Not_Upload = rcp.Do_Not_Upload
                            };

                            if (string.IsNullOrEmpty(row.Receipt_Product_Local_ID))
                                product.Receipt_Product_Local_ID = GenerateLocalID(db, rcp.Company_ID, Pattern_Type.Receipt_Product, Pattern_Prefix.Receipt_Product);

                            db.POS_Products_Rcp.Add(product);

                            if (tran == null & doTran)
                            {
                                tran = new Inventory_Transaction();
                                tran.Company_ID = rcp.Company_ID.Value;
                                tran.Product_ID = row.Product_ID.Value;
                                tran.Qty = row.Qty;
                                tran.Selling_Price = row.Price;
                                tran.Transaction_Type = InventoryType.Sale;
                                tran.Receipt_ID = rcp.Receipt_ID;
                                tran.Create_By = rcp.Create_By;
                                tran.Create_On = rcp.Create_On;
                                tran.Update_By = rcp.Update_By;
                                tran.Update_On = rcp.Update_On;
                                tran.Do_Not_Upload = rcp.Do_Not_Upload;
                                tran.Transaction_Local_ID = GenerateLocalID(db, rcp.Company_ID, Pattern_Type.Transaction, Pattern_Prefix.Transaction);
                                db.Inventory_Transaction.Add(tran);
                            }
                        }
                        else
                        {
                            var product = db.POS_Products_Rcp.Where(w => w.ID == row.ID).FirstOrDefault();
                            if (product != null)
                                product.Do_Not_Upload = rcp.Do_Not_Upload;

                            if (doTran)
                            {
                                if (tran == null)
                                {
                                    tran = new Inventory_Transaction();
                                    tran.Company_ID = rcp.Company_ID.Value;
                                    tran.Product_ID = row.Product_ID.Value;
                                    tran.Qty = row.Qty;
                                    tran.Selling_Price = row.Price;
                                    tran.Transaction_Type = InventoryType.Sale;
                                    tran.Receipt_ID = rcp.Receipt_ID;
                                    tran.Receipt_Local_ID = rcp.Receipt_Local_ID;
                                    tran.Create_By = row.Create_By;
                                    tran.Create_On = row.Create_On;
                                    tran.Update_By = row.Update_By;
                                    tran.Update_On = row.Update_On;
                                    tran.Do_Not_Upload = rcp.Do_Not_Upload;
                                    tran.Transaction_Local_ID = GenerateLocalID(db, rcp.Company_ID, Pattern_Type.Transaction, Pattern_Prefix.Transaction);
                                    db.Inventory_Transaction.Add(tran);
                                }
                                else
                                {
                                    tran.Company_ID = rcp.Company_ID.Value;
                                    tran.Product_ID = row.Product_ID.Value;
                                    tran.Qty = row.Qty;
                                    tran.Selling_Price = row.Price;
                                    tran.Transaction_Type = InventoryType.Sale;
                                    tran.Receipt_ID = rcp.Receipt_ID;
                                    tran.Receipt_Local_ID = rcp.Receipt_Local_ID;
                                    tran.Create_By = row.Create_By;
                                    tran.Create_On = row.Create_On;
                                    tran.Update_By = row.Update_By;
                                    tran.Update_On = row.Update_On;
                                    tran.Do_Not_Upload = rcp.Do_Not_Upload;
                                    if (string.IsNullOrEmpty(tran.Transaction_Local_ID))
                                        tran.Transaction_Local_ID = GenerateLocalID(db, rcp.Company_ID, Pattern_Type.Transaction, Pattern_Prefix.Transaction);
                                }

                            }
                        }
                    }
                }

                current.Cash_Payment = rcp.Cash_Payment;
                current.Cashier = rcp.Cashier;
                current.Changes = rcp.Changes;
                current.Company_ID = rcp.Company_ID;
                current.Discount = rcp.Discount;
                current.Discount_Type = rcp.Discount_Type;
                current.Net_Amount = rcp.Net_Amount;
                current.Payment_Type = rcp.Payment_Type;
                current.Receipt_Date = rcp.Receipt_Date;
                current.Receipt_No = rcp.Receipt_No;
                current.Remark = rcp.Remark;
                current.Status = rcp.Status;
                current.Table_No = rcp.Table_No;
                current.Total_Amount = rcp.Total_Amount;
                current.Total_Discount = rcp.Total_Discount;
                current.Total_Qty = rcp.Total_Qty;
                current.Shift_ID = rcp.Shift_ID;
                current.Do_Not_Upload = rcp.Do_Not_Upload;
                //Added by Nay on 12-Aug-2015
                current.Total_GST_Amount = rcp.Total_GST_Amount;
            }

            //Added by Nay on 02-Sept-2015
            //To show correct message if current transaction is still in "Hold Bill". 
            if (rcp.Status == ReceiptStatus.Hold)
            {
                return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_Hold_Bill), Field = "Receipt" };
            }
            else
            {
                return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Receipt" };
            }

        }


        public ServiceResult InsertPOSHoldBillReceipt(POS_Receipt rcp)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {

                    POS_Terminal terminal = null;
                    if (AppSetting.POS_OFFLINE_CLIENT)
                    {
                        var device = db.Device_Configuration.Where(w => w.Field_Name == "MacAddress").FirstOrDefault();
                        if (device != null)
                            terminal = db.POS_Terminal.Where(w => w.Mac_Address == device.Field_Value).FirstOrDefault();
                    }
                    else
                    {
                        terminal = db.POS_Terminal.Where(w => w.Cashier_ID == rcp.Cashier).FirstOrDefault();
                    }

                    if (terminal == null)
                        return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Terminal" };

                    var shift = GetCurrentOpenShift(rcp.Company_ID, terminal.Terminal_ID);
                    if (shift != null)
                    {
                        rcp.Shift_ID = shift.Shift_ID;
                        rcp.Shift_Local_ID = shift.Shift_Local_ID;
                    }

                    if (AppSetting.POS_OFFLINE_CLIENT)
                    {
                        // running on local side
                        var rcplocalno = GenerateLocalID(db, rcp.Company_ID, Pattern_Type.Receipt, Pattern_Prefix.Receipt);
                        rcp.Receipt_Local_ID = rcplocalno;
                        rcp.Receipt_Local_No = rcplocalno;

                        foreach (var payment in rcp.POS_Receipt_Payment)
                        {
                            payment.Receipt_Payment_Local_ID = GenerateLocalID(db, rcp.Company_ID, Pattern_Type.Receipt_Payment, Pattern_Prefix.Receipt_Payment);
                            payment.Receipt_Local_ID = rcp.Receipt_Local_ID;
                            payment.Do_Not_Upload = rcp.Do_Not_Upload;
                        }
                        foreach (var product in rcp.POS_Products_Rcp)
                        {
                            product.Receipt_Product_Local_ID = GenerateLocalID(db, rcp.Company_ID, Pattern_Type.Receipt_Product, Pattern_Prefix.Receipt_Product);
                            product.Receipt_Local_ID = rcp.Receipt_Local_ID;
                            product.Do_Not_Upload = rcp.Do_Not_Upload;
                        }
                        foreach (var tran in rcp.Inventory_Transaction)
                        {
                            tran.Transaction_Local_ID = GenerateLocalID(db, rcp.Company_ID, Pattern_Type.Transaction, Pattern_Prefix.Transaction);
                            tran.Receipt_Local_ID = rcp.Receipt_Local_ID;
                            tran.Do_Not_Upload = rcp.Do_Not_Upload;
                        }
                    }


                    db.Entry(rcp).State = EntityState.Added;
                    db.SaveChanges();

                    db.Entry(rcp).GetDatabaseValues();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_Hold_Bill), Field = "Receipt" };

                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = "Receipt" };
            }
        }

        public ServiceResult DeletePOSReceipt(Nullable<int> pReceiptID)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var products = db.POS_Products_Rcp.Where(w => w.Receipt_ID == pReceiptID);
                    db.POS_Products_Rcp.RemoveRange(products);

                    var payments = db.POS_Receipt_Payment.Where(w => w.Receipt_ID == pReceiptID);
                    db.POS_Receipt_Payment.RemoveRange(payments);

                    var rcp = db.POS_Receipt.Where(w => w.Receipt_ID == pReceiptID).FirstOrDefault();
                    db.POS_Receipt.Remove(rcp);

                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = "Receipt" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = "Receipt" };
            }
        }

        public List<vwQuantityOnHand> GetProductQuantityOnHand(Nullable<int> Company_ID,
            Nullable<int> Category_ID = null,
            string text = "",
            bool orderByAsc = false)
        {

            List<vwQuantityOnHand> p = new List<vwQuantityOnHand>();

            try
            {
                using (var db = new SBS2DBContext())
                {
                    var q = db.vwQuantityOnHands
                        .Where(w => w.Company_ID == Company_ID);

                    if (Category_ID > 0)
                    {
                        q = q.Where(w => w.Product_Category_ID == Category_ID);
                    }

                    if (!string.IsNullOrEmpty(text))
                    {
                        q = q.Where(w => w.Category_Name.Contains(text) | w.Product_Code.Contains(text) | w.Product_Name.Contains(text));
                    }

                    if (orderByAsc)
                    {
                        p = q.OrderBy(o => o.Category_Name)
                            .ThenBy(o => o.Product_Code)
                            .ToList();
                    }
                    else
                    {
                        p = q.OrderByDescending(o => o.Category_Name)
                               .ThenByDescending(o => o.Product_Code)
                               .ToList();
                    }
                }
            }
            catch
            {
            }

            return p;
        }




    }

    public class MemberCriteria : CriteriaBase
    {
        public string Member_Name { get; set; }
        public string Member_NRIC { get; set; }
        public string Member_Email { get; set; }
        public string Member_Card_No { get; set; }
        public Nullable<int> Member_ID { get; set; }
        public DateTime? Birthdate { get; set; }
    }

    public class MemberService
    {
        public Member_Configuration GetMemberConfig(Nullable<int> pCompanyID)
        {

            using (var db = new SBS2DBContext())
            {
                return db.Member_Configuration
                    .Where(w => w.Company_ID == pCompanyID).FirstOrDefault();
            }

        }

        // Added by Jane 07-08-2015
        public List<Member> LstMember(MemberCriteria cri)
        {

            using (var db = new SBS2DBContext())
            {
                var c = db.Members.Where(w => w.Company_ID == cri.Company_ID);

                if (!string.IsNullOrEmpty(cri.Member_Name))
                {
                    c = c.Where(w => w.Member_Name.Contains(cri.Member_Name));
                }
                if (!string.IsNullOrEmpty(cri.Member_NRIC))
                {
                    c = c.Where(w => w.NRIC_No.Contains(cri.Member_NRIC));
                }
                if (!string.IsNullOrEmpty(cri.Member_Card_No))
                {
                    c = c.Where(w => w.Member_Card_No.Contains(cri.Member_Card_No));
                }
                if (cri.Update_On.HasValue)
                {
                    c = c.Where(w => EntityFunctions.CreateDateTime(w.Update_On.Value.Year, w.Update_On.Value.Month, w.Update_On.Value.Day, w.Update_On.Value.Hour, w.Update_On.Value.Minute, w.Update_On.Value.Second) > EntityFunctions.CreateDateTime(cri.Update_On.Value.Year, cri.Update_On.Value.Month, cri.Update_On.Value.Day, cri.Update_On.Value.Hour, cri.Update_On.Value.Minute, cri.Update_On.Value.Second));
                }
                if (cri.Member_ID.HasValue)
                {
                    c = c.Where(w => w.Member_ID == cri.Member_ID);
                }
                if (!string.IsNullOrEmpty(cri.Text_Search))
                {
                    c = c.Where(w => w.Member_Name.Contains(cri.Text_Search) || w.Member_Card_No.Contains(cri.Text_Search) || w.Phone_No.Contains(cri.Text_Search) || w.NRIC_No.Contains(cri.Text_Search));
                }
                return c.OrderBy(o => o.Member_Name).ToList();
            }

        }

        public List<Member> LstMemberBirthdays(MemberCriteria cri) 
        {
            using (var db = new SBS2DBContext()) {
                var c = db.Members.Where(w => w.Company_ID == cri.Company_ID);

                if (cri.Birthdate.HasValue) {
                    c = c.Where(w => (w.DOB.HasValue ? w.DOB.Value.Month == cri.Birthdate.Value.Month : true));
                }

                if (!string.IsNullOrEmpty(cri.Text_Search)) {
                    c = c.Where(w => w.Member_Name.Contains(cri.Text_Search) || w.Member_Card_No.Contains(cri.Text_Search) || w.Phone_No.Contains(cri.Text_Search) || w.NRIC_No.Contains(cri.Text_Search));
                }

                return c.OrderBy(o => o.Member_Name).ToList();
            }
        }

        public Member GetMember(Nullable<int> pMemberID)
        {

            using (var db = new SBS2DBContext())
            {
                return db.Members
                    .Include(w => w.POS_Receipt)
                    .Include(w => w.POS_Receipt.Select(r => r.User_Profile))
                    .Include(w => w.POS_Receipt.Select(r => r.POS_Shift))
                    .Include(w => w.POS_Receipt.Select(r => r.POS_Shift.POS_Terminal))
                    .Include(w => w.POS_Receipt.Select(r => r.POS_Shift.POS_Terminal).Select(t => t.Branch))
                    .Include(w => w.POS_Receipt.Select(r => r.POS_Products_Rcp))
                    .Include(w => w.POS_Receipt.Select(r => r.POS_Products_Rcp.Select(p => p.Product)))
                    .Include(w => w.POS_Receipt.Select(r => r.POS_Products_Rcp.Select(p => p.Product.Product_Category)))
                    .Include(w => w.POS_Receipt.Select(r => r.POS_Receipt_Payment))
                    .Where(w => w.Member_ID == pMemberID).FirstOrDefault();
            }

        }

        public ServiceResult InsertMember(Member pMem)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {

                    var conf = db.Member_Configuration.Where(w => w.Company_ID == pMem.Company_ID).FirstOrDefault();

                    if (conf == null)
                    {
                        return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Member Configuration" };
                    }
                    string memNo = "";
                    int no = conf.Ref_Count.HasValue ? conf.Ref_Count.Value : 0;
                    no++;
                    memNo = conf.Prefix + no.ToString().PadLeft(conf.Num_Lenght.Value, '0');


                    conf.Ref_Count = no;
                    pMem.Member_Card_No = memNo;

                    db.Entry(pMem).State = EntityState.Added;
                    db.SaveChanges();


                    db.Entry(pMem).GetDatabaseValues();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = "Member" };

                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = "Member" };
            }
        }

        public ServiceResult UpdateMember(Member pMem)
        {
            try
            {

                using (var db = new SBS2DBContext())
                {
                    db.Entry(pMem).State = EntityState.Modified;
                    db.SaveChanges();

                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Member" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = "Member" };
            }
        }

    }

    public class CustomerPurchaseCriteria : CriteriaBase {
        public Nullable<int> Branch_ID { get; set; }
        public Nullable<int> Product_ID { get; set; }
        public Nullable<int> Product_Category_ID { get; set; }
    }
}
