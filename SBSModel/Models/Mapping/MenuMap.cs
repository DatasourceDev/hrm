using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class MenuMap : EntityTypeConfiguration<Menu>
    {
        public MenuMap()
        {
            // Primary Key
            this.HasKey(t => t.Menu_Code);

            // Properties
            this.Property(t => t.Menu_Code)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Menu_Name)
                .HasMaxLength(300);

            this.Property(t => t.Location)
                .HasMaxLength(50);

            this.Property(t => t.Action_Link)
                .HasMaxLength(300);

            this.Property(t => t.Keyword)
                .HasMaxLength(500);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Menu");
            this.Property(t => t.Menu_Code).HasColumnName("Menu_Code");
            this.Property(t => t.Menu_Name).HasColumnName("Menu_Name");
            this.Property(t => t.Location).HasColumnName("Location");
            this.Property(t => t.Action_Link).HasColumnName("Action_Link");
            this.Property(t => t.Keyword).HasColumnName("Keyword");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
        }
    }
}
