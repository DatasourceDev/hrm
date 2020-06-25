using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSWorkFlowAPI.Models.Mapping
{
    public class Task_AssignmentMap : EntityTypeConfiguration<Task_Assignment>
    {
        public Task_AssignmentMap()
        {
            // Primary Key
            this.HasKey(t => t.Task_ID);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(50);

            this.Property(t => t.Email)
                .HasMaxLength(50);

            this.Property(t => t.Record_Status)
                .IsRequired()
                .HasMaxLength(1);

            this.Property(t => t.Status)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Task_Assignment");
            this.Property(t => t.Task_ID).HasColumnName("Task_ID");
            this.Property(t => t.Request_ID).HasColumnName("Request_ID");
            this.Property(t => t.Profile_ID).HasColumnName("Profile_ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Record_Status).HasColumnName("Record_Status");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Approval_Level).HasColumnName("Approval_Level");
            this.Property(t => t.Is_Indent).HasColumnName("Is_Indent");
            this.Property(t => t.Indent_Closed).HasColumnName("Indent_Closed");

            // Relationships
            this.HasRequired(t => t.Request)
                .WithMany(t => t.Task_Assignment)
                .HasForeignKey(d => d.Request_ID);

        }
    }
}
