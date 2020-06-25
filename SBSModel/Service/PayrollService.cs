using SBSModel.Common;
using SBSModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using System.Data.Entity;
using SBSResourceAPI;
using System.Web.Configuration;
using System.IO;


namespace SBSModel.Models
{
    public class _Applicable_Employee
    {
        public Nullable<int> Employee_Profile_ID { get; set; }
        public Nullable<int> Department_ID { get; set; }
        public Nullable<int> Profile_ID { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }

    public class _PRD
    {
        public Nullable<int> Payroll_Detail_ID { get; set; }
        public string Type { get; set; }
        public Nullable<int> PRM_ID { get; set; }

        public Nullable<int> PRT_ID { get; set; }

        public Nullable<int> PRC_ID { get; set; }

        public Nullable<int> Currency_ID { get; set; }

        public Nullable<decimal> Amount { get; set; }

        public string Description { get; set; }
        public Nullable<decimal> Hours_Worked { get; set; }
        public string Row_Type { get; set; }
        public Nullable<int> History_Allowance_ID { get; set; }
    }

    public class _PRAL
    {
        public int I { get; set; }
        public List<_Applicable_Employee> EmpList { get; set; }
        public int Index { get; set; }
        public Nullable<int> PRAL_ID { get; set; }

        public Nullable<int> Employee_Profile_ID { get; set; }
        public Nullable<int> Profile_ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Row_Type { get; set; }
    }

    public class PayrollService
    {
        #region Payroll
        public List<PRM> LstPayrollByEmpID(int pEmployeeID, Nullable<int> pProcessMonth = 0, Nullable<int> pProcessYear = 0)
        {
            using (var db = new SBS2DBContext())
            {
                var Payrolls = db.PRMs
                      .Include(i => i.Employee_Profile)
                      .Include(i => i.Employee_Profile.User_Profile)
                      .Where(w => w.Employee_Profile_ID == pEmployeeID)
                     ;

                if (pProcessYear.HasValue && pProcessYear > 0)
                    Payrolls = Payrolls.Where(w => w.Process_Year == pProcessYear);
                if (pProcessMonth.HasValue && pProcessMonth > 0)
                    Payrolls = Payrolls.Where(w => w.Process_Month == pProcessMonth);
                return Payrolls.OrderByDescending(o => o.Process_Year)
                      .ThenByDescending(o => o.Process_Month)
                      .ToList();
            }
        }
        public PREL getPREL(int pEmpProfileID)
        {
            PREL p = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    p = db.PRELs
                        .Where(w => w.Employee_Profile_ID == pEmpProfileID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return p;
        }
        public List<Employee_Profile> ListPayroll(int? pComID, Nullable<int> pDepartment = null, int pProcessMonth = 0, int pProcessYear = 0, string pProcess = "")
        {
            var currentdate = StoredProcedure.GetCurrentDate();
            var db = new SBS2DBContext();

            //var groups = (from a in db.PRALs where a.Employee_Profile.Profile_ID == pHRUserProfileID select a.PRG_ID).ToList();

            //if (groups.Count == 0) return new List<Employee_Profile>();

            //var employees = (from a in db.PRELs where groups.Contains(a.PRG_ID) select a.Employee_Profile_ID).ToList();

            var payrolldate = DateUtil.ToDate(1, pProcessMonth, pProcessYear);

            //var empPayrolls = db.Employee_Profile.Where(w => employees.Contains(w.Employee_Profile_ID));

            //Edit by sun 16-11-2015
            var empPayrolls = db.Employee_Profile
                               .Include(i => i.User_Profile)
                               .Where(w => w.User_Profile.Company_ID == pComID && w.User_Profile.User_Status == RecordStatus.Active);
            //var ab = empPayrolls.ToList();

            //Added by Moet on 8-Aug-2016 to validate the employee who has acces right to Payroll module to be displayed
            //empPayrolls = empPayrolls.Where(w => w.User_Profile.User_Assign_Module.Where(we => we.Subscription.SBS_Module_Detail.Module_Detail_Name == "Payroll").FirstOrDefault() != null);
            //ab = empPayrolls.ToList();
            //var empActions = db.Employee_Profile_Action.Where(w => employees.Contains(w.Employee_Profile_ID.Value));
            //var a3 = empActions.ToList();
            //var a1 = empActions.Where(w => (w.Effective_Date < payrolldate)).ToList();

            //var a2 = empActions.Where(w => (w.Effective_Date < payrolldate && w.Action == "Hire")
            //                   || (w.Effective_Date.Value.Month == payrolldate.Value.Month && w.Effective_Date.Value.Year == payrolldate.Value.Year ));
            //var a4 = a2.ToList();
            //var y = empPayrolls.ToList();

            //empPayrolls = empPayrolls
            //    .Where(e => e.Employee_Profile_Action
            //        .Where(w => (w.Effective_Date < payrolldate && w.Action == "Hire")
            //            || (w.Effective_Date.Value.Month == payrolldate.Value.Month && w.Effective_Date.Value.Year == payrolldate.Value.Year))
            //            .OrderByDescending(o => o.Effective_Date)
            //            .FirstOrDefault() != null);
            //var y = empPayrolls.ToList();

            //empPayrolls = (from a in empPayrolls
            //               where a.Employee_Profile_Action
            //               .Where(w => (w.Effective_Date < payrolldate && w.Action == "Hire")
            //                   || (w.Effective_Date.Value.Month == payrolldate.Value.Month && w.Effective_Date.Value.Year == payrolldate.Value.Year ))
            //                   .OrderByDescending(o => o.Effective_Date)
            //                   .FirstOrDefault() != null
            //               select a);



            if (!string.IsNullOrEmpty(pProcess))
            {
                //y = empPayrolls.ToList();
                if (pProcess == "Y")
                {
                    empPayrolls = (from a in empPayrolls
                                   join b in db.PRMs on a.Employee_Profile_ID equals b.Employee_Profile_ID
                                   where (b.Process_Status == PayrollStatus.Process || b.Process_Status == PayrollStatus.Comfirm) &
                                   b.Process_Month == pProcessMonth & b.Process_Year == pProcessYear
                                   select a);

                }
                else
                {

                    var prms = (from a in db.PRMs where a.Process_Month == pProcessMonth & a.Process_Year == pProcessYear select a.Employee_Profile_ID);
                    empPayrolls = (from a in empPayrolls
                                   where prms.Contains(a.Employee_Profile_ID) == false
                                   select a);

                    var e = empPayrolls.ToList();
                }
            }
            if (pDepartment.HasValue)
            {
                var newEmpPayrolls = new List<Employee_Profile>();

                foreach (var emp in empPayrolls)
                {
                    var empHist = (from a in db.Employment_History
                                   where a.Effective_Date <= currentdate
                                   & a.Employee_Profile_ID == emp.Employee_Profile_ID
                                   & a.Department_ID == pDepartment.Value
                                   orderby a.Effective_Date descending
                                   select a).FirstOrDefault();

                    if (empHist != null)
                    {
                        newEmpPayrolls.Add(emp);
                    }

                }
                return newEmpPayrolls;
            }

            return empPayrolls.ToList();
        }

        public List<PRM> ListPayroll(Nullable<int> pCompanyID, Nullable<int> pDepartment = null, Nullable<int> pProcessMonthFrom = 0, Nullable<int> pProcessYearFrom = 0, Nullable<int> pProcessMonthTo = 0, Nullable<int> pProcessYearTo = 0)
        {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
                var prms = db.PRMs.Where(w => w.Employee_Profile.User_Profile.Company_ID == pCompanyID && w.Process_Status == PayrollStatus.Comfirm);

                if (pProcessMonthFrom.HasValue && pProcessMonthFrom.Value > 0)
                {
                    prms = prms.Where(w => w.Process_Month >= pProcessMonthFrom);
                }
                if (pProcessMonthTo.HasValue && pProcessMonthTo.Value > 0)
                {
                    prms = prms.Where(w => w.Process_Month <= pProcessMonthTo);
                }
                if (pProcessYearFrom.HasValue && pProcessYearFrom.Value > 0)
                {
                    prms = prms.Where(w => w.Process_Year >= pProcessYearFrom);
                }
                if (pProcessYearTo.HasValue && pProcessYearTo.Value > 0)
                {
                    prms = prms.Where(w => w.Process_Year <= pProcessYearTo);
                }


                if (pDepartment.HasValue && pDepartment.Value > 0)
                {
                    var selectEmps = new List<Employee_Profile>();
                    var emps = prms.Select(s => s.Employee_Profile).Distinct();
                    foreach (var emp in emps)
                    {
                        var empHist = (from a in db.Employment_History
                                       where a.Effective_Date <= currentdate
                                       & a.Employee_Profile_ID == emp.Employee_Profile_ID
                                       & a.Department_ID == pDepartment.Value
                                       orderby a.Effective_Date descending
                                       select a).FirstOrDefault();

                        if (empHist != null)
                        {
                            selectEmps.Add(emp);
                        }

                    }
                    var selectEmpID = selectEmps.Select(s => s.Employee_Profile_ID).ToList();
                    prms = prms.Where(w => selectEmpID.Contains(w.Employee_Profile_ID));

                }
                return prms
                    .Include(i => i.Employee_Profile)
                    .Include(i => i.Employee_Profile.User_Profile)
                    .Include(i => i.Employee_Profile.User_Profile.User_Authentication)
                    .Include(i => i.Employee_Profile.Employment_History)
                    .Include(i => i.PRDs)
                    .Include(i => i.PRDEs)
                    .Include(i => i.PRDEs.Select(s => s.Expenses_Application_Document))
                    .Include(i => i.PRDLs)
                    .Include(i => i.PRDs.Select(s => s.PRT))
                    .OrderBy(o => o.Employee_Profile_ID).ToList();
            }
        }

        public PRM GetPayroll(Nullable<int> pPrmID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.PRMs
                  .Include(i => i.Employee_Profile)
                  .Include(i => i.Employee_Profile.User_Profile)
                   .Include(i => i.Employee_Profile.User_Profile.User_Authentication)
                  .Include(i => i.Selected_Donation_Formula)
                  .Include(i => i.Selected_Donation_Formula.Donation_Formula)
                  .Include(i => i.Selected_Donation_Formula.Donation_Formula.Donation_Type)
                  .Include(i => i.Global_Lookup_Data)
                  .Where(w => w.PRM_ID == pPrmID).FirstOrDefault();
            }
        }

        public PRM GetPayrollByEmployeeID(Nullable<int> pEmployeeID, int pMonth, int pYear)
        {
            using (var db = new SBS2DBContext())
            {
                return db.PRMs
                    .Include(i => i.PRDEs)
                    .Include(i => i.PRDLs)
                    .Include(i => i.Selected_Donation_Formula)
                    .Include(i => i.Selected_Donation_Formula.Donation_Formula)
                    .Include(i => i.Selected_Donation_Formula.Donation_Formula.Donation_Type)
                    .Include(i => i.Global_Lookup_Data)
                    .Where(w => w.Employee_Profile_ID == pEmployeeID & w.Process_Month == pMonth & w.Process_Year == pYear)
                    .OrderByDescending(o => o.PRM_ID)
                    .FirstOrDefault();

            }
        }


        public List<DateTime> GetHoliday(int? pCompanyID, int? pMonth = null, int? pYear = null)
        {
            using (var db = new SBS2DBContext())
            {
                var bankHolidays = new List<DateTime>();
                var holidays = db.Holiday_Config.Where(w => w.Company_ID == pCompanyID);
                foreach (var h in holidays)
                {
                    if (h.End_Date.HasValue & h.Start_Date.HasValue)
                    {
                        for (var dt = h.Start_Date.Value; dt <= h.End_Date.Value; dt = dt.AddDays(1))
                        {
                            if (pMonth.HasValue)
                                if (dt.Month != pMonth)
                                    continue;
                            if (pYear.HasValue)
                                if (dt.Year != pYear)
                                    continue;
                            bankHolidays.Add(dt);
                        }
                    }
                    else
                    {
                        if (pMonth.HasValue)
                            if (h.Start_Date.Value.Month != pMonth)
                                continue;
                        if (pYear.HasValue)
                            if (h.Start_Date.Value.Year != pYear)
                                continue;
                        bankHolidays.Add(h.Start_Date.Value);
                    }
                }
                return bankHolidays;
            }
        }

