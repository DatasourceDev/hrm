using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Screen_Capture_ImageMap : EntityTypeConfiguration<Screen_Capture_Image>
    {
        public Screen_Capture_ImageMap()
        {
            // Primary Key
            this.HasKey(t => t.SC_Image_ID);

            // Properties
            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.File_Name)
                .HasMaxLength(300);

            // Table & Column Mappings
            this.ToTable("Screen_Capture_Image");
            this.Property(t => t.SC_Image_ID).HasColumnName("SC_Image_ID");
            this.Property(t => t.SC_Log_ID).HasColumnName("SC_Log_ID");
            this.Property(t => t.Image).HasColumnName("Image");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.File_Name).HasColumnName("File_Name");

            // Relationships
            this.HasOptional(t => t.Screen_Capture_Log)
                .WithMany(t => t.Screen_Capture_Image)
                .HasForeignKey(d => d.SC_Log_ID);

        }
    }
}
