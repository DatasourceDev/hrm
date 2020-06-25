using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class HR_FileExport_IR8B_DetailMap : EntityTypeConfiguration<HR_FileExport_IR8B_Detail>
    {
        public HR_FileExport_IR8B_DetailMap()
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

            this.Property(t => t.Sex)
                .HasMaxLength(10);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("HR_FileExport_IR8B_Detail");
            this.Property(t => t.Detail_ID).HasColumnName("Detail_ID");
            this.Property(t => t.Generated_ID).HasColumnName("Generated_ID");
            this.Property(t => t.Process_Year).HasColumnName("Process_Year");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Employee_No).HasColumnName("Employee_No");
            this.Property(t => t.NRIC).HasColumnName("NRIC");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Department_ID).HasColumnName("Department_ID");
            this.Property(t => t.Designation_ID).HasColumnName("Designation_ID");
            this.Property(t => t.Sex).HasColumnName("Sex");
            this.Property(t => t.A_Gross_Amt_NotQualify).HasColumnName("A_Gross_Amt_NotQualify");
            this.Property(t => t.A_Gross_Amt_Gained).HasColumnName("A_Gross_Amt_Gained");
            this.Property(t => t.B_Gross_Amt_NotQualify).HasColumnName("B_Gross_Amt_NotQualify");
            this.Property(t => t.B_Gross_Amt_Gained).HasColumnName("B_Gross_Amt_Gained");
            this.Property(t => t.C_Gross_Amt_NotQualify).HasColumnName("C_Gross_Amt_NotQualify");
            this.Property(t => t.C_Gross_Amt_Gained).HasColumnName("C_Gross_Amt_Gained");
            this.Property(t => t.D_Gross_Amt_NotQualify).HasColumnName("D_Gross_Amt_NotQualify");
            this.Property(t => t.D_Gross_Amt_Gained).HasColumnName("D_Gross_Amt_Gained");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
        }
    }
}
