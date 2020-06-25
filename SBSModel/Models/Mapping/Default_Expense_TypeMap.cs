using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Default_Expense_TypeMap : EntityTypeConfiguration<Default_Expense_Type>
    {
        public Default_Expense_TypeMap()
        {
            // Primary Key
            this.HasKey(t => t.Default_Expense_Type_ID);

            // Properties
            this.Property(t => t.Expense_Type_Name)
                .HasMaxLength(50);

            this.Property(t => t.Expense_Type_Desc)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("Default_Expense_Type");
            this.Property(t => t.Default_Expense_Type_ID).HasColumnName("Default_Expense_Type_ID");
            this.Property(t => t.Expense_Category_ID).HasColumnName("Expense_Category_ID");
            this.Property(t => t.Expense_Type_Name).HasColumnName("Expense_Type_Name");
            this.Property(t => t.Expense_Type_Desc).HasColumnName("Expense_Type_Desc");

            // Relationships
            this.HasRequired(t => t.Global_Lookup_Data)
                .WithMany(t => t.Default_Expense_Type)
                .HasForeignKey(d => d.Expense_Category_ID);

        }
    }
}
