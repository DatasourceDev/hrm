using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Exchange_CurrencyMap : EntityTypeConfiguration<Exchange_Currency>
    {
        public Exchange_CurrencyMap()
        {
            // Primary Key
            this.HasKey(t => t.Exchange_Currency_ID);

            // Properties
            this.Property(t => t.Exchange_Period)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Exchange_Currency");
            this.Property(t => t.Exchange_Currency_ID).HasColumnName("Exchange_Currency_ID");
            this.Property(t => t.Exchange_ID).HasColumnName("Exchange_ID");
            this.Property(t => t.Currency_ID).HasColumnName("Currency_ID");
            this.Property(t => t.Exchange_Period).HasColumnName("Exchange_Period");

            // Relationships
            this.HasOptional(t => t.Exchange)
                .WithMany(t => t.Exchange_Currency)
                .HasForeignKey(d => d.Exchange_ID);

        }
    }
}
