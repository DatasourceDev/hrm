using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class PRELMap : EntityTypeConfiguration<PREL>
    {
        public PRELMap()
        {
            // Primary Key
            this.HasKey(t => t.PREL_ID);

            // Properties
            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("PREL");
            this.Property(t => t.PREL_ID).HasColumnName("PREL_ID");
            this.Property(t => t.PRG_ID).HasColumnName("PRG_ID");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasRequired(t => t.Employee_Profile)
                .WithMany(t => t.PRELs)
                .HasForeignKey(d => d.Employee_Profile_ID);
            this.HasRequired(t => t.PRG)
                .WithMany(t => t.PRELs)
                .HasForeignKey(d => d.PRG_ID);

        }
    }
}
