using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using HR.Common;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;
using HR.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using SBSModel.Models;
using SBSModel.Common;
using SBSResourceAPI;


namespace HR.Models
{
   

    public class PayrollConfigurationViewModel : ModelBase
    {
        public string tabAction { get; set; }
        public List<ComboViewModel> prtList { get; set; }
        public List<ComboViewModel> departmentList { get; set; }
        public List<ComboViewModel> raceList { get; set; }
        public List<ComboViewModel> donationTypelist { get; set; }

        //******** Allowance/Deduction ***********
        public Nullable<int> search_Allowance_Type { get; set; }
        public Nullable<int> search_Department { get; set; }
        public List<PRC> PRCList { get; set; }
        public Nullable<int> Allowance_PRC_ID { get; set; }
        public Nullable<int> Allowance_PRT_ID { get; set; } // Allowance Type

        [LocalizedValidMaxLength(150)]
        [LocalizedDisplayName("Allowance_Type_Name", typeof(Resource))]
        public string Allowance_Name { get; set; }

        [LocalizedValidMaxLength(500)]
        [LocalizedDisplayName("Description", typeof(Resource))]
        public string Allowance_Description { get; set; }

        [LocalizedDisplayName("CPF_Deductable", typeof(Resource))]
        public bool Allowance_CPF_Deductable { get; set; }

        [LocalizedDisplayName("Department", typeof(Resource))]
        public List<int> Allowance_Departments { get; set; }
        public Nullable<decimal> Allowance_OT_Multiplier { get; set; }

        //******** Donation ***********
        [LocalizedDisplayName("Race", typeof(Resource))]
        public Nullable<int> search_Race { get; set; }
        public List<Donation_Formula> DonationFormulaList { get; set; }

        [LocalizedDisplayName("Donation", typeof(Resource))]
        public Nullable<int> Donation_Formula_ID { get; set; }

        [LocalizedDisplayName("Formula", typeof(Resource))]
        public string Donation_Formula { get; set; }

        [LocalizedValidMaxLength(500)]
        [LocalizedDisplayName("Formula_Name", typeof(Resource))]
        public string Donation_Formula_Name { get; set; }

        [LocalizedDisplayName("Donation_Type", typeof(Resource))]
        public Nullable<int> Donation_Type_ID { get; set; }

        [LocalizedDisplayName("Race", typeof(Resource))]
        public Nullable<int> Donation_Race { get; set; }
        public List<Selected_Donation_Formula> Donation_Selected_Formula_IDs { get; set; }

        //******** CPF ***********
        [LocalizedDisplayName("Year", typeof(Resource))]
        public Nullable<int> search_Cpf_Year { get; set; }
        public List<CPF_Formula> CpfFormulaList { get; set; }

        [LocalizedDisplayName("CPF", typeof(Resource))]
        public Nullable<int> CPF_Formula_ID { get; set; }

        [LocalizedDisplayName("CPF_Formula", typeof(Resource))]
        public string Cpf_Formula { get; set; }

        [LocalizedValidMaxLength(500)]
        [LocalizedDisplayName("Formula_Name", typeof(Resource))]
        public string Cpf_Formula_Name { get; set; }

        [LocalizedValidMaxLength(500)]
        [LocalizedDisplayName("Formula_Desc", typeof(Resource))]
        public string Cpf_Formula_Desc { get; set; }
        public Nullable<int> CPF_Selected_Formula_ID { get; set; }

        //******** Authen ***********
        public Nullable<int> search_Authen_Department { get; set; }
        public List<PRG> PRGList { get; set; }

        //********* Notification ********************//
        public List<ComboViewModel> NotificationMonthList { get; set; }

        public int Notification_Scheduler_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public string Notice_Type { get; set; }
        public bool Trigger_Set_Up { get; set; }
        public string Trigger_Period { get; set; }

        [LocalizedDisplayName("Time", typeof(Resource))]
        //[DataType(DataType.Time)]
        public string Start_Time { get; set; }

        [LocalizedDisplayName("Start", typeof(Resource))]
        [DataType(DataType.Date)]
        public string Start_Date { get; set; }

        [LocalizedDisplayName("Recur_Every", typeof(Resource))]
        public Nullable<int> Recur_Every_Days { get; set; }

        [LocalizedDisplayName("Recur_Every", typeof(Resource))]
        public Nullable<int> Recur_Every_Weeks { get; set; }
        public bool Selected_Sunday { get; set; }
        public bool Selected_Monday { get; set; }
        public bool Selected_Tuesday { get; set; }
        public bool Selected_Wednesday { get; set; }
        public bool Selected_Thursday { get; set; }
        public bool Selected_Friday { get; set; }
        public bool Selected_Saturday { get; set; }
        public Nullable<int> Selected_Months { get; set; }
        public string Selected_Days { get; set; }
    }

