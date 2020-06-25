using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Leave_Config_ConditionMap : EntityTypeConfiguration<Leave_Config_Condition>
    {
        public Leave_Config_ConditionMap()
        {
            // Primary Key
            this.HasKey(t => t.Leave_Config_Condition_ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("Leave_Config_Condition");
            this.Property(t => t.Leave_Config_Condition_ID).HasColumnName("Leave_Config_Condition_ID");
            this.Property(t => t.Lookup_Data_ID).HasColumnName("Lookup_Data_ID");
            this.Property(t => t.Leave_Config_ID).HasColumnName("Leave_Config_ID");

            // Relationships
            this.HasOptional(t => t.Global_Lookup_Data)
                .WithMany(t => t.Leave_Config_Condition)
                .HasForeignKey(d => d.Lookup_Data_ID);
            this.HasOptional(t => t.Leave_Config)
                .WithMany(t => t.Leave_Config_Condition)
                .HasForeignKey(d => d.Leave_Config_ID);

        }
    }
}
