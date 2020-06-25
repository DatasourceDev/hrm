using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Access_RightMap : EntityTypeConfiguration<Access_Right>
    {
        public Access_RightMap()
        {
            // Primary Key
            this.HasKey(t => t.Access_ID);

            // Properties
            this.Property(t => t.Access_Name)
                .HasMaxLength(100);

            this.Property(t => t.Access_Description)
                .HasMaxLength(500);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Access_Right");
            this.Property(t => t.Access_ID).HasColumnName("Access_ID");
            this.Property(t => t.Access_Name).HasColumnName("Access_Name");
            this.Property(t => t.Access_Description).HasColumnName("Access_Description");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
        }
    }
}
