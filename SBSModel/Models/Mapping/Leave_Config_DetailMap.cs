using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Leave_Config_DetailMap : EntityTypeConfiguration<Leave_Config_Detail>
    {
        public Leave_Config_DetailMap()
        {
            // Primary Key
            this.HasKey(t => t.Leave_Config_Detail_ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("Leave_Config_Detail");
            this.Property(t => t.Leave_Config_Detail_ID).HasColumnName("Leave_Config_Detail_ID");
            this.Property(t => t.Leave_Config_ID).HasColumnName("Leave_Config_ID");
            this.Property(t => t.Designation_ID).HasColumnName("Designation_ID");
            this.Property(t => t.Default_Leave_Amount).HasColumnName("Default_Leave_Amount");
            this.Property(t => t.Year_Service).HasColumnName("Year_Service");
            this.Property(t => t.Group_ID).HasColumnName("Group_ID");
            this.Property(t => t.Bring_Forward_Days).HasColumnName("Bring_Forward_Days");
            this.Property(t => t.Is_Bring_Forward_Percent).HasColumnName("Is_Bring_Forward_Percent");

            // Relationships
            this.HasOptional(t => t.Designation)
                .WithMany(t => t.Leave_Config_Detail)
                .HasForeignKey(d => d.Designation_ID);
            this.HasOptional(t => t.Leave_Config)
                .WithMany(t => t.Leave_Config_Detail)
                .HasForeignKey(d => d.Leave_Config_ID);

        }
    }
}
