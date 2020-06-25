using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Time_SheetMap : EntityTypeConfiguration<Time_Sheet>
    {
        public Time_SheetMap()
        {
            // Primary Key
            this.HasKey(t => t.Time_Sheet_ID);

            // Properties
            this.Property(t => t.Employee_Name)
                .HasMaxLength(150);

            this.Property(t => t.Overall_Status)
                .HasMaxLength(50);

            this.Property(t => t.Cancel_Status)
                .HasMaxLength(50);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Time_Sheet");
            this.Property(t => t.Time_Sheet_ID).HasColumnName("Time_Sheet_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Supervisor).HasColumnName("Supervisor");
            this.Property(t => t.Employee_Name).HasColumnName("Employee_Name");
            this.Property(t => t.Request_ID).HasColumnName("Request_ID");
            this.Property(t => t.Request_Cancel_ID).HasColumnName("Request_Cancel_ID");
            this.Property(t => t.Overall_Status).HasColumnName("Overall_Status");
            this.Property(t => t.Cancel_Status).HasColumnName("Cancel_Status");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasOptional(t => t.Employee_Profile)
                .WithMany(t => t.Time_Sheet)
                .HasForeignKey(d => d.Employee_Profile_ID);

        }
    }
}
