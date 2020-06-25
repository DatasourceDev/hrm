using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class DesignationMap : EntityTypeConfiguration<Designation>
    {
        public DesignationMap()
        {
            // Primary Key
            this.HasKey(t => t.Designation_ID);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(150);

            this.Property(t => t.Record_Status)
                .HasMaxLength(50);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Designation");
            this.Property(t => t.Designation_ID).HasColumnName("Designation_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Record_Status).HasColumnName("Record_Status");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Employee_Grading_ID).HasColumnName("Employee_Grading_ID");

            // Relationships
            this.HasRequired(t => t.Company)
                .WithMany(t => t.Designations)
                .HasForeignKey(d => d.Company_ID);

        }
    }
}
