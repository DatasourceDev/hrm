using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSTimeModel.Models.Mapping
{
    public class Time_TransactionMap : EntityTypeConfiguration<Time_Transaction>
    {
        public Time_TransactionMap()
        {
            // Primary Key
            this.HasKey(t => t.Time_Transaction_ID);

            // Properties
            this.Property(t => t.Job_Code)
                .HasMaxLength(50);

            this.Property(t => t.Transaction_Type)
                .HasMaxLength(50);

            this.Property(t => t.Card_ID)
                .HasMaxLength(50);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.Employee_Name)
                .HasMaxLength(300);

            // Table & Column Mappings
            this.ToTable("Time_Transaction");
            this.Property(t => t.Time_Transaction_ID).HasColumnName("Time_Transaction_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Device_ID).HasColumnName("Device_ID");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Device_Transaction_ID).HasColumnName("Device_Transaction_ID");
            this.Property(t => t.Device_Transaction_Date).HasColumnName("Device_Transaction_Date");
            this.Property(t => t.Device_Employee_Pin).HasColumnName("Device_Employee_Pin");
            this.Property(t => t.Job_Code).HasColumnName("Job_Code");
            this.Property(t => t.Transaction_Type).HasColumnName("Transaction_Type");
            this.Property(t => t.Card_ID).HasColumnName("Card_ID");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Employee_Name).HasColumnName("Employee_Name");

            // Relationships
            this.HasOptional(t => t.Time_Device)
                .WithMany(t => t.Time_Transaction)
                .HasForeignKey(d => d.Device_ID);

        }
    }
}
