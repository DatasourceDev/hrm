using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Company_DetailsMap : EntityTypeConfiguration<Company_Details>
    {
        public Company_DetailsMap()
        {
            // Primary Key
            this.HasKey(t => t.Company_Detail_ID);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(300);

            this.Property(t => t.Phone)
                .HasMaxLength(100);

            this.Property(t => t.Address)
                .HasMaxLength(500);

            this.Property(t => t.Billing_Address)
                .HasMaxLength(500);

            this.Property(t => t.Email)
                .HasMaxLength(50);

            this.Property(t => t.Website)
                .HasMaxLength(200);

            this.Property(t => t.Fax)
                .HasMaxLength(100);

            this.Property(t => t.Registry)
                .HasMaxLength(150);

            this.Property(t => t.Zip_Code)
                .HasMaxLength(10);

            this.Property(t => t.Billing_Zip_Code)
                .HasMaxLength(10);

            this.Property(t => t.Billing_Tagline)
                .HasMaxLength(150);

            this.Property(t => t.Report_Footer)
                .HasMaxLength(500);

            this.Property(t => t.Tax_No)
                .HasMaxLength(100);

            this.Property(t => t.Company_Status)
                .HasMaxLength(50);

            this.Property(t => t.Belong_To)
                .HasMaxLength(1000);

            this.Property(t => t.Tagline)
                .HasMaxLength(150);

            this.Property(t => t.URL)
                .HasMaxLength(300);

            this.Property(t => t.Business_Type)
                .HasMaxLength(50);

            this.Property(t => t.GST_Registration)
                .HasMaxLength(150);

            this.Property(t => t.APIUsername)
                .HasMaxLength(300);

            this.Property(t => t.APIPassword)
                .HasMaxLength(300);

            this.Property(t => t.APISignature)
                .HasMaxLength(500);

            this.Property(t => t.Company_Level)
                .HasMaxLength(100);

            this.Property(t => t.CPF_Submission_No)
                .HasMaxLength(100);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.patUser_ID)
                .HasMaxLength(10);

            this.Property(t => t.patPassword)
                .HasMaxLength(20);

            this.Property(t => t.Company_Source)
                .HasMaxLength(1);

            this.Property(t => t.PayerID_Type)
                .HasMaxLength(1);

            this.Property(t => t.PayerID_No)
                .HasMaxLength(20);

            this.Property(t => t.A7_Group_ID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Company_Details");
            this.Property(t => t.Company_Detail_ID).HasColumnName("Company_Detail_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Phone).HasColumnName("Phone");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.Billing_Address).HasColumnName("Billing_Address");
            this.Property(t => t.Country_ID).HasColumnName("Country_ID");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Website).HasColumnName("Website");
            this.Property(t => t.Fax).HasColumnName("Fax");
            this.Property(t => t.State_ID).HasColumnName("State_ID");
            this.Property(t => t.Registry).HasColumnName("Registry");
            this.Property(t => t.Zip_Code).HasColumnName("Zip_Code");
            this.Property(t => t.Is_Billing_same).HasColumnName("Is_Billing_same");
            this.Property(t => t.Billing_Country_ID).HasColumnName("Billing_Country_ID");
            this.Property(t => t.Billing_State_ID).HasColumnName("Billing_State_ID");
            this.Property(t => t.Billing_Zip_Code).HasColumnName("Billing_Zip_Code");
            this.Property(t => t.Billing_Tagline).HasColumnName("Billing_Tagline");
            this.Property(t => t.Report_Footer).HasColumnName("Report_Footer");
            this.Property(t => t.Currency_ID).HasColumnName("Currency_ID");
            this.Property(t => t.Effective_Date).HasColumnName("Effective_Date");
            this.Property(t => t.Tax_No).HasColumnName("Tax_No");
            this.Property(t => t.Schduler_Range_Days).HasColumnName("Schduler_Range_Days");
            this.Property(t => t.Purchase_Lead_Time).HasColumnName("Purchase_Lead_Time");
            this.Property(t => t.Security_Days).HasColumnName("Security_Days");
            this.Property(t => t.Company_Status).HasColumnName("Company_Status");
            this.Property(t => t.Belong_To).HasColumnName("Belong_To");
            this.Property(t => t.Tagline).HasColumnName("Tagline");
            this.Property(t => t.Registration_Date).HasColumnName("Registration_Date");
            this.Property(t => t.Belong_To_ID).HasColumnName("Belong_To_ID");
            this.Property(t => t.URL).HasColumnName("URL");
            this.Property(t => t.Business_Type).HasColumnName("Business_Type");
            this.Property(t => t.GST_Registration).HasColumnName("GST_Registration");
            this.Property(t => t.Default_Fiscal_Year).HasColumnName("Default_Fiscal_Year");
            this.Property(t => t.Custom_Fiscal_Year).HasColumnName("Custom_Fiscal_Year");
            this.Property(t => t.APIUsername).HasColumnName("APIUsername");
            this.Property(t => t.APIPassword).HasColumnName("APIPassword");
            this.Property(t => t.APISignature).HasColumnName("APISignature");
            this.Property(t => t.No_Of_Employees).HasColumnName("No_Of_Employees");
            this.Property(t => t.Company_Level).HasColumnName("Company_Level");
            this.Property(t => t.CPF_Submission_No).HasColumnName("CPF_Submission_No");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.patUser_ID).HasColumnName("patUser_ID");
            this.Property(t => t.patPassword).HasColumnName("patPassword");
            this.Property(t => t.Company_Source).HasColumnName("Company_Source");
            this.Property(t => t.PayerID_Type).HasColumnName("PayerID_Type");
            this.Property(t => t.PayerID_No).HasColumnName("PayerID_No");
            this.Property(t => t.Branch_ID).HasColumnName("Branch_ID");
            this.Property(t => t.Is_Sandbox).HasColumnName("Is_Sandbox");
            this.Property(t => t.A7_Group_ID).HasColumnName("A7_Group_ID");
            this.Property(t => t.Is_PostPaid).HasColumnName("Is_PostPaid");
            this.Property(t => t.Leave_Start_Date).HasColumnName("Leave_Start_Date");
            this.Property(t => t.Is_Indent).HasColumnName("Is_Indent");

            // Relationships
            this.HasRequired(t => t.Company)
                .WithMany(t => t.Company_Details)
                .HasForeignKey(d => d.Company_ID);
            this.HasOptional(t => t.Company1)
                .WithMany(t => t.Company_Details1)
                .HasForeignKey(d => d.Belong_To_ID);
            this.HasOptional(t => t.Country)
                .WithMany(t => t.Company_Details)
                .HasForeignKey(d => d.Billing_Country_ID);
            this.HasOptional(t => t.State)
                .WithMany(t => t.Company_Details)
                .HasForeignKey(d => d.Billing_State_ID);
            this.HasOptional(t => t.Country1)
                .WithMany(t => t.Company_Details1)
                .HasForeignKey(d => d.Country_ID);
            this.HasOptional(t => t.Currency)
                .WithMany(t => t.Company_Details)
                .HasForeignKey(d => d.Currency_ID);
            this.HasOptional(t => t.State1)
                .WithMany(t => t.Company_Details1)
                .HasForeignKey(d => d.State_ID);

        }
    }
}
