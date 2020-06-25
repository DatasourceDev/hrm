using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class ExchangeMap : EntityTypeConfiguration<Exchange>
    {
        public ExchangeMap()
        {
            // Primary Key
            this.HasKey(t => t.Exchange_ID);

            // Properties
            this.Property(t => t.Record_Status)
                .HasMaxLength(50);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Exchange");
            this.Property(t => t.Exchange_ID).HasColumnName("Exchange_ID");
            this.Property(t => t.Fiscal_Year).HasColumnName("Fiscal_Year");
            this.Property(t => t.Record_Status).HasColumnName("Record_Status");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
        }
    }
}
