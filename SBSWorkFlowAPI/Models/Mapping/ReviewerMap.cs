using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSWorkFlowAPI.Models.Mapping
{
    public class ReviewerMap : EntityTypeConfiguration<Reviewer>
    {
        public ReviewerMap()
        {
            // Primary Key
            this.HasKey(t => t.Reviewer_ID);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(100);

            this.Property(t => t.Email)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Reviewer");
            this.Property(t => t.Reviewer_ID).HasColumnName("Reviewer_ID");
            this.Property(t => t.Approval_Flow_ID).HasColumnName("Approval_Flow_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Profile_ID).HasColumnName("Profile_ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Email).HasColumnName("Email");

            // Relationships
            this.HasRequired(t => t.Approval_Flow)
                .WithMany(t => t.Reviewers)
                .HasForeignKey(d => d.Approval_Flow_ID);

        }
    }
}
