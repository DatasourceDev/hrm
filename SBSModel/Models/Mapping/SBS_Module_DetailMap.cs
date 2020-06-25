using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class SBS_Module_DetailMap : EntityTypeConfiguration<SBS_Module_Detail>
    {
        public SBS_Module_DetailMap()
        {
            // Primary Key
            this.HasKey(t => t.Module_Detail_ID);

            // Properties
            this.Property(t => t.Module_Detail_Name)
                .HasMaxLength(100);

            this.Property(t => t.Module_Detail_Description)
                .HasMaxLength(500);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("SBS_Module_Detail");
            this.Property(t => t.Module_Detail_ID).HasColumnName("Module_Detail_ID");
            this.Property(t => t.Module_ID).HasColumnName("Module_ID");
            this.Property(t => t.Module_Detail_Name).HasColumnName("Module_Detail_Name");
            this.Property(t => t.Module_Detail_Description).HasColumnName("Module_Detail_Description");
            this.Property(t => t.Price).HasColumnName("Price");
            this.Property(t => t.Price_Per_Person).HasColumnName("Price_Per_Person");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Order_Index).HasColumnName("Order_Index");

            // Relationships
            this.HasRequired(t => t.SBS_Module)
                .WithMany(t => t.SBS_Module_Detail)
                .HasForeignKey(d => d.Module_ID);

        }
    }
}
