using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class PREDLMap : EntityTypeConfiguration<PREDL>
    {
        public PREDLMap()
        {
            // Primary Key
            this.HasKey(t => t.PREDL_ID);

            // Properties
            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("PREDL");
            this.Property(t => t.PREDL_ID).HasColumnName("PREDL_ID");
            this.Property(t => t.PRG_ID).HasColumnName("PRG_ID");
            this.Property(t => t.Department_ID).HasColumnName("Department_ID");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasRequired(t => t.Department)
                .WithMany(t => t.PREDLs)
                .HasForeignKey(d => d.Department_ID);
            this.HasRequired(t => t.PRG)
                .WithMany(t => t.PREDLs)
                .HasForeignKey(d => d.PRG_ID);

        }
    }
}
