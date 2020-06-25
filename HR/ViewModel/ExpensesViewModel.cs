using HR.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Configuration;
using SBSModel.Models;
using SBSModel.Common;
using SBSResourceAPI;


namespace HR.Models
{
   public class ExpensesConfigurationViewModel : ModelBase
   {
      public string tabAction { get; set; }

      public List<ComboViewModel> departmentList { get; set; }

      [LocalizedDisplayName("Department", typeof(Resource))]
      public Nullable<int> search_Expenses_Type_Department { get; set; }

      [LocalizedDisplayName("Expenses_Type", typeof(Resource))]
      public string search_Expenses_Type { get; set; } // global lookup

      //Added By sun 25-08-2015
      //******** Workflow ***********
      public List<Expenses_Category> ExpensesCategoryList { get; set; }

      //******** expenses type ***********
      public List<Expenses_Config> ExpensesTypeList { get; set; }

      //******** approval type ***********
      public List<SBSWorkFlowAPI.Models.Approval_Flow> ApprovalList { get; set; }

      [LocalizedDisplayName("Department", typeof(Resource))]
      public Nullable<int> search_Approval_Department { get; set; }
   }

   //public class ExpenseService
   //{
   //    public List<Expenses_Config> getExpenseTypes(Nullable<int> Company_ID, Nullable<int> sDepartment = null, Nullable<int> sExpenseType = null)
   //    {
   //        List<Expenses_Config> expense = null;

   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                expense = db.Expenses_Config
   //                    .Include(i => i.Department)
   //                    .Include(i => i.Expenses_Config_Detail)
   //                    .Include(i => i.Global_Lookup_Data)
   //                    .Include(i => i.Global_Lookup_Data1)
   //                    .Where(w => w.Company_ID == Company_ID).ToList();

   //                if (expense != null)
   //                {
   //                    if (sDepartment.HasValue && sDepartment.Value > 0)
   //                    {
   //                        expense = expense.Where(w => w.Department_ID == sDepartment).ToList();
   //                    }

   //                    if (sExpenseType.HasValue && sExpenseType.Value > 0)
   //                    {
   //                        expense = expense.Where(w => w.Expenses_Type == sExpenseType).ToList();
   //                    }
   //                }
   //            }
   //        }
   //        catch
   //        {
   //        }

   //        return expense;
   //    }

   //    public Expenses_Config getExpenseType(Nullable<int> Expenses_Config_ID)
   //    {
   //        Expenses_Config expense = null;

   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                expense = db.Expenses_Config
   //                    .Include(i => i.Department)
   //                    .Include(i => i.Global_Lookup_Data) // Claim Type
   //                     .Include(i => i.Global_Lookup_Data1) // Expenses Type
   //                    .Include(i => i.Expenses_Config_Detail)
   //                    .Where(w => w.Expenses_Config_ID == Expenses_Config_ID).FirstOrDefault();
   //            }
   //        }
   //        catch
   //        {
   //        }

   //        return expense;
   //    }

   //    public ServiceResult InsertExpenseType(Expenses_Config expense, ExpensesTypeDetailViewModel[] details)
   //    {

   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                if (details != null)
   //                {
   //                    var i = 0;
   //                    var gcnt = 1;
   //                    foreach (var row in details)
   //                    {
   //                        if (row.Row_Type == RowType.ADD)
   //                        {
   //                            foreach (var ldrow in row.Designations)
   //                            {
   //                                Nullable<int> designationID = ldrow;
   //                                if (ldrow == 0)
   //                                    designationID = null;

   //                                var eConfigDetail = new Expenses_Config_Detail()
   //                                {
   //                                    Amount_Per_Year = row.Amount_Per_Year,
   //                                    Designation_ID = designationID,
   //                                    Group_ID = gcnt,
   //                                    Year_Service = row.Year_Service
   //                                };

   //                                if (row.Select_Amount)
   //                                {
   //                                    eConfigDetail.Select_Amount = true;
   //                                    eConfigDetail.Select_Pecentage = false;
   //                                    eConfigDetail.Amount = row.Amount;
   //                                }
   //                                else
   //                                {
   //                                    eConfigDetail.Select_Amount = false;
   //                                    eConfigDetail.Select_Pecentage = true;
   //                                    eConfigDetail.Pecentage = row.Pecentage;
   //                                }

   //                                expense.Expenses_Config_Detail.Add(eConfigDetail);


   //                            }


   //                        }
   //                        i++;
   //                        gcnt++;
   //                    }
   //                }

   //                db.Entry(expense).State = EntityState.Added;
   //                db.SaveChanges();
   //                return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resources.ResourceExpenses.ExpensesType };
   //            }
   //        }
   //        catch
   //        {
   //            return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resources.ResourceExpenses.ExpensesType };
   //        }
   //    }

   //    public ServiceResult UpdateExpenseType(Expenses_Config expense, ExpensesTypeDetailViewModel[] details)
   //    {

   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                var current = db.Expenses_Config.Where(w => w.Expenses_Config_ID == expense.Expenses_Config_ID).FirstOrDefault();
   //                if (current != null)
   //                {
   //                    if (details != null)
   //                    {
   //                        var i = 0;
   //                        var gcnt = 1;
   //                        foreach (var row in details)
   //                        {
   //                            row.Expenses_Config_ID = expense.Expenses_Config_ID;
   //                            if (row.Row_Type == RowType.ADD)
   //                            {

   //                                foreach (var ldrow in row.Designations)
   //                                {
   //                                    Nullable<int> designationID = ldrow;
   //                                    if (ldrow == 0)
   //                                        designationID = null;

   //                                    var eConfigDetail = new Expenses_Config_Detail()
   //                                    {
   //                                        Amount_Per_Year = row.Amount_Per_Year,
   //                                        Designation_ID = designationID,
   //                                        Group_ID = gcnt,
   //                                        Expenses_Config_ID = row.Expenses_Config_ID,
   //                                        Year_Service = row.Year_Service
   //                                    };

   //                                    if (row.Select_Amount)
   //                                    {
   //                                        eConfigDetail.Select_Amount = true;
   //                                        eConfigDetail.Select_Pecentage = false;
   //                                        eConfigDetail.Amount = row.Amount;
   //                                    }
   //                                    else
   //                                    {
   //                                        eConfigDetail.Select_Amount = false;
   //                                        eConfigDetail.Select_Pecentage = true;
   //                                        eConfigDetail.Pecentage = row.Pecentage;
   //                                    }

   //                                    db.Expenses_Config_Detail.Add(eConfigDetail);
   //                                }
   //                                gcnt++;
   //                            }
   //                            else if (row.Row_Type == RowType.EDIT)
   //                            {
   //                                if (row.Designations != null)
   //                                {
   //                                    foreach (var ldrow in row.Designations)
   //                                    {

   //                                        Nullable<int> designationID = ldrow;
   //                                        if (ldrow == 0)
   //                                        {
   //                                            designationID = null;
   //                                        }

   //                                        var currentdetail = (from a in db.Expenses_Config_Detail
   //                                                             where a.Group_ID == row.Group_ID
   //                                                             && a.Expenses_Config_ID == row.Expenses_Config_ID
   //                                                             && a.Designation_ID == designationID
   //                                                             select a).FirstOrDefault();

   //                                        if (currentdetail == null)
   //                                        {

   //                                            var eConfigDetail = new Expenses_Config_Detail()
   //                                            {
   //                                                Amount_Per_Year = row.Amount_Per_Year,
   //                                                Designation_ID = designationID,
   //                                                Group_ID = gcnt,
   //                                                Expenses_Config_ID = row.Expenses_Config_ID,
   //                                                Year_Service = row.Year_Service
   //                                            };

   //                                            if (row.Select_Amount)
   //                                            {
   //                                                eConfigDetail.Select_Amount = true;
   //                                                eConfigDetail.Select_Pecentage = false;
   //                                                eConfigDetail.Amount = row.Amount;
   //                                            }
   //                                            else
   //                                            {
   //                                                eConfigDetail.Select_Amount = false;
   //                                                eConfigDetail.Select_Pecentage = true;
   //                                                eConfigDetail.Pecentage = row.Pecentage;
   //                                            }

   //                                            db.Expenses_Config_Detail.Add(eConfigDetail);

   //                                        }
   //                                        else
   //                                        {
   //                                            currentdetail.Amount_Per_Year = row.Amount_Per_Year;
   //                                            currentdetail.Year_Service = row.Year_Service;
   //                                            currentdetail.Designation_ID = designationID;
   //                                            currentdetail.Group_ID = gcnt;
   //                                            if (row.Select_Amount)
   //                                            {
   //                                                currentdetail.Select_Amount = true;
   //                                                currentdetail.Select_Pecentage = false;
   //                                            }
   //                                            else
   //                                            {
   //                                                currentdetail.Select_Amount = false;
   //                                                currentdetail.Select_Pecentage = true;
   //                                            }

   //                                        }
   //                                    }

