using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSWorkFlowAPI.Models.Mapping
{
    public class Approval_FlowMap : EntityTypeConfiguration<Approval_Flow>
    {
        public Approval_FlowMap()
        {
            // Primary Key
            this.HasKey(t => t.Approval_Flow_ID);

            // Properties
            this.Property(t => t.Approval_Type)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Module)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Branch_Name)
                .HasMaxLength(200);

            this.Property(t => t.Record_Status)
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("Approval_Flow");
            this.Property(t => t.Approval_Flow_ID).HasColumnName("Approval_Flow_ID");
            this.Property(t => t.Approval_Type).HasColumnName("Approval_Type");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Module).HasColumnName("Module");
            this.Property(t => t.Branch_ID).HasColumnName("Branch_ID");
            this.Property(t => t.Branch_Name).HasColumnName("Branch_Name");
            this.Property(t => t.Record_Status).HasColumnName("Record_Status");
            this.Property(t => t.Created_By).HasColumnName("Created_By");
            this.Property(t => t.Created_On).HasColumnName("Created_On");
            this.Property(t => t.Updated_By).HasColumnName("Updated_By");
            this.Property(t => t.Updated_On).HasColumnName("Updated_On");
        }
    }
}
