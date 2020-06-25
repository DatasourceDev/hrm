using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class PRMMap : EntityTypeConfiguration<PRM>
    {
        public PRMMap()
        {
            // Primary Key
            this.HasKey(t => t.PRM_ID);

            // Properties
            this.Property(t => t.Process_Status)
                .HasMaxLength(10);

            this.Property(t => t.Cheque_No)
                .HasMaxLength(150);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("PRM");
            this.Property(t => t.PRM_ID).HasColumnName("PRM_ID");
            this.Property(t => t.Selected_OT_Formula_ID).HasColumnName("Selected_OT_Formula_ID");
            this.Property(t => t.Selected_CPF_Formula_ID).HasColumnName("Selected_CPF_Formula_ID");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Process_Status).HasColumnName("Process_Status");
            this.Property(t => t.Process_Month).HasColumnName("Process_Month");
            this.Property(t => t.Process_Year).HasColumnName("Process_Year");
            this.Property(t => t.Run_date).HasColumnName("Run_date");
            this.Property(t => t.Total_Allowance).HasColumnName("Total_Allowance");
            this.Property(t => t.Total_Deduction).HasColumnName("Total_Deduction");
            this.Property(t => t.Total_Work_Days).HasColumnName("Total_Work_Days");
            this.Property(t => t.CPF_Employee).HasColumnName("CPF_Employee");
            this.Property(t => t.CPF_Emplyer).HasColumnName("CPF_Emplyer");
            this.Property(t => t.Gross_Wages).HasColumnName("Gross_Wages");
            this.Property(t => t.Nett_Wages).HasColumnName("Nett_Wages");
            this.Property(t => t.Leave_Period_From).HasColumnName("Leave_Period_From");
            this.Property(t => t.Leave_Period_to).HasColumnName("Leave_Period_to");
            this.Property(t => t.Expenses_Period_From).HasColumnName("Expenses_Period_From");
            this.Property(t => t.Expenses_Period_to).HasColumnName("Expenses_Period_to");
            this.Property(t => t.Cheque_No).HasColumnName("Cheque_No");
            this.Property(t => t.Bonus_Issue).HasColumnName("Bonus_Issue");
            this.Property(t => t.Total_Bonus).HasColumnName("Total_Bonus");
            this.Property(t => t.Director_Fee_Issue).HasColumnName("Director_Fee_Issue");
            this.Property(t => t.Total_Director_Fee).HasColumnName("Total_Director_Fee");
            this.Property(t => t.Basic_Salary).HasColumnName("Basic_Salary");
            this.Property(t => t.Total_Extra_Donation).HasColumnName("Total_Extra_Donation");
            this.Property(t => t.Donation).HasColumnName("Donation");
            this.Property(t => t.Selected_Donation_Formula_ID).HasColumnName("Selected_Donation_Formula_ID");
            this.Property(t => t.Payment_Type).HasColumnName("Payment_Type");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.No_Of_Hours).HasColumnName("No_Of_Hours");
            this.Property(t => t.Revision_No).HasColumnName("Revision_No");
            this.Property(t => t.Process_Date_From).HasColumnName("Process_Date_From");
            this.Property(t => t.Process_Date_To).HasColumnName("Process_Date_To");
            this.Property(t => t.Hourly_Rate).HasColumnName("Hourly_Rate");
            this.Property(t => t.Total_Allowance_Basic_Salary).HasColumnName("Total_Allowance_Basic_Salary");

            // Relationships
            this.HasRequired(t => t.Employee_Profile)
                .WithMany(t => t.PRMs)
                .HasForeignKey(d => d.Employee_Profile_ID);
            this.HasOptional(t => t.Global_Lookup_Data)
                .WithMany(t => t.PRMs)
                .HasForeignKey(d => d.Payment_Type);
            this.HasOptional(t => t.Selected_CPF_Formula)
                .WithMany(t => t.PRMs)
                .HasForeignKey(d => d.Selected_CPF_Formula_ID);
            this.HasOptional(t => t.Selected_Donation_Formula)
                .WithMany(t => t.PRMs)
                .HasForeignKey(d => d.Selected_Donation_Formula_ID);
            this.HasOptional(t => t.Selected_OT_Formula)
                .WithMany(t => t.PRMs)
                .HasForeignKey(d => d.Selected_OT_Formula_ID);

        }
    }
}
