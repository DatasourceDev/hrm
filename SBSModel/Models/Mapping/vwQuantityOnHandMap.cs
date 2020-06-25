using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class vwQuantityOnHandMap : EntityTypeConfiguration<vwQuantityOnHand>
    {
        public vwQuantityOnHandMap()
        {
            // Primary Key
            this.HasKey(t => new { t.Product_Category_ID, t.Product_ID });

            // Properties
            this.Property(t => t.Product_Category_ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Category_Name)
                .HasMaxLength(150);

            this.Property(t => t.Product_ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Product_Code)
                .HasMaxLength(50);

            this.Property(t => t.Product_Name)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("vwQuantityOnHand");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Product_Category_ID).HasColumnName("Product_Category_ID");
            this.Property(t => t.Category_Name).HasColumnName("Category_Name");
            this.Property(t => t.Product_ID).HasColumnName("Product_ID");
            this.Property(t => t.Product_Code).HasColumnName("Product_Code");
            this.Property(t => t.Product_Name).HasColumnName("Product_Name");
            this.Property(t => t.InvIN).HasColumnName("InvIN");
            this.Property(t => t.InvOUT).HasColumnName("InvOUT");
            this.Property(t => t.POSOUT).HasColumnName("POSOUT");
            this.Property(t => t.QoH).HasColumnName("QoH");
        }
    }
}
