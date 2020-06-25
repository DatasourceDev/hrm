using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Expenses_CalculationMap : EntityTypeConfiguration<Expenses_Calculation>
    {
        public Expenses_CalculationMap()
        {
            // Primary Key
            this.HasKey(t => t.Calculation_ID);

            // Properties
            this.Property(t => t.Year_Assigned)
                .HasMaxLength(4);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Expenses_Calculation");
            this.Property(t => t.Calculation_ID).HasColumnName("Calculation_ID");
            this.Property(t => t.Expenses_Config_ID).HasColumnName("Expenses_Config_ID");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Entitle).HasColumnName("Entitle");
            this.Property(t => t.Amount_Used).HasColumnName("Amount_Used");
            this.Property(t => t.Expiry_Date).HasColumnName("Expiry_Date");
            this.Property(t => t.Year_Assigned).HasColumnName("Year_Assigned");
            this.Property(t => t.Start_Date).HasColumnName("Start_Date");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasOptional(t => t.Employee_Profile)
                .WithMany(t => t.Expenses_Calculation)
                .HasForeignKey(d => d.Employee_Profile_ID);
            this.HasOptional(t => t.Expenses_Config)
                .WithMany(t => t.Expenses_Calculation)
                .HasForeignKey(d => d.Expenses_Config_ID);

        }
    }
}
