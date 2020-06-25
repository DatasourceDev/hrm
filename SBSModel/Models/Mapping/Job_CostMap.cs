using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Job_CostMap : EntityTypeConfiguration<Job_Cost>
    {
        public Job_CostMap()
        {
            // Primary Key
            this.HasKey(t => t.Job_Cost_ID);

            // Properties
            this.Property(t => t.Indent_No)
                .HasMaxLength(150);

            this.Property(t => t.Indent_Name)
                .HasMaxLength(300);

            this.Property(t => t.Record_Status)
                .HasMaxLength(50);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Job_Cost");
            this.Property(t => t.Job_Cost_ID).HasColumnName("Job_Cost_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Indent_No).HasColumnName("Indent_No");
            this.Property(t => t.Indent_Name).HasColumnName("Indent_Name");
            this.Property(t => t.Customer_ID).HasColumnName("Customer_ID");
            this.Property(t => t.Date_Of_Date).HasColumnName("Date_Of_Date");
            this.Property(t => t.Sell_Price).HasColumnName("Sell_Price");
            this.Property(t => t.Delivery_Date).HasColumnName("Delivery_Date");
            this.Property(t => t.Term_Of_Deliver).HasColumnName("Term_Of_Deliver");
            this.Property(t => t.Warranty_Term).HasColumnName("Warranty_Term");
            this.Property(t => t.Costing).HasColumnName("Costing");
            this.Property(t => t.Record_Status).HasColumnName("Record_Status");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Using).HasColumnName("Using");
            this.Property(t => t.Supervisor).HasColumnName("Supervisor");

            // Relationships
            this.HasOptional(t => t.Company)
                .WithMany(t => t.Job_Cost)
                .HasForeignKey(d => d.Company_ID);
            this.HasOptional(t => t.Customer)
                .WithMany(t => t.Job_Cost)
                .HasForeignKey(d => d.Customer_ID);
            this.HasOptional(t => t.Employee_Profile)
                .WithMany(t => t.Job_Cost)
                .HasForeignKey(d => d.Supervisor);

        }
    }
}
