using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class HR_FileExport_History_DetailMap : EntityTypeConfiguration<HR_FileExport_History_Detail>
    {
        public HR_FileExport_History_DetailMap()
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

            this.Property(t => t.Residential_Status)
                .HasMaxLength(1);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("HR_FileExport_History_Detail");
            this.Property(t => t.Detail_ID).HasColumnName("Detail_ID");
            this.Property(t => t.Generated_ID).HasColumnName("Generated_ID");
            this.Property(t => t.Process_Year).HasColumnName("Process_Year");
            this.Property(t => t.Process_Month).HasColumnName("Process_Month");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Employee_No).HasColumnName("Employee_No");
            this.Property(t => t.NRIC).HasColumnName("NRIC");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Department_ID).HasColumnName("Department_ID");
            this.Property(t => t.Designation_ID).HasColumnName("Designation_ID");
            this.Property(t => t.CPF_Contribution).HasColumnName("CPF_Contribution");
            this.Property(t => t.CPF_Employee).HasColumnName("CPF_Employee");
            this.Property(t => t.Ordinary_Wages).HasColumnName("Ordinary_Wages");
            this.Property(t => t.Additional_Wages).HasColumnName("Additional_Wages");
            this.Property(t => t.Race).HasColumnName("Race");
            this.Property(t => t.Residential_Status).HasColumnName("Residential_Status");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.SDL).HasColumnName("SDL");
            this.Property(t => t.MBMF).HasColumnName("MBMF");
            this.Property(t => t.SIDA).HasColumnName("SIDA");
            this.Property(t => t.CDAC).HasColumnName("CDAC");
            this.Property(t => t.ECF).HasColumnName("ECF");

            // Relationships
            this.HasOptional(t => t.Global_Lookup_Data)
                .WithMany(t => t.HR_FileExport_History_Detail)
                .HasForeignKey(d => d.Race);
            this.HasOptional(t => t.HR_FileExport_History)
                .WithMany(t => t.HR_FileExport_History_Detail)
                .HasForeignKey(d => d.Generated_ID);

        }
    }
}