   //                                    var notUseRows = (from a in db.Expenses_Config_Detail
   //                                                      where a.Group_ID == row.Group_ID && a.Expenses_Config_ID == row.Expenses_Config_ID
   //                                                      && !row.Designations.Contains(a.Designation_ID.HasValue ? a.Designation_ID.Value : 0)
   //                                                      select a);
   //                                    db.Expenses_Config_Detail.RemoveRange(notUseRows);



   //                                }
   //                                gcnt++;

   //                            }
   //                            else if (row.Row_Type == RowType.DELETE)
   //                            {
   //                                if (row.Designations != null)
   //                                {
   //                                    foreach (var ldrow in row.Designations)
   //                                    {
   //                                        Nullable<int> designationID = ldrow;
   //                                        if (ldrow == 0)
   //                                            designationID = null;

   //                                        var currentdetail = (from a in db.Expenses_Config_Detail
   //                                                             where a.Group_ID == row.Group_ID
   //                                                             && a.Expenses_Config_ID == row.Expenses_Config_ID
   //                                                             && a.Designation_ID == designationID
   //                                                             select a).FirstOrDefault();
   //                                        if (currentdetail != null)
   //                                        {
   //                                            db.Expenses_Config_Detail.Remove(currentdetail);
   //                                        }
   //                                    }
   //                                }


   //                            }
   //                            i++;
   //                        }
   //                    }

   //                    db.Entry(current).CurrentValues.SetValues(expense);
   //                    db.SaveChanges();
   //                }

   //                return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resources.ResourceExpenses.ExpensesType };
   //            }
   //        }
   //        catch
   //        {
   //            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resources.ResourceExpenses.ExpensesType };
   //        }
   //    }

   //    public ServiceResult DeleteExpenseType(Nullable<int> eid)
   //    {

   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                var expense = db.Expenses_Config.Where(w => w.Expenses_Config_ID == eid).FirstOrDefault();
   //                if (expense != null)
   //                {
   //                    var eDetail = db.Expenses_Config_Detail.Where(w => w.Expenses_Config_ID == eid);
   //                    db.Expenses_Config_Detail.RemoveRange(eDetail);

   //                    db.Expenses_Config.Remove(expense);
   //                    db.SaveChanges();
   //                }

   //                return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resources.ResourceExpenses.ExpensesType };
   //            }
   //        }
   //        catch
   //        {
   //            return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resources.ResourceExpenses.ExpensesType };
   //        }
   //    }


   //    public List<Expenses_Approval> getApprovalWorkflows(int Company_ID)
   //    {
   //        List<Expenses_Approval> expense = new List<Expenses_Approval>();
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                List<Department> departments = db.Departments.Where(w => w.Company_ID == Company_ID).ToList();
   //                if (departments != null)
   //                {
   //                    foreach (Department d in departments)
   //                    {
   //                        expense.AddRange(db.Expenses_Approval
   //                            .Include(i => i.Department)
   //                            .Include(i => i.Employee_Profile)
   //                            .Include(i => i.Employee_Profile1)
   //                            .Include(i => i.Employee_Profile.User_Profile)
   //                            .Include(i => i.Employee_Profile1.User_Profile)
   //                            .Where(w => w.Department_ID == d.Department_ID).ToList());
   //                    }
   //                }
   //            }
   //        }
   //        catch
   //        {
   //        }
   //        return expense;
   //    }

   //    public Expenses_Approval getApprovalWorkflow(int Approval_ID)
   //    {
   //        Expenses_Approval expense = null;
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                expense = db.Expenses_Approval
   //                            .Include(i => i.Department)
   //                            .Include(i => i.Employee_Profile)
   //                            .Include(i => i.Employee_Profile1)
   //                            .Include(i => i.Employee_Profile.User_Profile)
   //                            .Include(i => i.Employee_Profile1.User_Profile)
   //                            .Where(w => w.Approval_ID == Approval_ID).FirstOrDefault();
   //            }
   //        }
   //        catch
   //        {
   //        }
   //        return expense;
   //    }

   //    public int insertApprovalWorkflow(Expenses_Approval expense)
   //    {

   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                db.Entry(expense).State = EntityState.Added;
   //                db.SaveChanges();
   //                return ERROR_CODE.SUCCESS;
   //            }
   //        }
   //        catch
   //        {
   //        }
   //        return ERROR_CODE.ERROR_500_DB;
   //    }

   //    public int updateApprovalWorkflow(Expenses_Approval expense)
   //    {

   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                db.Entry(expense).State = EntityState.Modified;
   //                db.SaveChanges();
   //                return ERROR_CODE.SUCCESS;
   //            }
   //        }
   //        catch
   //        {
   //        }
   //        return ERROR_CODE.ERROR_500_DB;
   //    }

   //    public int deleteApprovalWorkflow(Expenses_Approval expense)
   //    {

   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                db.Entry(expense).State = EntityState.Deleted;
   //                db.SaveChanges();
   //                return ERROR_CODE.SUCCESS;
   //            }
   //        }
   //        catch
   //        {
   //        }
   //        return ERROR_CODE.ERROR_500_DB;
   //    }

   //    public bool isDuplicateDepartment(int Department_ID, decimal Range_Amount, int Approval_ID = 0)
   //    {
   //        bool isDuplicateDepartment = true;
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                List<Expenses_Approval> expense = new List<Expenses_Approval>();

   //                expense.AddRange(db.Expenses_Approval
   //                    .Where(w => w.Department_ID == Department_ID)
   //                    .Where(w => w.Range_Amount == Range_Amount).ToList());

   //                if (Approval_ID > 0)
   //                {
   //                    expense = expense.Where(w => w.Approval_ID != Approval_ID).ToList();
   //                }

   //                if (expense.Count == 0)
   //                {
   //                    isDuplicateDepartment = false;
   //                }
   //            }
   //        }
   //        catch
   //        {
   //        }
   //        return isDuplicateDepartment;
   //    }


   //    public List<Currency_Conversion> getCurencySetups(int Company_ID)
   //    {
   //        List<Currency_Conversion> currency = new List<Currency_Conversion>();
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                currency = db.Currency_Conversion.Include(i => i.Conversion_Item).Where(w => w.Company_ID == Company_ID).ToList();
   //            }
   //        }
   //        catch
   //        {
   //        }
   //        return currency;
   //    }

   //    public Currency_Conversion getCurencySetup(int Conversion_ID)
   //    {
   //        Currency_Conversion currency = new Currency_Conversion();
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                currency = db.Currency_Conversion.Include(i => i.Conversion_Item)
   //                    .Where(w => w.Conversion_ID == Conversion_ID).FirstOrDefault();
   //            }
   //        }
   //        catch
   //        {
   //        }
   //        return currency;
   //    }

   //    public int insertCurencySetup(Currency_Conversion currency)
   //    {
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                db.Entry(currency).State = EntityState.Added;
   //                db.SaveChanges();
   //                return ERROR_CODE.SUCCESS;
   //            }
   //        }
   //        catch
   //        {
   //        }
   //        return ERROR_CODE.ERROR_500_DB;
   //    }

   //    public int updateCurencySetup(Conversion_Item convert)
   //    {
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                db.Entry(convert).State = EntityState.Modified;
   //                db.SaveChanges();
   //                return ERROR_CODE.SUCCESS;
   //            }
   //        }
   //        catch
   //        {
   //        }
   //        return ERROR_CODE.ERROR_500_DB;
   //    }

   //    public int deleteCurencySetup(Currency_Conversion currency)
   //    {
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                //db.Conversion_Item.RemoveRange(currency.Conversion_Item);
   //                db.Entry(currency).State = EntityState.Deleted;
   //                db.SaveChanges();
   //                return ERROR_CODE.SUCCESS;
   //            }
   //        }
   //        catch
   //        {
   //        }
   //        return ERROR_CODE.ERROR_500_DB;
   //    }

   //    public bool isDuplicateCurencySetup(int Company_ID, int Currency_ID, int Conversion_ID)
   //    {
   //        bool isDuplicateCurencySetup = true;
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                List<Currency_Conversion> currencies = getCurencySetups(Company_ID);
   //                List<Conversion_Item> convert = new List<Conversion_Item>();

   //                foreach (Currency_Conversion currency in currencies)
   //                {
   //                    if (currency.Conversion_Item.FirstOrDefault().Currency_ID.Value == Currency_ID)
   //                    {
   //                        convert.Add(currency.Conversion_Item.FirstOrDefault());
   //                    }
   //                }

   //                if (Conversion_ID > 0)
   //                {
   //                    convert = convert.Where(w => w.Conversion_ID != Conversion_ID).ToList();
   //                }

   //                if (convert.Count == 0)
   //                {
   //                    isDuplicateCurencySetup = false;
   //                }
   //            }
   //        }
   //        catch
   //        {
   //        }
   //        return isDuplicateCurencySetup;
   //    }

   //    //For mgmt
   //    public List<Expenses_Application_Document> getExpenseApprovalApplications(int Company_ID, int Employee_Profile_ID)
   //    {
   //        List<Expenses_Application_Document> expense = new List<Expenses_Application_Document>();
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                //Get Expense Approval
   //                List<Expenses_Approval> expenseApprovals = getApprovalWorkflows(Company_ID);

