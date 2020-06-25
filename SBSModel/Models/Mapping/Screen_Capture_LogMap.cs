using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Screen_Capture_LogMap : EntityTypeConfiguration<Screen_Capture_Log>
    {
        public Screen_Capture_LogMap()
        {
            // Primary Key
            this.HasKey(t => t.SC_Log_ID);

            // Properties
            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Screen_Capture_Log");
            this.Property(t => t.SC_Log_ID).HasColumnName("SC_Log_ID");
            this.Property(t => t.Profile_ID).HasColumnName("Profile_ID");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasOptional(t => t.User_Profile)
                .WithMany(t => t.Screen_Capture_Log)
                .HasForeignKey(d => d.Profile_ID);

        }
    }
}
