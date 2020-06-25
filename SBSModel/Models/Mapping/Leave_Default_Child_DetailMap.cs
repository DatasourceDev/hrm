using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Leave_Default_Child_DetailMap : EntityTypeConfiguration<Leave_Default_Child_Detail>
    {
        public Leave_Default_Child_DetailMap()
        {
            // Primary Key
            this.HasKey(t => t.Leave_Default_Child_Detail_ID);

            // Properties
            this.Property(t => t.Residential_Status)
                .HasMaxLength(50);

            this.Property(t => t.Period)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Leave_Default_Child_Detail");
            this.Property(t => t.Leave_Default_Child_Detail_ID).HasColumnName("Leave_Default_Child_Detail_ID");
            this.Property(t => t.Default_ID).HasColumnName("Default_ID");
            this.Property(t => t.Residential_Status).HasColumnName("Residential_Status");
            this.Property(t => t.Default_Leave_Amount).HasColumnName("Default_Leave_Amount");
            this.Property(t => t.Period).HasColumnName("Period");

            // Relationships
            this.HasOptional(t => t.Leave_Default)
                .WithMany(t => t.Leave_Default_Child_Detail)
                .HasForeignKey(d => d.Default_ID);

        }
    }
}
