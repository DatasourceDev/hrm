using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Time_Sheet_DtlMap : EntityTypeConfiguration<Time_Sheet_Dtl>
    {
        public Time_Sheet_DtlMap()
        {
            // Primary Key
            this.HasKey(t => t.Dtl_ID);

            // Properties
            this.Property(t => t.Note)
                .HasMaxLength(300);

            this.Property(t => t.Indent_No)
                .HasMaxLength(150);

            this.Property(t => t.Indent_Name)
                .HasMaxLength(300);

            this.Property(t => t.Customer_Name)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Time_Sheet_Dtl");
            this.Property(t => t.Dtl_ID).HasColumnName("Dtl_ID");
            this.Property(t => t.Time_Sheet_ID).HasColumnName("Time_Sheet_ID");
            this.Property(t => t.Date_Of_Date).HasColumnName("Date_Of_Date");
            this.Property(t => t.Clock_In).HasColumnName("Clock_In");
            this.Property(t => t.Clock_Out).HasColumnName("Clock_Out");
            this.Property(t => t.Note).HasColumnName("Note");
            this.Property(t => t.Job_Cost_ID).HasColumnName("Job_Cost_ID");
            this.Property(t => t.Indent_No).HasColumnName("Indent_No");
            this.Property(t => t.Indent_Name).HasColumnName("Indent_Name");
            this.Property(t => t.Customer_Name).HasColumnName("Customer_Name");
            this.Property(t => t.Hour_Rate).HasColumnName("Hour_Rate");
            this.Property(t => t.Total_Amount).HasColumnName("Total_Amount");
            this.Property(t => t.Launch_Duration).HasColumnName("Launch_Duration");
            this.Property(t => t.Duration).HasColumnName("Duration");

            // Relationships
            this.HasOptional(t => t.Job_Cost)
                .WithMany(t => t.Time_Sheet_Dtl)
                .HasForeignKey(d => d.Job_Cost_ID);
            this.HasOptional(t => t.Time_Sheet)
                .WithMany(t => t.Time_Sheet_Dtl)
                .HasForeignKey(d => d.Time_Sheet_ID);

        }
    }
}
