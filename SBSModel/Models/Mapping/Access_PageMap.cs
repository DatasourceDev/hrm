using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Access_PageMap : EntityTypeConfiguration<Access_Page>
    {
        public Access_PageMap()
        {
            // Primary Key
            this.HasKey(t => t.Access_Page_ID);

            // Properties
            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Access_Page");
            this.Property(t => t.Access_Page_ID).HasColumnName("Access_Page_ID");
            this.Property(t => t.Access_ID).HasColumnName("Access_ID");
            this.Property(t => t.Page_Role_ID).HasColumnName("Page_Role_ID");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasRequired(t => t.Access_Right)
                .WithMany(t => t.Access_Page)
                .HasForeignKey(d => d.Access_ID);
            this.HasRequired(t => t.Page_Role)
                .WithMany(t => t.Access_Page)
                .HasForeignKey(d => d.Page_Role_ID);

        }
    }
}
