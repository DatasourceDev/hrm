using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Expenses_CategoryMap : EntityTypeConfiguration<Expenses_Category>
    {
        public Expenses_CategoryMap()
        {
            // Primary Key
            this.HasKey(t => t.Expenses_Category_ID);

            // Properties
            this.Property(t => t.Category_Name)
                .HasMaxLength(150);

            this.Property(t => t.Category_Description)
                .HasMaxLength(500);

            this.Property(t => t.Record_Status)
                .HasMaxLength(50);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Expenses_Category");
            this.Property(t => t.Expenses_Category_ID).HasColumnName("Expenses_Category_ID");
            this.Property(t => t.Category_Name).HasColumnName("Category_Name");
            this.Property(t => t.Category_Description).HasColumnName("Category_Description");
            this.Property(t => t.Record_Status).HasColumnName("Record_Status");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasOptional(t => t.Company)
                .WithMany(t => t.Expenses_Category)
                .HasForeignKey(d => d.Company_ID);

        }
    }
}
