using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DownloadZKAccessData.Models.Mapping
{
    public class Time_DeviceMap : EntityTypeConfiguration<Time_Device>
    {
        public Time_DeviceMap()
        {
            // Primary Key
            this.HasKey(t => t.Device_ID);

            // Properties
            this.Property(t => t.Device_No)
                .HasMaxLength(150);

            this.Property(t => t.IP_Address)
                .HasMaxLength(150);

            this.Property(t => t.User_Name)
                .HasMaxLength(150);

            this.Property(t => t.Password)
                .HasMaxLength(300);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.Record_Status)
                .HasMaxLength(50);

            this.Property(t => t.Branch_Name)
                .HasMaxLength(150);

            this.Property(t => t.Brand_Name)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Time_Device");
            this.Property(t => t.Device_ID).HasColumnName("Device_ID");
            this.Property(t => t.Branch_ID).HasColumnName("Branch_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Device_No).HasColumnName("Device_No");
            this.Property(t => t.IP_Address).HasColumnName("IP_Address");
            this.Property(t => t.Port).HasColumnName("Port");
            this.Property(t => t.User_Name).HasColumnName("User_Name");
            this.Property(t => t.Password).HasColumnName("Password");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Record_Status).HasColumnName("Record_Status");
            this.Property(t => t.Branch_Name).HasColumnName("Branch_Name");
            this.Property(t => t.Min_Transaction_Id).HasColumnName("Min_Transaction_Id");
            this.Property(t => t.Max_Transaction_Id).HasColumnName("Max_Transaction_Id");
            this.Property(t => t.Brand_Name).HasColumnName("Brand_Name");
        }
    }
}
