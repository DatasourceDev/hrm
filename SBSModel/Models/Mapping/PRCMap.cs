using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class PRCMap : EntityTypeConfiguration<PRC>
    {
        public PRCMap()
        {
            // Primary Key
            this.HasKey(t => t.PRC_ID);

            // Properties
            this.Property(t => t.Description)
                .HasMaxLength(500);

            this.Property(t => t.Is_System)
                .HasMaxLength(1);

            this.Property(t => t.Record_Status)
                .HasMaxLength(50);

            this.Property(t => t.Name)
                .HasMaxLength(150);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("PRC");
            this.Property(t => t.PRC_ID).HasColumnName("PRC_ID");
            this.Property(t => t.PRT_ID).HasColumnName("PRT_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Is_System).HasColumnName("Is_System");
            this.Property(t => t.Record_Status).HasColumnName("Record_Status");
            this.Property(t => t.OT_Multiplier).HasColumnName("OT_Multiplier");
            this.Property(t => t.CPF_Deductable).HasColumnName("CPF_Deductable");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasRequired(t => t.Company)
                .WithMany(t => t.PRCs)
                .HasForeignKey(d => d.Company_ID);
            this.HasRequired(t => t.PRT)
                .WithMany(t => t.PRCs)
                .HasForeignKey(d => d.PRT_ID);

        }
    }
}
