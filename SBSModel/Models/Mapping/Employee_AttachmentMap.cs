using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Employee_AttachmentMap : EntityTypeConfiguration<Employee_Attachment>
    {
        public Employee_AttachmentMap()
        {
            // Primary Key
            this.HasKey(t => t.Attachment_ID);

            // Properties
            this.Property(t => t.File_Name)
                .HasMaxLength(150);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Uploaded_by)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Employee_Attachment");
            this.Property(t => t.Attachment_ID).HasColumnName("Attachment_ID");
            this.Property(t => t.Attachment_File).HasColumnName("Attachment_File");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Attachment_Type).HasColumnName("Attachment_Type");
            this.Property(t => t.File_Name).HasColumnName("File_Name");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Uploaded_On).HasColumnName("Uploaded_On");
            this.Property(t => t.Uploaded_by).HasColumnName("Uploaded_by");
           

            // Relationships
            this.HasOptional(t => t.Global_Lookup_Data)
                .WithMany(t => t.Employee_Attachment)
                .HasForeignKey(d => d.Attachment_Type);
            this.HasOptional(t => t.Employee_Profile)
                .WithMany(t => t.Employee_Attachment)
                .HasForeignKey(d => d.Employee_Profile_ID);

        }
    }
}
