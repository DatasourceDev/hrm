using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class ModuleMap : EntityTypeConfiguration<Module>
    {
        public ModuleMap()
        {
            // Primary Key
            this.HasKey(t => t.Module_Code);

            // Properties
            this.Property(t => t.Module_Code)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Module_Name)
                .HasMaxLength(100);

            this.Property(t => t.Module_Description)
                .HasMaxLength(500);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Module");
            this.Property(t => t.Module_Code).HasColumnName("Module_Code");
            this.Property(t => t.Module_Name).HasColumnName("Module_Name");
            this.Property(t => t.Module_Description).HasColumnName("Module_Description");
            this.Property(t => t.Price).HasColumnName("Price");
            this.Property(t => t.Price_Per_Person).HasColumnName("Price_Per_Person");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
        }
    }
}
