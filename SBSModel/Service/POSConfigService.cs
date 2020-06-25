using SBSModel.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBSModel.Models
{
    public class POSConfigService
    {
        public List<Inventory_Location> LstBranch(int pCompanyID)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.Inventory_Location
                        where a.Company_ID == pCompanyID
                        orderby a.Name
                        select a).ToList();
            }
        }

        public List<POS_Terminal> LstTerminal(Nullable<int> pCompanyID,string pMacAddress = "", Nullable<DateTime> pUpdateOn = null)
        {
            using (var db = new SBS2DBContext())
            {
                var terminals = db.POS_Terminal.Where(w => w.Company_ID == pCompanyID);
                if (!string.IsNullOrEmpty(pMacAddress))
                {
                    terminals = terminals.Where(w => w.Mac_Address == pMacAddress);
                }
                if (pUpdateOn.HasValue)
                {
                    terminals = terminals.Where(w => EntityFunctions.CreateDateTime(w.Update_On.Value.Year, w.Update_On.Value.Month, w.Update_On.Value.Day, w.Update_On.Value.Hour, w.Update_On.Value.Minute, w.Update_On.Value.Second) > EntityFunctions.CreateDateTime(pUpdateOn.Value.Year, pUpdateOn.Value.Month, pUpdateOn.Value.Day, pUpdateOn.Value.Hour, pUpdateOn.Value.Minute, pUpdateOn.Value.Second));
                }

                return terminals.OrderBy(o => o.Terminal_Name).ToList();
            }
        }

        public List<POS_Shift> LstShift(Nullable<int> pCompanyID, Nullable<int> pTerminalID, Nullable<DateTime> pUpdateOn = null)
        {
            using (var db = new SBS2DBContext())
            {
                var shifts = db.POS_Shift.Where(w => w.Company_ID == pCompanyID && w.Terminal_ID == pTerminalID);
              
                if (pUpdateOn.HasValue)
                {
                    shifts = shifts.Where(w => EntityFunctions.CreateDateTime(w.Update_On.Value.Year, w.Update_On.Value.Month, w.Update_On.Value.Day, w.Update_On.Value.Hour, w.Update_On.Value.Minute, w.Update_On.Value.Second) > EntityFunctions.CreateDateTime(pUpdateOn.Value.Year, pUpdateOn.Value.Month, pUpdateOn.Value.Day, pUpdateOn.Value.Hour, pUpdateOn.Value.Minute, pUpdateOn.Value.Second));
                }

                return shifts.ToList();
            }
        }

        public string GetMacAddress()
        {
            using (var db = new SBS2DBContext())
            {
                var dConf = (from a in db.Device_Configuration
                        where a.Field_Name == "MacAddress"
                        select a).FirstOrDefault();
                if (dConf != null)
                    return dConf.Field_Value;
                return "";
            }
        }

        public POS_Terminal GetTerminal(Nullable<int> pTerminalID)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.POS_Terminal
                        where a.Terminal_ID == pTerminalID
                        select a).FirstOrDefault();
            }
        }

        public POS_Terminal GetTerminalByMacAddress(string pMac)
        {
            using (var db = new SBS2DBContext())
            {
                return db.POS_Terminal.Include(i => i.User_Profile).Where(w => w.Mac_Address == pMac).FirstOrDefault();
            }
        }

        public POS_Terminal GetTerminalByTerminalName( Nullable<int> pCompanyID,string pTName)
        {
            using (var db = new SBS2DBContext())
            {
                return db.POS_Terminal.Include(i => i.User_Profile).Where(w => w.Terminal_Name == pTName && w.Company_ID == pCompanyID).FirstOrDefault();
            }
        }

        public POS_Terminal GetTerminalByLocalID(string pTerminalLocalID, Nullable<int> pCompanyID)
        {
           
                using (var db = new SBS2DBContext())
                {
                    return db.POS_Terminal.Include(i => i.User_Profile).Where(w => w.Terminal_Local_ID == pTerminalLocalID && w.Company_ID == pCompanyID).FirstOrDefault();
                }
            
        }

        public POS_Terminal GetTerminalByCashierID(int pCashierID)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.POS_Terminal
                        where a.Cashier_ID == pCashierID
                        select a)
                        .Include(p=> p.User_Profile).FirstOrDefault();
            }
        }
        public ServiceResult InsertTerminal(POS_Terminal pTerminal)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var runnig = db.SBS_No_Pattern.Where(w => w.Company_ID == pTerminal.Company_ID && w.Pattern_Type == Pattern_Type.Terminal).FirstOrDefault();
                    if(runnig == null)
                    {
                        runnig = new SBS_No_Pattern() { Company_ID = pTerminal.Company_ID, Pattern_Type = Pattern_Type.Terminal, Ref_Count = 1 };
                        var terinallcNo = "TLC" + pTerminal.Company_ID + "-" + (runnig.Ref_Count.ToString().PadLeft(6, '0'));
                        pTerminal.Terminal_Local_ID = terinallcNo;

                        runnig.Ref_Count = runnig.Ref_Count + 1;
                        db.SBS_No_Pattern.Add(runnig);

                    }
                    else
                    {
                        var terinallcNo = "TLC" + pTerminal.Company_ID + "-" + (runnig.Ref_Count.ToString().PadLeft(6, '0'));
                        pTerminal.Terminal_Local_ID = terinallcNo;
                        runnig.Ref_Count = runnig.Ref_Count + 1;
                    }

                  

                    db.Entry(pTerminal).State = EntityState.Added;
                    db.SaveChanges();

                    db.Entry(pTerminal).GetDatabaseValues();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = "Configuration" };
                }
            }
            catch 
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = "Configuration" };
            }
        }

        public ServiceResult UpdateTerminal(POS_Terminal pTerminal)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(pTerminal).State = EntityState.Modified;
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Configuration" };
                }
            }
            catch 
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = "Configuration" };
            }
        }

        public ServiceResult DeleteTerminal(POS_Terminal pTerminal)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(pTerminal).State = EntityState.Deleted;
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = "Configuration" };
                }
            }
            catch 
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = "Configuration" };
            }
        }

        public POS_Shift GetShift(Nullable<int> pShiftID)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.POS_Shift
                        where a.Shift_ID == pShiftID
                        select a).OrderByDescending(x => x.Shift_ID).FirstOrDefault();
            }
        }

        public POS_Shift GetShiftByLocalID(string pShiftLocalID, Nullable<int> pCompanyID)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.POS_Shift
                        where a.Shift_Local_ID == pShiftLocalID && a.POS_Terminal.Company_ID == pCompanyID
                        select a).FirstOrDefault();
            }
        }


        public ServiceResult InsertShift(POS_Shift pShift)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var runnig = db.SBS_No_Pattern.Where(w => w.Company_ID == pShift.Company_ID && w.Pattern_Type == Pattern_Type.Shift).FirstOrDefault();
                    if (runnig == null)
                    {
                        runnig = new SBS_No_Pattern() { Company_ID = pShift.Company_ID, Pattern_Type = Pattern_Type.Shift, Ref_Count = 1 };
                        var no = "SLC" + pShift.Company_ID + "-" + (runnig.Ref_Count.ToString().PadLeft(6, '0'));
                        pShift.Shift_Local_ID = no;

                        runnig.Ref_Count = runnig.Ref_Count + 1;
                        db.SBS_No_Pattern.Add(runnig);
                    }
                    else
                    {
                        var no = "SLC" + pShift.Company_ID + "-" + runnig.Ref_Count.ToString().PadLeft(6, '0');
                        pShift.Shift_Local_ID = no;
                        runnig.Ref_Count = runnig.Ref_Count + 1;
                    }
                    db.Entry(pShift).State = EntityState.Added;
                    db.SaveChanges();

                    db.Entry(pShift).GetDatabaseValues();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = "Shift" };
                }
            }
            catch 
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = "Shift" };
            }
        }

        public ServiceResult UpdateShift(POS_Shift pShift)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(pShift).State = EntityState.Modified;
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Shift" };
                }
            }
            catch 
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = "Shift" };
            }
        }

        public POS_Shift GetOpenShift(int CompanyID, int BranchID, string EmailID)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.POS_Shift
                        where a.Company_ID == CompanyID && a.Branch_ID == BranchID && a.Create_By == EmailID && a.Status == "Open"
                        select a).OrderByDescending(x => x.Shift_ID).FirstOrDefault();
            }
        }
    }

    public class ReceiptConfigService
    {
        public POS_Receipt_Configuration GetReceiptConfigByCompany(Nullable<int> pCompanyID)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.POS_Receipt_Configuration
                        where a.Company_ID == pCompanyID
                        select a).FirstOrDefault();
            }
        }

        public POS_Receipt_Configuration GetReceiptConfig(int pReceiptConfID)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.POS_Receipt_Configuration
                        where a.Receipt_Conf_ID == pReceiptConfID
                        select a).FirstOrDefault();
            }
        }

        public ServiceResult InsertReceiptConfig(POS_Receipt_Configuration rpConf)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(rpConf).State = EntityState.Added;
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = "Configuration" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = "Configuration" };
            }
        }

        public ServiceResult UpdateReceiptConfig(POS_Receipt_Configuration rpConf)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(rpConf).State = EntityState.Modified;
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Configuration" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = "Configuration" };
            }
        }

          // Added by Jane 07-08-2015
        public List<Member> LstMember(int pCompanyID, string pMemberName)
        {

            using (var db = new SBS2DBContext())
            {
                var c = db.Members.Where(w => w.Company_ID == pCompanyID);

                if (!string.IsNullOrEmpty(pMemberName))
                {
                    c = c.Where(w => w.Member_Name.Contains(pMemberName));
                }

                return c.OrderBy(o => o.Member_Name).ToList();
            }

        }

        public Member GetMember(int pMemberID)
        {

            using (var db = new SBS2DBContext())
            {
                return db.Members
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
                var currentdate = StoredProcedure.GetCurrentDate();
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

    public class MemberConfigService
    {
        public Member_Configuration GetMemberConfig(Nullable<int> pCompanyID, Nullable<int> MemberConfID = null)
        {
            using (var db = new SBS2DBContext())
            {
                var  memberCon = db.Member_Configuration.Where(w => w.Company_ID == pCompanyID);

                if (MemberConfID.HasValue && MemberConfID.Value > 0)
                {
                    memberCon = memberCon.Where(w => w.Member_Configuration_ID == MemberConfID);
                }
                   
                return memberCon.FirstOrDefault();
            }
        }

        public ServiceResult InsertMemberConfig(Member_Configuration MemberConf)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(MemberConf).State = EntityState.Added;
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = "Member Configuration" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = "Member Configuration" };
            }
        }

        public ServiceResult UpdatMemberConfig(Member_Configuration MemberConf)
        {
             try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(MemberConf).State = EntityState.Modified;
                    db.SaveChanges();

                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Member Configuration" };
                }
            }
            catch 
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = "Member Configuration" };
            }
        
        }

    }

}
