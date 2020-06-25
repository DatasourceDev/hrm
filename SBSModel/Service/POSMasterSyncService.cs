using SBSModel.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace SBSModel.Models
{
    public class _POS_Receipt_Result
    {
        public string Receipt_Local_ID { get; set; }
        public string Receipt_No { get; set; }
        public string Receipt_Date { get; set; }
        public string Receipt_Time { get; set; }
        public Nullable<int> Receipt_ID { get; set; }
        public string Status { get; set; }
    }
    public class _POS_Terminal_Result
    {
        public Nullable<int> Terminal_ID { get; set; }
        public string Terminal_Local_ID { get; set; }
    }
    public class _POS_Shift_Result
    {
        public Nullable<int> Shift_ID { get; set; }
        public string Shift_Local_ID { get; set; }
    }


    public class POSMasterSyncService
    {
        public bool InsertPOS_ReceiptConfig(POS_Receipt_Configuration vReceiptConfig)
        {
            bool result = false;

            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(vReceiptConfig).State = EntityState.Added;
                    db.SaveChanges();
                    result = true;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public bool UpdatePOS_ReceiptConfig(POS_Receipt_Configuration vReceiptConfig)
        {
            bool result = false;

            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(vReceiptConfig).State = EntityState.Modified;
                    db.SaveChanges();
                    result = true;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public bool InsertPOS_Shift(POS_Shift vPOSShift)
        {
            bool result = false;

            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(vPOSShift).State = EntityState.Added;
                    db.SaveChanges();
                    result = true;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public bool UpdatePOS_Shift(POS_Shift vPOSShift)
        {
            bool result = false;

            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(vPOSShift).State = EntityState.Modified;
                    db.SaveChanges();
                    result = true;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public bool InsertPOS_Terminal(POS_Terminal vPOSTerminal)
        {
            bool result = false;

            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(vPOSTerminal).State = EntityState.Added;
                    db.SaveChanges();
                    result = true;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public bool UpdatePOS_Terminal(POS_Terminal vPOSTerminal)
        {
            bool result = false;

            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(vPOSTerminal).State = EntityState.Modified;
                    db.SaveChanges();
                    result = true;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public bool InsertPOS_Receipt(POS_Receipt vPOSReceipt)
        {
            bool result = false;

            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(vPOSReceipt).State = EntityState.Added;
                    db.SaveChanges();
                    result = true;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public bool UpdatePOS_Receipt(POS_Receipt vPOSReceipt)
        {
            bool result = false;

            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(vPOSReceipt).State = EntityState.Modified;
                    db.SaveChanges();
                    result = true;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public bool InsertPOS_Receipt_Products(POS_Products_Rcp vReceiptProduct)
        {
            bool result = false;

            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(vReceiptProduct).State = EntityState.Added;
                    db.SaveChanges();
                    result = true;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public bool UpdatePOS_Receipt_Products(POS_Products_Rcp vReceiptProduct)
        {
            bool result = false;

            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(vReceiptProduct).State = EntityState.Modified;
                    db.SaveChanges();
                    result = true;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public bool InsertPOS_Receipt_Payment(POS_Receipt_Payment vReceiptPayment)
        {
            bool result = false;

            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(vReceiptPayment).State = EntityState.Added;
                    db.SaveChanges();
                    result = true;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public bool UpdatePOS_Receipt_Payment(POS_Receipt_Payment vReceiptPayment)
        {
            bool result = false;

            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(vReceiptPayment).State = EntityState.Modified;
                    db.SaveChanges();
                    result = true;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public bool InsertInventory_Transaction(Inventory_Transaction vInventoryTransaction)
        {
            bool result = false;

            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(vInventoryTransaction).State = EntityState.Added;
                    db.SaveChanges();
                    result = true;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public bool UpdateInventory_Transaction(Inventory_Transaction vInventoryTransaction)
        {
            bool result = false;

            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(vInventoryTransaction).State = EntityState.Modified;
                    db.SaveChanges();
                    result = true;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public Company GetCompany(Nullable<int> pCompanyID, Nullable<DateTime> pUpdateOn = null)
        {
            using (var db = new SBS2DBContext())
            {
                var coms = db.Companies
                    .Include(i => i.Company_Details.Select(s => s.Currency))
                    .Include(i => i.Company_Details.Select(s => s.State1))
                    .Include(i => i.Company_Details.Select(s => s.Country1))
                    .Include(i => i.POS_Receipt_Configuration)
                    .Include(i => i.Taxes)
                     .Include(i => i.Taxes.Select(s => s.Tax_GST))
                     .Include(i => i.Taxes.Select(s => s.Tax_Surcharge))
                    .Include(i => i.Product_Table)
                    .Include(i => i.Member_Configuration)
                    .Where(w => w.Company_ID == pCompanyID)
                    ;

                if (pUpdateOn.HasValue)
                    coms = coms
                        .Where(w => w.Update_On > pUpdateOn
                            | w.POS_Receipt_Configuration.Where(w1=> EntityFunctions.CreateDateTime(w1.Update_On.Value.Year, w1.Update_On.Value.Month, w1.Update_On.Value.Day, w1.Update_On.Value.Hour, w1.Update_On.Value.Minute, w1.Update_On.Value.Second) > EntityFunctions.CreateDateTime(pUpdateOn.Value.Year, pUpdateOn.Value.Month, pUpdateOn.Value.Day, pUpdateOn.Value.Hour, pUpdateOn.Value.Minute, pUpdateOn.Value.Second)).Count() > 0
                            | w.Product_Table.Where(w2 => EntityFunctions.CreateDateTime(w2.Update_On.Value.Year, w2.Update_On.Value.Month, w2.Update_On.Value.Day, w2.Update_On.Value.Hour, w2.Update_On.Value.Minute, w2.Update_On.Value.Second) > EntityFunctions.CreateDateTime(pUpdateOn.Value.Year, pUpdateOn.Value.Month, pUpdateOn.Value.Day, pUpdateOn.Value.Hour, pUpdateOn.Value.Minute, pUpdateOn.Value.Second)).Count() > 0
                            | w.Member_Configuration.Where(w3 => EntityFunctions.CreateDateTime(w3.Update_On.Value.Year, w3.Update_On.Value.Month, w3.Update_On.Value.Day, w3.Update_On.Value.Hour, w3.Update_On.Value.Minute, w3.Update_On.Value.Second) > EntityFunctions.CreateDateTime(pUpdateOn.Value.Year, pUpdateOn.Value.Month, pUpdateOn.Value.Day, pUpdateOn.Value.Hour, pUpdateOn.Value.Minute, pUpdateOn.Value.Second)).Count() > 0
                            | w.Taxes.Where(w4 => EntityFunctions.CreateDateTime(w4.Update_On.Value.Year, w4.Update_On.Value.Month, w4.Update_On.Value.Day, w4.Update_On.Value.Hour, w4.Update_On.Value.Minute, w4.Update_On.Value.Second) > EntityFunctions.CreateDateTime(pUpdateOn.Value.Year, pUpdateOn.Value.Month, pUpdateOn.Value.Day, pUpdateOn.Value.Hour, pUpdateOn.Value.Minute, pUpdateOn.Value.Second)).Count() > 0);

                return coms.FirstOrDefault();
            }
        }


        public ServiceSynResult POSSyn(List<POS_Terminal> Terminal, List<POS_Shift> Shift, List<POS_Receipt> Receipt)
        {
            var currentdate = StoredProcedure.GetCurrentDate();
            var pService = new POSService();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (Terminal != null)
                    {
                        foreach (var row in Terminal)
                        {
                            if (row.Terminal_ID > 0)
                            {
                                //update
                                db.Entry(row).State = EntityState.Modified;
                            }
                            else
                            {
                                //insert
                                db.Entry(row).State = EntityState.Added;
                            }
                        }
                    }

                    var terminallocal = db.POS_Terminal.Local;
                    if (Shift != null)
                    {
                        foreach (var row in Shift)
                        {
                            POS_Terminal terminal = null;
                            if (!row.Terminal_ID.HasValue || row.Terminal_ID.Value == 0)
                            {
                                terminal = terminallocal.Where(w => w.Terminal_Local_ID == row.Terminal_Local_ID && w.Company_ID == row.Company_ID).FirstOrDefault();
                                if (terminal == null)
                                {
                                    terminal = db.POS_Terminal.Where(w => w.Terminal_Local_ID == row.Terminal_Local_ID && w.Company_ID == row.Company_ID).FirstOrDefault();
                                    if (terminal == null)
                                        return new ServiceSynResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Terminal" };

                                    row.Terminal_ID = terminal.Terminal_ID;
                                    row.Branch_ID = terminal.Branch_ID;
                                }
                                else
                                {
                                    row.POS_Terminal = terminal;
                                    row.Branch_ID = terminal.Branch_ID;
                                }
                            }
                            else
                            {
                                terminal = db.POS_Terminal.Where(w => w.Terminal_ID == row.Terminal_ID && w.Company_ID == row.Company_ID).FirstOrDefault();
                                if (terminal == null)
                                    return new ServiceSynResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Terminal" };

                                row.Terminal_ID = terminal.Terminal_ID;
                                row.Branch_ID = terminal.Branch_ID;
                            }


                            if (row.Shift_ID > 0)
                            {
                                //update
                                db.Entry(row).State = EntityState.Modified;
                            }
                            else
                            {
                                //insert
                                db.POS_Shift.Add(row);
                            }
                        }
                    }

                    var shiftlocal = db.POS_Shift.Local;
                    var rcpConfigs = db.POS_Receipt_Configuration;
                    if (Receipt != null)
                    {
                        int no = 0;
                        int comID = 0;
                        POS_Receipt_Configuration rcpConfig = null;
                        foreach (var row in Receipt)
                        {
                            //row.POS_Receipt_Payment = null;
                            //row.POS_Products_Rcp = null;
                            //row.Inventory_Transaction = null;

                            if (!row.Company_ID.HasValue)
                                return new ServiceSynResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Company" };

                            POS_Shift shift;
                            if (!row.Shift_ID.HasValue || row.Shift_ID.Value == 0)
                            {
                                shift = shiftlocal.Where(w => w.Shift_Local_ID == row.Shift_Local_ID && w.POS_Terminal.Company_ID == row.Company_ID).FirstOrDefault();
                                if (shift == null)
                                    return new ServiceSynResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Shift" };

                                row.POS_Shift = shift;
                            }
                            else
                            {
                                shift = db.POS_Shift.Where(w => w.Shift_ID == row.Shift_ID && w.POS_Terminal.Company_ID == row.Company_ID).FirstOrDefault();
                                if (shift == null)
                                    return new ServiceSynResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Shift" };

                                row.Shift_ID = shift.Shift_ID;
                            }

                            if (row.Status == ReceiptStatus.Hold)
                            {
                                db.Entry(row).State = EntityState.Added;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(row.Receipt_No))
                                {
                                    string rcpNo = "";
                                    if (comID != row.Company_ID.Value)
                                    {
                                        rcpConfig = rcpConfigs.Where(w => w.Company_ID == row.Company_ID).FirstOrDefault();
                                        if (rcpConfig == null)
                                            return new ServiceSynResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Receipt Configuration" };

                                        if (no == 0)
                                            no = rcpConfig.Ref_Count.HasValue ? rcpConfig.Ref_Count.Value : 0;

                                        comID = row.Company_ID.Value;
                                    }

                                    no++;
                                    rcpNo = rcpConfig.Prefix + currentdate.Date.ToString(rcpConfig.Date_Format) + no.ToString().PadLeft(rcpConfig.Num_Lenght.Value, '0') + rcpConfig.Suffix;
                                    rcpConfig.Ref_Count = no;
                                    row.Receipt_No = rcpNo;
                                }

                                if (row.Receipt_ID > 0)
                                {
                                    //update
                                    pService.UpdatePOSReceipt(db, row);
                                }
                                else
                                {
                                    pService.InsertPOSReceipt(db, row);
                                }
                            }

                        }
                    }
                    db.SaveChanges();

                    var tresult = new List<_POS_Terminal_Result>();
                    var sresult = new List<_POS_Shift_Result>();
                    var rcpresult = new List<_POS_Receipt_Result>();

                    if (Terminal != null)
                    {

                        foreach (var row in Terminal)
                        {
                            if (row.Terminal_ID == 0)
                            {
                                db.Entry(row).GetDatabaseValues();
                            }

                            tresult.Add((_POS_Terminal_Result)ObjectUtil.BindDefault(new _POS_Terminal_Result()
                            {
                                Terminal_Local_ID = row.Terminal_Local_ID,
                                Terminal_ID = row.Terminal_ID,
                            }));
                        }
                    }

                    if (Shift != null)
                    {
                        foreach (var row in Shift)
                        {
                            if (row.Shift_ID == 0)
                            {
                                db.Entry(row).GetDatabaseValues();
                            }
                            sresult.Add((_POS_Shift_Result)ObjectUtil.BindDefault(new _POS_Shift_Result()
                           {
                               Shift_Local_ID = row.Shift_Local_ID,
                               Shift_ID = row.Shift_ID,
                           }));
                        }
                    }
                    if (Receipt != null)
                    {
                        foreach (var row in Receipt)
                        {
                            if (row.Receipt_ID == 0)
                            {
                                db.Entry(row).GetDatabaseValues();
                            }
                            rcpresult.Add((_POS_Receipt_Result)ObjectUtil.BindDefault(new _POS_Receipt_Result()
                            {
                                Receipt_Local_ID = row.Receipt_Local_ID,
                                Receipt_No = row.Receipt_No,
                                Receipt_Date = DateUtil.ToDisplayDate2(row.Receipt_Date),
                                Receipt_Time = DateUtil.ToDisplayTime(row.Receipt_Date),
                                Receipt_ID = row.Receipt_ID,
                                Status = row.Status,
                            }));
                        }
                    }


                    return new ServiceSynResult()
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg = new Error().getError(ERROR_CODE.SUCCESS),
                        Field = "POS Syn",
                        TerminalResult = tresult,
                        ShiftResult = sresult,
                        RcpResult = rcpresult
                    };
                }
            }
            catch (DbUpdateException ex)
            {


                return new ServiceSynResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = "POS Syn" };
            }
        }


        public ServiceResult POSClear(string pMacAddress)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var terminals = db.POS_Terminal.Where(w => w.Mac_Address == pMacAddress);
                    foreach (var terminal in terminals)
                    {
                        var shifts = terminal.POS_Shift;
                        foreach (var shift in shifts)
                        {
                            var receipts = shift.POS_Receipt;
                            foreach (var receipt in receipts)
                            {
                                db.Inventory_Transaction.RemoveRange(receipt.Inventory_Transaction);
                                db.POS_Receipt_Payment.RemoveRange(receipt.POS_Receipt_Payment);
                                db.POS_Products_Rcp.RemoveRange(receipt.POS_Products_Rcp);
                            }

                            db.POS_Receipt.RemoveRange(receipts);
                        }
                        db.POS_Shift.RemoveRange(shifts);
                    }
                    db.POS_Terminal.RemoveRange(terminals);
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Error().getError(ERROR_CODE.SUCCESS), Field = "POS" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = "POS" };
            }
        }



    }


}