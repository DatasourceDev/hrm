using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Exchange_RateMap : EntityTypeConfiguration<Exchange_Rate>
    {
        public Exchange_RateMap()
        {
            // Primary Key
            this.HasKey(t => t.Exchange_Rate_ID);

            // Properties
            this.Property(t => t.Exchange_Period)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Exchange_Rate");
            this.Property(t => t.Exchange_Rate_ID).HasColumnName("Exchange_Rate_ID");
            this.Property(t => t.Exchange_Currency_ID).HasColumnName("Exchange_Currency_ID");
            this.Property(t => t.Rate).HasColumnName("Rate");
            this.Property(t => t.Exchange_Period).HasColumnName("Exchange_Period");
            this.Property(t => t.Exchange_Date).HasColumnName("Exchange_Date");
            this.Property(t => t.Exchange_Month).HasColumnName("Exchange_Month");

            // Relationships
            this.HasOptional(t => t.Exchange_Currency)
                .WithMany(t => t.Exchange_Rate)
                .HasForeignKey(d => d.Exchange_Currency_ID);

        }
    }
}
