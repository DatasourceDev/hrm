using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class HR_FileExport_IR8A_DetailMap : EntityTypeConfiguration<HR_FileExport_IR8A_Detail>
    {
        public HR_FileExport_IR8A_DetailMap()
        {
            // Primary Key
            this.HasKey(t => t.Detail_ID);

            // Properties
            this.Property(t => t.Employee_No)
                .HasMaxLength(150);

            this.Property(t => t.NRIC)
                .HasMaxLength(30);

            this.Property(t => t.Name)
                .HasMaxLength(150);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("HR_FileExport_IR8A_Detail");
            this.Property(t => t.Detail_ID).HasColumnName("Detail_ID");
            this.Property(t => t.Generated_ID).HasColumnName("Generated_ID");
            this.Property(t => t.Process_Year).HasColumnName("Process_Year");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Employee_No).HasColumnName("Employee_No");
            this.Property(t => t.NRIC).HasColumnName("NRIC");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Department_ID).HasColumnName("Department_ID");
            this.Property(t => t.Designation_ID).HasColumnName("Designation_ID");
            this.Property(t => t.Total_Amount).HasColumnName("Total_Amount");
            this.Property(t => t.Donation).HasColumnName("Donation");
            this.Property(t => t.CPF_Desgn_Pension).HasColumnName("CPF_Desgn_Pension");
            this.Property(t => t.Insurance).HasColumnName("Insurance");
            this.Property(t => t.Salary).HasColumnName("Salary");
            this.Property(t => t.Bonus).HasColumnName("Bonus");
            this.Property(t => t.Directors_Fee).HasColumnName("Directors_Fee");
            this.Property(t => t.Others).HasColumnName("Others");
            this.Property(t => t.Gross_Commission).HasColumnName("Gross_Commission");
            this.Property(t => t.Pension).HasColumnName("Pension");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
        }
    }
}
