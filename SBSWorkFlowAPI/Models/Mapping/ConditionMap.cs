using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSWorkFlowAPI.Models.Mapping
{
    public class ConditionMap : EntityTypeConfiguration<Condition>
    {
        public ConditionMap()
        {
            // Primary Key
            this.HasKey(t => t.Condition_ID);

            // Properties
            this.Property(t => t.StrCondition)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Condition");
            this.Property(t => t.Condition_ID).HasColumnName("Condition_ID");
            this.Property(t => t.Approval_Flow_ID).HasColumnName("Approval_Flow_ID");
            this.Property(t => t.StrCondition).HasColumnName("StrCondition");
            this.Property(t => t.LeftRange).HasColumnName("LeftRange");
            this.Property(t => t.RightRange).HasColumnName("RightRange");

            // Relationships
            this.HasRequired(t => t.Approval_Flow)
                .WithMany(t => t.Conditions)
                .HasForeignKey(d => d.Approval_Flow_ID);

        }
    }
}
