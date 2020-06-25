using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class SBS_ModuleMap : EntityTypeConfiguration<SBS_Module>
    {
        public SBS_ModuleMap()
        {
            // Primary Key
            this.HasKey(t => t.Module_ID);

            // Properties
            this.Property(t => t.Module_Name)
                .HasMaxLength(100);

            this.Property(t => t.Module_Description)
                .HasMaxLength(500);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("SBS_Module");
            this.Property(t => t.Module_ID).HasColumnName("Module_ID");
            this.Property(t => t.Module_Name).HasColumnName("Module_Name");
            this.Property(t => t.Module_Description).HasColumnName("Module_Description");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
        }
    }
}
