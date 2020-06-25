using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class PageMap : EntityTypeConfiguration<Page>
    {
        public PageMap()
        {
            // Primary Key
            this.HasKey(t => t.Page_ID);

            // Properties
            this.Property(t => t.Page_Url)
                .HasMaxLength(150);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.Page_Name)
                .HasMaxLength(500);

            this.Property(t => t.Position)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Page");
            this.Property(t => t.Page_ID).HasColumnName("Page_ID");
            this.Property(t => t.Page_Url).HasColumnName("Page_Url");
            this.Property(t => t.Module_Detail_ID).HasColumnName("Module_Detail_ID");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Page_Name).HasColumnName("Page_Name");
            this.Property(t => t.Page_Attempt).HasColumnName("Page_Attempt");
            this.Property(t => t.Position).HasColumnName("Position");
            this.Property(t => t.Displayed).HasColumnName("Displayed");
            this.Property(t => t.Order_Index).HasColumnName("Order_Index");
            this.Property(t => t.Is_Indent).HasColumnName("Is_Indent");

            // Relationships
            this.HasOptional(t => t.SBS_Module_Detail)
                .WithMany(t => t.Pages)
                .HasForeignKey(d => d.Module_Detail_ID);

        }
    }
}