        public ServiceResult PayrollConfirm(int? pPRMID)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var current = (from a in db.PRMs where a.PRM_ID == pPRMID select a).FirstOrDefault();
                    if (current != null)
                    {
                        current.Process_Status = PayrollStatus.Comfirm;
                    }
                    db.SaveChanges();
                }
                return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CONFIRM), Field = Resource.Payroll };
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_507_CONFIRM_ERROR), Field = Resource.Payroll };
            }
        }

        public ServiceResult PayrollConfirm(int[] pEmpIds, int pProcessMonth, int pProcessYear)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var prms = (from a in db.PRMs
                                where pEmpIds.Contains(a.Employee_Profile_ID)
                                && a.Process_Month == pProcessMonth
                                && a.Process_Year == pProcessYear
                                && a.Process_Status != PayrollStatus.Comfirm
                                select a);

                    foreach (var prm in prms)
                    {
                        prm.Process_Status = PayrollStatus.Comfirm;
                    }
                    var obj = prms.Select(s => s.PRM_ID).ToList();
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CONFIRM), Field = Resource.Payroll, Object = obj };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Payroll };
            }
        }

        public ServiceResult InsertPayroll(PRM[] prms)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.PRMs.AddRange(prms);
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Payroll };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Payroll };
            }
        }

        public ServiceResult InsertPayroll(List<PRM> prms)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    foreach (var prm in prms)
                    {
                        db.PRMs.Add(prm);
                    }
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Payroll };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Payroll };
            }
        }

        public ServiceResult InsertPayroll(PRM prm, List<_PRD> allowances, List<_PRD> extradonations, List<_PRD> overtimes, int[] leaves, int[] expenses, int company_id, decimal besicsalay)
        {
            try
            {
                var dws = new LeaveService().GetWorkingDayOfWeek(company_id, null, prm.Employee_Profile_ID);
                using (var db = new SBS2DBContext())
                {
                    //------leave doc-----------
                    var leavedocs = (from a in db.Leave_Application_Document where a.Employee_Profile_ID == prm.Employee_Profile_ID & a.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Closed select a);

                    var hollidays = (new List<DateTime>());
                    decimal wdays = 5;
                    if (leaves != null && leaves.Length > 0)
                    {
                        hollidays = GetHoliday(company_id);
                        var workingday = (from a in db.Working_Days where a.Company_ID == company_id select a).FirstOrDefault();
                        if (workingday != null)
                            wdays = workingday.Days.Value;
                    }

                    var expensesdocs = (from a in db.Expenses_Application_Document where a.Employee_Profile_ID == prm.Employee_Profile_ID & a.Expenses_Application.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Closed select a);

                    foreach (var e in expensesdocs)
                    {

                        if (expenses != null && expenses.Contains(e.Expenses_Application_Document_ID))
                        {
                            var haveprde = (from a in db.PRDEs
                                            where a.Expenses_Application_Document_ID == e.Expenses_Application_Document_ID &
                                            a.Expenses_Date == e.Date_Applied
                                            select a).FirstOrDefault();

                            if (haveprde == null)
                            {
                                var prde = new PRDE()
                                {
                                    Expenses_Application_Document_ID = e.Expenses_Application_Document_ID,
                                    PRM = prm,
                                    Expenses_Date = e.Date_Applied,
                                    Create_On = prm.Create_On,
                                    Create_By = prm.Create_By,
                                    Update_On = prm.Update_On,
                                    Update_By = prm.Update_By
                                };

                                db.PRDEs.Add(prde);
                                e.Payroll_Flag = PayrollFlag.Yes;
                            }

                        }
                    }

                    foreach (var l in leavedocs)
                    {
                        if (leaves != null && leaves.Contains(l.Leave_Application_Document_ID))
                        {
                            var otherprdls = (from a in db.PRDLs where a.Leave_Application_Document_ID == l.Leave_Application_Document_ID select a).ToList();
                            var newprlds = ProcessPrdl(otherprdls, prm.Leave_Period_From.Value, prm.Leave_Period_to.Value);

                            foreach (var p in newprlds)
                            {
                                var enddate = l.End_Date.HasValue ? l.End_Date : l.Start_Date;
                                if (p.Start_Date <= enddate)
                                {
                                    var prdl = ProcessPayrollLeave(l, p.Start_Date, p.End_Date, hollidays, dws, besicsalay, new PRDL());
                                    prdl.PRM = prm;
                                    db.PRDLs.Add(prdl);
                                }
                            }
                        }

                        var prdls = new List<PRDL>();
                        prdls.AddRange(from a in db.PRDLs where a.Leave_Application_Document_ID == l.Leave_Application_Document_ID select a);
                        prdls.AddRange(from a in db.PRDLs.Local where a.Leave_Application_Document_ID == l.Leave_Application_Document_ID select a);

                        var processday = (from a in prdls where a.Leave_Application_Document_ID == l.Leave_Application_Document_ID select a.Process_Day).Sum();
                        l.Processed_Day = processday;
                        l.Balance_Day = l.Days_Taken - processday;
                        if (processday == 0)
                            l.Payroll_Flag = PayrollFlag.No;
                        else
                        {
                            if (processday == l.Days_Taken)
                                l.Payroll_Flag = PayrollFlag.Yes;
                            else
                                l.Payroll_Flag = PayrollFlag.Partial;
                        }
                    }

                    //---------allowance----------
                    if (allowances != null)
                    {
                        var i = 0;
                        foreach (var row in allowances)
                        {
                            if (row.Row_Type == RowType.ADD)
                                InsertPayrollDetail(db, row, prm);
                            i++;
                        }
                    }
                    if (extradonations != null)
                    {
                        var i = 0;
                        foreach (var row in extradonations)
                        {
                            if (row.Row_Type == RowType.ADD)
                                InsertPayrollDetail(db, row, prm);
                            i++;
                        }
                    }
                    if (overtimes != null)
                    {
                        var i = 0;
                        foreach (var row in overtimes)
                        {
                            if (row.Row_Type == RowType.ADD)
                                InsertPayrollDetail(db, row, prm);
                            i++;
                        }
                    }

                    var rv = db.PRMs.Where(w => w.Employee_Profile_ID == prm.Employee_Profile_ID & w.Process_Month == prm.Process_Month & prm.Process_Year == prm.Process_Year).OrderByDescending(w => w.PRM_ID).FirstOrDefault();
                    if (rv != null)
                        prm.Revision_No = (rv.Revision_No.HasValue ? rv.Revision_No.Value : 0) + 1;

                    db.PRMs.Add(prm);
                    db.SaveChanges();
                    db.Entry(prm).GetDatabaseValues();
                }
                return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Payroll };
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Payroll };
            }
        }

        public ServiceResult UpdatePayroll(PRM prm, List<_PRD> allowances, List<_PRD> extradonations, List<_PRD> overtimes, int[] leaves, int[] expenses, decimal besicsalay)
        {
            try
            {

                using (var db = new SBS2DBContext())
                {
                    var current = (from a in db.PRMs where a.PRM_ID == prm.PRM_ID select a).FirstOrDefault();
                    if (current == null)
                        return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND) };

                    var expensesdocs = (from a in db.Expenses_Application_Document where a.Employee_Profile_ID == current.Employee_Profile_ID & a.Expenses_Application.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Closed select a);

                    foreach (var e in expensesdocs)
                    {
                        var delprdes = (from a in db.PRDEs where a.PRM_ID == prm.PRM_ID & a.Expenses_Application_Document_ID == e.Expenses_Application_Document_ID select a);
                        if (delprdes.FirstOrDefault() != null)
                        {
                            e.Payroll_Flag = PayrollFlag.No;
                            db.PRDEs.RemoveRange(delprdes);
                        }
                        if (expenses != null && expenses.Contains(e.Expenses_Application_Document_ID))
                        {

                            var haveprde = (from a in db.PRDEs
                                            where a.Expenses_Application_Document_ID == e.Expenses_Application_Document_ID &
                                            a.PRM_ID != prm.PRM_ID &
                                            a.Expenses_Date == e.Date_Applied
                                            select a).FirstOrDefault();

                            if (haveprde == null)
                            {
                                var prde = new PRDE()
                                {
                                    Expenses_Application_Document_ID = e.Expenses_Application_Document_ID,
                                    PRM_ID = prm.PRM_ID,
                                    Expenses_Date = e.Date_Applied,
                                    Create_On = prm.Create_On,
                                    Create_By = prm.Create_By,
                                    Update_On = prm.Update_On,
                                    Update_By = prm.Update_By
                                };

                                db.PRDEs.Add(prde);

                                e.Payroll_Flag = PayrollFlag.Yes;
                            }

                        }
                    }

                    var leavedocs = (from a in db.Leave_Application_Document where a.Employee_Profile_ID == current.Employee_Profile_ID & a.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Closed select a);

                    var dws = new List<int>();
                    List<DateTime> hollidays = new List<DateTime>();

                    if (leaves != null && leaves.Length > 0)
                    {
                        hollidays = GetHoliday(current.Employee_Profile.User_Profile.Company_ID.Value);
                        dws = new LeaveService().GetWorkingDayOfWeek(current.Employee_Profile.User_Profile.Company_ID, null, current.Employee_Profile_ID);
                    }

                    foreach (var l in leavedocs)
                    {
                        var delprdls = (from a in db.PRDLs where a.PRM_ID == prm.PRM_ID & a.Leave_Application_Document_ID == l.Leave_Application_Document_ID select a);
                        db.PRDLs.RemoveRange(delprdls);

                        if (leaves != null && leaves.Contains(l.Leave_Application_Document_ID))
                        {
                            var otherprdls = (from a in db.PRDLs where a.Leave_Application_Document_ID == l.Leave_Application_Document_ID where a.PRM_ID != prm.PRM_ID select a).ToList();
                            var newprlds = ProcessPrdl(otherprdls, prm.Leave_Period_From.Value, prm.Leave_Period_to.Value);

                            foreach (var p in newprlds)
                            {
                                var enddate = l.End_Date.HasValue ? l.End_Date : l.Start_Date;
                                if (p.Start_Date <= enddate)
                                {
                                    var prdl = ProcessPayrollLeave(l, p.Start_Date, p.End_Date, hollidays, dws, besicsalay, new PRDL());
                                    prdl.PRM_ID = prm.PRM_ID;
                                    db.PRDLs.Add(prdl);
                                }
                            }
                        }


                        var prdls = new List<PRDL>();
                        prdls.AddRange(from a in db.PRDLs.Local where a.Leave_Application_Document_ID == l.Leave_Application_Document_ID select a);

                        var prdlids = (from a in prdls select a.PRDL_ID);
                        prdls.AddRange(from a in db.PRDLs where a.Leave_Application_Document_ID == l.Leave_Application_Document_ID & !prdlids.Contains(a.PRDL_ID) select a);


                        var processday = (from a in prdls where a.Leave_Application_Document_ID == l.Leave_Application_Document_ID select a.Process_Day).Sum();
                        l.Processed_Day = processday;
                        l.Balance_Day = l.Days_Taken - processday;
                        l.Update_On = prm.Update_On;
                        l.Update_By = prm.Update_By;

                        if (processday == 0)
                            l.Payroll_Flag = PayrollFlag.No;
                        else
                        {
                            if (processday == l.Days_Taken)
                                l.Payroll_Flag = PayrollFlag.Yes;
                            else
                                l.Payroll_Flag = PayrollFlag.Partial;
                        }
                    }

                    if (allowances != null)
                    {
                        var i = 0;
                        foreach (var row in allowances)
                        {
                            var row_type = row.Row_Type;
                            row.PRM_ID = prm.PRM_ID;

                            UpdatePayrollDetail(db, row, prm);
                            i++;
                        }
                    }
                    if (extradonations != null)
                    {
                        var i = 0;
                        foreach (var row in extradonations)
                        {
                            var row_type = row.Row_Type;
                            row.PRM_ID = prm.PRM_ID;

                            UpdatePayrollDetail(db, row, prm);
                            i++;
                        }
                    }
                    if (overtimes != null)
                    {
                        var i = 0;
                        foreach (var row in overtimes)
                        {
                            row.PRM_ID = prm.PRM_ID;
                            UpdatePayrollDetail(db, row, prm);
                            i++;
                        }
                    }
                    db.Entry(current).CurrentValues.SetValues(prm);
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Payroll };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Payroll };
            }
        }

        private void InsertPayrollDetail(SBS2DBContext db, _PRD row, PRM prm)
        {

            if (row.PRC_ID > 0)
            {
                row.Description = null;
            }
            else
            {
                row.PRC_ID = null;
            }

            var prd = new PRD();
            prd.PRM_ID = row.PRM_ID;
            prd.Amount = row.Amount;
            prd.Currency_ID = row.Currency_ID;
            prd.Description = row.Description;
            prd.Hours_Worked = row.Hours_Worked;
            prd.PRT_ID = row.PRT_ID;
            prd.PRC_ID = row.PRC_ID;
            prd.PRM = prm;
            prd.Employment_History_Allowance_ID = row.History_Allowance_ID;
            prd.Create_On = prm.Create_On;
            prd.Create_By = prm.Create_By;
            prd.Update_On = prm.Update_On;
            prd.Update_By = prm.Update_By;

            db.PRDs.Add(prd);
        }

        private void UpdatePayrollDetail(SBS2DBContext db, _PRD row, PRM prm)
        {
            if (row.PRC_ID == 0) row.PRC_ID = null;

            if (row.Row_Type == RowType.ADD)
            {
                var prd = new PRD();
                prd.PRM_ID = row.PRM_ID;
                prd.Amount = row.Amount;
                prd.Currency_ID = row.Currency_ID;
                prd.Description = row.Description;
                prd.Hours_Worked = row.Hours_Worked;
                prd.PRT_ID = row.PRT_ID;
                prd.PRC_ID = row.PRC_ID;
                prd.Create_By = prm.Create_By;
                prd.Create_On = prm.Create_On;
                prd.Update_By = prm.Update_By;
                prd.Update_On = prm.Update_On;

                db.PRDs.Add(prd);
            }
            else if (row.Row_Type == RowType.EDIT)
            {
                var currentprd = (from a in db.PRDs where a.Payroll_Detail_ID == row.Payroll_Detail_ID select a).FirstOrDefault();
                if (currentprd != null)
                {
                    currentprd.Amount = row.Amount;
                    currentprd.Currency_ID = row.Currency_ID;
                    currentprd.Description = row.Description;
                    currentprd.Hours_Worked = row.Hours_Worked;
                    currentprd.PRM_ID = row.PRM_ID;
                    currentprd.PRT_ID = row.PRT_ID;
                    currentprd.PRC_ID = row.PRC_ID;
                    currentprd.Update_By = prm.Update_By;
                    currentprd.Update_On = prm.Update_On;
                }
            }
            else if (row.Row_Type == RowType.DELETE)
            {
                var currentprd = (from a in db.PRDs where a.Payroll_Detail_ID == row.Payroll_Detail_ID select a).FirstOrDefault();
                if (currentprd != null)
                {
                    db.PRDs.Remove(currentprd);
                }
            }
        }

        public List<PRDL> ProcessPrdl(List<PRDL> prdlsuse, DateTime datefrom, DateTime dateto)
        {
            var prdls = new List<PRDL>();
            Nullable<DateTime> startdate = null;
            //datefrom = DateUtil.ToDate("14/04/2014").Value;
            //dateto = DateUtil.ToDate("14/04/2014").Value;

            for (var dt = datefrom; dt <= dateto; dt = dt.AddDays(1))
            {
                if (startdate == null)
                {
                    var havedate = (from a in prdlsuse where dt >= a.Start_Date & dt <= a.End_Date select a).FirstOrDefault();
                    if (havedate == null)
                    {
                        startdate = dt;
                        var havenextdate = (from a in prdlsuse where dt.AddDays(1) >= a.Start_Date & dt.AddDays(1) <= a.End_Date select a).FirstOrDefault();
                        if (havenextdate != null)
                        {
                            prdls.Add(new PRDL() { Start_Date = startdate, End_Date = dt });
                            startdate = null;
                        }
                    }
                }
                else
                {
                    var havenextdate = (from a in prdlsuse where dt.AddDays(1) >= a.Start_Date & dt.AddDays(1) <= a.End_Date select a).FirstOrDefault();
                    if (havenextdate != null)
                    {
                        prdls.Add(new PRDL() { Start_Date = startdate, End_Date = dt });
                        startdate = null;
                    }
                }
                if (dt == dateto & startdate != null)
                {
                    prdls.Add(new PRDL() { Start_Date = startdate, End_Date = dt });
                    startdate = null;
                }
            }
            return prdls;
        }

        public PRDL ProcessPayrollLeave(Leave_Application_Document l, Nullable<DateTime> datefrom, Nullable<DateTime> dateto, List<DateTime> holidays, List<int> dws, decimal besicsalary, PRDL prdl)
        {
            if (dws == null)
                dws = new List<int>();

            decimal processday = 0;
            if (l.Start_Date.HasValue & l.End_Date.HasValue)
            {
                if (datefrom.HasValue & dateto.HasValue)
                {
                    if (datefrom <= l.Start_Date & dateto >= l.End_Date)
                    {
                        // Yes
                        //  |------------------------------------|
                        //      l.Start_Date         l.End_Date           
                        processday = (decimal)DateCal.BusinessDaysUntil(l.Start_Date.Value, l.Start_Date_Period, l.End_Date.Value, l.End_Date_Period, dws, holidays);
                        prdl.Start_Date = l.Start_Date.Value;
                        prdl.End_Date = l.End_Date.Value;
                    }
                    else if (datefrom >= l.Start_Date & dateto <= l.End_Date)
                    {
                        //Patial
                        //                 |--------|
                        //      l.Start_Date         l.End_Date
                        processday = (decimal)DateCal.BusinessDaysUntil(datefrom.Value, "", dateto.Value, "", dws, holidays);
                        prdl.Start_Date = datefrom.Value;
                        prdl.End_Date = dateto.Value;
                    }
                    else
                    {
                        //Patial
                        if (datefrom <= l.Start_Date & dateto <= l.End_Date)
                        {
                            //|---------------------|
                            //      l.Start_Date         l.End_Date
                            processday = (decimal)DateCal.BusinessDaysUntil(l.Start_Date.Value, l.Start_Date_Period, dateto.Value, "", dws, holidays);
                            prdl.Start_Date = l.Start_Date.Value;
                            prdl.End_Date = dateto.Value;
                        }
                        else if (datefrom >= l.Start_Date & dateto >= l.End_Date)
                        {
                            //      l.Start_Date         l.End_Date
                            //                     |--------------------|
                            processday = (decimal)DateCal.BusinessDaysUntil(datefrom.Value, "", l.End_Date.Value, l.End_Date_Period, dws, holidays);
                            prdl.Start_Date = datefrom.Value;
                            prdl.End_Date = l.End_Date.Value;
                        }

                    }
                }
            }
            else
            {
                processday = (decimal)DateCal.BusinessDaysUntil(l.Start_Date.Value, l.Start_Date_Period, null, "", dws, holidays);
                prdl.Start_Date = l.Start_Date.Value;
                prdl.End_Date = l.Start_Date.Value;

            }

            var wcnt = 5;
            if (dws.Count() > 0)
                wcnt = dws.Count();

            prdl.Leave_Application_Document_ID = l.Leave_Application_Document_ID;
            prdl.Process_Day = processday;
            prdl.Amount = NumUtil.ParseDecimal((((12 * besicsalary) / (52 * wcnt)) * processday).ToString("n2"));
            return prdl;
        }

        public PRD[] LstPRD(int pPRMID, string pPRTType, string pPRTType2 = "")
        {
            using (var db = new SBS2DBContext())
            {
                if (string.IsNullOrEmpty(pPRTType2))
                {
                    return db.PRDs.Include(i => i.PRC)
                .Include(i => i.PRM)
                .Include(i => i.PRT)
                .Where(w => w.PRM_ID == pPRMID & w.PRT.Type.Equals(pPRTType)).ToArray();
                }
                else
                {
                    return db.PRDs.Include(i => i.PRC)
               .Include(i => i.PRM)
               .Include(i => i.PRT)
               .Where(w => w.PRM_ID == pPRMID & (w.PRT.Type.Equals(pPRTType) | w.PRT.Type.Equals(pPRTType2))).ToArray();
                }

            }
        }

        public List<Employment_History_Allowance> LstHistoryAllowance(Nullable<int> pHistoryID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.Employment_History_Allowance
                    .Include(i => i.PRC)
                    .Include(i => i.PRT)
                    .Where(w => w.History_ID == pHistoryID).ToList();

            }
        }

        public List<Leave_Application_Document> GetLeaveApplicationDocument(int prm_id)
        {
            List<Leave_Application_Document> leaves = new List<Leave_Application_Document>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    return db.Leave_Application_Document
                       .Include(i => i.Employee_Profile)
                       .Include(i => i.Employee_Profile.User_Profile)
                       .Include(i => i.Leave_Config)
                       .Include(i => i.PRDLs)
                       .Where(w => w.PRDLs.Where(pw => pw.PRM_ID == prm_id).FirstOrDefault() != null).ToList();
                }
            }
            catch
            {
                return leaves;
            }
        }

        public List<Leave_Application_Document> GetLeaveApplicationDocument(int[] leaves_id)
        {
            List<Leave_Application_Document> leaves = new List<Leave_Application_Document>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var e = db.Leave_Application_Document
                       .Include(i => i.Employee_Profile)
                       .Include(i => i.Employee_Profile.User_Profile)
                       .Include(i => i.Leave_Config)
                       .Include(i => i.PRDLs);

                    if (leaves_id != null)
                    {
                        e = e.Where(w => leaves_id.Contains(w.Leave_Application_Document_ID));
                    }

                    return e.ToList();
                }
            }
            catch
            {
                return leaves;
            }
        }

        public List<Leave_Application_Document> GetLeaveApplicationDocument(int Employee_Profile_ID, string status = "", Nullable<DateTime> date_from = null, Nullable<DateTime> date_to = null, Nullable<int> prm_id = null)
        {
            List<Leave_Application_Document> leaves = new List<Leave_Application_Document>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var e = db.Leave_Application_Document
                          .Include(i => i.Employee_Profile)
                          .Include(i => i.Employee_Profile.User_Profile)
                          .Include(i => i.Leave_Config)
                          .Include(i => i.PRDLs)
                          .Where(w => w.Employee_Profile_ID == Employee_Profile_ID);

                    if (prm_id.HasValue)
                    {
                        e = e.Where(w => w.Days_Taken > (db.PRDLs.Where(pw => pw.Leave_Application_Document_ID == w.Leave_Application_Document_ID & pw.PRM_ID != prm_id).Sum(pw => (decimal?)pw.Process_Day) ?? 0));
                    }
                    else
                    {
                        e = e.Where(w => w.Days_Taken > (db.PRDLs.Where(pw => pw.Leave_Application_Document_ID == w.Leave_Application_Document_ID).Sum(pw => (decimal?)pw.Process_Day) ?? 0));
                    }

                    if (date_from.HasValue & date_to.HasValue)
                    {
                        e = e.Where(w => (w.Start_Date <= date_from & w.End_Date >= date_from) |
                            w.Start_Date == date_from |
                            w.End_Date == date_from |
                            (w.Start_Date <= date_to & w.End_Date >= date_to) |
                             w.Start_Date == date_to |
                            w.End_Date == date_to |
                            (date_from <= w.Start_Date & date_to >= w.Start_Date) |
                            (date_from <= w.End_Date & date_to >= w.End_Date));
                    }
                    else if (date_from.HasValue)
                    {
                        e = e.Where(w => (w.Start_Date <= date_from & w.End_Date >= date_from) | w.Start_Date == date_from | w.End_Date == date_from);
                    }
                    else if (date_to.HasValue)
                    {
                        e = e.Where(w => (w.Start_Date <= date_to & w.End_Date >= date_to) | w.Start_Date == date_to | w.End_Date == date_to);
                    }
                    if (!string.IsNullOrEmpty(status))
                    {
                        e = e.Where(w => w.Overall_Status == status);
                    }
                    e = e.OrderBy(o => o.Start_Date);
                    return e.ToList();
                }
            }
            catch
            {
                return leaves;
            }
        }

        public List<Expenses_Application_Document> GetExpenseApplications(int prm_id)
        {
            List<Expenses_Application_Document> expense = new List<Expenses_Application_Document>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    return db.Expenses_Application_Document
                          .Include(i => i.Employee_Profile)
                          .Include(i => i.Employee_Profile.User_Profile)
                          .Include(i => i.Currency)
                          .Include(i => i.Expenses_Config)
                          .Where(w => w.PRDEs.Where(pw => pw.PRM_ID == prm_id).FirstOrDefault() != null).ToList();
                }
            }
            catch
            {
            }
            return expense;
        }

        public List<Expenses_Application_Document> GetExpenseApplications(int[] expenses_id)
        {
            List<Expenses_Application_Document> expense = new List<Expenses_Application_Document>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var e = db.Expenses_Application_Document
                          .Include(i => i.Expenses_Application)
                          .Include(i => i.Employee_Profile)
                          .Include(i => i.Employee_Profile.User_Profile)
                          .Include(i => i.Currency)
                          .Include(i => i.Expenses_Config);

                    if (expenses_id != null)
                    {
                        e = e.Where(w => expenses_id.Contains(w.Expenses_Application_Document_ID));
                    }
                    return e.ToList();
                }
            }
            catch
            {
            }
            return expense;
        }

        public List<Expenses_Application_Document> GetPayrollExpense(int Employee_Profile_ID, string status = "", Nullable<DateTime> date_from = null, Nullable<DateTime> date_to = null, Nullable<int> prm_id = null, bool closed_status = false)
        {
            List<Expenses_Application_Document> expense = new List<Expenses_Application_Document>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var e = db.Expenses_Application_Document
                          .Include(i => i.Expenses_Application)
                          .Include(i => i.Employee_Profile)
                          .Include(i => i.Employee_Profile.User_Profile)
                          .Include(i => i.Currency)
                          .Include(i => i.Expenses_Config)
                       .Where(w => w.Employee_Profile_ID == Employee_Profile_ID && w.Expenses_Application.Overall_Status != RecordStatus.Delete);

                    if (date_from.HasValue & date_to.HasValue)
                    {
                        var dfrom = DateUtil.ToDate(date_from.Value.Day.ToString("00") + "/" + date_from.Value.Month.ToString("00") + "/" + date_from.Value.Year.ToString("0000") + " 00:00");
                        var dto = DateUtil.ToDate(date_to.Value.Day.ToString("00") + "/" + date_to.Value.Month.ToString("00") + "/" + date_to.Value.Year.ToString("0000") + " 23:59");
                        e = e.Where(w => w.Expenses_Application.Date_Applied >= dfrom & w.Expenses_Application.Date_Applied <= dto);

                        //e = e.Where(w => EntityFunctions.CreateDateTime(w.Expenses_Application.Date_Applied.Value.Year, w.Expenses_Application.Date_Applied.Value.Month, w.Expenses_Application.Date_Applied.Value.Day, w.Expenses_Application.Date_Applied.Value.Hour, w.Expenses_Application.Date_Applied.Value.Minute, w.Expenses_Application.Date_Applied.Value.Second) >=
                        //    EntityFunctions.CreateDateTime(date_from.Value.Year, date_from.Value.Month, date_from.Value.Day, date_from.Value.Hour, date_from.Value.Minute, date_from.Value.Second) &
                        //    EntityFunctions.CreateDateTime(w.Expenses_Application.Date_Applied.Value.Year, w.Expenses_Application.Date_Applied.Value.Month, w.Expenses_Application.Date_Applied.Value.Day, w.Expenses_Application.Date_Applied.Value.Hour, w.Expenses_Application.Date_Applied.Value.Minute, w.Expenses_Application.Date_Applied.Value.Second) <=
                        //    EntityFunctions.CreateDateTime(date_to.Value.Year, date_to.Value.Month, date_to.Value.Day, date_to.Value.Hour, date_to.Value.Minute, date_to.Value.Second));

                    }
                    else if (date_from.HasValue)
                    {
                        var dfrom = DateUtil.ToDate(date_from.Value.Day.ToString("00") + "/" + date_from.Value.Month.ToString("00") + "/" + date_from.Value.Year.ToString("0000") + " 00:00");
                        var dto = DateUtil.ToDate(date_from.Value.Day.ToString("00") + "/" + date_from.Value.Month.ToString("00") + "/" + date_from.Value.Year.ToString("0000") + " 23:59");

                        e = e.Where(w => w.Expenses_Application.Date_Applied >= dfrom & w.Expenses_Application.Date_Applied <= dto);
                    }
                    else if (date_to.HasValue)
                    {
                        var dfrom = DateUtil.ToDate(date_to.Value.Day.ToString("00") + "/" + date_to.Value.Month.ToString("00") + "/" + date_to.Value.Year.ToString("0000") + " 00:00");
                        var dto = DateUtil.ToDate(date_to.Value.Day.ToString("00") + "/" + date_to.Value.Month.ToString("00") + "/" + date_to.Value.Year.ToString("0000") + " 23:59");
                        e = e.Where(w => w.Expenses_Application.Date_Applied >= dfrom & w.Expenses_Application.Date_Applied <= dto);
                    }

                    if (!string.IsNullOrEmpty(status))
                        e = e.Where(w => w.Expenses_Application.Overall_Status == status);

                    if (closed_status)
                        e = e.Where(w => w.Expenses_Application.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Closed
                            && w.Expenses_Application.Cancel_Status != SBSWorkFlowAPI.Constants.WorkflowStatus.Cancelled);

                    foreach (var row in e)
                    {
                        if (prm_id == null)
                        {
                            //new Payroll
                            if (row.PRDEs.Count() == 0)
                            {
                                expense.Add(row);
                            }
                        }
                        else
                        {
                            if (row.PRDEs.Count() == 0)
                            {
                                expense.Add(row);
                            }
                            else
                            {
                                if (row.PRDEs.Where(w => w.PRM_ID == prm_id).FirstOrDefault() != null)
                                {
                                    expense.Add(row);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            return expense;
        }

        //Get PRM
        public List<PRM> GetPRMEmployee(Nullable<int> Employee_Profile_ID)
        {
            List<PRM> prm = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    prm = db.PRMs
                        .Include(i => i.PRDs.Select(s => s.PRT))
                        .Include(i => i.PRDs.Select(s => s.PRC))
                        .Include(i => i.PRDEs)
                        .Include(i => i.PRDEs.Select(s => s.Expenses_Application_Document))
                        .Include(i => i.PRDLs)
                        .Where(w => w.Employee_Profile_ID == Employee_Profile_ID).ToList();
                }
            }
            catch
            {
            }

            return prm;
        }

        //Get PRM
        public Employment_History GetPRMEmpHist(int Employee_Profile_ID, int Process_Month, int Process_Year)
        {


            using (var db = new SBS2DBContext())
            {
                var Effective_Date = DateUtil.ToDate(DateTime.DaysInMonth(Process_Year, Process_Month), Process_Month, Process_Year);

                return db.Employment_History
                    .Where(w => w.Employee_Profile_ID == Employee_Profile_ID && (w.Effective_Date <= Effective_Date || (w.Effective_Date.Value.Month == Effective_Date.Value.Month && w.Effective_Date.Value.Year == Effective_Date.Value.Year)))
                    .OrderByDescending(o => o.Effective_Date)
                    .FirstOrDefault();



            }

        }

        //Get ETIRA8
        public ETIRA8 GetETIRA8(int ETIRA8_ID)
        {
            ETIRA8 eTIRA8 = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    eTIRA8 = db.ETIRA8.Where(w => w.ETIRA8_ID == ETIRA8_ID).FirstOrDefault();
                }
            }
            catch
            {
            }

            return eTIRA8;
        }

        //Get ETIRA8
        public List<ETIRA8> GetETIRA8s(Nullable<int> Company_ID)
        {
            List<ETIRA8> eTIRA8 = new List<ETIRA8>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    //var et = db.ETIRA8.ToList();
                    eTIRA8 = db.ETIRA8.Where(w => w.Company_Running_ID == Company_ID).ToList();
                }
            }
            catch
            {
            }

            return eTIRA8;
        }

        //Get ETIRA8
        public decimal GetCommisionAmount(int empid, Nullable<DateTime> sdate = null, Nullable<DateTime> edate = null)
        {
            decimal amount = 0;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var coms = db.PRDs.Where(w => w.PRM.Employee_Profile_ID == empid && w.PRT.Type == PayrollAllowanceType.Commission);

                    if (sdate.HasValue)
                    {
                        coms = coms.Where(w => w.PRM.Process_Year > sdate.Value.Year || (w.PRM.Process_Year == sdate.Value.Year ? (w.PRM.Process_Month >= sdate.Value.Month) : false));
                    }
                    if (edate.HasValue)
                    {
                        coms = coms.Where(w => w.PRM.Process_Year < edate.Value.Year || (w.PRM.Process_Year == edate.Value.Year ? (w.PRM.Process_Month <= edate.Value.Month) : false));
                    }

                    amount = coms.Sum(s => (s.Amount.HasValue ? s.Amount.Value : 0));
                }
            }
            catch
            {
            }

            return amount;
        }

        //Get ETIRA8 of Employee
        public List<ETIRA8> GetETIRA8Employee(int Employee_Profile_ID)
        {
            List<ETIRA8> eTIRA8 = new List<ETIRA8>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    eTIRA8 = db.ETIRA8.Where(w => w.Employee_Profile_ID == Employee_Profile_ID).ToList();
                }
            }
            catch
            {
            }

            return eTIRA8;
        }

        //Insert ETIRA8
        public ServiceResult insertETIRA8(ETIRA8 eTIRA8)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(eTIRA8).State = EntityState.Added;
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.IR8A };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.IR8A };
            }
        }

        //Update ETIRA8
        public ServiceResult UpdateETIRA8(ETIRA8 eTIRA8)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(eTIRA8).State = EntityState.Modified;
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.IR8A };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.IR8A };
            }
        }

        //Delete ETIRA8
        public ServiceResult deleteETIRA8(ETIRA8 eTIRA8)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(eTIRA8).State = EntityState.Deleted;
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.IR8A };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.IR8A };
            }
        }

        //Added by Nay on 17-Aug-2015
        //Purpose : to retrieve year list of Paid payroll
        public List<int> GetCPFYear(Nullable<int> compID)
        {
            List<int> paidYears = new List<int>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    paidYears = (from p in db.PRMs
                                 join emp in db.Employee_Profile
                                     on p.Employee_Profile_ID equals emp.Employee_Profile_ID
                                 join user in db.User_Profile
                                     on emp.Profile_ID equals user.Profile_ID
                                 where user.Company_ID == compID
                                 select p.Process_Year.Value).Distinct().ToList();
                }
            }
            catch
            {
            }
            return paidYears;
        }

        public List<HR_FileExport_History> GetFileExport_HistoryList(Nullable<int> compID, string fileType)
        {
            var list = new List<HR_FileExport_History>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var q = (from a in db.HR_FileExport_History
                             where a.Company_ID == compID
                             && a.File_Type == fileType
                             select a).OrderByDescending(w => w.RFF_Code).ToList();

                    list = q;
                }
            }
            catch
            {

            }
            return list;
        }

        public List<HR_FileExport_History_Detail> GetCPF_HistoryDetailList(Nullable<int> generatedID)
        {
            var list = new List<HR_FileExport_History_Detail>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var q = (from a in db.HR_FileExport_History_Detail
                             where a.Generated_ID == generatedID
                             select a).ToList();

                    list = q;
                }
            }
            catch
            {

            }
            return list;
        }

        public List<HR_FileExport_History_Detail> GetCPF_HistoryDetailList(Nullable<int> generatedID, int DeptID, int DesgID, int Race, string Status)
        {
            var list = new List<HR_FileExport_History_Detail>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var q = (from a in db.HR_FileExport_History_Detail
                             where a.Generated_ID == generatedID
                             select a);

                    if (DeptID > 0)
                        q = q.Where(w => w.Department_ID == DeptID);

                    if (DesgID > 0)
                        q = q.Where(w => w.Designation_ID == DesgID);

                    if (Race > 0)
                        q = q.Where(w => w.Race == Race);

                    if (Status != "-")
                        q = q.Where(w => w.Residential_Status == Status);

                    list = q.ToList();
                }
            }
            catch { }
            return list;
        }

        public List<Department> GetDepartment(Nullable<int> compID)
        {
            var list = new List<Department>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var q = (from a in db.Departments
                             where a.Company_ID == compID
                             select a);

                    list = q.ToList();
                }
            }
            catch { }
            return list;
        }

        public List<Designation> GetDesignation(Nullable<int> compID)
        {
            var list = new List<Designation>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var q = (from a in db.Designations
                             where a.Company_ID == compID
                             select a);

                    list = q.ToList();
                }
            }
            catch { }
            return list;
        }

        public List<Global_Lookup_Data> GetRace()
        {
            var list = new List<Global_Lookup_Data>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var q = (from a in db.Global_Lookup_Data
                             where a.Def_ID == 4
                             select a);

                    list = q.ToList();
                }
            }
            catch { }
            return list;
        }

        //Added by Nay on 17-Aug-2015
        //Purpose : to check is there any payroll records for current month & year
        public ServiceResult chkCPFExists(Nullable<int> compID, int month, int year)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var q = (from p in db.PRMs
                             join emp in db.Employee_Profile
                                 on p.Employee_Profile_ID equals emp.Employee_Profile_ID
                             join user in db.User_Profile
                                 on emp.Profile_ID equals user.Profile_ID
                             where user.Company_ID == compID
                             && p.Process_Month == month
                             && p.Process_Year == year
                             select p);

                    if (q.Count() > 0)
                        return new ServiceResult() { Code = ERROR_CODE.SUCCESS_GENERATE, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_GENERATE), Field = Resource.CPF_File };
                    else
                        return new ServiceResult() { Code = ERROR_CODE.ERROR_18_NO_PAYROLL_RECORDS, Msg = new Error().getError(ERROR_CODE.ERROR_18_NO_PAYROLL_RECORDS), Field = Resource.CPF_File };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_18_NO_PAYROLL_RECORDS, Msg = new Error().getError(ERROR_CODE.ERROR_18_NO_PAYROLL_RECORDS), Field = Resource.CPF_File };
            }
        }

        //Added by Nay on 12-Oct-2015 
        //Purpose : to Get IR8A year list 
        public List<string> GetIR8AYear(Nullable<int> comp_ID)
        {
            using (var db = new SBS2DBContext())
            {
                var q = (from a in db.ETIRA8
                         select a.P_YEAR).Distinct().ToList();

                return q;
            }
        }

        public ServiceResult chkIR8AExists(Nullable<int> compID, int year)
        {
            string yr = Convert.ToString(year);
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var q = (from i in db.ETIRA8
                             where i.Company_Running_ID == compID
                             && i.P_YEAR == yr
                             select i);

                    if (q.Count() > 0)
                        return new ServiceResult() { Code = ERROR_CODE.SUCCESS_GENERATE, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_GENERATE), Field = Resource.IR8A_File };
                    else
                        return new ServiceResult() { Code = ERROR_CODE.ERROR_21_NO_IR8A_RECORDS, Msg = new Error().getError(ERROR_CODE.ERROR_18_NO_PAYROLL_RECORDS), Field = Resource.IR8A_File };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_21_NO_IR8A_RECORDS, Msg = new Error().getError(ERROR_CODE.ERROR_18_NO_PAYROLL_RECORDS), Field = Resource.IR8A_File };
            }
        }

        public ServiceResult chkIRA8AExists(Nullable<int> compID, int year)
        {
            string yr = Convert.ToString(year);
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var q = (from i in db.ETIRA8
                             where i.Company_Running_ID == compID
                             && i.P_YEAR == yr
                             select i);

                    if (q.Count() > 0)
                        return new ServiceResult() { Code = ERROR_CODE.SUCCESS_GENERATE, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_GENERATE), Field = Resource.IRA8A_File };
                    else
                        return new ServiceResult() { Code = ERROR_CODE.ERROR_21_NO_IR8A_RECORDS, Msg = new Error().getError(ERROR_CODE.ERROR_18_NO_PAYROLL_RECORDS), Field = Resource.IRA8A_File };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_21_NO_IR8A_RECORDS, Msg = new Error().getError(ERROR_CODE.ERROR_18_NO_PAYROLL_RECORDS), Field = Resource.IRA8A_File };
            }
        }

        public ServiceResult chkCPFConfig(Nullable<int> compID)
        {
            using (var db = new SBS2DBContext())
            {
                var q = (from a in db.Company_Details
                         where a.Company_ID == compID
                         select a).SingleOrDefault();

                if (q != null)
                {
                    if (q.CPF_Submission_No == null || q.patUser_ID == null || q.patPassword == null)
                        return new ServiceResult() { Code = ERROR_CODE.ERROR_22_NO_PAT_CONFIG, Msg = new Error().getError(ERROR_CODE.ERROR_22_NO_PAT_CONFIG), Field = Resource.CPF_File };
                    else
                        return new ServiceResult() { Code = ERROR_CODE.SUCCESS_GENERATE, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_GENERATE), Field = Resource.CPF_File };
                }
                else
                {
                    return new ServiceResult() { Code = ERROR_CODE.ERROR_22_NO_PAT_CONFIG, Msg = new Error().getError(ERROR_CODE.ERROR_22_NO_PAT_CONFIG), Field = Resource.CPF_File };
                }
            }
        }

        public ServiceResult chkIR8AConfig(Nullable<int> compID)
        {
            using (var db = new SBS2DBContext())
            {
                var q = (from a in db.Company_Details
                         where a.Company_ID == compID
                         select a).SingleOrDefault();

                if (q != null)
                {
                    if (q.Company_Source == null || q.PayerID_Type == null || q.PayerID_No == null || q.Branch_ID == null)
                        return new ServiceResult() { Code = ERROR_CODE.ERROR_22_NO_PAT_CONFIG, Msg = new Error().getError(ERROR_CODE.ERROR_22_NO_PAT_CONFIG), Field = Resource.IR8A_File };
                    else
                        return new ServiceResult() { Code = ERROR_CODE.SUCCESS_GENERATE, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_GENERATE), Field = Resource.IR8A_File };
                }
                else
                {
                    return new ServiceResult() { Code = ERROR_CODE.ERROR_22_NO_PAT_CONFIG, Msg = new Error().getError(ERROR_CODE.ERROR_22_NO_PAT_CONFIG), Field = Resource.IR8A_File };
                }
            }
        }

        //Added by Nay on 11-Nov-2015
        //To connect sftp
        public SftpClient checkConnection(string Address, string Port, string UserName)
        {
            SftpClient client = null;
            int sftpPort = Convert.ToInt16(Port);
            string sPrivateKeyPath = WebConfigurationManager.AppSettings["sFTPKeyFilePath"] + "\\" + WebConfigurationManager.AppSettings["sFTPKeyFileName"];
            PrivateKeyFile ObjPrivateKey = null;
            PrivateKeyAuthenticationMethod ObjPrivateKeyAutentication = null;
            using (Stream stream = File.OpenRead(sPrivateKeyPath))
            {
                if (WebConfigurationManager.AppSettings["PassPhraseCode"] != null)
                {
                    string sPassPhrase = WebConfigurationManager.AppSettings["PassPhraseCode"];
                    ObjPrivateKey = new PrivateKeyFile(stream, sPassPhrase);
                    ObjPrivateKeyAutentication = new PrivateKeyAuthenticationMethod(UserName, ObjPrivateKey);
                }
                else
                {
                    ObjPrivateKey = new PrivateKeyFile(stream);
                    ObjPrivateKeyAutentication = new PrivateKeyAuthenticationMethod(UserName, ObjPrivateKey);
                }

                ConnectionInfo objConnectionInfo = new ConnectionInfo(Address, sftpPort, UserName, ObjPrivateKeyAutentication);
                client = new SftpClient(objConnectionInfo);
            }
            //SftpClient client = new SftpClient(Address, Port, UserName, Password);

            return client;
        }

        //Added by Nay on 11-Nov-2015
        //to uplodate the file into sftp path
        public void UploadFileToSFTPServer(string localFilePath, SftpClient client, string sftpFolderPath)
        {
            client.Connect();

            if (!string.IsNullOrEmpty(sftpFolderPath))
            {
                client.ChangeDirectory(sftpFolderPath + @"/");
            }

            using (var fileStream = new FileStream(localFilePath, FileMode.Open))
            {
                client.BufferSize = 4 * 1024;
                client.UploadFile(fileStream, Path.GetFileName(localFilePath), null);
                fileStream.Close();
            }
            client.Disconnect();
            client.Dispose();

            //return rtnValue;
        }

        //Added by Nay on 11-Nov-2015
        //to retrieve out current FileExport header record.
        public HR_FileExport_History GetFileHistoryHeader(Nullable<int> generatedID)
        {
            var list = new List<HR_FileExport_History>();
            using (var db = new SBS2DBContext())
            {
                var q = (from a in db.HR_FileExport_History
                         where a.Generated_ID == generatedID.Value
                         select a).SingleOrDefault();

                return q;
            }
        }

        public List<HR_FileExport_IR8A_Detail> GetIR8A_DetailList(Nullable<int> generatedID)
        {
            var list = new List<HR_FileExport_IR8A_Detail>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var q = (from a in db.HR_FileExport_IR8A_Detail
                             where a.Generated_ID == generatedID
                             select a).ToList();

                    list = q;
                }
            }
            catch
            {

            }
            return list;
        }
        public List<HR_FileExport_IRA8A_Detail> GetIRA8A_DetailList(Nullable<int> generatedID)
        {
            var list = new List<HR_FileExport_IRA8A_Detail>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var q = (from a in db.HR_FileExport_IRA8A_Detail
                             where a.Generated_ID == generatedID
                             select a).ToList();

                    list = q;
                }
            }
            catch
            {

            }
            return list;
        }


        public List<HR_FileExport_IR8B_Detail> GetIR8B_DetailList(Nullable<int> generatedID)
        {
            var list = new List<HR_FileExport_IR8B_Detail>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var q = (from a in db.HR_FileExport_IR8B_Detail
                             where a.Generated_ID == generatedID
                             select a).ToList();

                    list = q;
                }
            }
            catch
            {

            }
            return list;
        }

        public List<HR_FileExport_IR8B_Detail> GetIR8B_DetailList(Nullable<int> generatedID, int DeptID, int DesgID)
        {
            var list = new List<HR_FileExport_IR8B_Detail>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var q = (from a in db.HR_FileExport_IR8B_Detail
                             where a.Generated_ID == generatedID
                             select a);

                    if (DeptID > 0)
                        q = q.Where(w => w.Department_ID == DeptID);

                    if (DesgID > 0)
                        q = q.Where(w => w.Designation_ID == DesgID);

                    list = q.ToList();
                }
            }
            catch { }
            return list;
        }

        public List<HR_FileExport_IR8S_Detail> GetIR8S_DetailList(Nullable<int> generatedID)
        {
            var list = new List<HR_FileExport_IR8S_Detail>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var q = (from a in db.HR_FileExport_IR8S_Detail
                             where a.Generated_ID == generatedID
                             select a).ToList();

                    list = q;
                }
            }
            catch
            {

            }
            return list;
        }

        public List<HR_FileExport_IR8A_Detail> GetIR8A_DetailList(Nullable<int> generatedID, int DeptID, int DesgID)
        {
            var list = new List<HR_FileExport_IR8A_Detail>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var q = (from a in db.HR_FileExport_IR8A_Detail
                             where a.Generated_ID == generatedID
                             select a);

                    if (DeptID > 0)
                        q = q.Where(w => w.Department_ID == DeptID);

                    if (DesgID > 0)
                        q = q.Where(w => w.Designation_ID == DesgID);

                    list = q.ToList();
                }
            }
            catch { }
            return list;
        }

        public List<HR_FileExport_IR8S_Detail> GetIR8S_DetailList(Nullable<int> generatedID, int DeptID, int DesgID)
        {
            var list = new List<HR_FileExport_IR8S_Detail>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var q = (from a in db.HR_FileExport_IR8S_Detail
                             where a.Generated_ID == generatedID
                             select a);

                    if (DeptID > 0)
                        q = q.Where(w => w.Department_ID == DeptID);

                    if (DesgID > 0)
                        q = q.Where(w => w.Designation_ID == DesgID);

                    list = q.ToList();
                }
            }
            catch { }
            return list;
        }
        #endregion


        //Added by sun 05-10-2015
        public ServiceResult InsertPRG(PRG pData)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {

                    db.PRGs.Add(pData);
                    db.SaveChanges();

                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Authorization };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Authorization };
            }

        }

        //Added by Moet on 2/Sep/2016
        public ServiceResult InsertPREL(PREL pData)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {

                    db.PRELs.Add(pData);
                    db.SaveChanges();

                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Authorization };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Authorization };
            }

        }

        //Added by Moet on 2/Sep/2016
        public ServiceResult InsertPRAL(PRAL pData)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {

                    db.PRALs.Add(pData);
                    db.SaveChanges();

                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Authorization };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Authorization };
            }

        }

        //Added by sun 05-10-2015
        public ServiceResult UpdatePRG(PRG pData)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var PRALsRemove = new List<PRAL>();
                    PRG pPRG = null;

                    pPRG = db.PRGs
                          .Include(i => i.PRALs)
                          .Include(i => i.PRALs.Select(s => s.Employee_Profile))
                          .Include(i => i.PRALs.Select(s => s.Employee_Profile.User_Profile))
                          .Include(i => i.PRALs.Select(s => s.Employee_Profile.User_Profile.User_Authentication))
                          .Include(i => i.PRELs)
                          .Include(i => i.PREDLs)
                          .Where(w => w.PRG_ID == pData.PRG_ID).FirstOrDefault();

                    if (pPRG == null)
                        return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.PRG + " " + Resource.Not_Found_Msg, Field = Resource.PRG };

                    foreach (var row in pPRG.PRALs.ToList())
                    {
                        if (pData.PRALs == null || !pData.PRALs.Select(s => s.Employee_Profile_ID).Contains(row.Employee_Profile_ID))
                        {
                            PRALsRemove.Add(row);
                            var p = db.PRALs.Where(w => w.PRAL_ID == row.PRAL_ID).FirstOrDefault();
                            if (p != null) db.PRALs.Remove(p);
                        }
                    }

                    if (PRALsRemove.Count > 0)
                    {
                        db.PRALs.RemoveRange(PRALsRemove);
                    }

                    if (pData.PRALs.Count != 0)
                    {
                        foreach (var row in pData.PRALs)
                        {
                            if (row.Employee_Profile_ID == 0 || !pPRG.PRALs.Select(s => s.Employee_Profile_ID).Contains(row.Employee_Profile_ID))
                            {
                                PRAL p = new PRAL();
                                p.Employee_Profile_ID = row.Employee_Profile_ID;
                                p.PRG_ID = pPRG.PRG_ID;
                                p.Create_By = row.Create_By;
                                p.Create_On = row.Create_On;
                                p.Update_By = row.Update_By;
                                p.Update_On = row.Update_On;
                                db.PRALs.Add(p);
                            }
                            else
                            {
                                var currPRALs = db.PRALs.Where(w => w.PRAL_ID == row.PRAL_ID).FirstOrDefault();
                                if (currPRALs != null)
                                {
                                    currPRALs.Employee_Profile_ID = row.Employee_Profile_ID;
                                    currPRALs.Update_By = pData.Update_By;
                                    currPRALs.Update_On = pData.Update_On;
                                    db.Entry(currPRALs).State = EntityState.Modified;
                                }
                            }
                        }
                    }

                    //---------------Employee-------------------//
                    if (pData.PRELs != null && pData.PRELs.Count > 0)
                    {
                        List<int> chk = new List<int>();
                        //Remove unchk
                        if (pPRG.PRELs != null && pPRG.PRELs.Count > 0)
                        {
                            foreach (PREL row in pPRG.PRELs.ToList())
                            {
                                if (pData.PRELs == null || !pData.PRELs.Select(s => s.Employee_Profile_ID).Contains(row.Employee_Profile_ID))
                                {
                                    var prel = db.PRELs.Where(w => w.PRG_ID == pPRG.PRG_ID && w.Employee_Profile_ID == row.Employee_Profile_ID).FirstOrDefault();
                                    if (prel != null) db.PRELs.Remove(prel);
                                }
                                chk.Add(row.Employee_Profile_ID);
                            }
                        }
                        //Add new 
                        foreach (var row in pData.PRELs)
                        {
                            if (!chk.Contains(row.Employee_Profile_ID))
                            {
                                PREL p = new PREL();
                                p.Employee_Profile_ID = row.Employee_Profile_ID;
                                p.PRG_ID = pPRG.PRG_ID;
                                p.Create_By = row.Create_By;
                                p.Create_On = row.Create_On;
                                p.Update_By = row.Update_By;
                                p.Update_On = row.Update_On;
                                db.PRELs.Add(p);
                            }
                        }
                    }
                    else //remove all
                    {
                        if (pPRG.PRELs != null && pPRG.PRELs.Count > 0)
                        {
                            var prels = db.PRELs.Where(w => w.PRG_ID == pPRG.PRG_ID);
                            db.PRELs.RemoveRange(prels);
                        }
                    }

                    //--------------Department--------------------//
                    if (pData.PREDLs != null && pData.PREDLs.Count > 0)
                    {
                        List<int> chk = new List<int>();
                        //Remove unchk
                        if (pPRG.PREDLs != null && pPRG.PREDLs.Count > 0)
                        {
                            foreach (PREDL row in pPRG.PREDLs.ToList())
                            {
                                if (pData.PREDLs == null || !pData.PREDLs.Select(s => s.Department_ID).Contains(row.Department_ID))
                                {
                                    var predl = db.PREDLs.Where(w => w.PRG_ID == pPRG.PRG_ID && w.Department_ID == row.Department_ID).FirstOrDefault();
                                    if (predl != null) db.PREDLs.Remove(predl);
                                }
                                chk.Add(row.Department_ID);
                            }
                        }
                        foreach (var row in pData.PREDLs)
                        {
                            if (!chk.Contains(row.Department_ID))
                            {
                                PREDL p = new PREDL();
                                p.Department_ID = row.Department_ID;
                                p.PRG_ID = pPRG.PRG_ID;
                                p.Create_By = row.Create_By;
                                p.Create_On = row.Create_On;
                                p.Update_By = row.Update_By;
                                p.Update_On = row.Update_On;
                                db.PREDLs.Add(p);
                            }
                        }
                    }
                    else //remove all
                    {
                        if (pPRG.PREDLs != null && pPRG.PREDLs.Count > 0)
                        {
                            var predls = db.PREDLs.Where(w => w.PRG_ID == pPRG.PRG_ID);
                            db.PREDLs.RemoveRange(predls);
                        }
                    }

                    pPRG.Update_By = pData.Update_By;
                    pPRG.Update_On = pData.Update_On;

                    db.SaveChanges();
                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Authorization };
                }
            }
            catch
            {
                return new ServiceResult { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Authorization };
            }
        }

        //--------------------Move All by sun 05-10-2015--------------------------//
        //Delete PRG
        public ServiceResult DeletePRG(PRG PRG)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {

                    if (PRG.PRALs != null && PRG.PRALs.Count > 0)
                    {
                        foreach (PRAL p in PRG.PRALs.ToList())
                        {
                            db.Entry(p).State = EntityState.Deleted;
                        }
                    }

                    if (PRG.PRELs != null && PRG.PRELs.Count > 0)
                    {
                        foreach (PREL p in PRG.PRELs.ToList())
                        {
                            db.Entry(p).State = EntityState.Deleted;
                        }
                    }
                    if (PRG.PREDLs != null && PRG.PREDLs.Count > 0)
                    {
                        foreach (PREDL p in PRG.PREDLs.ToList())
                        {
                            db.Entry(p).State = EntityState.Deleted;
                        }
                    }
                    db.Entry(PRG).State = EntityState.Deleted;
                    db.SaveChanges();
                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Authorization };
                }
            }
            catch
            {
                return new ServiceResult { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Authorization };
            }
        }

        //Move by sun 05-10-2015
        public List<PRG> GetPRGs(Nullable<int> pCompanyID, Nullable<int> pDepartmentID = null)
        {
            List<PRG> PRG = new List<PRG>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var PRGs = db.PRGs
                         .Include(i => i.PRALs)
                         .Include(i => i.PRALs.Select(s => s.Employee_Profile))
                         .Include(i => i.PRALs.Select(s => s.Employee_Profile.User_Profile))
                         .Include(i => i.PRELs)
                         .Include(i => i.PRELs.Select(s => s.Employee_Profile))
                         .Include(i => i.PRELs.Select(s => s.Employee_Profile.User_Profile))
                         .Include(i => i.PREDLs)
                         .Include(i => i.PREDLs.Select(s => s.Department))
                         .Where(w => w.Company_ID == pCompanyID && w.Record_Status != RecordStatus.Delete);

                    if (pDepartmentID.HasValue)
                    {
                        PRGs = PRGs.Where(w => w.PREDLs.Where(w2 => w2.Department_ID == pDepartmentID).FirstOrDefault() != null);
                    }
                    PRG = PRGs.ToList();
                }
            }
            catch
            {
            }
            return PRG;
        }

        //Get PRG
        public PRG GetPRG(Nullable<int> PRG_ID)
        {
            PRG PRG = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    PRG = db.PRGs
                        .Include(i => i.PRALs)
                        .Include(i => i.PRALs.Select(s => s.Employee_Profile))
                        .Include(i => i.PRALs.Select(s => s.Employee_Profile.User_Profile))
                        .Include(i => i.PRALs.Select(s => s.Employee_Profile.User_Profile.User_Authentication))
                        .Include(i => i.PRELs)
                        .Include(i => i.PREDLs)
                        .Where(w => w.PRG_ID == PRG_ID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return PRG;
        }

        //Added by Nay on 15-Jul-2015 
        //to delete multiple allowance records
        public ServiceResult MultipleDeletePRC(int[] PRCs)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    foreach (var PRC_ID in PRCs)
                    {
                        var prc = db.PRCs.Where(w => w.PRC_ID == PRC_ID).FirstOrDefault();
                        if (prc != null)
                        {
                            var departments = db.PRC_Department.Where(w => w.PRC_ID == PRC_ID);
                            db.PRC_Department.RemoveRange(departments);

                            db.PRCs.Remove(prc);
                        }
                    }
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Allowance_Or_Deduction };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Allowance_Or_Deduction };
            }
        }

        //Added by Nay on 15-Jul-2015 
        //Purpose : to check is there any references for PRC data or not
        public bool ChkPRDsed(Nullable<int> pPRC_ID)
        {
            var chkProblem = false;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var prcList = (from a in db.PRDs where a.PRC_ID == pPRC_ID select a).ToList();

                    if (prcList.Count > 0)
                        chkProblem = true;
                }
                return chkProblem;
            }
            catch
            {
                return true;
            }
        }

        //Added by Nay on 15-Jul-2015 
        //Purpose : to check is there any references for PRC data or not
        public bool ChkEmpHistoryAllwUsed(Nullable<int> pPRC_ID)
        {
            var chkProblem = false;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var prcList = (from a in db.Employment_History_Allowance where a.PRC_ID == pPRC_ID select a).ToList();

                    if (prcList.Count > 0)
                        chkProblem = true;
                }
                return chkProblem;
            }
            catch
            {
                return true;
            }
        }

        //Delete PRC
        public ServiceResult DeletePRC(Nullable<int> PRC_ID)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var prc = db.PRCs.Where(w => w.PRC_ID == PRC_ID).FirstOrDefault();
                    if (prc != null)
                    {
                        var departments = db.PRC_Department.Where(w => w.PRC_ID == PRC_ID);
                        db.PRC_Department.RemoveRange(departments);

                        db.PRCs.Remove(prc);
                    }
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Allowance_Or_Deduction };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Allowance_Or_Deduction };
            }
        }
        //Update PRC
        public ServiceResult UpdatePRC(PRC PRC)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var current = (from a in db.PRCs
                                   where a.PRC_ID == PRC.PRC_ID
                                   select a).FirstOrDefault();

                    if (current != null)
                    {
                        db.PRC_Department.RemoveRange(current.PRC_Department);
                        db.PRC_Department.AddRange(PRC.PRC_Department);
                        PRC.PRC_Department = null;

                        db.Entry(current).CurrentValues.SetValues(PRC);
                    }
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Allowance_Or_Deduction };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Allowance_Or_Deduction };
            }
        }

        //Insert PRC
        public ServiceResult InsertPRC(PRC PRC)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(PRC).State = EntityState.Added;
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Allowance_Or_Deduction };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Allowance_Or_Deduction };
            }
        }

        //Get PRCs
        public List<PRC> GetPRCs(Nullable<int> pCompanyID, Nullable<int> Type = null, Nullable<int> pDepartmentID = null)
        {
            List<PRC> PRC = new List<PRC>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var prcs = db.PRCs
                        .Include(i => i.PRT)
                        .Include(i => i.PRC_Department)
                        .Include(i => i.PRC_Department.Select(s => s.Department))
                        .Where(w => w.Company_ID == pCompanyID && w.Record_Status != RecordStatus.Delete);

                    if (pDepartmentID.HasValue)
                    {
                        prcs = prcs.Where(w => w.PRC_Department.Where(w2 => w2.Department_ID == pDepartmentID).Count() > 0);
                    }
                    if (Type != null)
                    {
                        prcs = prcs.Where(w => w.PRT_ID == Type);
                    }
                    PRC = prcs.ToList();
                }
            }
            catch
            {
            }
            return PRC;
        }

        //Get PRC
        public PRC GetPRC(Nullable<int> ID)
        {
            PRC PRC = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    PRC = db.PRCs.Include(i => i.PRT).Include(i => i.PRC_Department).Where(w => w.PRC_ID == ID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return PRC;
        }

        //Update Selected_Donation_Formula
        public ServiceResult UpdateSelectedDonationFormula(Selected_Donation_Formula formula)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(formula).State = EntityState.Modified;
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Selected_Donation_Formula };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Selected_Donation_Formula };
            }
        }

        //Insert Selected_Donation_Formula
        public ServiceResult InsertSelectedDonationFormula(Selected_Donation_Formula formula)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(formula).State = EntityState.Added;
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Selected_Donation_Formula };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Selected_Donation_Formula };
            }
        }

        //Get Selected_CPF_Formula
        public Selected_Donation_Formula GetSelectedDonationFormulas(Nullable<int> pCompanyID, Nullable<DateTime> Effective_Date, Nullable<int> pRaceID)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var race = db.Global_Lookup_Data.Where(w => w.Lookup_Data_ID == pRaceID).FirstOrDefault();

                    return db.Selected_Donation_Formula
                        .Include(i => i.Donation_Formula)
                        .Include(i => i.Donation_Formula.Donation_Type)
                        .Where(w => w.Company_ID == pCompanyID & w.Effective_Date <= Effective_Date && w.Donation_Formula.Global_Lookup_Data.Name == race.Name)
                        .OrderByDescending(o => o.Effective_Date).FirstOrDefault();
                }
            }
            catch
            {
            }
            return null;
        }

        //Get Selected_CPF_Formula
        public List<Selected_Donation_Formula> GetCurrentSelectedDonationFormulas(Nullable<int> pCompanyID)
        {
            var selected = new List<Selected_Donation_Formula>();
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var races = (from a in db.Global_Lookup_Data where a.Global_Lookup_Def.Name == ComboType.Race & (a.Company_ID == pCompanyID | a.Company_ID == null) & a.Record_Status == RecordStatus.Active orderby a.Description select a);
                    foreach (var race in races)
                    {
                        var formula = db.Selected_Donation_Formula
                            .Include(i => i.Donation_Formula)
                            .Where(w => w.Company_ID == pCompanyID & w.Effective_Date <= currentdate & w.Donation_Formula.Global_Lookup_Data.Name == race.Name)
                            .OrderByDescending(o => o.Effective_Date).FirstOrDefault();

                        if (formula != null)
                        {
                            selected.Add(formula);
                        }
                    }


                }
            }
            catch
            {
            }
            return selected;
        }

        //Get Selected_CPF_Formula
        public Selected_Donation_Formula GetSelectedDonationFormula(int Donation_Formula_ID, Nullable<int> pCompanyID)
        {
            Selected_Donation_Formula formula = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    formula = db.Selected_Donation_Formula.Include(i => i.Donation_Formula).Where(w => w.Donation_Formula_ID == Donation_Formula_ID && w.Company_ID == pCompanyID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return formula;
        }

        //Get Selected_CPF_Formula
        public Selected_Donation_Formula GetSelectedDonationFormula(Nullable<int> ID)
        {
            Selected_Donation_Formula formula = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    formula = db.Selected_Donation_Formula.Include(i => i.Donation_Formula).Where(w => w.ID == ID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return formula;
        }

        //Added by Nay on 15-Jul-2015 
        //to check is there any references for Donation Formula or not.
        public bool ChkDonationFormulaUsed(Nullable<int> pFormulaID)
        {
            var chkValue = false;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var usedDonation = db.Selected_Donation_Formula.Where(w => w.Donation_Formula_ID == pFormulaID).ToList();
                    if (usedDonation.Count > 0)
                    {
                        chkValue = true;
                    }
                    return chkValue;
                }
            }
            catch
            {
                return false;
            }
        }

        //Delete CPF_Formula
        public ServiceResult DeleteDonationFormula(Donation_Formula formula)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(formula).State = EntityState.Deleted;
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Donation_Formula };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Donation_Formula };
            }
        }

        //Update CPF_Formula
        public ServiceResult UpdateDonationFormula(Donation_Formula formula)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var current = (from a in db.Donation_Formula where a.Donation_Formula_ID == formula.Donation_Formula_ID select a).FirstOrDefault();
                    if (current == null)
                        return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = Resource.Donation_Formula };

                    db.Entry(current).CurrentValues.SetValues(formula);
                    db.SaveChanges();

                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Donation_Formula };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Donation_Formula };
            }
        }

        //Insert CPF_Formula
        public ServiceResult InsertDonationFormula(Donation_Formula formula)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(formula).State = EntityState.Added;
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Donation_Formula };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Donation_Formula };
            }
        }

        //Get CPF_Formula list
        public List<Donation_Formula> GetDonationFormulas(Nullable<int> pCompanyID = null, Nullable<int> pRace = null)
        {
            List<Donation_Formula> formula = new List<Donation_Formula>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    formula = db.Donation_Formula
                        .Include(i => i.Donation_Type)
                        .Include(i => i.Global_Lookup_Data)
                        .Where(w => w.Record_Status != RecordStatus.Delete)
                        .ToList();
                    if (pCompanyID.HasValue)
                    {
                        formula = formula.Where(w => w.Company_ID == pCompanyID || w.Company_ID == null).ToList();
                    }
                    else
                    {
                        formula = formula.Where(w => w.Company_ID == null).ToList();
                    }
                    if (pRace.HasValue)
                    {
                        formula = formula.Where(w => w.Race == pRace).ToList();
                    }
                }
            }
            catch
            {
            }
            return formula.OrderBy(o => o.Formula_Name).ToList();
        }
        //Get CPF_Formula
        public Donation_Formula GetDonationFormula(Nullable<int> Donation_Formula_ID)
        {
            Donation_Formula formula = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    formula = db.Donation_Formula.Include(i => i.Donation_Type).Include(i => i.Global_Lookup_Data).Where(w => w.Donation_Formula_ID == Donation_Formula_ID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return formula;
        }

        //Get Selected_CPF_Formula
        public Selected_OT_Formula GetCurrentSelectedOTFormulas(Nullable<int> pCompanyID)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    return db.Selected_OT_Formula.Include(i => i.OT_Formula)
                        .Where(w => w.Company_ID == pCompanyID & w.Effective_Date <= currentdate)
                        .OrderByDescending(o => o.Effective_Date).FirstOrDefault();
                }
            }
            catch
            {
            }
            return null;
        }

        //Get Selected_OT_Formula
        public Selected_OT_Formula GetSelectedOTFormula(int ID)
        {
            Selected_OT_Formula formula = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    formula = db.Selected_OT_Formula.Include(i => i.OT_Formula).Where(w => w.ID == ID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return formula;
        }

        //Update Selected_CPF_Formula
        public ServiceResult UpdateSelectedCPFFormula(Selected_CPF_Formula formula)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(formula).State = EntityState.Modified;
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Selected_CPF_Formula };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Selected_CPF_Formula };
            }
        }

        //Insert Selected_CPF_Formula
        public ServiceResult InsertSelectedCPFFormula(Selected_CPF_Formula formula)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(formula).State = EntityState.Added;
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Selected_CPF_Formula };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Selected_CPF_Formula };
            }
        }

        //Get Selected_CPF_Formula
        public Selected_CPF_Formula GetSelectedCPFFormulas(Nullable<int> pCompanyID, Nullable<DateTime> Effective_Date)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    return db.Selected_CPF_Formula.Include(i => i.CPF_Formula)
                        .Where(w => w.Company_ID == pCompanyID & w.Effective_Date <= Effective_Date)
                        .OrderByDescending(o => o.Effective_Date).FirstOrDefault();
                }
            }
            catch
            {
            }
            return null;
        }

        //Get Selected_CPF_Formula
        public Selected_CPF_Formula GetCurrentSelectedCPFFormulas(Nullable<int> pCompanyID)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    return db.Selected_CPF_Formula.Include(i => i.CPF_Formula)
                        .Where(w => w.Company_ID == pCompanyID & w.Effective_Date <= currentdate)
                        .OrderByDescending(o => o.Effective_Date).FirstOrDefault();
                }
            }
            catch
            {
            }
            return null;
        }

        //Get Selected_CPF_Formula
        public Selected_CPF_Formula GetSelectedCPFFormula(int CPF_Formula_ID, Nullable<int> pCompanyID)
        {
            Selected_CPF_Formula formula = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    formula = db.Selected_CPF_Formula.Include(i => i.CPF_Formula)
                        .Where(w => w.CPF_Formula_ID == CPF_Formula_ID).Where(w => w.Company_ID == pCompanyID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return formula;
        }

        //Get Selected_CPF_Formula
        public Selected_CPF_Formula GetSelectedCPFFormula(Nullable<int> ID)
        {
            Selected_CPF_Formula formula = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    formula = db.Selected_CPF_Formula.Include(i => i.CPF_Formula).Where(w => w.ID == ID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return formula;
        }

        //Added by Nay on 15-Jul-2015 
        //to check is there any references for Donation Formula or not.
        public bool ChkCPFFormulaUsed(Nullable<int> pFormulaID)
        {
            var chkValue = false;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var usedCPF = db.Selected_CPF_Formula.Where(w => w.CPF_Formula_ID == pFormulaID).ToList();
                    if (usedCPF.Count > 0)
                    {
                        chkValue = true;
                    }
                    return chkValue;
                }
            }
            catch
            {
                return false;
            }
        }

        //Delete CPF_Formula
        public ServiceResult DeleteCPFFormula(CPF_Formula formula)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(formula).State = EntityState.Deleted;
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.CPF_Formula };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.CPF_Formula };
            }
        }

        //Update CPF_Formula
        public ServiceResult UpdateCPFFormula(CPF_Formula formula)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(formula).State = EntityState.Modified;
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.CPF_Formula };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.CPF_Formula };
            }
        }

        //Insert CPF_Formula
        public ServiceResult InsertCPFFormula(CPF_Formula formula)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(formula).State = EntityState.Added;
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.CPF_Formula };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.CPF_Formula };
            }
        }

        //Get CPF_Formula list
        public List<CPF_Formula> GetCPFFormulas(Nullable<int> pYear = null)
        {
            List<CPF_Formula> formulas = new List<CPF_Formula>();
            try
            {
                using (var db = new SBS2DBContext())
                {

                    formulas = db.CPF_Formula.Where(w => w.Record_Status != RecordStatus.Delete).ToList();

                    if (pYear.HasValue)
                    {
                        formulas = formulas.Where(w => w.Year == pYear).ToList();
                    }

                }
            }
            catch
            {

            }
            return formulas;
        }
       
        public CPF_Formula GetLatestCPFFormulas()
        {
           try
           {
              using (var db = new SBS2DBContext())
              {
                 return db.CPF_Formula.Where(w => w.Record_Status != RecordStatus.Delete).OrderByDescending(o=>o.Year).FirstOrDefault();
              }
           }
           catch
           {
              return null;
           }
        }
        //Get CPF_Formula
        public CPF_Formula GetCPFFormula(Nullable<int> CPF_Formula_ID)
        {
            CPF_Formula formula = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    formula = db.CPF_Formula.Where(w => w.CPF_Formula_ID == CPF_Formula_ID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return formula;
        }

        //if error return index of string formula else return -1
        public int ValidateFormula(string formula)
        {
            formula = formula.TrimEnd() + ' ';
            formula = formula.Replace("–", "-");

            string[] variables = { FormulaVariable.Basic_Salary, FormulaVariable.Deduction_Ad_Hoc, FormulaVariable.Leave_Amount, FormulaVariable.Allowance, FormulaVariable.Deduction,
                                    FormulaVariable.Adjustment_Allowance, FormulaVariable.Adjustment_Deductions, FormulaVariable.Commission, 
                                    FormulaVariable.Overtime, FormulaVariable.Employee_Contribution,FormulaVariable.Total_CPF_Contribution, FormulaVariable.Employee_Residential_Status, 
                                    FormulaVariable.Employee_Age, FormulaVariable.PR_Years, FormulaVariable.Employee_Total_Wages, FormulaVariable.Current_Date, FormulaVariable.Local, 
                                    FormulaVariable.PR,FormulaVariable.Internship,FormulaVariable.Employee_Status,
                                    FormulaVariable.Bonus, FormulaVariable.Donation 
                                 };
            string[] opertaions = { "+", "-", "*", "/", }; //"=", "(", ")",
            string[] condition_opertions = { ">", "<", ">=", "<=", "==", "&", "|" };
            string[] conditions = { "If", "Else", "Else if", "End if", "Then" };

            string vars = "";
            bool validateVar = false;
            bool validateCon = false;
            bool isNum = false;
            int index = 0;
            Stack<string> stack = new Stack<string>();
            Stack<string> stack_condition = new Stack<string>();

            foreach (char f in formula)
            {

                System.Diagnostics.Debug.Write(f.ToString());

                //check variable if found [
                if (f == '[' | validateVar)
                {
                    if (f == '[')
                    {
                        validateVar = true;
                        vars = "";
                    }

                    if (f == ']') //found ]
                    {
                        vars += f;

                        if (!variables.Contains(vars)) //
                        {
                            return index - vars.Count();
                        }
                        else
                        {
                            validateVar = false;
                            if (validateCon)
                                stack_condition.Push(vars);
                            else
                                stack.Push(vars);
                            vars = "";
                        }
                    }
                    else
                    {
                        vars += f;
                    }
                }
                else if (!validateCon && f == '=' && (formula.Length >= index + 1 && formula[index + 1] != '=')) // if contain '=' x 2, error
                {
                    if (stack.Contains("="))
                    {
                        return index - 1;
                    }
                    else
                    {
                        //chk i

                        stack.Push("=");
                    }
                }
                else if (opertaions.Contains(f.ToString())) //Operation
                {
                    //chk previous, cannot be +,-,*,/,=
                    /*if (opertaions.Contains(stack.LastOrDefault()) | stack.LastOrDefault() == "=")
                    {
                        return index - 1;
                    }
                    else
                    {
                        stack.Push(f.ToString());
                    }
                    */
                    if (validateCon)
                        stack_condition.Push(f.ToString());
                    else
                        stack.Push(f.ToString());

                }
                else if (f == '(') // (
                {
                    if (validateCon)
                        stack_condition.Push(f.ToString());
                    else
                        stack.Push(f.ToString());
                }
                else if (f == ')') // )
                {
                    if (validateCon)
                        stack_condition.Push(f.ToString());
                    else
                        stack.Push(f.ToString());
                }
                else if (char.IsNumber(f) | f == '.' | isNum) // 0-9
                {
                    if (formula.Length < index + 1 || (formula[index + 1] != '.' && !char.IsNumber(formula[index + 1])))
                    {
                        vars += f;
                        if (validateCon)
                            stack_condition.Push(vars);
                        else
                            stack.Push(vars);
                        isNum = false;
                        vars = "";
                    }
                    else if (char.IsNumber(f) | f == '.')
                    {
                        isNum = true;
                        vars += f;
                    }
                    else
                    {
                        return index - 1;
                    }
                }
                else if ((f == '>' | f == '<') && formula.Length >= index + 1 && formula[index + 1] == ' ') //>, <
                {
                    stack_condition.Push(f.ToString());
                    vars = "";
                }
                else if (f == '=' && (formula[index - 1] == '>' | formula[index - 1] == '<' | formula[index - 1] == '=')) // >=, <=, ==
                {
                    stack_condition.Push(formula[index - 1].ToString() + "=");
                    vars = "";
                }
                else if (f == '&' | f == '|') // & |
                {
                    stack_condition.Push(f + "");
                }
                else if ((f == '>' | f == '>' | f == '=') && formula.Length >= index + 1 && formula[index + 1] == '=') // >=, <=, ==
                {
                    //cont.
                }
                else if (f == '\r')
                {
                    //cont.
                }
                else if (f == '\n') //next line; formula or condition
                {
                    //pop to chk stack

                    string str;
                    Stack<string> pars = new Stack<string>();
                    string previous = "";
                    while (stack.Count > 0)
                    {
                        str = stack.Pop();

                        if (opertaions.Contains(str)) //+, -, *, /
                        {
                            if (opertaions.Contains(previous)) //++
                            {
                                return index;
                            }
                            else if (previous == "") // .... +;
                            {
                                return index;
                            }
                            else
                            {
                                previous = str;
                            }
                        }
                        else if (variables.Contains(str)) //var
                        {
                            if (variables.Contains(previous)) //[A][B]
                            {
                                return index;
                            }
                            else
                            {
                                previous = str;
                            }
                        }
                        else if (str == "=") // =
                        {
                            if (opertaions.Contains(previous)) // +=, ==
                            {
                                return index;
                            }
                            else
                            {
                                //Num or var

                                previous = str;
                            }
                        }
                        else if (str == "(" | str == ")") //var
                        {
                            pars.Push(str);
                        }
                        else //Number
                        {
                            if (previous == "")
                            {
                                //cont.
                                previous = str;
                            }
                            else if (!opertaions.Contains(previous) && !variables.Contains(previous)) //1 2
                            {
                                return index;
                            }
                            else
                            {
                                previous = str;
                            }

                            //CAST TO DOUBLE
                            try
                            {
                                Convert.ToDouble(str);
                            }
                            catch
                            {
                                return index;
                            }
                        }
                    }

                    //chk ()
                    int close = 0;
                    while (pars.Count > 0)
                    {
                        string par = pars.Pop();
                        if (par == ")")
                        {
                            close--;
                        }
                        else
                        {
                            close++;
                        }

                        if (close < 0)
                        {
                            return index;
                        }
                    }

                    if (close != 0)
                    {
                        return index;
                    }

                    if (validateCon)
                    {
                        stack_condition.Push(vars);
                        stack_condition.Push("");
                    }

                    validateCon = false;
                    stack = new Stack<string>();
                    vars = "";
                }
                else if (f == 'I' | f == 'E' | f == 'T' | validateCon) // If/Else/Else if/End if
                {

                    if (!validateCon && (f == 'I' | f == 'E' | f == 'T'))
                    {
                        vars += f;
                        validateCon = true;
                    }
                    else if (formula.Length > index + 1 && formula[index + 1] == ' '
                        && formula.Length > index + 2 && formula[index + 2] == 'i') // _if
                    {
                        //cont.
                        vars += f;
                    }
                    else if (formula.Length == index | (formula.Length > index + 1 && (formula[index + 1] == ' ' | formula[index + 1] == '\n' | formula[index + 1] == '\0')))
                    {
                        vars += f;

                        if (vars == " ")
                        {
                            //cont.
                            vars = "";
                        }
                        else if (!conditions.Contains(vars))
                        {
                            return index - vars.Length;
                        }
                        else
                        {
                            stack_condition.Push(vars);
                            vars = "";
                        }
                    }
                    else if (f == ' ' && formula.Length > index + 1 && formula[index + 1] == 'i')
                    {
                        vars += f;
                    }
                    else if (f != ' ')
                    {
                        vars += f;
                    }
                }
                else if (f == ' ')
                {

                }
                else if (f == '\t')
                {

                }
                else
                {
                    return index;
                }

                index++;
            }

            //pop to chk stack_conditon
            string cond = "";
            Stack<string> cond_pars = new Stack<string>();
            Stack<string> cond_cond = new Stack<string>();
            string cond_previous = "";
            while (stack_condition.Count > 0)
            {
                cond = stack_condition.Pop();

                if (conditions.Contains(cond))
                {
                    cond_cond.Push(cond);
                }
                else if (condition_opertions.Contains(cond))
                {
                    if (condition_opertions.Contains(cond_previous)) // >=>=
                    {
                        return index;
                    }
                    if (opertaions.Contains(cond_previous)) // >=+
                    {
                        return index;
                    }
                    else
                    {
                        cond_previous = cond;
                    }
                }
                else if (variables.Contains(cond))
                {
                    if (variables.Contains(cond_previous)) // [A][B]
                    {
                        return index;
                    }
                    else
                    {
                        cond_previous = cond;
                    }
                }
                else if (opertaions.Contains(cond)) //+, -, *, /
                {
                    if (opertaions.Contains(cond_previous)) //++
                    {
                        return index;
                    }
                    else if (cond_previous == "") // .... +;
                    {
                        return index;
                    }
                    else
                    {
                        cond_previous = cond;
                    }
                }
                else if (cond == ")" | cond == "(")
                {
                    cond_pars.Push(cond);
                }
                else
                {
                    if (cond_previous == "")
                    {
                        //cont.
                    }
                    else if (cond == "")
                    {
                        //cont.
                    }
                    else if (!opertaions.Contains(cond_previous) && !variables.Contains(cond_previous)) //1 2
                    {
                        if (cond_previous == "|" | cond_previous == "&")
                        {

                        }
                        else
                        {
                            return index;
                        }

                    }
                    else
                    {

                        //CAST TO DOUBLE
                        try
                        {
                            Convert.ToDouble(cond);
                        }
                        catch
                        {
                            return index;
                        }
                    }

                    cond_previous = cond;
                }
            }

            //chk ()
            int cond_close = 0;
            while (cond_pars.Count > 0)
            {
                string par = cond_pars.Pop();
                if (par == ")")
                {
                    cond_close--;
                }
                else
                {
                    cond_close++;
                }

                if (cond_close < 0)
                {
                    return index;
                }
            }

            if (cond_close != 0)
            {
                return index;
            }

            cond_close = 0;
            string pre_cond = "";
            while (cond_cond.Count > 0)
            {
                string par = cond_cond.Pop();
                if (par == "End if")
                {
                    if (pre_cond == "Then" | pre_cond == "Else" | pre_cond == "End if")
                    {

                    }
                    else
                    {
                        return index;
                    }
                    cond_close--;
                    pre_cond = "End if";
                }
                else if (par == "Then")
                {
                    if (pre_cond == "Then" | pre_cond == "Else" | pre_cond == "End if")
                    {
                        return index;
                    }
                    pre_cond = "Then";
                }
                else if (par == "Else if")
                {
                    if (pre_cond == "Then" | pre_cond == "End if")
                    {

                    }
                    else
                    {
                        return index;
                    }
                    pre_cond = "Else if";
                }
                else if (par == "Else")
                {
                    if (pre_cond == "Then" | pre_cond == "End if")
                    {

                    }
                    else
                    {
                        return index;
                    }
                    pre_cond = "Else";
                }
                else if (par == "If")
                {
                    cond_close++;
                    pre_cond = "If";
                }

                if (cond_close < 0)
                {
                    return index;
                }
            }

            if (cond_close != 0)
            {
                return index;
            }

            return -1;
        }


        //Added by sun 16-10-2015
        //Update multiple Delete status 
        public ServiceResult UpdateMultipleDeleteDonationFormulaStatus(int[] pDonationsID, string pStatus, string pUpdateBy)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var current = db.Donation_Formula.Where(w => pDonationsID.Contains(w.Donation_Formula_ID));
                    if (current != null)
                    {
                        foreach (var d in current)
                        {
                            d.Update_On = currentdate;
                            d.Update_By = pUpdateBy;
                            d.Record_Status = pStatus;
                        }
                        db.SaveChanges();

                    }
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Donation_Formula };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Donation_Formula };
            }
        }

        public ServiceResult UpdateMultipleDeleteCpfFormulaStatus(int[] pCpfformulasID, string pStatus, string pUpdateBy)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var current = db.CPF_Formula.Where(w => pCpfformulasID.Contains(w.CPF_Formula_ID));
                    if (current != null)
                    {
                        foreach (var d in current)
                        {
                            d.Update_On = currentdate;
                            d.Update_By = pUpdateBy;
                            d.Record_Status = pStatus;
                        }
                        db.SaveChanges();

                    }
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Donation_Formula };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Donation_Formula };
            }
        }

        public ServiceResult UpdateMultipleDeletePRCStatus(int[] pPRCsID, string pStatus, string pUpdateBy)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var current = db.PRCs.Where(w => pPRCsID.Contains(w.PRC_ID));
                    if (current != null)
                    {
                        foreach (var prc in current)
                        {
                            prc.Update_On = currentdate;
                            prc.Update_By = pUpdateBy;
                            prc.Record_Status = pStatus;

                            var departments = db.PRC_Department.Where(w => w.PRC_ID == prc.PRC_ID);
                            foreach (var prcD in departments)
                            {
                                prcD.Update_On = currentdate;
                                prcD.Update_By = pUpdateBy;
                            }
                        }
                        db.SaveChanges();
                    }
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Allowance_Or_Deduction };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Allowance_Or_Deduction };
            }
        }

        public ServiceResult UpdateMultipleDeletePRGStatus(int[] pPRGsID, string pStatus, string pUpdateBy)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var current = db.PRGs.Where(w => pPRGsID.Contains(w.PRG_ID));
                    if (current != null)
                    {
                        foreach (var prg in current)
                        {
                            prg.Update_On = currentdate;
                            prg.Update_By = pUpdateBy;
                            prg.Record_Status = pStatus;

                            var currentPr = db.PRALs.Where(w => w.PRG_ID == prg.PRG_ID);
                            if (currentPr != null)
                            {
                                foreach (var pr in currentPr)
                                {
                                    pr.Update_On = currentdate;
                                    pr.Update_By = pUpdateBy;
                                }
                            }

                            var currentPrel = db.PRELs.Where(w => w.PRG_ID == prg.PRG_ID);
                            if (currentPrel != null)
                            {
                                foreach (var prel in currentPrel)
                                {
                                    prel.Update_On = currentdate;
                                    prel.Update_By = pUpdateBy;
                                }
                            }

                            var currentPredl = db.PREDLs.Where(w => w.PRG_ID == prg.PRG_ID);
                            if (currentPredl != null)
                            {
                                foreach (var predl in currentPredl)
                                {
                                    predl.Update_On = currentdate;
                                    predl.Update_By = pUpdateBy;
                                }
                            }
                        }
                        db.SaveChanges();
                    }
                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Authorization };
                }
            }
            catch
            {
                return new ServiceResult { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Authorization };
            }
        }

        //Update  Delete status 
        public ServiceResult UpdateDeleteDonationFormulaStatus(Nullable<int> pDoFormulaID, string pStatus, string pUpdateBy)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var current = db.Donation_Formula.Where(w => w.Donation_Formula_ID == pDoFormulaID).FirstOrDefault();
                    if (current != null)
                    {
                        current.Record_Status = pStatus;
                        current.Update_By = pUpdateBy;
                        current.Update_On = currentdate;
                        db.SaveChanges();
                    }
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Donation_Formula };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Donation_Formula };
            }
        }

        public ServiceResult UpdateDeleteCPFFormulaStatus(Nullable<int> pCpfFormulaID, string pStatus, string pUpdateBy)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var current = db.CPF_Formula.Where(w => w.CPF_Formula_ID == pCpfFormulaID).FirstOrDefault();
                    if (current != null)
                    {
                        current.Record_Status = pStatus;
                        current.Update_By = pUpdateBy;
                        current.Update_On = currentdate;
                        db.SaveChanges();
                    }
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.CPF_Formula };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.CPF_Formula };
            }
        }

        public ServiceResult UpdateDeletePRCStatus(Nullable<int> PRC_ID, string pStatus, string pUpdateBy)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var current = db.PRCs.Where(w => w.PRC_ID == PRC_ID).FirstOrDefault();
                    if (current != null)
                    {
                        current.Record_Status = pStatus;
                        current.Update_By = pUpdateBy;
                        current.Update_On = currentdate;
                        db.SaveChanges();
                    }
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Allowance_Or_Deduction };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Allowance_Or_Deduction };
            }
        }

        public ServiceResult UpdateDeletePRGStatus(Nullable<int> pPrgID, string pStatus, string pUpdateBy)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {

                    var current = db.PRGs.Where(w => w.PRG_ID == pPrgID).FirstOrDefault();
                    if (current != null)
                    {
                        current.Update_On = currentdate;
                        current.Update_By = pUpdateBy;
                        current.Record_Status = pStatus;

                        var currentPr = db.PRALs.Where(w => w.PRG_ID == current.PRG_ID);
                        if (currentPr != null)
                        {
                            foreach (var pr in currentPr)
                            {
                                pr.Update_On = currentdate;
                                pr.Update_By = pUpdateBy;
                            }
                        }

                        var currentPrel = db.PRELs.Where(w => w.PRG_ID == current.PRG_ID);
                        if (currentPrel != null)
                        {
                            foreach (var prel in currentPrel)
                            {
                                prel.Update_On = currentdate;
                                prel.Update_By = pUpdateBy;
                            }
                        }

                        var currentPredl = db.PREDLs.Where(w => w.PRG_ID == current.PRG_ID);
                        if (currentPredl != null)
                        {
                            foreach (var predl in currentPredl)
                            {
                                predl.Update_On = currentdate;
                                predl.Update_By = pUpdateBy;
                            }
                        }

                        db.SaveChanges();
                    }
                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Authorization };
                }
            }
            catch
            {
                return new ServiceResult { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Authorization };
            }
        }

        //----------------------------------No using---------------------------------------------//

        //Get Selected_CPF_Formula
        public List<Selected_CPF_Formula> GetSelectedCPFFormulas(Nullable<int> pCompanyID)
        {
            List<Selected_CPF_Formula> formula = new List<Selected_CPF_Formula>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    formula = db.Selected_CPF_Formula.Include(i => i.CPF_Formula).Where(w => w.Company_ID == pCompanyID).ToList();
                }
            }
            catch
            {
            }
            return formula;
        }

        //Delete Selected_CPF_Formula
        public ServiceResult DeleteSelectedCPFFormula(Selected_CPF_Formula formula)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(formula).State = EntityState.Deleted;
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Selected_CPF_Formula };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Selected_CPF_Formula };
            }
        }

        //Get OT_Formula
        public OT_Formula GetOTFormula(int OT_Formula_ID)
        {
            OT_Formula formula = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    formula = db.OT_Formula.Where(w => w.OT_Formula_ID == OT_Formula_ID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return formula;
        }

        //Get OT_Formula list
        public List<OT_Formula> GetOTFormulas()
        {
            List<OT_Formula> formula = new List<OT_Formula>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    formula = db.OT_Formula.ToList();
                }
            }
            catch
            {
            }
            return formula;
        }

        //Insert OT_Formula
        public int InsertOTFormula(OT_Formula formula)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(formula).State = EntityState.Added;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        //Update OT_Formula
        public int UpdateOTFormula(OT_Formula formula)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(formula).State = EntityState.Modified;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        //Delete OT_Formula
        public int DeleteOTFormula(OT_Formula formula)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(formula).State = EntityState.Deleted;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        //Get Selected_OT_Formula
        public Selected_OT_Formula GetSelectedOTFormula(int OT_Formula_ID, Nullable<int> pCompanyID)
        {
            Selected_OT_Formula formula = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    formula = db.Selected_OT_Formula.Include(i => i.OT_Formula)
                        .Where(w => w.OT_Formula_ID == OT_Formula_ID).Where(w => w.Company_ID == pCompanyID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return formula;
        }

        //Get Selected_OT_Formula
        public List<Selected_OT_Formula> GetSelectedOTFormulas(Nullable<int> pCompanyID)
        {
            List<Selected_OT_Formula> formula = new List<Selected_OT_Formula>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    formula = db.Selected_OT_Formula.Include(i => i.OT_Formula).Where(w => w.Company_ID == pCompanyID).ToList();
                }
            }
            catch
            {
            }
            return formula;
        }

        //Insert Selected_OT_Formula
        public int InsertSelectedOTFormula(Selected_OT_Formula formula)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(formula).State = EntityState.Added;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        //Update Selected_OT_Formula
        public int UpdateSelectedOTFormula(Selected_OT_Formula formula)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(formula).State = EntityState.Modified;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        //Delete Selected_OT_Formula
        public int DeleteSelectedOTFormula(Selected_OT_Formula formula)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(formula).State = EntityState.Deleted;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        //Get Selected_CPF_Formula
        public List<Selected_Donation_Formula> GetSelectedDonationFormulas(Nullable<int> pCompanyID)
        {
            List<Selected_Donation_Formula> formula = new List<Selected_Donation_Formula>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    formula = db.Selected_Donation_Formula.Include(i => i.Donation_Formula).Where(w => w.Company_ID == pCompanyID).ToList();
                }
            }
            catch
            {
            }
            return formula;
        }

        //Delete Selected_Donation_Formula
        public ServiceResult deleteSelectedDonationFormula(Selected_Donation_Formula formula)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(formula).State = EntityState.Deleted;
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Selected_Donation_Formula };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Selected_Donation_Formula };
            }
        }

        //Get PRT
        public PRT GetPRT(int ID)
        {
            PRT prt = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    prt = db.PRTs.Where(w => w.ID == ID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return prt;
        }

        //Get PRTs
        public List<PRT> GetPRTs()
        {
            List<PRT> prt = new List<PRT>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    prt = db.PRTs.ToList();
                }
            }
            catch
            {
            }
            return prt;
        }

        //Insert PRT
        public int InsertPRT(PRT prt)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(prt).State = EntityState.Added;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        //Update PRT
        public int UpdatePRT(PRT prt)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(prt).State = EntityState.Modified;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        //Delete PRT
        public int DeletePRT(PRT prt)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(prt).State = EntityState.Deleted;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public List<User_Profile> GetEmployeeList(Nullable<int> pCompanyID)
        {
            List<User_Profile> employee = new List<User_Profile>();

            try
            {
                using (var db = new SBS2DBContext())
                {
                    employee = db.User_Profile
                        .Include(i => i.Employee_Profile)
                        .Include(i => i.User_Authentication).Include(i => i.User_Authentication.User_Assign_Role)
                        .Where(w => w.Company_ID == pCompanyID && w.User_Status == "Active")
                        .OrderBy(o => o.Name).ToList();
                }
            }
            catch
            {

            }

            return employee;

        }

        //Added by sun 09-02-2015
        public Notification_Scheduler GetNotificationScheduler(Nullable<int> pCompanyID)
        {
            Notification_Scheduler notification = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    notification = db.Notification_Scheduler.Where(w => w.Company_ID == pCompanyID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return notification;
        }

        public ServiceResult InsertNotificationScheduler(Notification_Scheduler pNotificationScheduler)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Notification_Scheduler.Add(pNotificationScheduler);
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Notification_Scheduler };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Notification_Scheduler };
            }
        }

        public ServiceResult UpdateNotificationScheduler(Notification_Scheduler pNotificationScheduler)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var current = (from a in db.Notification_Scheduler where a.Notification_Scheduler_ID == pNotificationScheduler.Notification_Scheduler_ID select a).FirstOrDefault();
                    if (current != null)
                    {
                        db.Entry(current).CurrentValues.SetValues(pNotificationScheduler);
                        db.SaveChanges();
                    }
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Notification_Scheduler };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Notification_Scheduler };
            }
        }


    }


}
