using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class TsEXMap : EntityTypeConfiguration<TsEX>
    {
        public TsEXMap()
        {
            // Primary Key
            this.HasKey(t => t.TsEx_ID);

            // Properties
            this.Property(t => t.Doc_No)
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("TsEX");
            this.Property(t => t.TsEx_ID).HasColumnName("TsEx_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Doc_No).HasColumnName("Doc_No");
            this.Property(t => t.Doc_Date).HasColumnName("Doc_Date");
            this.Property(t => t.Month).HasColumnName("Month");
            this.Property(t => t.Year).HasColumnName("Year");
            this.Property(t => t.Expenses_Application_ID).HasColumnName("Expenses_Application_ID");
            this.Property(t => t.Time_Sheet_ID).HasColumnName("Time_Sheet_ID");
            this.Property(t => t.Ex_Total_Amount).HasColumnName("Ex_Total_Amount");
            this.Property(t => t.Ts_Total_Amount).HasColumnName("Ts_Total_Amount");

            // Relationships
            this.HasOptional(t => t.Company)
                .WithMany(t => t.TsEXes)
                .HasForeignKey(d => d.Company_ID);
            this.HasOptional(t => t.Employee_Profile)
                .WithMany(t => t.TsEXes)
                .HasForeignKey(d => d.Employee_Profile_ID);
            this.HasOptional(t => t.Expenses_Application)
                .WithMany(t => t.TsEXes)
                .HasForeignKey(d => d.Expenses_Application_ID);
            this.HasOptional(t => t.Time_Sheet)
                .WithMany(t => t.TsEXes)
                .HasForeignKey(d => d.Time_Sheet_ID);

        }
    }
}
