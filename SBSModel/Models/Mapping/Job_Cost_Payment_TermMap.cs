using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Job_Cost_Payment_TermMap : EntityTypeConfiguration<Job_Cost_Payment_Term>
    {
        public Job_Cost_Payment_TermMap()
        {
            // Primary Key
            this.HasKey(t => t.Job_Cost_PayMent_Term_ID);

            // Properties
            this.Property(t => t.Payment_Type)
                .HasMaxLength(10);

            this.Property(t => t.Invoice_No)
                .HasMaxLength(150);

            this.Property(t => t.Note)
                .HasMaxLength(300);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Job_Cost_Payment_Term");
            this.Property(t => t.Job_Cost_PayMent_Term_ID).HasColumnName("Job_Cost_PayMent_Term_ID");
            this.Property(t => t.Job_Cost_ID).HasColumnName("Job_Cost_ID");
            this.Property(t => t.Payment).HasColumnName("Payment");
            this.Property(t => t.Payment_Type).HasColumnName("Payment_Type");
            this.Property(t => t.Invoice_No).HasColumnName("Invoice_No");
            this.Property(t => t.Invoice_Date).HasColumnName("Invoice_Date");
            this.Property(t => t.Note).HasColumnName("Note");
            this.Property(t => t.Actual_Price).HasColumnName("Actual_Price");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasOptional(t => t.Job_Cost)
                .WithMany(t => t.Job_Cost_Payment_Term)
                .HasForeignKey(d => d.Job_Cost_ID);

        }
    }
}