   //                //Filter mgmt user who in Expenses_Approval
   //                expenseApprovals = expenseApprovals.Where(w => w.First_Approval == Employee_Profile_ID || w.Second_Approval == Employee_Profile_ID).ToList();

   //                /*
   //                int[] departments = expenseApprovals.Select(s => s.Department_ID.Value).ToArray();

   //                //Get Users in company Company_ID
   //                List<User_Profile> users = new UserService().getUsers(Company_ID);

   //                foreach (User_Profile user in users)
   //                {
   //                    if (user.Employee_Profile != null && user.Employee_Profile.FirstOrDefault() != null)
   //                    {
   //                        //Is user in dep.
   //                        int empid = user.Employee_Profile.FirstOrDefault().Employee_Profile_ID;
   //                        var userhist = getEmploymentHistory(empid);

   //                        if (userhist != null && departments.Contains(userhist.Department_ID.Value))
   //                        {
   //                            List<Expenses_Application_Document> expenseTemp = new List<Expenses_Application_Document>();

   //                            //Get Application of user
   //                            expenseTemp.AddRange(db.Expenses_Application_Document.Include(i => i.Employee_Profile)
   //                                .Include(i => i.Employee_Profile.User_Profile)
   //                                .Include(i => i.Expenses_Config)
   //                                .Where(w => w.Employee_Profile_ID == empid).ToList());

   //                            //Filter Approval Right on Amount 
   //                            foreach (var exp in expenseTemp)
   //                            {
   //                                foreach (var approval in expenseApprovals)
   //                                {
   //                                    if (exp.Amount_Claiming.Value <= approval.Range_Amount.Value)
   //                                    {
   //                                        expense.Add(exp);
   //                                    }
   //                                }
   //                            }

   //                        }
   //                    }
   //                }
   //                */

   //                foreach (Expenses_Approval aprroval in expenseApprovals)
   //                {

   //                    List<Expenses_Application_Document> expenseTemp = new List<Expenses_Application_Document>();
   //                    expenseTemp = getDepartmentExpenseApplications(aprroval.Department_ID.Value);

   //                    //Filter Approval Right on Amount 
   //                    foreach (var exp in expenseTemp)
   //                    {
   //                        foreach (var approval in expenseApprovals)
   //                        {
   //                            if (exp.Amount_Claiming.Value <= approval.Range_Amount.Value)
   //                            {
   //                                expense.Add(exp);
   //                            }
   //                        }
   //                    }
   //                }
   //            }
   //        }
   //        catch
   //        {
   //        }

   //        expense = expense.OrderBy(o => o.Create_On).Distinct().ToList();

   //        return expense;
   //    }


   //    public List<Expenses_Application_Document> getDepartmentExpenseApplications(Nullable<int> Department_ID)
   //    {
   //        List<Expenses_Application_Document> expense = new List<Expenses_Application_Document>();
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                expense = db.Expenses_Application_Document
   //                    .Include(i => i.Employee_Profile)
   //                    .Include(i => i.Employee_Profile.User_Profile)
   //                    .Include(i => i.Expenses_Config)
   //                     .Include(i => i.Expenses_Application)
   //                    .Where(w => w.Department_ID == Department_ID)
   //                    .ToList()
   //                    ;
   //            }
   //        }
   //        catch
   //        {
   //        }
   //        return expense;
   //    }

   //    public Employment_History getEmploymentHistory(int Employee_Profile_ID, DateTime? date = null)
   //    {
   //        var currentdate = StoredProcedure.GetCurrentDate();
   //        Employment_History userhist = new Employment_History();
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                if (date == null)
   //                {
   //                    userhist = (from a in db.Employment_History
   //                                where a.Effective_Date <= currentdate
   //                                    & a.Employee_Profile_ID == Employee_Profile_ID
   //                                orderby a.Effective_Date descending
   //                                select a).FirstOrDefault();
   //                }
   //                else
   //                {
   //                    userhist = (from a in db.Employment_History
   //                                where a.Effective_Date <= date & a.Employee_Profile_ID == Employee_Profile_ID
   //                                orderby a.Effective_Date descending
   //                                select a).FirstOrDefault();
   //                }
   //            }
   //        }
   //        catch
   //        {
   //        }
   //        return userhist;
   //    }

   //    public Expenses_Application getExpenseApplication(Nullable<int> Expenses_Application_ID)
   //    {
   //        var expense = new Expenses_Application();
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                expense = db.Expenses_Application
   //                    .Include(i => i.Employee_Profile)
   //                    .Include(i => i.Employee_Profile.User_Profile)
   //                    .Include(i => i.Expenses_Application_Document)
   //                    .Include(i => i.Expenses_Application_Document.Select(s => s.Expenses_Config))
   //                    .Include(i => i.Expenses_Application_Document.Select(s => s.Expenses_Config.Global_Lookup_Data1)) // Expenses Type
   //                    .Where(w => w.Expenses_Application_ID == Expenses_Application_ID).FirstOrDefault();
   //            }
   //        }
   //        catch
   //        {
   //        }
   //        return expense;
   //    }

   //    public List<Expenses_Application> getExpenseApplications(Nullable<int> pCompanyID, Nullable<int> pProfileID = null, string pDateApplied = "", string pStatus = "")
   //    {
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                var expenses = db.Expenses_Application
   //                        .Include(i => i.Employee_Profile)
   //                        .Include(i => i.Employee_Profile.User_Profile)
   //                        .Include(i => i.Expenses_Application_Document)
   //                        .Where(w => w.Employee_Profile.User_Profile.Company_ID == pCompanyID);

   //                if (!string.IsNullOrEmpty(pDateApplied))
   //                {
   //                    var d = DateUtil.ToDate(pDateApplied);
   //                    if (d != null)
   //                    {
   //                        expenses = expenses.Where(w => w.Date_Applied.Value.Day == d.Value.Day & w.Date_Applied.Value.Month == d.Value.Month & w.Date_Applied.Value.Year == d.Value.Year);
   //                    }

   //                }
   //                if (pProfileID.HasValue)
   //                {
   //                    expenses = expenses.Where(w => w.Employee_Profile.Profile_ID == pProfileID);
   //                }
   //                if (!string.IsNullOrEmpty(pStatus))
   //                {
   //                    expenses = expenses.Where(w => w.Overall_Status == pStatus);
   //                }
   //                return expenses.ToList();
   //            }
   //        }
   //        catch
   //        {
   //            return new List<Expenses_Application>();
   //        }
   //    }

   //    public List<Expenses_Application_Document> getExpenseApplicationDocs(Nullable<int> pProfileID, string pDateApplied = "", string pStatus = "")
   //    {
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                var expenses = db.Expenses_Application_Document
   //                        .Include(i => i.Employee_Profile)
   //                        .Include(i => i.Employee_Profile.User_Profile)
   //                        .Include(i => i.Currency)
   //                        .Include(i => i.Expenses_Config)
   //                        .Include(i => i.Expenses_Config.Global_Lookup_Data1)
   //                        .Include(i => i.Expenses_Application)
   //                        .Where(w => w.Employee_Profile.Profile_ID == pProfileID);

   //                if (!string.IsNullOrEmpty(pDateApplied))
   //                {
   //                    var d = DateUtil.ToDate(pDateApplied);
   //                    if (d != null)
   //                    {
   //                        expenses = expenses.Where(w => w.Expenses_Application.Date_Applied.Value.Day == d.Value.Day & w.Expenses_Application.Date_Applied.Value.Month == d.Value.Month & w.Expenses_Application.Date_Applied.Value.Year == d.Value.Year);
   //                    }

   //                }
   //                if (!string.IsNullOrEmpty(pStatus))
   //                {
   //                    expenses = expenses.Where(w => w.Expenses_Application.Overall_Status == pStatus);
   //                }
   //                return expenses.ToList();
   //            }
   //        }
   //        catch
   //        {
   //            return new List<Expenses_Application_Document>();
   //        }
   //    }

   //    public List<Expenses_Application_Document> getExpenseApplicationsInCompany(int Company_ID)
   //    {
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                return db.Expenses_Application_Document
   //                        .Include(i => i.Employee_Profile)
   //                        .Include(i => i.Employee_Profile.User_Profile)
   //                        .Include(i => i.Currency)
   //                        .Include(i => i.Expenses_Config)
   //                        .Include(i => i.Expenses_Config.Global_Lookup_Data1)
   //                        .Include(i => i.Department)
   //                        .Where(w => w.Department.Company_ID == Company_ID).ToList();
   //            }
   //        }
   //        catch
   //        {
   //            return new List<Expenses_Application_Document>();
   //        }
   //    }

   //    public ServiceResult insertExpenseApplication(Expenses_Application expense, ExpensesDetailViewModel[] details)
   //    {
   //        try
   //        {
   //            var currentdate = StoredProcedure.GetCurrentDate();
   //            using (var db = new SBS2DBContext())
   //            {
   //                var emp = db.Employee_Profile.Where(w => w.Employee_Profile_ID == expense.Employee_Profile_ID).FirstOrDefault();
   //                if (emp == null)
   //                    return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = Resources.ResourceExpenses.Employee };

