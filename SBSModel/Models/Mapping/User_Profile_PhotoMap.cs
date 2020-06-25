using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class User_Profile_PhotoMap : EntityTypeConfiguration<User_Profile_Photo>
    {
        public User_Profile_PhotoMap()
        {
            // Primary Key
            this.HasKey(t => t.User_Profile_Photo_ID);

            // Properties
            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("User_Profile_Photo");
            this.Property(t => t.User_Profile_Photo_ID).HasColumnName("User_Profile_Photo_ID");
            this.Property(t => t.Photo).HasColumnName("Photo");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Profile_ID).HasColumnName("Profile_ID");

            // Relationships
            this.HasOptional(t => t.User_Profile)
                .WithMany(t => t.User_Profile_Photo)
                .HasForeignKey(d => d.Profile_ID);

        }
    }
}
