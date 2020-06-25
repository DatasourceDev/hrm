using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using SBSModel.Models.Mapping;
using Microsoft.AspNet.Identity.EntityFramework;
using SBSModel.Models.Mapping;

namespace SBSModel.Models
{
    public partial class SBS2DBContext : IdentityDbContext<ApplicationUser>
    {
        static SBS2DBContext()
        {
            Database.SetInitializer<SBS2DBContext>(null);
           
        }

        public SBS2DBContext()
            : base("Name=SBS2DBContext")
        {
            
        }

        public DbSet<Access_Page> Access_Page { get; set; }
        public DbSet<Access_Right> Access_Right { get; set; }
        public DbSet<Activation_Link> Activation_Link { get; set; }
        public DbSet<Bank_Details> Bank_Details { get; set; }
        public DbSet<Banking_Info> Banking_Info { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Company_Details> Company_Details { get; set; }
        public DbSet<Company_Logo> Company_Logo { get; set; }
        public DbSet<Company_Mail_Config> Company_Mail_Config { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<CPF_Formula> CPF_Formula { get; set; }
        public DbSet<Ctl_Table_Syn> Ctl_Table_Syn { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Default_Expense_Type> Default_Expense_Type { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Donation_Formula> Donation_Formula { get; set; }
        public DbSet<Donation_Type> Donation_Type { get; set; }
        public DbSet<Email_Attachment> Email_Attachment { get; set; }
        public DbSet<Email_Notification> Email_Notification { get; set; }
        public DbSet<Employee_Attachment> Employee_Attachment { get; set; }
        public DbSet<Employee_Emergency_Contact> Employee_Emergency_Contact { get; set; }
        public DbSet<Employee_No_Pattern> Employee_No_Pattern { get; set; }
        public DbSet<Employee_Profile> Employee_Profile { get; set; }
        public DbSet<Employment_History> Employment_History { get; set; }
        public DbSet<Employment_History_Allowance> Employment_History_Allowance { get; set; }
        public DbSet<ETIRA8> ETIRA8 { get; set; }
        public DbSet<Exchange> Exchanges { get; set; }
        public DbSet<Exchange_Currency> Exchange_Currency { get; set; }
        public DbSet<Exchange_Rate> Exchange_Rate { get; set; }
        public DbSet<Expenses_Application> Expenses_Application { get; set; }
        public DbSet<Expenses_Application_Document> Expenses_Application_Document { get; set; }
        public DbSet<Expenses_Calculation> Expenses_Calculation { get; set; }
        public DbSet<Expenses_Category> Expenses_Category { get; set; }
        public DbSet<Expenses_Config> Expenses_Config { get; set; }
        public DbSet<Expenses_Config_Budget> Expenses_Config_Budget { get; set; }
        public DbSet<Expenses_Config_Detail> Expenses_Config_Detail { get; set; }
        public DbSet<Expenses_No_Pattern> Expenses_No_Pattern { get; set; }
        public DbSet<Global_Lookup_Data> Global_Lookup_Data { get; set; }
        public DbSet<Global_Lookup_Def> Global_Lookup_Def { get; set; }
        public DbSet<Holiday_Config> Holiday_Config { get; set; }
        public DbSet<HR_FileExport_History> HR_FileExport_History { get; set; }
        public DbSet<HR_FileExport_History_Detail> HR_FileExport_History_Detail { get; set; }
        public DbSet<HR_FileExport_IR8A_Detail> HR_FileExport_IR8A_Detail { get; set; }
        public DbSet<HR_FileExport_IR8B_Detail> HR_FileExport_IR8B_Detail { get; set; }
        public DbSet<HR_FileExport_IR8S_Detail> HR_FileExport_IR8S_Detail { get; set; }
        public DbSet<HR_FileExport_IRA8A_Detail> HR_FileExport_IRA8A_Detail { get; set; }
        public DbSet<Invoice_Details> Invoice_Details { get; set; }
        public DbSet<Invoice_Header> Invoice_Header { get; set; }
        public DbSet<IR8A_Bank> IR8A_Bank { get; set; }
        public DbSet<IR8A_Country> IR8A_Country { get; set; }
        public DbSet<IR8A_Nationality> IR8A_Nationality { get; set; }
        public DbSet<Job_Cost> Job_Cost { get; set; }
        public DbSet<Job_Cost_Payment_Term> Job_Cost_Payment_Term { get; set; }
        public DbSet<Leave_Adjustment> Leave_Adjustment { get; set; }
        public DbSet<Leave_Application_Document> Leave_Application_Document { get; set; }
        public DbSet<Leave_Calculation> Leave_Calculation { get; set; }
        public DbSet<Leave_Config> Leave_Config { get; set; }
        public DbSet<Leave_Config_Child_Detail> Leave_Config_Child_Detail { get; set; }
        public DbSet<Leave_Config_Condition> Leave_Config_Condition { get; set; }
        public DbSet<Leave_Config_Detail> Leave_Config_Detail { get; set; }
        public DbSet<Leave_Config_Extra> Leave_Config_Extra { get; set; }
        public DbSet<Leave_Default> Leave_Default { get; set; }
        public DbSet<Leave_Default_Child_Detail> Leave_Default_Child_Detail { get; set; }
        public DbSet<Leave_Default_Condition> Leave_Default_Condition { get; set; }
        public DbSet<Leave_Default_Detail> Leave_Default_Detail { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Menu_Page> Menu_Page { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Module_Mapping> Module_Mapping { get; set; }
        public DbSet<Nationality> Nationalities { get; set; }
        public DbSet<Notification_Scheduler> Notification_Scheduler { get; set; }
        public DbSet<OT_Formula> OT_Formula { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Page_Role> Page_Role { get; set; }
        public DbSet<PRAL> PRALs { get; set; }
        public DbSet<PRC> PRCs { get; set; }
        public DbSet<PRC_Department> PRC_Department { get; set; }
        public DbSet<PRD> PRDs { get; set; }
        public DbSet<PRDE> PRDEs { get; set; }
        public DbSet<PRDL> PRDLs { get; set; }
        public DbSet<PREDL> PREDLs { get; set; }
        public DbSet<PREL> PRELs { get; set; }
        public DbSet<PRG> PRGs { get; set; }
        public DbSet<PRM> PRMs { get; set; }
        public DbSet<PRT> PRTs { get; set; }
        public DbSet<Relationship> Relationships { get; set; }
        public DbSet<SBS_Module> SBS_Module { get; set; }
        public DbSet<SBS_Module_Detail> SBS_Module_Detail { get; set; }
        public DbSet<Screen_Capture_Image> Screen_Capture_Image { get; set; }
        public DbSet<Screen_Capture_Log> Screen_Capture_Log { get; set; }
        public DbSet<Selected_CPF_Formula> Selected_CPF_Formula { get; set; }
        public DbSet<Selected_Donation_Formula> Selected_Donation_Formula { get; set; }
        public DbSet<Selected_OT_Formula> Selected_OT_Formula { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Storage_Upgrade> Storage_Upgrade { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<sysdiagram> sysdiagrams { get; set; }
        public DbSet<Time_Sheet> Time_Sheet { get; set; }
        public DbSet<Time_Sheet_Dtl> Time_Sheet_Dtl { get; set; }
        public DbSet<TsEX> TsEXes { get; set; }
        public DbSet<Upload_Document> Upload_Document { get; set; }
        public DbSet<Upload_Receipt> Upload_Receipt { get; set; }
        public DbSet<User_Assign_Module> User_Assign_Module { get; set; }
        public DbSet<User_Assign_Role> User_Assign_Role { get; set; }
        public DbSet<User_Authentication> User_Authentication { get; set; }
        public DbSet<User_Profile> User_Profile { get; set; }
        public DbSet<User_Profile_Photo> User_Profile_Photo { get; set; }
        public DbSet<User_Role> User_Role { get; set; }
        public DbSet<User_Session_Data> User_Session_Data { get; set; }
        public DbSet<User_Transactions> User_Transactions { get; set; }
        public DbSet<Working_Days> Working_Days { get; set; }
        public DbSet<vwQuantityOnHand> vwQuantityOnHands { get; set; }
        public DbSet<Time_Mobile_Map> Time_Mobile_Map { get; set; }
        public DbSet<Time_Mobile_Trans> Time_Mobile_Trans { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new Access_PageMap());
            modelBuilder.Configurations.Add(new Access_RightMap());
            modelBuilder.Configurations.Add(new Activation_LinkMap());
            modelBuilder.Configurations.Add(new Bank_DetailsMap());
            modelBuilder.Configurations.Add(new Banking_InfoMap());
            modelBuilder.Configurations.Add(new BranchMap());
            modelBuilder.Configurations.Add(new CompanyMap());
            modelBuilder.Configurations.Add(new Company_DetailsMap());
            modelBuilder.Configurations.Add(new Company_LogoMap());
            modelBuilder.Configurations.Add(new Company_Mail_ConfigMap());
            modelBuilder.Configurations.Add(new CountryMap());
            modelBuilder.Configurations.Add(new CPF_FormulaMap());
            modelBuilder.Configurations.Add(new Ctl_Table_SynMap());
            modelBuilder.Configurations.Add(new CurrencyMap());
            modelBuilder.Configurations.Add(new CustomerMap());
            modelBuilder.Configurations.Add(new Default_Expense_TypeMap());
            modelBuilder.Configurations.Add(new DepartmentMap());
            modelBuilder.Configurations.Add(new DesignationMap());
            modelBuilder.Configurations.Add(new Donation_FormulaMap());
            modelBuilder.Configurations.Add(new Donation_TypeMap());
            modelBuilder.Configurations.Add(new Email_AttachmentMap());
            modelBuilder.Configurations.Add(new Email_NotificationMap());
            modelBuilder.Configurations.Add(new Employee_AttachmentMap());
            modelBuilder.Configurations.Add(new Employee_Emergency_ContactMap());
            modelBuilder.Configurations.Add(new Employee_No_PatternMap());
            modelBuilder.Configurations.Add(new Employee_ProfileMap());
            modelBuilder.Configurations.Add(new Employment_HistoryMap());
            modelBuilder.Configurations.Add(new Employment_History_AllowanceMap());
            modelBuilder.Configurations.Add(new ETIRA8Map());
            modelBuilder.Configurations.Add(new ExchangeMap());
            modelBuilder.Configurations.Add(new Exchange_CurrencyMap());
            modelBuilder.Configurations.Add(new Exchange_RateMap());
            modelBuilder.Configurations.Add(new Expenses_ApplicationMap());
            modelBuilder.Configurations.Add(new Expenses_Application_DocumentMap());
            modelBuilder.Configurations.Add(new Expenses_CalculationMap());
            modelBuilder.Configurations.Add(new Expenses_CategoryMap());
            modelBuilder.Configurations.Add(new Expenses_ConfigMap());
            modelBuilder.Configurations.Add(new Expenses_Config_BudgetMap());
            modelBuilder.Configurations.Add(new Expenses_Config_DetailMap());
            modelBuilder.Configurations.Add(new Expenses_No_PatternMap());
            modelBuilder.Configurations.Add(new Global_Lookup_DataMap());
            modelBuilder.Configurations.Add(new Global_Lookup_DefMap());
            modelBuilder.Configurations.Add(new Holiday_ConfigMap());
            modelBuilder.Configurations.Add(new HR_FileExport_HistoryMap());
            modelBuilder.Configurations.Add(new HR_FileExport_History_DetailMap());
            modelBuilder.Configurations.Add(new HR_FileExport_IR8A_DetailMap());
            modelBuilder.Configurations.Add(new HR_FileExport_IR8B_DetailMap());
            modelBuilder.Configurations.Add(new HR_FileExport_IR8S_DetailMap());
            modelBuilder.Configurations.Add(new HR_FileExport_IRA8A_DetailMap());
            modelBuilder.Configurations.Add(new Invoice_DetailsMap());
            modelBuilder.Configurations.Add(new Invoice_HeaderMap());
            modelBuilder.Configurations.Add(new IR8A_BankMap());
            modelBuilder.Configurations.Add(new IR8A_CountryMap());
            modelBuilder.Configurations.Add(new IR8A_NationalityMap());
            modelBuilder.Configurations.Add(new Job_CostMap());
            modelBuilder.Configurations.Add(new Job_Cost_Payment_TermMap());
            modelBuilder.Configurations.Add(new Leave_AdjustmentMap());
            modelBuilder.Configurations.Add(new Leave_Application_DocumentMap());
            modelBuilder.Configurations.Add(new Leave_CalculationMap());
            modelBuilder.Configurations.Add(new Leave_ConfigMap());
            modelBuilder.Configurations.Add(new Leave_Config_Child_DetailMap());
            modelBuilder.Configurations.Add(new Leave_Config_ConditionMap());
            modelBuilder.Configurations.Add(new Leave_Config_DetailMap());
            modelBuilder.Configurations.Add(new Leave_Config_ExtraMap());
            modelBuilder.Configurations.Add(new Leave_DefaultMap());
            modelBuilder.Configurations.Add(new Leave_Default_Child_DetailMap());
            modelBuilder.Configurations.Add(new Leave_Default_ConditionMap());
            modelBuilder.Configurations.Add(new Leave_Default_DetailMap());
            modelBuilder.Configurations.Add(new MenuMap());
            modelBuilder.Configurations.Add(new Menu_PageMap());
            modelBuilder.Configurations.Add(new ModuleMap());
            modelBuilder.Configurations.Add(new Module_MappingMap());
            modelBuilder.Configurations.Add(new NationalityMap());
            modelBuilder.Configurations.Add(new Notification_SchedulerMap());
            modelBuilder.Configurations.Add(new OT_FormulaMap());
            modelBuilder.Configurations.Add(new PageMap());
            modelBuilder.Configurations.Add(new Page_RoleMap());
            modelBuilder.Configurations.Add(new PRALMap());
            modelBuilder.Configurations.Add(new PRCMap());
            modelBuilder.Configurations.Add(new PRC_DepartmentMap());
            modelBuilder.Configurations.Add(new PRDMap());
            modelBuilder.Configurations.Add(new PRDEMap());
            modelBuilder.Configurations.Add(new PRDLMap());
            modelBuilder.Configurations.Add(new PREDLMap());
            modelBuilder.Configurations.Add(new PRELMap());
            modelBuilder.Configurations.Add(new PRGMap());
            modelBuilder.Configurations.Add(new PRMMap());
            modelBuilder.Configurations.Add(new PRTMap());
            modelBuilder.Configurations.Add(new RelationshipMap());
            modelBuilder.Configurations.Add(new SBS_ModuleMap());
            modelBuilder.Configurations.Add(new SBS_Module_DetailMap());
            modelBuilder.Configurations.Add(new Screen_Capture_ImageMap());
            modelBuilder.Configurations.Add(new Screen_Capture_LogMap());
            modelBuilder.Configurations.Add(new Selected_CPF_FormulaMap());
            modelBuilder.Configurations.Add(new Selected_Donation_FormulaMap());
            modelBuilder.Configurations.Add(new Selected_OT_FormulaMap());
            modelBuilder.Configurations.Add(new StateMap());
            modelBuilder.Configurations.Add(new Storage_UpgradeMap());
            modelBuilder.Configurations.Add(new SubscriberMap());
            modelBuilder.Configurations.Add(new SubscriptionMap());
            modelBuilder.Configurations.Add(new sysdiagramMap());
            modelBuilder.Configurations.Add(new Time_SheetMap());
            modelBuilder.Configurations.Add(new Time_Sheet_DtlMap());
            modelBuilder.Configurations.Add(new TsEXMap());
            modelBuilder.Configurations.Add(new Upload_DocumentMap());
            modelBuilder.Configurations.Add(new Upload_ReceiptMap());
            modelBuilder.Configurations.Add(new User_Assign_ModuleMap());
            modelBuilder.Configurations.Add(new User_Assign_RoleMap());
            modelBuilder.Configurations.Add(new User_AuthenticationMap());
            modelBuilder.Configurations.Add(new User_ProfileMap());
            modelBuilder.Configurations.Add(new User_Profile_PhotoMap());
            modelBuilder.Configurations.Add(new User_RoleMap());
            modelBuilder.Configurations.Add(new User_Session_DataMap());
            modelBuilder.Configurations.Add(new User_TransactionsMap());
            modelBuilder.Configurations.Add(new Working_DaysMap());
            modelBuilder.Configurations.Add(new vwQuantityOnHandMap());
            modelBuilder.Configurations.Add(new Time_Mobile_MapMap());
            modelBuilder.Configurations.Add(new Time_Mobile_TransMap());
           
        }
    }
}
