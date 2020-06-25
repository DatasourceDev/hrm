using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class HR_FileExport_HistoryMap : EntityTypeConfiguration<HR_FileExport_History>
    {
        public HR_FileExport_HistoryMap()
        {
            // Primary Key
            this.HasKey(t => t.Generated_ID);

            // Properties
            this.Property(t => t.File_Type)
                .HasMaxLength(10);

            this.Property(t => t.File_LocalPath)
                .HasMaxLength(200);

            this.Property(t => t.File_Name)
                .HasMaxLength(50);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("HR_FileExport_History");
            this.Property(t => t.Generated_ID).HasColumnName("Generated_ID");
            this.Property(t => t.Process_Year).HasColumnName("Process_Year");
            this.Property(t => t.Process_Month).HasColumnName("Process_Month");
            this.Property(t => t.File_Type).HasColumnName("File_Type");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Generated_Date).HasColumnName("Generated_Date");
            this.Property(t => t.File_LocalPath).HasColumnName("File_LocalPath");
            this.Property(t => t.RFF_Code).HasColumnName("RFF_Code");
            this.Property(t => t.File_Name).HasColumnName("File_Name");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasOptional(t => t.Company)
                .WithMany(t => t.HR_FileExport_History)
                .HasForeignKey(d => d.Company_ID);

        }
    }
}
