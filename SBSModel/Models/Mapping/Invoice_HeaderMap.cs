using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Invoice_HeaderMap : EntityTypeConfiguration<Invoice_Header>
    {
        public Invoice_HeaderMap()
        {
            // Primary Key
            this.HasKey(t => t.Invoice_ID);

            // Properties
            this.Property(t => t.Invoice_No)
                .HasMaxLength(50);

            this.Property(t => t.Invoice_Status)
                .HasMaxLength(20);

            this.Property(t => t.Paid_By)
                .HasMaxLength(50);

            this.Property(t => t.Invoice_To)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Invoice_Header");
            this.Property(t => t.Invoice_ID).HasColumnName("Invoice_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Invoice_No).HasColumnName("Invoice_No");
            this.Property(t => t.Due_Amount).HasColumnName("Due_Amount");
            this.Property(t => t.Invoice_Month).HasColumnName("Invoice_Month");
            this.Property(t => t.Invoice_Year).HasColumnName("Invoice_Year");
            this.Property(t => t.Generated_On).HasColumnName("Generated_On");
            this.Property(t => t.Invoice_Status).HasColumnName("Invoice_Status");
            this.Property(t => t.Paid_On).HasColumnName("Paid_On");
            this.Property(t => t.Paid_By).HasColumnName("Paid_By");
            this.Property(t => t.Invoice_To).HasColumnName("Invoice_To");

            // Relationships
            this.HasRequired(t => t.Company)
                .WithMany(t => t.Invoice_Header)
                .HasForeignKey(d => d.Company_ID);

        }
    }
}
