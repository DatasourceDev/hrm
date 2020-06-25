using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSWorkFlowAPI.Models.Mapping
{
    public class ApproverMap : EntityTypeConfiguration<Approver>
    {
        public ApproverMap()
        {
            // Primary Key
            this.HasKey(t => t.Approver_ID);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(100);

            this.Property(t => t.Email)
                .HasMaxLength(100);

            this.Property(t => t.Approval_Flow_Type)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Approver");
            this.Property(t => t.Approver_ID).HasColumnName("Approver_ID");
            this.Property(t => t.Approval_Flow_ID).HasColumnName("Approval_Flow_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Approval_Level).HasColumnName("Approval_Level");
            this.Property(t => t.Profile_ID).HasColumnName("Profile_ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Approval_Flow_Type).HasColumnName("Approval_Flow_Type");

            // Relationships
            this.HasRequired(t => t.Approval_Flow)
                .WithMany(t => t.Approvers)
                .HasForeignKey(d => d.Approval_Flow_ID);

        }
    }
}
