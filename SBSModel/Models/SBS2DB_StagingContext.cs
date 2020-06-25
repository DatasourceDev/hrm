using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using SBSModel.Models.Mapping;

namespace SBSModel.Models
{
    public partial class SBS2DB_StagingContext : DbContext
    {
        static SBS2DB_StagingContext()
        {
            Database.SetInitializer<SBS2DB_StagingContext>(null);
        }

        public SBS2DB_StagingContext()
            : base("Name=SBS2DB_StagingContext")
        {
        }

        public DbSet<Access_Page> Access_Page { get; set; }
        public DbSet<Access_Right> Access_Right { get; set; }
        public DbSet<Activation_Link> Activation_Link { get; set; }
        public DbSet<AspNetRole> AspNetRoles { get; set; }
        public DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public DbSet<AspNetUser> AspNetUsers { get; set; }
        public DbSet<Bank_Details> Bank_Details { get; set; }
        public DbSet<Banking_Info> Banking_Info { get; set; }
        public DbSet<Bom> Boms { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Company_Details> Company_Details { get; set; }
        public DbSet<Company_Logo> Company_Logo { get; set; }
        public DbSet<Company_Mail_Config> Company_Mail_Config { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<CPF_Formula> CPF_Formula { get; set; }
        public DbSet<CRM_Details> CRM_Details { get; set; }
        public DbSet<CRM_History> CRM_History { get; set; }
        public DbSet<Ctl_Table_Syn> Ctl_Table_Syn { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Customer_Company> Customer_Company { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Device_Configuration> Device_Configuration { get; set; }
        public DbSet<Donation_Formula> Donation_Formula { get; set; }
        public DbSet<Donation_Type> Donation_Type { get; set; }
        public DbSet<Employee_Attachment> Employee_Attachment { get; set; }
        public DbSet<Employee_Emergency_Contact> Employee_Emergency_Contact { get; set; }
        public DbSet<Employee_Grading> Employee_Grading { get; set; }
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
        public DbSet<Extra_Donation> Extra_Donation { get; set; }
        public DbSet<Global_Lookup_Data> Global_Lookup_Data { get; set; }
        public DbSet<Global_Lookup_Def> Global_Lookup_Def { get; set; }
        public DbSet<Holiday_Config> Holiday_Config { get; set; }
        public DbSet<HR_FileExport_History> HR_FileExport_History { get; set; }
        public DbSet<HR_FileExport_History_Detail> HR_FileExport_History_Detail { get; set; }
        public DbSet<HR_FileExport_IR8A_Detail> HR_FileExport_IR8A_Detail { get; set; }
        public DbSet<HR_FileExport_IR8S_Detail> HR_FileExport_IR8S_Detail { get; set; }
        public DbSet<IA> IAs { get; set; }
        public DbSet<IG> IGs { get; set; }
        public DbSet<Inventory_Location> Inventory_Location { get; set; }
        public DbSet<Inventory_Preferences> Inventory_Preferences { get; set; }
        public DbSet<Inventory_Prefix_Config> Inventory_Prefix_Config { get; set; }
        public DbSet<Inventory_Stock_Quantity> Inventory_Stock_Quantity { get; set; }
        public DbSet<Inventory_Transaction> Inventory_Transaction { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Invoice_has_Product> Invoice_has_Product { get; set; }
        public DbSet<Invoice_has_Quotation> Invoice_has_Quotation { get; set; }
        public DbSet<Invoice_Payment> Invoice_Payment { get; set; }
        public DbSet<Invoice_Payment_Detail> Invoice_Payment_Detail { get; set; }
        public DbSet<IR> IRs { get; set; }
        public DbSet<IR8A_Bank> IR8A_Bank { get; set; }
        public DbSet<IR8A_Country> IR8A_Country { get; set; }
        public DbSet<IR8A_Nationality> IR8A_Nationality { get; set; }
        public DbSet<Job_Cost> Job_Cost { get; set; }
        public DbSet<Job_Cost_Payment_Term> Job_Cost_Payment_Term { get; set; }
        public DbSet<Kit> Kits { get; set; }
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
        public DbSet<Location_Authorize> Location_Authorize { get; set; }
        public DbSet<Measurement> Measurements { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Member_Configuration> Member_Configuration { get; set; }
        public DbSet<Member_Group> Member_Group { get; set; }
        public DbSet<Membership_Level> Membership_Level { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Menu_Assign_Module> Menu_Assign_Module { get; set; }
        public DbSet<Menu_Page> Menu_Page { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Module_Detail> Module_Detail { get; set; }
        public DbSet<Nationality> Nationalities { get; set; }
        public DbSet<Notification_Scheduler> Notification_Scheduler { get; set; }
        public DbSet<OT_Formula> OT_Formula { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Page_Role> Page_Role { get; set; }
        public DbSet<PO_Payment> PO_Payment { get; set; }
        public DbSet<POS_Message_Layout> POS_Message_Layout { get; set; }
        public DbSet<POS_Products_Rcp> POS_Products_Rcp { get; set; }
        public DbSet<POS_Receipt> POS_Receipt { get; set; }
        public DbSet<POS_Receipt_Configuration> POS_Receipt_Configuration { get; set; }
        public DbSet<POS_Receipt_Payment> POS_Receipt_Payment { get; set; }
        public DbSet<POS_Shift> POS_Shift { get; set; }
        public DbSet<POS_Terminal> POS_Terminal { get; set; }
        public DbSet<PRAL> PRALs { get; set; }
        public DbSet<PRC> PRCs { get; set; }
        public DbSet<PRC_Department> PRC_Department { get; set; }
        public DbSet<PRD> PRDs { get; set; }
        public DbSet<PRDE> PRDEs { get; set; }
        public DbSet<PRDL> PRDLs { get; set; }
        public DbSet<PREDL> PREDLs { get; set; }
        public DbSet<PREL> PRELs { get; set; }
        public DbSet<PRG> PRGs { get; set; }
        public DbSet<Pricing_Preferences> Pricing_Preferences { get; set; }
        public DbSet<PRM> PRMs { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Product_Attribute> Product_Attribute { get; set; }
        public DbSet<Product_Attribute_Bak> Product_Attribute_Bak { get; set; }
        public DbSet<Product_Attribute_Map> Product_Attribute_Map { get; set; }
        public DbSet<Product_Attribute_Map_Price> Product_Attribute_Map_Price { get; set; }
        public DbSet<Product_Attribute_Value> Product_Attribute_Value { get; set; }
        public DbSet<Product_Category> Product_Category { get; set; }
        public DbSet<Product_Color> Product_Color { get; set; }
        public DbSet<Product_Image> Product_Image { get; set; }
        public DbSet<Product_Image_Attribute> Product_Image_Attribute { get; set; }
        public DbSet<Product_Preferences> Product_Preferences { get; set; }
        public DbSet<Product_Price> Product_Price { get; set; }
        public DbSet<Product_RelatedProduct> Product_RelatedProduct { get; set; }
        public DbSet<Product_Size> Product_Size { get; set; }
        public DbSet<Product_Table> Product_Table { get; set; }
        public DbSet<Product_Tag> Product_Tag { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Promotion_Branch> Promotion_Branch { get; set; }
        public DbSet<Promotion_Codes> Promotion_Codes { get; set; }
        public DbSet<Promotion_Product> Promotion_Product { get; set; }
        public DbSet<Promotion_Spacial> Promotion_Spacial { get; set; }
        public DbSet<PRT> PRTs { get; set; }
        public DbSet<Purchase_Order> Purchase_Order { get; set; }
        public DbSet<Purchase_Order_Approval> Purchase_Order_Approval { get; set; }
        public DbSet<Purchase_Order_has_Vendor_Product> Purchase_Order_has_Vendor_Product { get; set; }
        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<Quotation_Approval> Quotation_Approval { get; set; }
        public DbSet<Quotation_Delegation> Quotation_Delegation { get; set; }
        public DbSet<Quotation_has_Product> Quotation_has_Product { get; set; }
        public DbSet<Quotation_Payment_Term> Quotation_Payment_Term { get; set; }
        public DbSet<QuotationTemplate> QuotationTemplates { get; set; }
        public DbSet<Receive> Receives { get; set; }
        public DbSet<Relationship> Relationships { get; set; }
        public DbSet<Return> Returns { get; set; }
        public DbSet<Sale_Order> Sale_Order { get; set; }
        public DbSet<Sale_Order_Approval> Sale_Order_Approval { get; set; }
        public DbSet<Sale_Order_has_Product> Sale_Order_has_Product { get; set; }
        public DbSet<SBS_Module> SBS_Module { get; set; }
        public DbSet<SBS_Module_Detail> SBS_Module_Detail { get; set; }
        public DbSet<SBS_No_Pattern> SBS_No_Pattern { get; set; }
        public DbSet<Screen_Capture_Image> Screen_Capture_Image { get; set; }
        public DbSet<Screen_Capture_Log> Screen_Capture_Log { get; set; }
        public DbSet<Selected_CPF_Formula> Selected_CPF_Formula { get; set; }
        public DbSet<Selected_Donation_Formula> Selected_Donation_Formula { get; set; }
        public DbSet<Selected_OT_Formula> Selected_OT_Formula { get; set; }
        public DbSet<SO_Return> SO_Return { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<sysdiagram> sysdiagrams { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Tax> Taxes { get; set; }
        public DbSet<Tax_GST> Tax_GST { get; set; }
        public DbSet<Tax_Scheme> Tax_Scheme { get; set; }
        public DbSet<Unit_Of_Measurement> Unit_Of_Measurement { get; set; }
        public DbSet<Upload_Document> Upload_Document { get; set; }
        public DbSet<Upload_Receipt> Upload_Receipt { get; set; }
        public DbSet<User_Assign_Module> User_Assign_Module { get; set; }
        public DbSet<User_Assign_Role> User_Assign_Role { get; set; }
        public DbSet<User_Authentication> User_Authentication { get; set; }
        public DbSet<User_Profile> User_Profile { get; set; }
        public DbSet<User_Profile_Photo> User_Profile_Photo { get; set; }
        public DbSet<User_Role> User_Role { get; set; }
        public DbSet<User_Session_Data> User_Session_Data { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Withdraw> Withdraws { get; set; }
        public DbSet<Working_Days> Working_Days { get; set; }
        public DbSet<vwQuantityOnHand> vwQuantityOnHands { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new Access_PageMap());
            modelBuilder.Configurations.Add(new Access_RightMap());
            modelBuilder.Configurations.Add(new Activation_LinkMap());
            modelBuilder.Configurations.Add(new AspNetRoleMap());
            modelBuilder.Configurations.Add(new AspNetUserClaimMap());
            modelBuilder.Configurations.Add(new AspNetUserLoginMap());
            modelBuilder.Configurations.Add(new AspNetUserMap());
            modelBuilder.Configurations.Add(new Bank_DetailsMap());
            modelBuilder.Configurations.Add(new Banking_InfoMap());
            modelBuilder.Configurations.Add(new BomMap());
            modelBuilder.Configurations.Add(new BranchMap());
            modelBuilder.Configurations.Add(new BrandMap());
            modelBuilder.Configurations.Add(new CompanyMap());
            modelBuilder.Configurations.Add(new Company_DetailsMap());
            modelBuilder.Configurations.Add(new Company_LogoMap());
            modelBuilder.Configurations.Add(new Company_Mail_ConfigMap());
            modelBuilder.Configurations.Add(new ContactMap());
            modelBuilder.Configurations.Add(new CountryMap());
            modelBuilder.Configurations.Add(new CPF_FormulaMap());
            modelBuilder.Configurations.Add(new CRM_DetailsMap());
            modelBuilder.Configurations.Add(new CRM_HistoryMap());
            modelBuilder.Configurations.Add(new Ctl_Table_SynMap());
            modelBuilder.Configurations.Add(new CurrencyMap());
            modelBuilder.Configurations.Add(new CustomerMap());
            modelBuilder.Configurations.Add(new Customer_CompanyMap());
            modelBuilder.Configurations.Add(new DepartmentMap());
            modelBuilder.Configurations.Add(new DesignationMap());
            modelBuilder.Configurations.Add(new Device_ConfigurationMap());
            modelBuilder.Configurations.Add(new Donation_FormulaMap());
            modelBuilder.Configurations.Add(new Donation_TypeMap());
            modelBuilder.Configurations.Add(new Employee_AttachmentMap());
            modelBuilder.Configurations.Add(new Employee_Emergency_ContactMap());
            modelBuilder.Configurations.Add(new Employee_GradingMap());
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
            modelBuilder.Configurations.Add(new Extra_DonationMap());
            modelBuilder.Configurations.Add(new Global_Lookup_DataMap());
            modelBuilder.Configurations.Add(new Global_Lookup_DefMap());
            modelBuilder.Configurations.Add(new Holiday_ConfigMap());
            modelBuilder.Configurations.Add(new HR_FileExport_HistoryMap());
            modelBuilder.Configurations.Add(new HR_FileExport_History_DetailMap());
            modelBuilder.Configurations.Add(new HR_FileExport_IR8A_DetailMap());
            modelBuilder.Configurations.Add(new HR_FileExport_IR8S_DetailMap());
            modelBuilder.Configurations.Add(new IAMap());
            modelBuilder.Configurations.Add(new IGMap());
            modelBuilder.Configurations.Add(new Inventory_LocationMap());
            modelBuilder.Configurations.Add(new Inventory_PreferencesMap());
            modelBuilder.Configurations.Add(new Inventory_Prefix_ConfigMap());
            modelBuilder.Configurations.Add(new Inventory_Stock_QuantityMap());
            modelBuilder.Configurations.Add(new Inventory_TransactionMap());
            modelBuilder.Configurations.Add(new InvoiceMap());
            modelBuilder.Configurations.Add(new Invoice_has_ProductMap());
            modelBuilder.Configurations.Add(new Invoice_has_QuotationMap());
            modelBuilder.Configurations.Add(new Invoice_PaymentMap());
            modelBuilder.Configurations.Add(new Invoice_Payment_DetailMap());
            modelBuilder.Configurations.Add(new IRMap());
            modelBuilder.Configurations.Add(new IR8A_BankMap());
            modelBuilder.Configurations.Add(new IR8A_CountryMap());
            modelBuilder.Configurations.Add(new IR8A_NationalityMap());
            modelBuilder.Configurations.Add(new Job_CostMap());
            modelBuilder.Configurations.Add(new Job_Cost_Payment_TermMap());
            modelBuilder.Configurations.Add(new KitMap());
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
            modelBuilder.Configurations.Add(new Location_AuthorizeMap());
            modelBuilder.Configurations.Add(new MeasurementMap());
            modelBuilder.Configurations.Add(new MemberMap());
            modelBuilder.Configurations.Add(new Member_ConfigurationMap());
            modelBuilder.Configurations.Add(new Member_GroupMap());
            modelBuilder.Configurations.Add(new Membership_LevelMap());
            modelBuilder.Configurations.Add(new MenuMap());
            modelBuilder.Configurations.Add(new Menu_Assign_ModuleMap());
            modelBuilder.Configurations.Add(new Menu_PageMap());
            modelBuilder.Configurations.Add(new ModuleMap());
            modelBuilder.Configurations.Add(new Module_DetailMap());
            modelBuilder.Configurations.Add(new NationalityMap());
            modelBuilder.Configurations.Add(new Notification_SchedulerMap());
            modelBuilder.Configurations.Add(new OT_FormulaMap());
            modelBuilder.Configurations.Add(new PageMap());
            modelBuilder.Configurations.Add(new Page_RoleMap());
            modelBuilder.Configurations.Add(new PO_PaymentMap());
            modelBuilder.Configurations.Add(new POS_Message_LayoutMap());
            modelBuilder.Configurations.Add(new POS_Products_RcpMap());
            modelBuilder.Configurations.Add(new POS_ReceiptMap());
            modelBuilder.Configurations.Add(new POS_Receipt_ConfigurationMap());
            modelBuilder.Configurations.Add(new POS_Receipt_PaymentMap());
            modelBuilder.Configurations.Add(new POS_ShiftMap());
            modelBuilder.Configurations.Add(new POS_TerminalMap());
            modelBuilder.Configurations.Add(new PRALMap());
            modelBuilder.Configurations.Add(new PRCMap());
            modelBuilder.Configurations.Add(new PRC_DepartmentMap());
            modelBuilder.Configurations.Add(new PRDMap());
            modelBuilder.Configurations.Add(new PRDEMap());
            modelBuilder.Configurations.Add(new PRDLMap());
            modelBuilder.Configurations.Add(new PREDLMap());
            modelBuilder.Configurations.Add(new PRELMap());
            modelBuilder.Configurations.Add(new PRGMap());
            modelBuilder.Configurations.Add(new Pricing_PreferencesMap());
            modelBuilder.Configurations.Add(new PRMMap());
            modelBuilder.Configurations.Add(new ProductMap());
            modelBuilder.Configurations.Add(new Product_AttributeMap());
            modelBuilder.Configurations.Add(new Product_Attribute_BakMap());
            modelBuilder.Configurations.Add(new Product_Attribute_MapMap());
            modelBuilder.Configurations.Add(new Product_Attribute_Map_PriceMap());
            modelBuilder.Configurations.Add(new Product_Attribute_ValueMap());
            modelBuilder.Configurations.Add(new Product_CategoryMap());
            modelBuilder.Configurations.Add(new Product_ColorMap());
            modelBuilder.Configurations.Add(new Product_ImageMap());
            modelBuilder.Configurations.Add(new Product_Image_AttributeMap());
            modelBuilder.Configurations.Add(new Product_PreferencesMap());
            modelBuilder.Configurations.Add(new Product_PriceMap());
            modelBuilder.Configurations.Add(new Product_RelatedProductMap());
            modelBuilder.Configurations.Add(new Product_SizeMap());
            modelBuilder.Configurations.Add(new Product_TableMap());
            modelBuilder.Configurations.Add(new Product_TagMap());
            modelBuilder.Configurations.Add(new PromotionMap());
            modelBuilder.Configurations.Add(new Promotion_BranchMap());
            modelBuilder.Configurations.Add(new Promotion_CodesMap());
            modelBuilder.Configurations.Add(new Promotion_ProductMap());
            modelBuilder.Configurations.Add(new Promotion_SpacialMap());
            modelBuilder.Configurations.Add(new PRTMap());
            modelBuilder.Configurations.Add(new Purchase_OrderMap());
            modelBuilder.Configurations.Add(new Purchase_Order_ApprovalMap());
            modelBuilder.Configurations.Add(new Purchase_Order_has_Vendor_ProductMap());
            modelBuilder.Configurations.Add(new QuotationMap());
            modelBuilder.Configurations.Add(new Quotation_ApprovalMap());
            modelBuilder.Configurations.Add(new Quotation_DelegationMap());
            modelBuilder.Configurations.Add(new Quotation_has_ProductMap());
            modelBuilder.Configurations.Add(new Quotation_Payment_TermMap());
            modelBuilder.Configurations.Add(new QuotationTemplateMap());
            modelBuilder.Configurations.Add(new ReceiveMap());
            modelBuilder.Configurations.Add(new RelationshipMap());
            modelBuilder.Configurations.Add(new ReturnMap());
            modelBuilder.Configurations.Add(new Sale_OrderMap());
            modelBuilder.Configurations.Add(new Sale_Order_ApprovalMap());
            modelBuilder.Configurations.Add(new Sale_Order_has_ProductMap());
            modelBuilder.Configurations.Add(new SBS_ModuleMap());
            modelBuilder.Configurations.Add(new SBS_Module_DetailMap());
            modelBuilder.Configurations.Add(new SBS_No_PatternMap());
            modelBuilder.Configurations.Add(new Screen_Capture_ImageMap());
            modelBuilder.Configurations.Add(new Screen_Capture_LogMap());
            modelBuilder.Configurations.Add(new Selected_CPF_FormulaMap());
            modelBuilder.Configurations.Add(new Selected_Donation_FormulaMap());
            modelBuilder.Configurations.Add(new Selected_OT_FormulaMap());
            modelBuilder.Configurations.Add(new SO_ReturnMap());
            modelBuilder.Configurations.Add(new StateMap());
            modelBuilder.Configurations.Add(new SubscriptionMap());
            modelBuilder.Configurations.Add(new sysdiagramMap());
            modelBuilder.Configurations.Add(new TagMap());
            modelBuilder.Configurations.Add(new TaxMap());
            modelBuilder.Configurations.Add(new Tax_GSTMap());
            modelBuilder.Configurations.Add(new Tax_SchemeMap());
            modelBuilder.Configurations.Add(new Unit_Of_MeasurementMap());
            modelBuilder.Configurations.Add(new Upload_DocumentMap());
            modelBuilder.Configurations.Add(new Upload_ReceiptMap());
            modelBuilder.Configurations.Add(new User_Assign_ModuleMap());
            modelBuilder.Configurations.Add(new User_Assign_RoleMap());
            modelBuilder.Configurations.Add(new User_AuthenticationMap());
            modelBuilder.Configurations.Add(new User_ProfileMap());
            modelBuilder.Configurations.Add(new User_Profile_PhotoMap());
            modelBuilder.Configurations.Add(new User_RoleMap());
            modelBuilder.Configurations.Add(new User_Session_DataMap());
            modelBuilder.Configurations.Add(new VendorMap());
            modelBuilder.Configurations.Add(new WithdrawMap());
            modelBuilder.Configurations.Add(new Working_DaysMap());
            modelBuilder.Configurations.Add(new vwQuantityOnHandMap());
        }
    }
}
