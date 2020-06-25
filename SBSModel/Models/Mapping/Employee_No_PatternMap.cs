using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Employee_No_PatternMap : EntityTypeConfiguration<Employee_No_Pattern>
    {
        public Employee_No_PatternMap()
        {
            // Primary Key
            this.HasKey(t => t.Employee_No_Pattern_ID);

            // Properties
            this.Property(t => t.Company_Code)
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("Employee_No_Pattern");
            this.Property(t => t.Employee_No_Pattern_ID).HasColumnName("Employee_No_Pattern_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Select_Nationality).HasColumnName("Select_Nationality");
            this.Property(t => t.Select_Year).HasColumnName("Select_Year");
            this.Property(t => t.Year_2_Digit).HasColumnName("Year_2_Digit");
            this.Property(t => t.Year_4_Digit).HasColumnName("Year_4_Digit");
            this.Property(t => t.Select_Company_code).HasColumnName("Select_Company_code");
            this.Property(t => t.Company_Code).HasColumnName("Company_Code");
            this.Property(t => t.Current_Running_Number).HasColumnName("Current_Running_Number");
            this.Property(t => t.Select_Branch_Code).HasColumnName("Select_Branch_Code");
            this.Property(t => t.Branch_ID).HasColumnName("Branch_ID");
            this.Property(t => t.Initiated).HasColumnName("Initiated");

            // Relationships
            this.HasOptional(t => t.Branch)
                .WithMany(t => t.Employee_No_Pattern)
                .HasForeignKey(d => d.Branch_ID);
            this.HasRequired(t => t.Company)
                .WithMany(t => t.Employee_No_Pattern)
                .HasForeignKey(d => d.Company_ID);

        }
    }
}
