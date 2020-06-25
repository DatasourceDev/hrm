using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSWorkFlowAPI.Models.Mapping
{
    public class HistoryMap : EntityTypeConfiguration<History>
    {
        public HistoryMap()
        {
            // Primary Key
            this.HasKey(t => t.History_ID);

            // Properties
            this.Property(t => t.Action)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.Action_By)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Action_Email)
                .HasMaxLength(50);

            this.Property(t => t.Remarks)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("History");
            this.Property(t => t.History_ID).HasColumnName("History_ID");
            this.Property(t => t.Request_ID).HasColumnName("Request_ID");
            this.Property(t => t.Profile_ID).HasColumnName("Profile_ID");
            this.Property(t => t.Action).HasColumnName("Action");
            this.Property(t => t.Action_On).HasColumnName("Action_On");
            this.Property(t => t.Action_By).HasColumnName("Action_By");
            this.Property(t => t.Action_Email).HasColumnName("Action_Email");
            this.Property(t => t.Remarks).HasColumnName("Remarks");

            // Relationships
            this.HasRequired(t => t.Request)
                .WithMany(t => t.Histories)
                .HasForeignKey(d => d.Request_ID);

        }
    }
}
