using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class PRC_DepartmentMap : EntityTypeConfiguration<PRC_Department>
    {
        public PRC_DepartmentMap()
        {
            // Primary Key
            this.HasKey(t => t.PRC_Department_ID);

            // Properties
            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("PRC_Department");
            this.Property(t => t.PRC_Department_ID).HasColumnName("PRC_Department_ID");
            this.Property(t => t.PRC_ID).HasColumnName("PRC_ID");
            this.Property(t => t.Department_ID).HasColumnName("Department_ID");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasRequired(t => t.Department)
                .WithMany(t => t.PRC_Department)
                .HasForeignKey(d => d.Department_ID);
            this.HasRequired(t => t.PRC)
                .WithMany(t => t.PRC_Department)
                .HasForeignKey(d => d.PRC_ID);

        }
    }
}
