using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class CPF_FormulaMap : EntityTypeConfiguration<CPF_Formula>
    {
        public CPF_FormulaMap()
        {
            // Primary Key
            this.HasKey(t => t.CPF_Formula_ID);

            // Properties
            this.Property(t => t.Formula_Name)
                .HasMaxLength(500);

            this.Property(t => t.Formula_Desc)
                .HasMaxLength(500);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.Record_Status)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("CPF_Formula");
            this.Property(t => t.CPF_Formula_ID).HasColumnName("CPF_Formula_ID");
            this.Property(t => t.Formula).HasColumnName("Formula");
            this.Property(t => t.Formula_Name).HasColumnName("Formula_Name");
            this.Property(t => t.Formula_Desc).HasColumnName("Formula_Desc");
            this.Property(t => t.Year).HasColumnName("Year");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Record_Status).HasColumnName("Record_Status");
        }
    }
}
