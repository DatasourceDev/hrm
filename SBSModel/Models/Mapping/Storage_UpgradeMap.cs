using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Storage_UpgradeMap : EntityTypeConfiguration<Storage_Upgrade>
    {
        public Storage_UpgradeMap()
        {
            // Primary Key
            this.HasKey(t => t.Transaction_ID);

            // Properties
            this.Property(t => t.Upgrade_By)
                .HasMaxLength(50);

            this.Property(t => t.Record_Status)
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("Storage_Upgrade");
            this.Property(t => t.Transaction_ID).HasColumnName("Transaction_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Upgrade_Space).HasColumnName("Upgrade_Space");
            this.Property(t => t.Price).HasColumnName("Price");
            this.Property(t => t.Upgrade_On).HasColumnName("Upgrade_On");
            this.Property(t => t.Upgrade_By).HasColumnName("Upgrade_By");
            this.Property(t => t.Expired_On).HasColumnName("Expired_On");
            this.Property(t => t.Record_Status).HasColumnName("Record_Status");

            // Relationships
            this.HasRequired(t => t.Company)
                .WithMany(t => t.Storage_Upgrade)
                .HasForeignKey(d => d.Company_ID);

        }
    }
}
