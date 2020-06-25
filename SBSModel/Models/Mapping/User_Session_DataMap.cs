using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class User_Session_DataMap : EntityTypeConfiguration<User_Session_Data>
    {
        public User_Session_DataMap()
        {
            // Primary Key
            this.HasKey(t => t.User_Session_ID);

            // Properties
            this.Property(t => t.Session_ID)
                .HasMaxLength(150);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("User_Session_Data");
            this.Property(t => t.User_Session_ID).HasColumnName("User_Session_ID");
            this.Property(t => t.Session_ID).HasColumnName("Session_ID");
            this.Property(t => t.Profile_ID).HasColumnName("Profile_ID");
            this.Property(t => t.Session_Data).HasColumnName("Session_Data");
            this.Property(t => t.Actived).HasColumnName("Actived");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasOptional(t => t.User_Profile)
                .WithMany(t => t.User_Session_Data)
                .HasForeignKey(d => d.Profile_ID);

        }
    }
}
