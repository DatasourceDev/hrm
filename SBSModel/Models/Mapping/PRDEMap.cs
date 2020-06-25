using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class PRDEMap : EntityTypeConfiguration<PRDE>
    {
        public PRDEMap()
        {
            // Primary Key
            this.HasKey(t => t.PRDE_ID);

            // Properties
            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("PRDE");
            this.Property(t => t.PRDE_ID).HasColumnName("PRDE_ID");
            this.Property(t => t.PRM_ID).HasColumnName("PRM_ID");
            this.Property(t => t.Expenses_Application_Document_ID).HasColumnName("Expenses_Application_Document_ID");
            this.Property(t => t.Expenses_Date).HasColumnName("Expenses_Date");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasRequired(t => t.Expenses_Application_Document)
                .WithMany(t => t.PRDEs)
                .HasForeignKey(d => d.Expenses_Application_Document_ID);
            this.HasRequired(t => t.PRM)
                .WithMany(t => t.PRDEs)
                .HasForeignKey(d => d.PRM_ID);

        }
    }
}