   //                var userhist = getEmploymentHistory(emp.Employee_Profile_ID, currentdate);
   //                if (userhist == null)
   //                    return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = Resources.ResourceEmployee.EmploymentHistory };

   //                if (details != null)
   //                {
   //                    foreach (var row in details)
   //                    {
   //                        if (row.Row_Type == RowType.ADD)
   //                        {
   //                            var expensesDetail = new Expenses_Application_Document()
   //                            {
   //                                Amount_Claiming = row.Amount_Claiming,
   //                                Employee_Profile_ID = expense.Employee_Profile_ID,
   //                                Expenses_Config_ID = row.Expenses_Config_ID,
   //                                Expenses_Date = DateUtil.ToDate(row.Expenses_Date),
   //                                Reasons = row.Notes,
   //                                Selected_Currency = row.Selected_Currency,
   //                                Tax = row.Tax,
   //                                Total_Amount = row.Total_Amount,
   //                                Department_ID = userhist.Department_ID,
   //                                Date_Applied = expense.Date_Applied
   //                            };

   //                            expense.Expenses_Application_Document.Add(expensesDetail);
   //                        }
   //                    }
   //                }



   //                var eNo = "";
   //                var pattern = db.Expenses_No_Pattern.Where(w => w.Company_ID == emp.User_Profile.Company_ID).FirstOrDefault();
   //                if(pattern == null)
   //                {
   //                    var newpattern = new Expenses_No_Pattern()
   //                    {
   //                        Company_ID = emp.User_Profile.Company_ID,
   //                        Current_Running_Number = 1
   //                    };
   //                    db.Expenses_No_Pattern.Add(newpattern);

   //                    eNo = "EX-" + currentdate.Year + "-" + 1.ToString("0000");
   //                }
   //                else
   //                {
   //                    pattern.Current_Running_Number = pattern.Current_Running_Number + 1;
   //                    eNo = "EX" + currentdate.Year + pattern.Current_Running_Number.Value.ToString("0000");
   //                }
   //                expense.Expenses_No = eNo;

   //                db.Entry(expense).State = EntityState.Added;
   //                db.SaveChanges();
   //                db.Entry(expense).GetDatabaseValues();
   //                return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resources.ResourceExpenses.Expenses };
   //            }
   //        }
   //        catch
   //        {
   //        }
   //        return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resources.ResourceExpenses.Expenses };
   //    }

   //    public ServiceResult updateExpenseApplication(Expenses_Application expense)
   //    {
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                db.Entry(expense).State = EntityState.Modified;
   //                db.SaveChanges();
   //                return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resources.ResourceLeave.Leave };
   //            }
   //        }
   //        catch
   //        {
   //            return new ServiceResult() { Code = ERROR_CODE.ERROR_506_SAVE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_506_SAVE_ERROR), Field = Resources.ResourceLeave.Leave };
   //        }
   //    }

   //    public ServiceResult updateExpenseApplication(Expenses_Application expense, ExpensesDetailViewModel[] details)
   //    {
   //        try
   //        {
   //            var currentdate = StoredProcedure.GetCurrentDate();
   //            using (var db = new SBS2DBContext())
   //            {
   //                var emp = db.Employee_Profile.Where(w => w.Employee_Profile_ID == expense.Employee_Profile_ID).FirstOrDefault();
   //                if (emp == null)
   //                    return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = Resources.ResourceExpenses.Employee };

   //                var userhist = getEmploymentHistory(emp.Employee_Profile_ID, currentdate);
   //                if (userhist == null)
   //                    return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = Resources.ResourceEmployee.EmploymentHistory };


   //                if (details != null)
   //                {
   //                    foreach (var row in details)
   //                    {
   //                        if (row.Row_Type == RowType.ADD)
   //                        {
   //                            var expensesDetail = new Expenses_Application_Document()
   //                            {
   //                                Amount_Claiming = row.Amount_Claiming,
   //                                Employee_Profile_ID = expense.Employee_Profile_ID,
   //                                Expenses_Config_ID = row.Expenses_Config_ID,
   //                                Expenses_Date = DateUtil.ToDate(row.Expenses_Date),
   //                                Reasons = row.Notes,
   //                                Selected_Currency = row.Selected_Currency,
   //                                Tax = row.Tax,
   //                                Total_Amount = row.Total_Amount,
   //                                Department_ID = row.Department_ID,
   //                                Date_Applied = expense.Date_Applied
   //                            };

   //                            db.Expenses_Application_Document.Add(expensesDetail);

   //                        }
   //                        else if (row.Row_Type == RowType.EDIT)
   //                        {
   //                            var currentdetail = (from a in db.Expenses_Application_Document
   //                                                 where a.Expenses_Application_Document_ID == row.Expenses_Application_Document_ID
   //                                                 select a).FirstOrDefault();
   //                            if (currentdetail != null)
   //                            {
   //                                currentdetail.Amount_Claiming = row.Amount_Claiming;
   //                                currentdetail.Expenses_Config_ID = row.Expenses_Config_ID;
   //                                currentdetail.Expenses_Date = DateUtil.ToDate(row.Expenses_Date);
   //                                currentdetail.Reasons = row.Notes;
   //                                currentdetail.Selected_Currency = row.Selected_Currency;
   //                                currentdetail.Tax = row.Tax;
   //                                currentdetail.Total_Amount = row.Total_Amount;
   //                                currentdetail.Date_Applied = expense.Date_Applied;
   //                                currentdetail.Department_ID = userhist.Department_ID;
   //                            }
   //                        }
   //                        else if (row.Row_Type == RowType.DELETE)
   //                        {
   //                            var currentdetail = (from a in db.Expenses_Application_Document
   //                                                 where a.Expenses_Application_Document_ID == row.Expenses_Application_Document_ID
   //                                                 select a).FirstOrDefault();
   //                            if (currentdetail != null)
   //                            {
   //                                db.Expenses_Application_Document.Remove(currentdetail);
   //                            }


   //                        }
   //                    }
   //                }

   //                db.Entry(expense).State = EntityState.Modified;
   //                db.SaveChanges();
   //                return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resources.ResourceExpenses.Expenses };
   //            }
   //        }
   //        catch
   //        {
   //        }
   //        return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resources.ResourceExpenses.Expenses };
   //    }

   //    public ServiceResult deleteExpenseApplication(Nullable<int> pExpensesDocID)
   //    {
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                var doc = db.Expenses_Application.Where(w => w.Expenses_Application_ID == pExpensesDocID).FirstOrDefault();
   //                if (doc != null)
   //                {
   //                    db.Expenses_Application_Document.RemoveRange(doc.Expenses_Application_Document);
   //                    db.Expenses_Application.Remove(doc);
   //                    db.SaveChanges();
   //                }
   //                return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resources.ResourceExpenses.Expenses };
   //            }
   //        }
   //        catch
   //        {
   //        }
   //        return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resources.ResourceExpenses.Expenses };
   //    }

   //    public ServiceResult UpdateCancelStatus(Nullable<int> pExpensesID, string pStatus, string pRemark,string domain)
   //    {
   //        try
   //        {
   //            var currentdate = StoredProcedure.GetCurrentDate();
   //            using (var db = new SBS2DBContext())
   //            {
   //                if (pExpensesID.HasValue)
   //                {
   //                    var current = (from a in db.Expenses_Application
   //                                   where a.Expenses_Application_ID == pExpensesID.Value
   //                                   select a).FirstOrDefault();
   //                    if (current != null)
   //                    {

   //                        var userhist = (from a in db.Employment_History
   //                                        where a.Effective_Date <= currentdate
   //                                        & a.Employee_Profile_ID == current.Employee_Profile_ID
   //                                        orderby a.Effective_Date descending
   //                                        select a).FirstOrDefault();

   //                        User_Profile firstApproverProfile = null;
   //                        var approval = (from a in db.Expenses_Approval where a.Department_ID == userhist.Department_ID select a).FirstOrDefault();
   //                        if (approval != null)
   //                        {
   //                            firstApproverProfile = approval.Employee_Profile.User_Profile;
   //                        }
   //                        var emplyeeProfile = current.Employee_Profile.User_Profile;

   //                        current.Approval_Cancel_Status = pStatus;
   //                        if (pStatus == LeaveStatus.ApprovedCancel)
   //                        {
   //                            // send approve cancel email to employee.
   //                            current.Remark = pRemark;
   //                            db.SaveChanges();

   //                            if (approval != null && firstApproverProfile != null)
   //                            {
   //                                var result = EmailTemplete.sendCancellationApproveEmail(emplyeeProfile.User_Authentication.Email_Address, emplyeeProfile.Name, Resources.ResourceExpenses.Expenses, firstApproverProfile.Name, domain);
   //                                if (result == false)
   //                                {
   //                                    return new ServiceResult() { Code = ERROR_CODE.ERROR_501_CANT_SEND_EMAIL, Msg = new Error().getError(ERROR_CODE.ERROR_501_CANT_SEND_EMAIL), Field = Resources.ResourceExpenses.Expenses };
   //                                }
   //                            }

