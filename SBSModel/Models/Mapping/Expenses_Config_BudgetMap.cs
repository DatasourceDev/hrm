using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Expenses_Config_BudgetMap : EntityTypeConfiguration<Expenses_Config_Budget>
    {
        public Expenses_Config_BudgetMap()
        {
            // Primary Key
            this.HasKey(t => t.Budget_ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("Expenses_Config_Budget");
            this.Property(t => t.Budget_ID).HasColumnName("Budget_ID");
            this.Property(t => t.Expenses_Config_ID).HasColumnName("Expenses_Config_ID");
            this.Property(t => t.Job_Cost_ID).HasColumnName("Job_Cost_ID");
            this.Property(t => t.Budget).HasColumnName("Budget");

            // Relationships
            this.HasOptional(t => t.Expenses_Config)
                .WithMany(t => t.Expenses_Config_Budget)
                .HasForeignKey(d => d.Expenses_Config_ID);
            this.HasOptional(t => t.Job_Cost)
                .WithMany(t => t.Expenses_Config_Budget)
                .HasForeignKey(d => d.Job_Cost_ID);

        }
    }
}
