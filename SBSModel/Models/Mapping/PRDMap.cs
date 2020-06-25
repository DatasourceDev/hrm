using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class PRDMap : EntityTypeConfiguration<PRD>
    {
        public PRDMap()
        {
            // Primary Key
            this.HasKey(t => t.Payroll_Detail_ID);

            // Properties
            this.Property(t => t.Description)
                .HasMaxLength(500);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("PRD");
            this.Property(t => t.Payroll_Detail_ID).HasColumnName("Payroll_Detail_ID");
            this.Property(t => t.PRM_ID).HasColumnName("PRM_ID");
            this.Property(t => t.PRT_ID).HasColumnName("PRT_ID");
            this.Property(t => t.PRC_ID).HasColumnName("PRC_ID");
            this.Property(t => t.Currency_ID).HasColumnName("Currency_ID");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Hours_Worked).HasColumnName("Hours_Worked");
            this.Property(t => t.Employment_History_Allowance_ID).HasColumnName("Employment_History_Allowance_ID");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasOptional(t => t.Currency)
                .WithMany(t => t.PRDs)
                .HasForeignKey(d => d.Currency_ID);
            this.HasOptional(t => t.Employment_History_Allowance)
                .WithMany(t => t.PRDs)
                .HasForeignKey(d => d.Employment_History_Allowance_ID);
            this.HasOptional(t => t.PRC)
                .WithMany(t => t.PRDs)
                .HasForeignKey(d => d.PRC_ID);
            this.HasOptional(t => t.PRM)
                .WithMany(t => t.PRDs)
                .HasForeignKey(d => d.PRM_ID);
            this.HasOptional(t => t.PRT)
                .WithMany(t => t.PRDs)
                .HasForeignKey(d => d.PRT_ID);

        }
    }
}
