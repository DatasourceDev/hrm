using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Leave_Default_ConditionMap : EntityTypeConfiguration<Leave_Default_Condition>
    {
        public Leave_Default_ConditionMap()
        {
            // Primary Key
            this.HasKey(t => t.Leave_Default_Condition_ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("Leave_Default_Condition");
            this.Property(t => t.Leave_Default_Condition_ID).HasColumnName("Leave_Default_Condition_ID");
            this.Property(t => t.Lookup_Data_ID).HasColumnName("Lookup_Data_ID");
            this.Property(t => t.Default_ID).HasColumnName("Default_ID");

            // Relationships
            this.HasOptional(t => t.Global_Lookup_Data)
                .WithMany(t => t.Leave_Default_Condition)
                .HasForeignKey(d => d.Lookup_Data_ID);
            this.HasOptional(t => t.Leave_Default)
                .WithMany(t => t.Leave_Default_Condition)
                .HasForeignKey(d => d.Default_ID);

        }
    }
}
