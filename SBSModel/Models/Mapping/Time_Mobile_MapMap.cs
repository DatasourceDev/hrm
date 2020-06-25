using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Time_Mobile_MapMap : EntityTypeConfiguration<Time_Mobile_Map>
    {
        public Time_Mobile_MapMap()
        {
            // Primary Key
            this.HasKey(t => t.Map_ID);

            // Properties
            this.Property(t => t.UUID)
                .HasMaxLength(30);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Time_Mobile_Map");
            this.Property(t => t.Map_ID).HasColumnName("Map_ID");
            this.Property(t => t.UUID).HasColumnName("UUID");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");

            // Relationships
            this.HasOptional(t => t.Employee_Profile)
                .WithMany(t => t.Time_Mobile_Map)
                .HasForeignKey(d => d.Employee_Profile_ID);

        }
    }
}
