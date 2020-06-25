using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class User_RoleMap : EntityTypeConfiguration<User_Role>
    {
        public User_RoleMap()
        {
            // Primary Key
            this.HasKey(t => t.User_Role_ID);

            // Properties
            this.Property(t => t.Role_Name)
                .HasMaxLength(100);

            this.Property(t => t.Role_Description)
                .HasMaxLength(1000);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("User_Role");
            this.Property(t => t.User_Role_ID).HasColumnName("User_Role_ID");
            this.Property(t => t.Role_Name).HasColumnName("Role_Name");
            this.Property(t => t.Role_Description).HasColumnName("Role_Description");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
        }
    }
}
