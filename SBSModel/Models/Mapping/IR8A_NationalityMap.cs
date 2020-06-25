using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class IR8A_NationalityMap : EntityTypeConfiguration<IR8A_Nationality>
    {
        public IR8A_NationalityMap()
        {
            // Primary Key
            this.HasKey(t => t.Nationality_Code);

            // Properties
            this.Property(t => t.Nationality_Code)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Description)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("IR8A_Nationality");
            this.Property(t => t.Nationality_Code).HasColumnName("Nationality_Code");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.SBS_Nationality_ID).HasColumnName("SBS_Nationality_ID");

            // Relationships
            this.HasOptional(t => t.Nationality)
                .WithMany(t => t.IR8A_Nationality)
                .HasForeignKey(d => d.SBS_Nationality_ID);

        }
    }
}
