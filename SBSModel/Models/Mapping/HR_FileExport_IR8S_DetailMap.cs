using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class HR_FileExport_IR8S_DetailMap : EntityTypeConfiguration<HR_FileExport_IR8S_Detail>
    {
        public HR_FileExport_IR8S_DetailMap()
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

            this.Property(t => t.MTH_OW_OEplrCPF_OEmpCPF_AW_AEplrCPF_AEmpCPF)
                .HasMaxLength(500);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("HR_FileExport_IR8S_Detail");
            this.Property(t => t.Detail_ID).HasColumnName("Detail_ID");
            this.Property(t => t.Generated_ID).HasColumnName("Generated_ID");
            this.Property(t => t.Process_Year).HasColumnName("Process_Year");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Employee_No).HasColumnName("Employee_No");
            this.Property(t => t.NRIC).HasColumnName("NRIC");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Department_ID).HasColumnName("Department_ID");
            this.Property(t => t.Designation_ID).HasColumnName("Designation_ID");
            this.Property(t => t.MTH_OW_OEplrCPF_OEmpCPF_AW_AEplrCPF_AEmpCPF).HasColumnName("MTH_OW_OEplrCPF_OEmpCPF_AW_AEplrCPF_AEmpCPF");
            this.Property(t => t.TTL_Emloyer_CPF).HasColumnName("TTL_Emloyer_CPF");
            this.Property(t => t.TTL_Employee_CPF).HasColumnName("TTL_Employee_CPF");
            this.Property(t => t.AW_1).HasColumnName("AW_1");
            this.Property(t => t.RefundToEmployer_1).HasColumnName("RefundToEmployer_1");
            this.Property(t => t.InterestToEmployer_1).HasColumnName("InterestToEmployer_1");
            this.Property(t => t.RefundToEmployee_1).HasColumnName("RefundToEmployee_1");
            this.Property(t => t.InterestToEmployee_1).HasColumnName("InterestToEmployee_1");
            this.Property(t => t.AW_2).HasColumnName("AW_2");
            this.Property(t => t.RefundToEmployer_2).HasColumnName("RefundToEmployer_2");
            this.Property(t => t.InterestToEmployer_2).HasColumnName("InterestToEmployer_2");
            this.Property(t => t.RefundToEmployee_2).HasColumnName("RefundToEmployee_2");
            this.Property(t => t.InterestToEmployee_2).HasColumnName("InterestToEmployee_2");
            this.Property(t => t.AW_3).HasColumnName("AW_3");
            this.Property(t => t.RefundToEmployer_3).HasColumnName("RefundToEmployer_3");
            this.Property(t => t.InterestToEmployer_3).HasColumnName("InterestToEmployer_3");
            this.Property(t => t.RefundToEmployee_3).HasColumnName("RefundToEmployee_3");
            this.Property(t => t.InterestToEmployee_3).HasColumnName("InterestToEmployee_3");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Excess_Emloyer_CPF).HasColumnName("Excess_Emloyer_CPF");
            this.Property(t => t.Excess_Employee_CPF).HasColumnName("Excess_Employee_CPF");

            // Relationships
            this.HasRequired(t => t.HR_FileExport_History)
                .WithMany(t => t.HR_FileExport_IR8S_Detail)
                .HasForeignKey(d => d.Generated_ID);

        }
    }
}
