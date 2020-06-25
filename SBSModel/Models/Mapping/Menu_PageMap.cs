using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Menu_PageMap : EntityTypeConfiguration<Menu_Page>
    {
        public Menu_PageMap()
        {
            // Primary Key
            this.HasKey(t => t.Menu_Page_Code);

            // Properties
            this.Property(t => t.Menu_Page_Code)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Menu_Code)
                .HasMaxLength(50);

            this.Property(t => t.Menu_Page_Name)
                .HasMaxLength(300);

            this.Property(t => t.Action_Link)
                .HasMaxLength(300);

            this.Property(t => t.Keyword)
                .HasMaxLength(500);

            this.Property(t => t.Page_Action)
                .HasMaxLength(300);

            this.Property(t => t.Page_Controller)
                .HasMaxLength(300);

            this.Property(t => t.Domain_Name)
                .HasMaxLength(300);

            this.Property(t => t.operation)
                .HasMaxLength(50);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Menu_Page");
            this.Property(t => t.Menu_Page_Code).HasColumnName("Menu_Page_Code");
            this.Property(t => t.Menu_Code).HasColumnName("Menu_Code");
            this.Property(t => t.Menu_Page_Name).HasColumnName("Menu_Page_Name");
            this.Property(t => t.Action_Link).HasColumnName("Action_Link");
            this.Property(t => t.Keyword).HasColumnName("Keyword");
            this.Property(t => t.Page_Action).HasColumnName("Page_Action");
            this.Property(t => t.Page_Controller).HasColumnName("Page_Controller");
            this.Property(t => t.Domain_Name).HasColumnName("Domain_Name");
            this.Property(t => t.operation).HasColumnName("operation");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasOptional(t => t.Menu)
                .WithMany(t => t.Menu_Page)
                .HasForeignKey(d => d.Menu_Code);

        }
    }
}
