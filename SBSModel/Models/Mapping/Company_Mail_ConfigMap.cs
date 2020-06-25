using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Company_Mail_ConfigMap : EntityTypeConfiguration<Company_Mail_Config>
    {
        public Company_Mail_ConfigMap()
        {
            // Primary Key
            this.HasKey(t => t.Config_ID);

            // Properties
            this.Property(t => t.SMTP_Server)
                .HasMaxLength(50);

            this.Property(t => t.Email)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Company_Mail_Config");
            this.Property(t => t.Config_ID).HasColumnName("Config_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.SMTP_Server).HasColumnName("SMTP_Server");
            this.Property(t => t.SMTP_Port).HasColumnName("SMTP_Port");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Password).HasColumnName("Password");
            this.Property(t => t.SSL).HasColumnName("SSL");
        }
    }
}
