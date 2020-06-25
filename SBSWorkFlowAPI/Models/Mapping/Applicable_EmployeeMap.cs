using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSWorkFlowAPI.Models.Mapping
{
    public class Applicable_EmployeeMap : EntityTypeConfiguration<Applicable_Employee>
    {
        public Applicable_EmployeeMap()
        {
            // Primary Key
            this.HasKey(t => t.Applicable_Employee_ID);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(100);

            this.Property(t => t.Email)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Applicable_Employee");
            this.Property(t => t.Applicable_Employee_ID).HasColumnName("Applicable_Employee_ID");
            this.Property(t => t.Approval_Flow_ID).HasColumnName("Approval_Flow_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Profile_ID).HasColumnName("Profile_ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Is_Applicable).HasColumnName("Is_Applicable");

            // Relationships
            this.HasRequired(t => t.Approval_Flow)
                .WithMany(t => t.Applicable_Employee)
                .HasForeignKey(d => d.Approval_Flow_ID);

        }
    }
}
