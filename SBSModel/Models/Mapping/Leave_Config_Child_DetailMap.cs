using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Leave_Config_Child_DetailMap : EntityTypeConfiguration<Leave_Config_Child_Detail>
    {
        public Leave_Config_Child_DetailMap()
        {
            // Primary Key
            this.HasKey(t => t.Leave_Config_Child_Detail_ID);

            // Properties
            this.Property(t => t.Residential_Status)
                .HasMaxLength(50);

            this.Property(t => t.Period)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Leave_Config_Child_Detail");
            this.Property(t => t.Leave_Config_Child_Detail_ID).HasColumnName("Leave_Config_Child_Detail_ID");
            this.Property(t => t.Leave_Config_ID).HasColumnName("Leave_Config_ID");
            this.Property(t => t.Residential_Status).HasColumnName("Residential_Status");
            this.Property(t => t.Default_Leave_Amount).HasColumnName("Default_Leave_Amount");
            this.Property(t => t.Period).HasColumnName("Period");
            this.Property(t => t.Group_ID).HasColumnName("Group_ID");

            // Relationships
            this.HasOptional(t => t.Leave_Config)
                .WithMany(t => t.Leave_Config_Child_Detail)
                .HasForeignKey(d => d.Leave_Config_ID);

        }
    }
}