    public class FormulaViewModel : ModelBase
    {
        public List<CPF_Formula> formula_list { get; set; }
        public List<OT_Formula> OTformula_list { get; set; }
        public List<Donation_Formula> Donationformula_list { get; set; }
        public List<ComboViewModel> racelist { get; set; }
        public List<ComboViewModel> donationlist { get; set; }
        public Nullable<int> Formula_ID { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Formula", typeof(Resource))]
        public string Formula { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Formula_Name", typeof(Resource))]
        [LocalizedValidMaxLength(500)]
        public string Formula_Name { get; set; }

        [LocalizedDisplayName("Formula_Desc", typeof(Resource))]
        [LocalizedValidMaxLength(500)]
        public string Formula_Desc { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Year", typeof(Resource))]
        public Nullable<int> Year { get; set; }
    }

    public class SelectedCPFFormulaViewModel : ModelBase
    {
        public Nullable<int> Selected_CPF_Formula_ID { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Formula", typeof(Resource))]
        public Nullable<int> CPF_Formula_ID { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Formula", typeof(Resource))]
        public string Formula { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Formula_Name", typeof(Resource))]
        [LocalizedValidMaxLength(500)]
        public string Formula_Name { get; set; }

         [LocalizedValidDate]
        [LocalizedRequired]
        [LocalizedDisplayName("Effective_Date", typeof(Resource))]
        public string Effective_Date { get; set; }

        [LocalizedDisplayName("Formula_Desc", typeof(Resource))]
        [LocalizedValidMaxLength(500)]
        public string Formula_Desc { get; set; }

        [LocalizedDisplayName("Year", typeof(Resource))]
        public Nullable<int> Year { get; set; }
    }

    public class PRTViewModel : ModelBase
    {
        public List<PRT> PRTlist { get; set; }
        public int pid { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Allowance_Type_Name", typeof(Resource))]
        [LocalizedValidMaxLength(300)]
        public string Name { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Allowance", typeof(Resource))]
        [LocalizedValidMaxLength(1)]
        public string Allowance { get; set; }
    }

    public class PRCViewModel : ModelBase
    {
        public List<PRC> PRClist { get; set; }
        public List<PRT> PRTlist { get; set; }
        public int pid { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Allowance_Type", typeof(Resource))]
        public int AllowanceType { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Status", typeof(Resource))]
        [LocalizedValidMaxLength(1)]
        public string Status { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Description", typeof(Resource))]
        [LocalizedValidMaxLength(500)]
        public string Description { get; set; }
        public Nullable<decimal> OT_Multiplier { get; set; }
    }

    public class PRALViewModel
    {
        //Check duplicated prof ID
        public int I { get; set; }
        public List<_Applicable_Employee> EmpList { get; set; }
        public int Index { get; set; }
        public Nullable<int> PRAL_ID { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Employee", typeof(Resource))]
        public Nullable<int> Employee_Profile_ID { get; set; }
        public Nullable<int> Profile_ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Row_Type { get; set; }
    }

    public class PRGViewModel : ModelBase
    {
        public List<ComboViewModel> departmentList { get; set; }
        public List<_Applicable_Employee> EmpList { get; set; }
        public int[] Departments { get; set; }
        public int[] Application_For { get; set; }
        public int[] Not_Application_For { get; set; }
        public PRALViewModel[] PRAL_Rows { get; set; }
        public Nullable<int> PRG_ID { get; set; }
    }

    public class DonationTypeViewModel : ModelBase
    {
        public List<Donation_Type> donationTypelst { get; set; }
        public List<ComboViewModel> StatusComboList { get; set; }
        public Nullable<int> Donation_Type_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Donation_Name", typeof(Resource))]
        [LocalizedValidMaxLength(150)]
        public string Donation_Name { get; set; }

        [LocalizedDisplayName("Donation_Desc", typeof(Resource))]
        [LocalizedValidMaxLength(500)]
        public string Donation_Description { get; set; }

        [LocalizedDisplayName("Status", typeof(Resource))]
        public string Record_Status { get; set; }
    }

   

    public class SelectedDonationFormulaViewModel : ModelBase
    {
        public List<Donation_Formula> Donationformula_list { get; set; }
        public List<ComboViewModel> donationTypelist { get; set; }
        public Nullable<int> Selected_Donation_Formula_ID { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Formula", typeof(Resource))]
        public string Formula { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Formula_Name", typeof(Resource))]
        [LocalizedValidMaxLength(500)]
        public string Formula_Name { get; set; }

         [LocalizedValidDate]
        [LocalizedRequired]
        [LocalizedDisplayName("Effective_Date", typeof(Resource))]
        public string Effective_Date { get; set; }

        [LocalizedDisplayName("Donation", typeof(Resource))]
        public Nullable<int> Donation_Type_ID { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Race", typeof(Resource))]
        public string Race { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Formula", typeof(Resource))]
        public Nullable<int> Donation_Formula_ID { get; set; }

        [LocalizedDisplayName("Formula_Desc", typeof(Resource))]
        [LocalizedValidMaxLength(500)]
        public string Formula_Desc { get; set; }

        [LocalizedDisplayName("Year", typeof(Resource))]
        public Nullable<int> Year { get; set; }
    }

    public class DonationFormulaViewModel : ModelBase
    {
        public List<Donation_Formula> Donationformula_list { get; set; }
        public List<ComboViewModel> donationTypelist { get; set; }
        public List<ComboViewModel> racelist { get; set; }
        public Nullable<int> Formula_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Formula", typeof(Resource))]
        public string Formula { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Formula_Name", typeof(Resource))]
        [LocalizedValidMaxLength(500)]
        public string Formula_Name { get; set; }

        [LocalizedDisplayName("Donation", typeof(Resource))]
        public Nullable<int> Donation_Type_ID { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Race", typeof(Resource))]
        public Nullable<int> Race { get; set; }

        [LocalizedDisplayName("Formula_Desc", typeof(Resource))]
        [LocalizedValidMaxLength(500)]
        public string Formula_Desc { get; set; }

        [LocalizedDisplayName("Year", typeof(Resource))]
        public Nullable<int> Year { get; set; }
    }


    //public class ExtraDonationService
    //{
    //    public List<Extra_Donation> LstExtraDonation(Nullable<int> pCompanyID = null)
    //    {
    //        using (var db = new SBS2DBContext())
    //        {
    //            return db.Extra_Donation
    //                .Where(w => w.Company_ID == null || w.Company_ID == pCompanyID)
    //                .Include(i => i.Donation_Type)
    //                .OrderBy(o => o.Donation_Type.Donation_Name)
    //                .ToList();
    //        }
    //    }
    //    public List<Extra_Donation> LstExtraDonationMaster()
    //    {
    //        using (var db = new SBS2DBContext())
    //        {
    //            return db.Extra_Donation
    //               .Where(w => w.Company_ID == null)
    //               .Include(i => i.Donation_Type)
    //               .OrderBy(o => o.Donation_Type.Donation_Name)
    //               .ToList();
    //        }
    //    }
    //    public Extra_Donation GetExtraDonation(Nullable<int> pExtraDonationID)
    //    {
    //        using (var db = new SBS2DBContext())
    //        {
    //            return db.Extra_Donation
    //               .Where(w => w.Extra_Donation_ID == pExtraDonationID)
    //               .Include(i => i.Donation_Type)
    //               .FirstOrDefault();
    //        }
    //    }

    //    public ServiceResult UpdateExtraDonation(Extra_Donation pExtraDonation)
    //    {
    //        try
    //        {
    //            using (var db = new SBS2DBContext())
    //            {
    //                if (pExtraDonation != null && pExtraDonation.Donation_Type_ID > 0)
    //                {
    //                    var current = (from a in db.Extra_Donation
    //                                   where a.Donation_Type_ID == pExtraDonation.Donation_Type_ID
    //                                   select a).FirstOrDefault();

    //                    if (current != null)
    //                    {
    //                        //Update
    //                        db.Entry(current).CurrentValues.SetValues(pExtraDonation);
    //                        db.SaveChanges();
    //                    }
    //                }
    //                return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Extra_Donation };
    //            }
    //        }
    //        catch
    //        {
    //            return new ServiceResult { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Extra_Donation };
    //        }

    //    }

    //    public ServiceResult InsertExtraDonation(Extra_Donation pExtraDonation)
    //    {
    //        try
    //        {
    //            using (var db = new SBS2DBContext())
    //            {
    //                if (pExtraDonation != null)
    //                {
    //                    //Insert    
    //                    db.Extra_Donation.Add(pExtraDonation);
    //                    db.SaveChanges();
    //                }
    //                return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Extra_Donation };
    //            }
    //        }
    //        catch
    //        {
    //            return new ServiceResult { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Extra_Donation };
    //        }

    //    }
    //    public ServiceResult DeleteExtraDonation(int pExtraDonationID)
    //    {
    //        try
    //        {
    //            using (var db = new SBS2DBContext())
    //            {
    //                if (pExtraDonationID > 0)
    //                {
    //                    var current = (from a in db.Extra_Donation where a.Extra_Donation_ID == pExtraDonationID select a).FirstOrDefault();
    //                    if (current != null)
    //                    {
    //                        db.Extra_Donation.Remove(current);
    //                        db.SaveChanges();
    //                    }

    //                }
    //                return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Extra_Donation };
    //            }
    //        }
    //        catch
    //        {
    //            return new ServiceResult { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Extra_Donation };
    //        }

    //    }

    //}

}
