using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class OT_FormulaMap : EntityTypeConfiguration<OT_Formula>
    {
        public OT_FormulaMap()
        {
            // Primary Key
            this.HasKey(t => t.OT_Formula_ID);

            // Properties
            this.Property(t => t.Formula_Name)
                .HasMaxLength(500);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("OT_Formula");
            this.Property(t => t.OT_Formula_ID).HasColumnName("OT_Formula_ID");
            this.Property(t => t.Formula).HasColumnName("Formula");
            this.Property(t => t.Formula_Name).HasColumnName("Formula_Name");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
        }
    }
}
