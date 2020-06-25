using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Leave_Config_ExtraMap : EntityTypeConfiguration<Leave_Config_Extra>
    {
        public Leave_Config_ExtraMap()
        {
            // Primary Key
            this.HasKey(t => t.Leave_Config_Extra_ID);

            // Properties
            this.Property(t => t.Adjustment_Type)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Leave_Config_Extra");
            this.Property(t => t.Leave_Config_Extra_ID).HasColumnName("Leave_Config_Extra_ID");
            this.Property(t => t.Leave_Config_ID).HasColumnName("Leave_Config_ID");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Adjustment_Type).HasColumnName("Adjustment_Type");
            this.Property(t => t.No_Of_Days).HasColumnName("No_Of_Days");

            // Relationships
            this.HasOptional(t => t.Employee_Profile)
                .WithMany(t => t.Leave_Config_Extra)
                .HasForeignKey(d => d.Employee_Profile_ID);
            this.HasOptional(t => t.Leave_Config)
                .WithMany(t => t.Leave_Config_Extra)
                .HasForeignKey(d => d.Leave_Config_ID);

        }
    }
}
