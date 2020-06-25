using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class PRDLMap : EntityTypeConfiguration<PRDL>
    {
        public PRDLMap()
        {
            // Primary Key
            this.HasKey(t => t.PRDL_ID);

            // Properties
            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("PRDL");
            this.Property(t => t.PRDL_ID).HasColumnName("PRDL_ID");
            this.Property(t => t.PRM_ID).HasColumnName("PRM_ID");
            this.Property(t => t.Leave_Application_Document_ID).HasColumnName("Leave_Application_Document_ID");
            this.Property(t => t.Process_Day).HasColumnName("Process_Day");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.Start_Date).HasColumnName("Start_Date");
            this.Property(t => t.End_Date).HasColumnName("End_Date");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasRequired(t => t.Leave_Application_Document)
                .WithMany(t => t.PRDLs)
                .HasForeignKey(d => d.Leave_Application_Document_ID);
            this.HasRequired(t => t.PRM)
                .WithMany(t => t.PRDLs)
                .HasForeignKey(d => d.PRM_ID);

        }
    }
}
