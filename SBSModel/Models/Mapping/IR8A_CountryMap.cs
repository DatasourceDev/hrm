using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class IR8A_CountryMap : EntityTypeConfiguration<IR8A_Country>
    {
        public IR8A_CountryMap()
        {
            // Primary Key
            this.HasKey(t => t.Country_Code);

            // Properties
            this.Property(t => t.Country_Code)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Description)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("IR8A_Country");
            this.Property(t => t.Country_Code).HasColumnName("Country_Code");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.SBS_Country_ID).HasColumnName("SBS_Country_ID");

            // Relationships
            this.HasOptional(t => t.Country)
                .WithMany(t => t.IR8A_Country)
                .HasForeignKey(d => d.SBS_Country_ID);

        }
    }
}