   //                            return new ServiceResult() { Code = ERROR_CODE.SUCCESS_APPROVE, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resources.ResourceExpenses.Expenses };
   //                        }
   //                        else if (pStatus == LeaveStatus.RejectedCancel)
   //                        {
   //                            current.Remark = pRemark;
   //                            current.Approval_Cancel_Status = null;
   //                            db.SaveChanges();

   //                            return new ServiceResult() { Code = ERROR_CODE.SUCCESS_REQUEST_CANCEL, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_REQUEST_CANCEL), Field = Resources.ResourceExpenses.Expenses };
   //                        }
   //                        else
   //                        {
   //                            current.Remark = pRemark;
   //                            db.SaveChanges();

   //                            // send request cancel email to first approver.                                
   //                            if (approval != null && firstApproverProfile != null)
   //                            {
   //                                var result = EmailTemplete.sendcancelRequestEmail(firstApproverProfile.User_Authentication.Email_Address, emplyeeProfile.Name, Resources.ResourceExpenses.Expenses, firstApproverProfile.Name, current.Expenses_Application_ID, domain);
   //                                if (result == false)
   //                                {
   //                                    return new ServiceResult() { Code = ERROR_CODE.ERROR_501_CANT_SEND_EMAIL, Msg = new Error().getError(ERROR_CODE.ERROR_501_CANT_SEND_EMAIL), Field = Resources.ResourceExpenses.Expenses };
   //                                }
   //                            }
   //                            return new ServiceResult() { Code = ERROR_CODE.SUCCESS_REQUEST_CANCEL, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_REQUEST_CANCEL), Field = Resources.ResourceExpenses.Expenses };
   //                        }

   //                    }
   //                }

   //            }
   //        }
   //        catch
   //        {
   //            return new ServiceResult() { Code = ERROR_CODE.ERROR_506_SAVE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_506_SAVE_ERROR), Field = Resources.ResourceExpenses.Expenses };
   //        }
   //        return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = Resources.ResourceExpenses.Expenses };
   //    }

   //    public ServiceResult UpdateStatus(User_Profile userlogin, Nullable<int> pExpensesID, Nullable<int> pEmployeeID, Nullable<int> pApproveProfileID, string pStatus, string pRemark, string domain)
   //    {
   //        try
   //        {
   //            var currentdate = StoredProcedure.GetCurrentDate();
   //            using (var db = new SBS2DBContext())
   //            {
   //                if (pExpensesID.HasValue && pApproveProfileID.HasValue && pEmployeeID.HasValue)
   //                {
   //                    var current = (from a in db.Expenses_Application
   //                                   where a.Expenses_Application_ID == pExpensesID.Value
   //                                   select a).FirstOrDefault();
   //                    if (current != null)
   //                    {
   //                        current.Remark = pRemark;


   //                        var userhist = (from a in db.Employment_History
   //                                        where a.Effective_Date <= currentdate
   //                                        & a.Employee_Profile_ID == pEmployeeID.Value
   //                                        orderby a.Effective_Date descending
   //                                        select a).FirstOrDefault();

   //                        var approvalemployee = (from a in db.Employee_Profile where a.Profile_ID == pApproveProfileID.Value select a).FirstOrDefault();
   //                        if (approvalemployee != null)
   //                        {
   //                            var approval = (from a in db.Expenses_Approval
   //                                            where a.Department_ID == userhist.Department_ID
   //                                            & (approvalemployee.Employee_Profile_ID == a.First_Approval || approvalemployee.Employee_Profile_ID == a.Second_Approval)
   //                                            select a).FirstOrDefault();
   //                            if (approval == null)
   //                            {
   //                                return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = Resources.ResourceExpenses.Expenses };
   //                            }

   //                            if (approval.First_Approval == approval.Second_Approval)
   //                            {
   //                                // is same approver
   //                                current.Approval_Status_1st = pStatus;
   //                                current.Approval_Status_2st = pStatus;
   //                                current.Overall_Status = pStatus;
   //                            }
   //                            else
   //                            {
   //                                if (approval.First_Approval == approvalemployee.Employee_Profile_ID)
   //                                {
   //                                    current.Approval_Status_1st = pStatus;
   //                                    if (pStatus == LeaveStatus.Rejected)
   //                                    {
   //                                        current.Approval_Status_2st = pStatus;
   //                                    }
   //                                }
   //                                else if (approval.Second_Approval == approvalemployee.Employee_Profile_ID)
   //                                {
   //                                    current.Approval_Status_2st = pStatus;
   //                                    current.Overall_Status = pStatus;
   //                                }
   //                            }

   //                            if (pStatus == LeaveStatus.Rejected)
   //                            {
   //                                current.Overall_Status = LeaveStatus.Rejected;
   //                            }

   //                            db.SaveChanges();

   //                            // send mail
   //                            var emp = (from a in db.Employee_Profile where a.Employee_Profile_ID == pEmployeeID select a).SingleOrDefault();
   //                            if (emp != null)
   //                            {
   //                                if (current.Overall_Status == LeaveStatus.Approved)
   //                                {
   //                                    //final approve
   //                                    var approverName = userlogin.Name;
   //                                    if (approval.First_Approval != approval.Second_Approval)
   //                                    {
   //                                        approverName = approval.Employee_Profile.User_Profile.Name + " and " + approval.Employee_Profile1.User_Profile.Name;
   //                                    }
   //                                    var result = EmailTemplete.sendApproveEmail(emp.User_Profile.User_Authentication.Email_Address, emp.User_Profile.Name, Resources.ResourceExpenses.Expenses, approverName, domain, getExpensesReviewerEmail(userlogin.Company_ID));
   //                                    if (result == false)
   //                                    {
   //                                        return new ServiceResult() { Code = ERROR_CODE.ERROR_501_CANT_SEND_EMAIL, Msg = new Error().getError(ERROR_CODE.ERROR_501_CANT_SEND_EMAIL), Field = Resources.ResourceExpenses.Expenses };
   //                                    }
   //                                }
   //                                else if (pStatus == LeaveStatus.Rejected)
   //                                {
   //                                    var result = EmailTemplete.sendRejectEmail(emp.User_Profile.User_Authentication.Email_Address, emp.User_Profile.Name, Resources.ResourceExpenses.Expenses, userlogin.Name, domain);
   //                                    if (result == false)
   //                                    {
   //                                        return new ServiceResult() { Code = ERROR_CODE.ERROR_501_CANT_SEND_EMAIL, Msg = new Error().getError(ERROR_CODE.ERROR_501_CANT_SEND_EMAIL), Field = Resources.ResourceExpenses.Expenses };
   //                                    }
   //                                }
   //                                else
   //                                {
   //                                    if (approval.First_Approval != approval.Second_Approval)
   //                                    {
   //                                        if (approval.First_Approval == approvalemployee.Employee_Profile_ID)
   //                                        {
   //                                            if (pStatus == LeaveStatus.Approved)
   //                                            {
   //                                                // send email to approval 2
   //                                                var result = EmailTemplete.sendRequestEmail(approval.Employee_Profile1.User_Profile.User_Authentication.Email_Address, emp.User_Profile.Name, Resources.ResourceExpenses.Expenses, approval.Employee_Profile1.User_Profile.Name, current.Expenses_Application_ID, domain);
   //                                                if (result == false)
   //                                                {
   //                                                    return new ServiceResult() { Code = ERROR_CODE.ERROR_501_CANT_SEND_EMAIL, Msg = new Error().getError(ERROR_CODE.ERROR_501_CANT_SEND_EMAIL), Field = Resources.ResourceExpenses.Expenses };
   //                                                }
   //                                            }
   //                                        }
   //                                    }
   //                                }
   //                            }


   //                            if (pStatus == LeaveStatus.Approved)
   //                            {
   //                                return new ServiceResult() { Code = ERROR_CODE.SUCCESS_APPROVE, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resources.ResourceExpenses.Expenses };
   //                            }
   //                            else if (pStatus == LeaveStatus.Rejected)
   //                            {
   //                                return new ServiceResult() { Code = ERROR_CODE.SUCCESS_REJECT, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_REJECT), Field = Resources.ResourceExpenses.Expenses };
   //                            }


   //                        }

   //                    }
   //                }

   //            }
   //        }
   //        catch
   //        {
   //            return new ServiceResult() { Code = ERROR_CODE.ERROR_506_SAVE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_506_SAVE_ERROR), Field = Resources.ResourceExpenses.Expenses };
   //        }
   //        return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = Resources.ResourceExpenses.Expenses };
   //    }

   //    public string getExpensesReviewerEmail(Nullable<int> Company_ID)
   //    {
   //        string email = "";
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                DateTime currentdate = StoredProcedure.GetCurrentDate();
   //                var comp = db.Company_Details
   //                    .Include(i => i.Company)
   //                    .Include(i => i.Company.Expenses_Reviewer)
   //                    .Where(w => w.Company_ID == Company_ID)
   //                    .Where(w => w.Effective_Date <= currentdate)
   //                    .FirstOrDefault();

