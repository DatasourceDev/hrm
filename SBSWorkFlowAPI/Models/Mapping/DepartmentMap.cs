using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSWorkFlowAPI.Models.Mapping
{
    public class DepartmentMap : EntityTypeConfiguration<Department>
    {
        public DepartmentMap()
        {
            // Primary Key
            this.HasKey(t => t.Worklow_Dept_ID);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Department");
            this.Property(t => t.Worklow_Dept_ID).HasColumnName("Worklow_Dept_ID");
            this.Property(t => t.Approval_Flow_ID).HasColumnName("Approval_Flow_ID");
            this.Property(t => t.User_Department_ID).HasColumnName("User_Department_ID");
            this.Property(t => t.Name).HasColumnName("Name");

            // Relationships
            this.HasRequired(t => t.Approval_Flow)
                .WithMany(t => t.Departments)
                .HasForeignKey(d => d.Approval_Flow_ID);

        }
    }
}
