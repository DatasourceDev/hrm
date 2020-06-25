using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class User_Assign_RoleMap : EntityTypeConfiguration<User_Assign_Role>
    {
        public User_Assign_RoleMap()
        {
            // Primary Key
            this.HasKey(t => t.User_Assign_Role_ID);

            // Properties
            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("User_Assign_Role");
            this.Property(t => t.User_Assign_Role_ID).HasColumnName("User_Assign_Role_ID");
            this.Property(t => t.User_Role_ID).HasColumnName("User_Role_ID");
            this.Property(t => t.User_Authentication_ID).HasColumnName("User_Authentication_ID");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasOptional(t => t.User_Authentication)
                .WithMany(t => t.User_Assign_Role)
                .HasForeignKey(d => d.User_Authentication_ID);
            this.HasOptional(t => t.User_Role)
                .WithMany(t => t.User_Assign_Role)
                .HasForeignKey(d => d.User_Role_ID);

        }
    }
}
