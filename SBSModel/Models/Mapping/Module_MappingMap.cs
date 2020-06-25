using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Module_MappingMap : EntityTypeConfiguration<Module_Mapping>
    {
        public Module_MappingMap()
        {
            // Primary Key
            this.HasKey(t => t.Mapping_ID);

            // Properties
            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Module_Mapping");
            this.Property(t => t.Mapping_ID).HasColumnName("Mapping_ID");
            this.Property(t => t.Product_ID).HasColumnName("Product_ID");
            this.Property(t => t.Module_Detail_ID).HasColumnName("Module_Detail_ID");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Promotion_ID).HasColumnName("Promotion_ID");
            this.Property(t => t.Module_ID).HasColumnName("Module_ID");
        }
    }
}
