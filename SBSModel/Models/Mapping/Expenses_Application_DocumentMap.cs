using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Expenses_Application_DocumentMap : EntityTypeConfiguration<Expenses_Application_Document>
    {
        public Expenses_Application_DocumentMap()
        {
            // Primary Key
            this.HasKey(t => t.Expenses_Application_Document_ID);

            // Properties
            this.Property(t => t.Reasons)
                .HasMaxLength(300);

            this.Property(t => t.Remarks)
                .HasMaxLength(300);

            this.Property(t => t.Overall_Status)
                .HasMaxLength(30);

            this.Property(t => t.Approval_Status_1st)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.Approval_Status_2st)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.Approval_Cancel_Status)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.Payroll_Flag)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.Doc_No)
                .HasMaxLength(300);

            this.Property(t => t.Tax_Type)
                .HasMaxLength(20);

            this.Property(t => t.Tax_Amount_Type)
                .HasMaxLength(10);

            this.Property(t => t.Withholding_Tax_Type)
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("Expenses_Application_Document");
            this.Property(t => t.Expenses_Application_Document_ID).HasColumnName("Expenses_Application_Document_ID");
            this.Property(t => t.Expenses_Application_ID).HasColumnName("Expenses_Application_ID");
            this.Property(t => t.Department_ID).HasColumnName("Department_ID");
            this.Property(t => t.Expenses_Config_ID).HasColumnName("Expenses_Config_ID");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Amount_Claiming).HasColumnName("Amount_Claiming");
            this.Property(t => t.Total_Amount).HasColumnName("Total_Amount");
            this.Property(t => t.Expenses_Date).HasColumnName("Expenses_Date");
            this.Property(t => t.Selected_Currency).HasColumnName("Selected_Currency");
            this.Property(t => t.Reasons).HasColumnName("Reasons");
            this.Property(t => t.Last_Date_Approved).HasColumnName("Last_Date_Approved");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.Date_Applied).HasColumnName("Date_Applied");
            this.Property(t => t.Overall_Status).HasColumnName("Overall_Status");
            this.Property(t => t.Approval_Status_1st).HasColumnName("Approval_Status_1st");
            this.Property(t => t.Approval_Status_2st).HasColumnName("Approval_Status_2st");
            this.Property(t => t.Approval_Cancel_Status).HasColumnName("Approval_Cancel_Status");
            this.Property(t => t.Payroll_Flag).HasColumnName("Payroll_Flag");
            this.Property(t => t.Tax).HasColumnName("Tax");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Mileage).HasColumnName("Mileage");
            this.Property(t => t.Job_Cost_ID).HasColumnName("Job_Cost_ID");
            this.Property(t => t.Withholding_Tax).HasColumnName("Withholding_Tax");
            this.Property(t => t.Doc_No).HasColumnName("Doc_No");
            this.Property(t => t.Tax_Type).HasColumnName("Tax_Type");
            this.Property(t => t.Withholding_Tax_Amount).HasColumnName("Withholding_Tax_Amount");
            this.Property(t => t.Tax_Amount).HasColumnName("Tax_Amount");
            this.Property(t => t.Tax_Amount_Type).HasColumnName("Tax_Amount_Type");
            this.Property(t => t.Withholding_Tax_Type).HasColumnName("Withholding_Tax_Type");

            // Relationships
            this.HasOptional(t => t.Currency)
                .WithMany(t => t.Expenses_Application_Document)
                .HasForeignKey(d => d.Selected_Currency);
            this.HasOptional(t => t.Department)
                .WithMany(t => t.Expenses_Application_Document)
                .HasForeignKey(d => d.Department_ID);
            this.HasOptional(t => t.Employee_Profile)
                .WithMany(t => t.Expenses_Application_Document)
                .HasForeignKey(d => d.Employee_Profile_ID);
            this.HasOptional(t => t.Expenses_Application)
                .WithMany(t => t.Expenses_Application_Document)
                .HasForeignKey(d => d.Expenses_Application_ID);
            this.HasOptional(t => t.Expenses_Config)
                .WithMany(t => t.Expenses_Application_Document)
                .HasForeignKey(d => d.Expenses_Config_ID);
            this.HasOptional(t => t.Job_Cost)
                .WithMany(t => t.Expenses_Application_Document)
                .HasForeignKey(d => d.Job_Cost_ID);

        }
    }
}
