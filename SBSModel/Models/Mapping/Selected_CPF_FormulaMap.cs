using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Selected_CPF_FormulaMap : EntityTypeConfiguration<Selected_CPF_Formula>
    {
        public Selected_CPF_FormulaMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Selected_CPF_Formula");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.CPF_Formula_ID).HasColumnName("CPF_Formula_ID");
            this.Property(t => t.Effective_Date).HasColumnName("Effective_Date");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasRequired(t => t.Company)
                .WithMany(t => t.Selected_CPF_Formula)
                .HasForeignKey(d => d.Company_ID);
            this.HasRequired(t => t.CPF_Formula)
                .WithMany(t => t.Selected_CPF_Formula)
                .HasForeignKey(d => d.CPF_Formula_ID);

        }
    }
}
