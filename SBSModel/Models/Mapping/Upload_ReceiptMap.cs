using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Upload_ReceiptMap : EntityTypeConfiguration<Upload_Receipt>
    {
        public Upload_ReceiptMap()
        {
            // Primary Key
            this.HasKey(t => t.Upload_Receipt_ID);

            // Properties
            this.Property(t => t.File_Name)
                .HasMaxLength(300);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Upload_Receipt");
            this.Property(t => t.Upload_Receipt_ID).HasColumnName("Upload_Receipt_ID");
            this.Property(t => t.Receipt).HasColumnName("Receipt");
            this.Property(t => t.File_Name).HasColumnName("File_Name");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Expenses_Application_Document_ID).HasColumnName("Expenses_Application_Document_ID");

            // Relationships
            this.HasOptional(t => t.Expenses_Application_Document)
                .WithMany(t => t.Upload_Receipt)
                .HasForeignKey(d => d.Expenses_Application_Document_ID);

        }
    }
}
