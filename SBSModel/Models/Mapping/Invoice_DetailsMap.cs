using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Invoice_DetailsMap : EntityTypeConfiguration<Invoice_Details>
    {
        public Invoice_DetailsMap()
        {
            // Primary Key
            this.HasKey(t => t.Invoice_Detail_ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("Invoice_Details");
            this.Property(t => t.Invoice_Detail_ID).HasColumnName("Invoice_Detail_ID");
            this.Property(t => t.Invoice_ID).HasColumnName("Invoice_ID");
            this.Property(t => t.Module_ID).HasColumnName("Module_ID");
            this.Property(t => t.AddOn_ID).HasColumnName("AddOn_ID");

            // Relationships
            this.HasRequired(t => t.Invoice_Header)
                .WithMany(t => t.Invoice_Details)
                .HasForeignKey(d => d.Invoice_ID);

        }
    }
}
