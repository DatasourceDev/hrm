using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Email_AttachmentMap : EntityTypeConfiguration<Email_Attachment>
    {
        public Email_AttachmentMap()
        {
            // Primary Key
            this.HasKey(t => t.Email_Attachment_ID);

            // Properties
            this.Property(t => t.Attachment_File_Name)
                .HasMaxLength(300);

            // Table & Column Mappings
            this.ToTable("Email_Attachment");
            this.Property(t => t.Email_Attachment_ID).HasColumnName("Email_Attachment_ID");
            this.Property(t => t.Email_ID).HasColumnName("Email_ID");
            this.Property(t => t.Attachment_File_Name).HasColumnName("Attachment_File_Name");
            this.Property(t => t.Attachment_File).HasColumnName("Attachment_File");

            // Relationships
            this.HasOptional(t => t.Email_Notification)
                .WithMany(t => t.Email_Attachment)
                .HasForeignKey(d => d.Email_ID);

        }
    }
}