   //                email = comp.Company.Expenses_Reviewer.FirstOrDefault().Email_Address;
   //            }
   //        }
   //        catch
   //        {
   //        }

   //        return email;
   //    }

   //    public Expenses_Config_Detail GetExpensesConfigDetail(Nullable<int> pExpensesConfigID, Nullable<int> pProfileID)
   //    {
   //        try
   //        {
   //            var currentdate = StoredProcedure.GetCurrentDate();
   //            using (var db = new SBS2DBContext())
   //            {
   //                var emp = db.Employee_Profile.Where(w => w.Profile_ID == pProfileID).FirstOrDefault();
   //                if (emp == null) { return null; }

   //                var userhist = getEmploymentHistory(emp.Employee_Profile_ID, currentdate);
   //                if (userhist == null) { return null; }

   //                var yearservice = 0;
   //                var firsthist = db.Employment_History.Where(w => w.Employee_Profile_ID == emp.Employee_Profile_ID).OrderBy(o => o.Effective_Date).FirstOrDefault();
   //                if (firsthist != null && firsthist.Effective_Date.HasValue)
   //                {
   //                    var hiredDate = firsthist.Effective_Date.Value;
   //                    var workspan = (currentdate.Date - hiredDate.Date);
   //                    yearservice = NumUtil.ParseInteger(Math.Floor((workspan.TotalDays + 1) / 365));
   //                }

