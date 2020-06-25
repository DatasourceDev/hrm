using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Email_NotificationMap : EntityTypeConfiguration<Email_Notification>
    {
        public Email_NotificationMap()
        {
            // Primary Key
            this.HasKey(t => t.Email_ID);

            // Properties
            this.Property(t => t.Sender)
                .HasMaxLength(300);

            this.Property(t => t.Receiver)
                .HasMaxLength(300);

            this.Property(t => t.CC)
                .HasMaxLength(500);

            this.Property(t => t.BCC)
                .HasMaxLength(500);

            this.Property(t => t.Subject)
                .HasMaxLength(300);

            this.Property(t => t.Status)
                .HasMaxLength(150);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.Module)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Email_Notification");
            this.Property(t => t.Email_ID).HasColumnName("Email_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Sender).HasColumnName("Sender");
            this.Property(t => t.Receiver).HasColumnName("Receiver");
            this.Property(t => t.CC).HasColumnName("CC");
            this.Property(t => t.BCC).HasColumnName("BCC");
            this.Property(t => t.Subject).HasColumnName("Subject");
            this.Property(t => t.Message).HasColumnName("Message");
            this.Property(t => t.Exception_Message).HasColumnName("Exception_Message");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Module).HasColumnName("Module");
            this.Property(t => t.Doc_ID).HasColumnName("Doc_ID");
            // Relationships
            this.HasOptional(t => t.Company)
                .WithMany(t => t.Email_Notification)
                .HasForeignKey(d => d.Company_ID);

        }
    }
}
