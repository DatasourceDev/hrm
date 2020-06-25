using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DownloadZKAccessData.Models.Mapping
{
    public class Time_Device_MapMap : EntityTypeConfiguration<Time_Device_Map>
    {
        public Time_Device_MapMap()
        {
            // Primary Key
            this.HasKey(t => t.Map_ID);

            // Properties
            this.Property(t => t.Employee_Email)
                .HasMaxLength(300);

            this.Property(t => t.Device_Employee_Name)
                .HasMaxLength(300);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.Record_Status)
                .HasMaxLength(50);

            this.Property(t => t.Employee_Name)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Time_Device_Map");
            this.Property(t => t.Map_ID).HasColumnName("Map_ID");
            this.Property(t => t.Device_ID).HasColumnName("Device_ID");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Employee_Email).HasColumnName("Employee_Email");
            this.Property(t => t.Device_Employee_Pin).HasColumnName("Device_Employee_Pin");
            this.Property(t => t.Device_Employee_Name).HasColumnName("Device_Employee_Name");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Record_Status).HasColumnName("Record_Status");
            this.Property(t => t.Employee_Name).HasColumnName("Employee_Name");

            // Relationships
            this.HasOptional(t => t.Time_Device)
                .WithMany(t => t.Time_Device_Map)
                .HasForeignKey(d => d.Device_ID);

        }
    }
}