   //                return db.Expenses_Config_Detail
   //                    .Where(w => w.Expenses_Config_ID == pExpensesConfigID & (w.Designation_ID == userhist.Designation_ID | w.Designation_ID == null) & w.Year_Service <= yearservice)
   //                    .OrderByDescending(o => o.Year_Service)
   //                    .FirstOrDefault();
   //            }
   //        }
   //        catch
   //        {
   //            return null;
   //        }
   //    }
   //    public decimal calulateBalance(Expenses_Config expenseType, Expenses_Config_Detail expenseTypeDetail, int profileID, DateTime appliedDate)
   //    {
   //        var currentdate = StoredProcedure.GetCurrentDate();
   //        decimal balance = 0;

   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {


   //                if (expenseType.Global_Lookup_Data == null) { return 0; }

   //                var emp = db.Employee_Profile.Where(w => w.Profile_ID == profileID).FirstOrDefault();
   //                if (emp == null) return 0;

   //                var userhist = getEmploymentHistory(emp.Employee_Profile_ID, appliedDate);
   //                if (userhist == null) { return 0; }


   //                //Expense Type - Per Department / Per Employee 
   //                bool isPerDepartment = false;
   //                if (expenseType.Global_Lookup_Data.Name == "Per Department") isPerDepartment = true;


   //                if (expenseTypeDetail == null) { return 0; }

   //                balance = expenseTypeDetail.Amount_Per_Year.HasValue ? expenseTypeDetail.Amount_Per_Year.Value : 0;

   //                //Set startDate and endDate for Filter yearly balance


   //                var com = db.Company_Details.Where(w => w.Company_ID == emp.User_Profile.Company_ID & w.Effective_Date <= currentdate).OrderByDescending(o => o.Effective_Date).FirstOrDefault();
   //                if (com == null) return 0;

   //                DateTime startDate, endDate;
   //                if (com.Default_Fiscal_Year.HasValue && com.Default_Fiscal_Year.Value)
   //                {
   //                    startDate = DateUtil.ToDate(1, 1, appliedDate.Year).Value;
   //                    endDate = DateUtil.ToDate(DateTime.DaysInMonth(appliedDate.Year, appliedDate.Month), 12, appliedDate.Year).Value;
   //                }
   //                else
   //                {
   //                    startDate = DateUtil.ToDate(com.Custom_Fiscal_Year.Value.Day, com.Custom_Fiscal_Year.Value.Month, (appliedDate.Year - 1)).Value;
   //                    endDate = DateUtil.ToDate(com.Custom_Fiscal_Year.Value.Day, com.Custom_Fiscal_Year.Value.Month, appliedDate.Year).Value;

   //                }

   //                //set to 00:00 of next day (24:00 of endDate)
   //                //endDate = endDate.AddDays(1);

   //                var expenses = new List<Expenses_Application_Document>();
   //                if (isPerDepartment)
   //                {
   //                    //Get Department ID 

   //                    //Get All expense application in department 'depID' 
   //                    expenses = getDepartmentExpenseApplications(expenseType.Department_ID);

   //                }
   //                else
   //                {
   //                    //Get all expense applications of Employee
   //                    expenses = getExpenseApplicationDocs(emp.Employee_Profile_ID);

   //                }

   //                //Filter with specific Expense Type between startDate and endDate
   //                expenses = expenses.Where(w => w.Expenses_Config_ID == expenseType.Expenses_Config_ID).Where(w => w.Expenses_Application.Date_Applied >= startDate).Where(w => w.Expenses_Application.Date_Applied <= endDate).ToList();

   //                //Calucate Balance
   //                if (expenses != null && expenses.Count > 0)
   //                {
   //                    foreach (Expenses_Application_Document expense in expenses)
   //                    {
   //                        if (expense.Expenses_Application.Overall_Status == LeaveStatus.Approved)
   //                        {
   //                            balance -= (expense.Amount_Claiming.HasValue ? expense.Amount_Claiming.Value : 0);
   //                        }
   //                    }
   //                }
   //            }
   //        }
   //        catch
   //        {
   //        }

   //        return balance;
   //    }


   //    public string getEmployeeEmail(int Employee_Profile_ID)
   //    {
   //        string email = "";
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                var emp = db.Employee_Profile.Include(i => i.User_Profile).Include(i => i.User_Profile.User_Authentication)
   //                    .Where(w => w.Employee_Profile_ID == Employee_Profile_ID).FirstOrDefault();

   //                email = emp.User_Profile.User_Authentication.Email_Address;

   //            }
   //        }
   //        catch
   //        {
   //        }

   //        return email;
   //    }

   //    public string getExpenseReviewerEmail(int Company_ID)
   //    {
   //        string email = "";
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {

   //                DateTime currentdate = StoredProcedure.GetCurrentDate();
   //                var comp = db.Company_Details.Include(i => i.Company).Include(i => i.Company.Expenses_Reviewer)
   //                    .Where(w => w.Company_ID == Company_ID).Where(w => w.Effective_Date <= currentdate).FirstOrDefault();

   //                email = comp.Company.Expenses_Reviewer.FirstOrDefault().Email_Address;
   //            }
   //        }
   //        catch
   //        {
   //        }

   //        return email;
   //    }

   //    public Expenses_Reviewer GetExpensesReviewer(Nullable<int> pCompanyID)
   //    {
   //        using (var db = new SBS2DBContext())
   //        {
   //            return (from a in db.Expenses_Reviewer where a.Company_ID == pCompanyID select a).SingleOrDefault();
   //        }
   //    }

   //    public ServiceResult SaveExpensesReviewer(Expenses_Reviewer expensesReviewer)
   //    {
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                if (expensesReviewer.Expenses_Reviewer_ID > 0)
   //                {
   //                    //Update
   //                    var current = (from a in db.Expenses_Reviewer where a.Expenses_Reviewer_ID == expensesReviewer.Expenses_Reviewer_ID select a).FirstOrDefault();
   //                    if (current == null)
   //                    {
   //                        return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = Resources.ResourceConfiguration.ExpensesReviewers };
   //                    }
   //                    current.Email_Address = expensesReviewer.Email_Address;
   //                    db.SaveChanges();
   //                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS_EDIT, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resources.ResourceConfiguration.ExpensesReviewers };
   //                }
   //                else
   //                {
   //                    //Insert
   //                    db.Expenses_Reviewer.Add(expensesReviewer);
   //                    db.SaveChanges();
   //                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS_CREATE, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resources.ResourceConfiguration.ExpensesReviewers };
   //                }
   //            }
   //        }
   //        catch
   //        {
   //            return new ServiceResult() { Code = ERROR_CODE.ERROR_506_SAVE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_506_SAVE_ERROR), Field = Resources.ResourceConfiguration.ExpensesReviewers };
   //        }
   //    }

   //}

   public class ExpensesDetailViewModel
   {
      public List<Expenses_Config> expensesConfigList { get; set; } // expense config
      public List<ComboViewModel> currencyList { get; set; }


      public int Index { get; set; }

      public Nullable<int> Expenses_Application_Document_ID { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Expenses_Type", typeof(Resource))]
      public Nullable<int> Expenses_Config_ID { get; set; }

      [LocalizedDisplayName("ClaimableType", typeof(Resource))]
      public string Claimable_Type { get; set; }

      [LocalizedDisplayName("Department", typeof(Resource))]
      public Nullable<int> Department_ID { get; set; }

      [LocalizedDisplayName("Date_Applied", typeof(Resource))]
      public string Date_Applied { get; set; }

      [LocalizedDisplayName("Total_Amount", typeof(Resource))]
      public Nullable<decimal> Total_Amount { get; set; }

      [LocalizedDisplayName("Amount_Claiming", typeof(Resource))]
      public Nullable<decimal> Amount_Claiming { get; set; }

      [LocalizedDisplayName("Balance", typeof(Resource))]
      public Nullable<decimal> Balance { get; set; }

      [LocalizedDisplayName("Tax", typeof(Resource))]
      public Nullable<decimal> Tax { get; set; }

      [LocalizedValidMaxLength(500)]
      [LocalizedDisplayName("Expenses_Type_Desc", typeof(Resource))]
      public string Expenses_Type_Desc { get; set; }
      public string Expenses_Type_Name { get; set; }

      [LocalizedValidMaxLength(500)]
      [LocalizedDisplayName("Notes", typeof(Resource))]
      public string Notes { get; set; }

      [LocalizedDisplayName("Upload_Receipt", typeof(Resource))]
      public Nullable<System.Guid> Upload_Receipt_ID { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Selected_Currency", typeof(Resource))]
      public Nullable<int> Selected_Currency { get; set; }

      [LocalizedValidDate]
      [LocalizedRequired]
      [LocalizedDisplayName("Doc_Date", typeof(Resource))]
      public string Expenses_Date { get; set; }

      [LocalizedValidMaxLength(300)]
      public string Doc_No { get; set; }

      public string Upload_Receipt { get; set; }
      public string Upload_Receipt_Name { get; set; }
      public string Row_Type { get; set; }
      public Nullable<int> UOM_ID { get; set; }
      public string UOM_Name { get; set; }
      public Nullable<decimal> Mileage { get; set; }
      public Nullable<decimal> Amount_Per_UOM { get; set; }

      //********  Smart Dev  ********//
      public Nullable<int> Job_Cost_ID { get; set; }
      public string Job_Cost_Name { get; set; }
      
      [LocalizedDisplayName("Withholding_Tax", typeof(Resource))]
      public Nullable<decimal> Withholding_Tax { get; set; }
      public string Tax_Type { get; set; }

      public Nullable<decimal> Withholding_Tax_Amount { get; set; }
      public Nullable<decimal> Tax_Amount { get; set; }

      [LocalizedValidMaxLength(10)]
      [LocalizedDisplayName("Type", typeof(Resource))]
      public string Tax_Amount_Type { get; set; }

      [LocalizedValidMaxLength(10)]
      [LocalizedDisplayName("Type", typeof(Resource))]
      public string Withholding_Tax_Type { get; set; }

      public Boolean View_Readonly { get; set; }
      
   }


   public class ExpensesSummaryViewModel : ModelBase
   {
     
      public List<Employee_Profile> EmployeeList { get; set; }
      public List<Expenses_Application> expensesApplicationList { get; set; }
      public Nullable<int> search_Emp { get; set; }
      public Nullable<int> search_Jobcost { get; set; }

      public List<ComboViewModel> Statuslst { get; set; }
      public List<ComboViewModel> Monthlst { get; set; }
      public List<ComboViewModel> JobCostList { get; set; }

      [LocalizedDisplayName("Month", typeof(Resource))]
      public Nullable<int> Search_Month { get; set; }

      [LocalizedDisplayName("Year", typeof(Resource))]
      public Nullable<int> Search_Year { get; set; }

      public string search_Status { get; set; }

   }

   public class ExpensesViewModel : ModelBase
   {
      public string tabAction { get; set; }
      //Added By sun 26-08-2015
      //public List<Expenses_Category> expensesCalculatioList { get; set; }
      //Added By sun 26-08-2015
     
      public List<ComboViewModel> expensesCalculatioList { get; set; }
      public string CkYear { get; set; }

      public List<Expenses_Config> expensesConfigList { get; set; }
      

        
      public List<ComboViewModel> currencyList { get; set; }
      public List<ComboViewModel> eStatuslist { get; set; }

      public string ApprStatus { get; set; }
      public int Approval_Level { get; set; }
      public string PageStatus { get; set; }

     


      //********  Management  ********//

      public List<Expenses_Application> ExpensesProcessedLst { get; set; }
      public List<Expenses_Application> ExpensesPendingLst { get; set; }

      public string Pending_Date_Applied { get; set; }
      public Nullable<int> Pending_Profile_ID { get; set; }

      public string Process_Date_Applied { get; set; }
      public Nullable<int> Process_Profile_ID { get; set; }



      public Nullable<int> search_Emp { get; set; }
     
      public string search_Expenses_Status { get; set; }
      public Nullable<int> search_Pending_Emp { get; set; }
      public string search_Pending_Date_Applied { get; set; }
      public Nullable<int> search_Process_Emp { get; set; }
      public string search_Process_Date_Applied { get; set; }
      public Nullable<int> Expenses_ID { get; set; }

      //Added By sun 26-08-2015
      public Nullable<int> Expenses_Category_ID { get; set; }
      public Nullable<int> Expenses_Config_ID { get; set; }

      [LocalizedDisplayName("Expenses_No", typeof(Resource))]
      public string Expenses_No { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(300)]
      [LocalizedDisplayName("Expenses_Title", typeof(Resource))]
      public string Expenses_Title { get; set; }

      [LocalizedValidDate]
      [LocalizedRequired]
      [DataType(DataType.Date)]
      [LocalizedDisplayName("Date_Applied", typeof(Resource))]
      public string Date_Applied { get; set; }

      public int Employee_Profile_ID { get; set; }
      public string Name { get; set; }
      public string Email { get; set; }

      [LocalizedDisplayName("Employee_Name", typeof(Resource))]
      public string Employee_Name { get; set; }

      [LocalizedValidMaxLength(300)]
      [LocalizedDisplayName("Remarks", typeof(Resource))]
      public string Remarks { get; set; }

      [LocalizedValidMaxLength(300)]
      [LocalizedDisplayName("Remarks", typeof(Resource))]
      public string Remark_Rej { get; set; }

      public string Approval_Status_1st { get; set; }
      public string Approval_Status_2st { get; set; }
      public string Approval_Cancel_Status { get; set; }
      public string Overall_Status { get; set; }

      [DataType(DataType.Date)]
      public string Last_Date_Approve { get; set; }

      public bool isFirstApproval { get; set; }
      public Nullable<int> Default_Currency_ID { get; set; }
      public string Default_Currency_Code { get; set; }
      public ExpensesDetailViewModel[] Detail_Rows { get; set; }
      public Nullable<int> Request_ID { get; set; }
      public List<SBSWorkFlowAPI.Models.Request> Expenses_Request { get; set; }
      public List<User_Profile> employeeList { get; set; }
      //Added by sun 18-02-2016
      public string Cancel_Status { get; set; }
      public string Default_Date { get; set; }
      public Nullable<int> Request_Cancel_ID { get; set; }
      public Nullable<bool> isRejectPopUp { get; set; }
      public int? Supervisor { get; set; }
      public string Supervisor_Name { get; set; }

      //********  Smart Dev  ********//
      public List<ComboViewModel> JobCostlst { get; set; }
      public List<ComboViewModel> TaxTypelst { get; set; }
      public List<ComboViewModel> AmountTypelst { get; set; }


      //***** Added by Moet for onbehalf ****//
      public List<ComboViewModel> EmployeeUnderMeList { get; set; }
      public Nullable<int> OnBehalf_Employee_Profile_ID { get; set; }
      public Nullable<int> OnBehalf_Profile_ID { get; set; }

      
     
     

   }

   //Added By sun 25-08-2015
   public class ExpensesCategoryViewModel : ModelBase
   {

      public Nullable<int> ExpensesCategory_ID { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(300)]
      [LocalizedDisplayName("Category_Name", typeof(Resource))]
      public string Category_Name { get; set; }

      [LocalizedValidMaxLength(300)]
      [LocalizedDisplayName("Category_Desc", typeof(Resource))]
      public string Category_Description { get; set; }

      //Edit By sun 07-09-2015
      [LocalizedRequired]
      [LocalizedDisplayName("Record_Status", typeof(Resource))]
      public string Record_Status { get; set; }
      public List<ComboViewModel> statusList { get; set; }
      public string pageAction { get; set; }

   }

   public class ExpensesTypeViewModel : ModelBase
   {
      //Added By sun 26-08-2015
      public List<ComboViewModel> ExpensesCategoryList { get; set; }
      public List<Expenses_Config> ExpensesType { get; set; }
      public List<ComboViewModel> expensesTypeList { get; set; }
      //public List<Department> departmentList { get; set; }
      public List<ComboViewModel> departmentList { get; set; }
      public List<ComboViewModel> claimtypeList { get; set; }
      public List<ComboViewModel> uomList { get; set; }

      public Nullable<int> eid { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(300)]
      [LocalizedDisplayName("Expenses_Type", typeof(Resource))]
      public String Expenses_Name { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Claimable_Type", typeof(Resource))]
      public string Claimable_Type { get; set; }

      //Added By sun 26-08-2015
      [LocalizedDisplayName("Expenses_Category", typeof(Resource))]
      public Nullable<int> Expenses_Category_ID { get; set; }

      [LocalizedValidMaxLength(300)]
      [LocalizedDisplayName("Expenses_Desc", typeof(Resource))]
      public String Expenses_Description { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Allowed_Probation", typeof(Resource))]
      public bool Allowed_Probation { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Allowed_Over_Amount_Per_Year", typeof(Resource))]
      public bool Allowed_Over_Amount_Per_Year { get; set; }

      public string Company_Currency { get; set; }

      [LocalizedDisplayName("Department", typeof(Resource))]
      public Nullable<int> Department_ID { get; set; }

      public Nullable<int> isDepartment { get; set; }

      public ExpensesTypeDetailViewModel[] Detail_Rows { get; set; }

      // *******  expenses type detail ********
      public List<ComboViewModel> designationList { get; set; }

      public int[] Designations { get; set; }

      [LocalizedDisplayName("Amount_Per_Year", typeof(Resource))]
      public Nullable<decimal> Amount_Per_Year { get; set; }

      [LocalizedDisplayName("Year_Service", typeof(Resource))]
      public Nullable<int> Year_Service { get; set; }

      public Nullable<int> Group_ID { get; set; }
      public bool Select_Percentage { get; set; }
      public bool Select_Amount { get; set; }
      public bool Select_Per_Month { get; set; }

      [LocalizedDisplayName("Amount_Per_Month", typeof(Resource))]
      public Nullable<decimal> Amount_Per_Month { get; set; }

      [LocalizedDisplayName("Amount", typeof(Resource))]
      public Nullable<decimal> Amount { get; set; }

      [LocalizedDisplayName("Pecentage", typeof(Resource))]
      public Nullable<decimal> Pecentage { get; set; }

      [LocalizedDisplayName("Mileage", typeof(Resource))]
      public Nullable<bool> Is_MileAge { get; set; }

      [LocalizedDisplayName("Unit_Of_Measurement", typeof(Resource))]
      public Nullable<int> UOM_ID { get; set; }

      [LocalizedDisplayName("Amount_Per_UOM", typeof(Resource))]
      public Nullable<decimal> Amount_Per_UOM { get; set; }

      [LocalizedDisplayName("Accumulative", typeof(Resource))]
      public bool Is_Accumulative { get; set; }

   }

   public class ExpensesTypeDetailViewModel
   {

      public int Index { get; set; }
      public int[] Designations { get; set; }
      public List<ComboViewModel> designationList { get; set; }
      public Nullable<int> Expenses_Config_ID { get; set; }
      public Nullable<decimal> Amount_Per_Year { get; set; }
      public Nullable<decimal> Amount_Per_Month { get; set; }
      public Nullable<int> Year_Service { get; set; }
      public Nullable<int> Group_ID { get; set; }
      public bool Select_Per_Month { get; set; }
      public bool Select_Percentage { get; set; }

      [LocalizedDisplayName("Amount", typeof(Resource))]
      public Nullable<decimal> Amount { get; set; }

      public bool Select_Amount { get; set; }

      [LocalizedDisplayName("Pecentage", typeof(Resource))]
      public Nullable<decimal> Pecentage { get; set; }

      public string Row_Type { get; set; }

   }

   public class ExpenseReportViewModel : ModelBase
   {

      [DataType(DataType.Date)]
      [LocalizedDisplayName("From", typeof(Resource))]
      public string sFrom { get; set; }

      [DataType(DataType.Date)]
      [LocalizedDisplayName("To", typeof(Resource))]
      public string sTo { get; set; }

      public List<Employee_Profile> employeeList { get; set; }
      public List<Expenses_Application_Document> expenseList { get; set; }
      public List<ComboViewModel> expensesTypeList { get; set; }
      public List<ComboViewModel> departmentlst { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Year", typeof(Resource))]
      public int Year { get; set; }
      public List<int> Yearlst { get; set; }

      [LocalizedDisplayName("Department", typeof(Resource))]
      public Nullable<int> Department_ID { get; set; }
      public string Expenses_Sel { get; set; }
      public List<int> Expenses_Type_Sel { get; set; }

   }

   public class PayrollReportViewModel : ModelBase
   {

      public int sDepartment { get; set; }

      [LocalizedDisplayName("Employee", typeof(Resource))]
      public string sEmployee { get; set; }

      [LocalizedDisplayName("From", typeof(Resource))]
      public string sFrom { get; set; }
      [LocalizedDisplayName("To", typeof(Resource))]
      public string sTo { get; set; }

      [LocalizedDisplayName("From", typeof(Resource))]
      public Nullable<int> sFromMonth { get; set; }
      public Nullable<int> sFromYear { get; set; }

      [LocalizedDisplayName("To", typeof(Resource))]
      public Nullable<int> sToMonth { get; set; }
      public Nullable<int> sToYear { get; set; }

      public List<PRM> prmList { get; set; }

      [LocalizedDisplayName("Department", typeof(Resource))]
      public Nullable<int> Department { get; set; }

      public List<ComboViewModel> departmentList { get; set; }
      public List<ComboViewModel> processDateList { get; set; }

   }

   public class ExpensesBalanceViewModel
   {

      public string Expenses_Type_Name { get; set; }
      public Nullable<decimal> Total_Amount { get; set; }
      public Nullable<decimal> Amount { get; set; }

   }

   public class ExpensesApplieViewModel
   {

      public Nullable<int> Expenses_Type { get; set; }
      public string Expenses_Type_Name { get; set; }
      public Nullable<decimal> Amount { get; set; }

   }

   public class ExpensesDashBoardViewModel : ModelBase
   {

      public List<ExpensesBalanceViewModel> ExpensesBalanceList { get; set; }
      public List<ExpensesApplieViewModel> ExpensesApplieList { get; set; }
      public List<Expenses_Application> ExpensesList { get; set; }
      public string Currency_Code { get; set; }

   }

   //Added by sun 05-02-2016
   public class ImportExpensesViewModels : ModelBase
   {

      public bool Validated_Main { get; set; }
      public ImportExpensesApplicationViewModels[] ExpensesAppDoc { get; set; }
      public List<string> ErrMsg { get; set; }

   }

   public class ImportExpensesApplicationViewModels : ModelBase
   {

      //Expenses_Application
      public string Overall_Status { get; set; }
      public Nullable<int> Request_ID { get; set; }
      public Nullable<int> Company_ID { get; set; }
      public bool Validate { get; set; }
      public string ErrMsg { get; set; }
      public string Employee_No { get; set; }
      public Nullable<int> Employee_Profile_ID { get; set; }
      public string Expenses_No { get; set; }
      public string Expenses_Title { get; set; }
      public string Date_Applied { get; set; }

      //Expenses_Application_Document
      public Nullable<int> Expenses_Config_ID { get; set; }
      public string Expenses_Config_Type { get; set; }
      public Nullable<int> Department_ID { get; set; }
      public decimal Amount_Claiming { get; set; }
      public decimal Total_Amount { get; set; }
      public decimal Tax { get; set; }
      public string Expenses_Date { get; set; }
      public Nullable<int> Selected_Currency { get; set; }
      public string Selected_Currency_ { get; set; }
      public string Remarks { get; set; }
      public string Doc_No { get; set; }
   }

   public class ExpensesListViewModel : ModelBase
   {

      public List<Expenses_List> ExpensesList { get; set; }
      public List<ComboViewModel> ExpensesTypelst { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Year", typeof(Resource))]
      public int Year { get; set; }
      public List<int> Yearlst { get; set; }

      [LocalizedDisplayName("Department", typeof(Resource))]
      public Nullable<int> Department_ID { get; set; }
      public List<ComboViewModel> departmentlst { get; set; }

   }

   public class Expenses_List : ModelBase
   {

      public Nullable<int> Expenses_ID { get; set; }
      public string Expenses_Type { get; set; }
      public decimal Claimable_Amount { get; set; }

   }

   //********  Smart Dev  ********//
   public class ExpensesDocPrintViewModel : ModelBase
   {

      public Nullable<int> Supervisor { get; set; }
      public string Supervisor_Name { get; set; }
      public string Supervisor_Designation { get; set; }

      public List<Expenses_Application_Document> ExpensesDetailList { get; set; }
      public List<SBSWorkFlowAPI.Models.Request> Expenses_Request { get; set; }
      public List<ComboViewModel> JobCostlst { get; set; }
      public List<ComboViewModel> TaxTypelst { get; set; }

      //Top Position 
      public string Company_Name { get; set; }
      public string Address_Row_1 { get; set; }
      public string Address_Row_2 { get; set; }
      public string Address_Row_3 { get; set; }

      //Detail
      public Nullable<int> Expenses_ID { get; set; }
      public string Expenses_Title { get; set; }
      public string Department_Name { get; set; }
      public string Designation_Name { get; set; }
      public string Employee_No { get; set; }
      public string Employee_Name { get; set; }
      public string Date { get; set; }
      public string Cancel_Status { get; set; }
      public string Overall_Status { get; set; }

      public Nullable<int> Request_ID { get; set; }
      public Nullable<int> Request_Cancel_ID { get; set; }
   }
}