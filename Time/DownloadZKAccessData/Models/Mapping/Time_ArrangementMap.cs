using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DownloadZKAccessData.Models.Mapping
{
    public class Time_ArrangementMap : EntityTypeConfiguration<Time_Arrangement>
    {
        public Time_ArrangementMap()
        {
            // Primary Key
            this.HasKey(t => t.Arrangement_ID);

            // Properties
            this.Property(t => t.Remark)
                .HasMaxLength(500);

            this.Property(t => t.Day_Of_Week)
                .HasMaxLength(50);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Time_Arrangement");
            this.Property(t => t.Arrangement_ID).HasColumnName("Arrangement_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Branch_ID).HasColumnName("Branch_ID");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.Repeat).HasColumnName("Repeat");
            this.Property(t => t.Day_Of_Week).HasColumnName("Day_Of_Week");
            this.Property(t => t.Effective_Date).HasColumnName("Effective_Date");
            this.Property(t => t.Time_From).HasColumnName("Time_From");
            this.Property(t => t.Time_To).HasColumnName("Time_To");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
        }
    }
}
