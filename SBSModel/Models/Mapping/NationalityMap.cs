using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class NationalityMap : EntityTypeConfiguration<Nationality>
    {
        public NationalityMap()
        {
            // Primary Key
            this.HasKey(t => t.Nationality_ID);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(150);

            this.Property(t => t.Description)
                .HasMaxLength(150);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Nationality");
            this.Property(t => t.Nationality_ID).HasColumnName("Nationality_ID");
            this.Property(t => t.Country_ID).HasColumnName("Country_ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasOptional(t => t.Country)
                .WithMany(t => t.Nationalities)
                .HasForeignKey(d => d.Country_ID);

        }
    }
}
