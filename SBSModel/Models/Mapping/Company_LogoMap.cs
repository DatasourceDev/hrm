using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Company_LogoMap : EntityTypeConfiguration<Company_Logo>
    {
        public Company_LogoMap()
        {
            // Primary Key
            this.HasKey(t => t.Company_Logo_ID);

            // Properties
            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Company_Logo");
            this.Property(t => t.Company_Logo_ID).HasColumnName("Company_Logo_ID");
            this.Property(t => t.Logo).HasColumnName("Logo");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");

            // Relationships
            this.HasOptional(t => t.Company)
                .WithMany(t => t.Company_Logo)
                .HasForeignKey(d => d.Company_ID);

        }
    }
}
