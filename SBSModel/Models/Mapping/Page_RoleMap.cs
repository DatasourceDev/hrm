using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Page_RoleMap : EntityTypeConfiguration<Page_Role>
    {
        public Page_RoleMap()
        {
            // Primary Key
            this.HasKey(t => t.Page_Role_ID);

            // Properties
            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.Menu_Page_Code)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Page_Role");
            this.Property(t => t.Page_Role_ID).HasColumnName("Page_Role_ID");
            this.Property(t => t.User_Role_ID).HasColumnName("User_Role_ID");
            this.Property(t => t.Page_ID).HasColumnName("Page_ID");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Menu_Page_Code).HasColumnName("Menu_Page_Code");

            // Relationships
            this.HasOptional(t => t.Menu_Page)
                .WithMany(t => t.Page_Role)
                .HasForeignKey(d => d.Menu_Page_Code);
            this.HasOptional(t => t.Page)
                .WithMany(t => t.Page_Role)
                .HasForeignKey(d => d.Page_ID);
            this.HasRequired(t => t.User_Role)
                .WithMany(t => t.Page_Role)
                .HasForeignKey(d => d.User_Role_ID);

        }
    }
}
