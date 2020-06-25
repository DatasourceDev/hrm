using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class ETIRA8Map : EntityTypeConfiguration<ETIRA8>
    {
        public ETIRA8Map()
        {
            // Primary Key
            this.HasKey(t => t.ETIRA8_ID);

            // Properties
            this.Property(t => t.P_YEAR)
                .HasMaxLength(4);

            this.Property(t => t.Nature)
                .HasMaxLength(200);

            this.Property(t => t.Reason_Payment)
                .HasMaxLength(300);

            this.Property(t => t.Basis_Payment)
                .HasMaxLength(300);

            this.Property(t => t.Retirement_Pension)
                .HasMaxLength(300);

            this.Property(t => t.Name_CPF)
                .HasMaxLength(300);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.Contribution_With_Name)
                .HasMaxLength(300);

            this.Property(t => t.Bank_Name)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("ETIRA8");
            this.Property(t => t.ETIRA8_ID).HasColumnName("ETIRA8_ID");
            this.Property(t => t.Company_Running_ID).HasColumnName("Company_Running_ID");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.P_YEAR).HasColumnName("P_YEAR");
            this.Property(t => t.Date_of_Cessation).HasColumnName("Date_of_Cessation");
            this.Property(t => t.Gross_Salary).HasColumnName("Gross_Salary");
            this.Property(t => t.Bonus).HasColumnName("Bonus");
            this.Property(t => t.Director_Fee).HasColumnName("Director_Fee");
            this.Property(t => t.Allowance_Transport).HasColumnName("Allowance_Transport");
            this.Property(t => t.Allowance_Entertain).HasColumnName("Allowance_Entertain");
            this.Property(t => t.Allowance_Others).HasColumnName("Allowance_Others");
            this.Property(t => t.Commission_Start).HasColumnName("Commission_Start");
            this.Property(t => t.Commission_End).HasColumnName("Commission_End");
            this.Property(t => t.Commission_Amount).HasColumnName("Commission_Amount");
            this.Property(t => t.Commission_Type).HasColumnName("Commission_Type");
            this.Property(t => t.Pension).HasColumnName("Pension");
            this.Property(t => t.Gratuity).HasColumnName("Gratuity");
            this.Property(t => t.Notice_Pay).HasColumnName("Notice_Pay");
            this.Property(t => t.Ex_Gratia).HasColumnName("Ex_Gratia");
            this.Property(t => t.Lump_Sum_Others).HasColumnName("Lump_Sum_Others");
            this.Property(t => t.Nature).HasColumnName("Nature");
            this.Property(t => t.Compensation_Loss).HasColumnName("Compensation_Loss");
            this.Property(t => t.Approval_IRAS).HasColumnName("Approval_IRAS");
            this.Property(t => t.Date_Approval).HasColumnName("Date_Approval");
            this.Property(t => t.Reason_Payment).HasColumnName("Reason_Payment");
            this.Property(t => t.Length_Service).HasColumnName("Length_Service");
            this.Property(t => t.Basis_Payment).HasColumnName("Basis_Payment");
            this.Property(t => t.Total_Lump_Sum).HasColumnName("Total_Lump_Sum");
            this.Property(t => t.Retirement_Pension).HasColumnName("Retirement_Pension");
            this.Property(t => t.Amount_Accured_1992).HasColumnName("Amount_Accured_1992");
            this.Property(t => t.Amount_Accured_1993).HasColumnName("Amount_Accured_1993");
            this.Property(t => t.Contribution_Out_Singapore).HasColumnName("Contribution_Out_Singapore");
            this.Property(t => t.Excess_Employer).HasColumnName("Excess_Employer");
            this.Property(t => t.Excess_Less).HasColumnName("Excess_Less");
            this.Property(t => t.Gain_Profit).HasColumnName("Gain_Profit");
            this.Property(t => t.Value_Benefit).HasColumnName("Value_Benefit");
            this.Property(t => t.Income_Tax_Employer).HasColumnName("Income_Tax_Employer");
            this.Property(t => t.Tax_Partially).HasColumnName("Tax_Partially");
            this.Property(t => t.Tax_Fixed).HasColumnName("Tax_Fixed");
            this.Property(t => t.Employee_CPF).HasColumnName("Employee_CPF");
            this.Property(t => t.Name_CPF).HasColumnName("Name_CPF");
            this.Property(t => t.Donation).HasColumnName("Donation");
            this.Property(t => t.Contribution_Mosque).HasColumnName("Contribution_Mosque");
            this.Property(t => t.Life_Insurance).HasColumnName("Life_Insurance");
            this.Property(t => t.Yayasan_Mendaki_Fund).HasColumnName("Yayasan_Mendaki_Fund");
            this.Property(t => t.Community_chest_of_Singapore).HasColumnName("Community_chest_of_Singapore");
            this.Property(t => t.SINDA).HasColumnName("SINDA");
            this.Property(t => t.CDAC).HasColumnName("CDAC");
            this.Property(t => t.ECF).HasColumnName("ECF");
            this.Property(t => t.Other_tax).HasColumnName("Other_tax");
            this.Property(t => t.Employer).HasColumnName("Employer");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Contribution_With_Singapore).HasColumnName("Contribution_With_Singapore");
            this.Property(t => t.Contribution_With_Name).HasColumnName("Contribution_With_Name");
            this.Property(t => t.Contribution_With_Madatory).HasColumnName("Contribution_With_Madatory");
            this.Property(t => t.Contribution_With_Establishment).HasColumnName("Contribution_With_Establishment");
            this.Property(t => t.Remission_Income).HasColumnName("Remission_Income");
            this.Property(t => t.Non_Tax_Income).HasColumnName("Non_Tax_Income");
            this.Property(t => t.Bank_Name).HasColumnName("Bank_Name");

            // Relationships
            this.HasOptional(t => t.Company)
                .WithMany(t => t.ETIRA8)
                .HasForeignKey(d => d.Company_Running_ID);
            this.HasOptional(t => t.Employee_Profile)
                .WithMany(t => t.ETIRA8)
                .HasForeignKey(d => d.Employee_Profile_ID);
            this.HasOptional(t => t.User_Profile)
                .WithMany(t => t.ETIRA8)
                .HasForeignKey(d => d.Employer);

        }
    }
}
