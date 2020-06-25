using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Expenses_No_PatternMap : EntityTypeConfiguration<Expenses_No_Pattern>
    {
        public Expenses_No_PatternMap()
        {
            // Primary Key
            this.HasKey(t => t.Expenses_No_Pattern_ID);

            // Properties
            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Expenses_No_Pattern");
            this.Property(t => t.Expenses_No_Pattern_ID).HasColumnName("Expenses_No_Pattern_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Current_Running_Number).HasColumnName("Current_Running_Number");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasOptional(t => t.Company)
                .WithMany(t => t.Expenses_No_Pattern)
                .HasForeignKey(d => d.Company_ID);

        }
    }
}
