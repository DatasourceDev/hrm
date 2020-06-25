using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class CurrencyMap : EntityTypeConfiguration<Currency>
    {
        public CurrencyMap()
        {
            // Primary Key
            this.HasKey(t => t.Currency_ID);

            // Properties
            this.Property(t => t.Currency_Code)
                .HasMaxLength(5);

            this.Property(t => t.Currency_Name)
                .HasMaxLength(100);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Currency");
            this.Property(t => t.Currency_ID).HasColumnName("Currency_ID");
            this.Property(t => t.Country_ID).HasColumnName("Country_ID");
            this.Property(t => t.Currency_Code).HasColumnName("Currency_Code");
            this.Property(t => t.Currency_Name).HasColumnName("Currency_Name");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasOptional(t => t.Country)
                .WithMany(t => t.Currencies)
                .HasForeignKey(d => d.Country_ID);

        }
    }
}
