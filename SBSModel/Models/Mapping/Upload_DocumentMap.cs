using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Upload_DocumentMap : EntityTypeConfiguration<Upload_Document>
    {
        public Upload_DocumentMap()
        {
            // Primary Key
            this.HasKey(t => t.Upload_Document_ID);

            // Properties
            this.Property(t => t.File_Name)
                .HasMaxLength(300);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Upload_Document");
            this.Property(t => t.Upload_Document_ID).HasColumnName("Upload_Document_ID");
            this.Property(t => t.Document).HasColumnName("Document");
            this.Property(t => t.File_Name).HasColumnName("File_Name");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Leave_Application_Document_ID).HasColumnName("Leave_Application_Document_ID");

            // Relationships
            this.HasOptional(t => t.Leave_Application_Document)
                .WithMany(t => t.Upload_Document)
                .HasForeignKey(d => d.Leave_Application_Document_ID);

        }
    }
}
