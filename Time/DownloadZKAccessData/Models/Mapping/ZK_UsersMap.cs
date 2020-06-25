using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DownloadZKAccessData.Models.Mapping
{
    public class ZK_UsersMap : EntityTypeConfiguration<ZK_Users>
    {
        public ZK_UsersMap()
        {
            // Primary Key
            this.HasKey(t => t.ZK_Users_ID);

            // Properties
            this.Property(t => t.User_Name)
                .HasMaxLength(150);

            this.Property(t => t.User_Level)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.User_Status)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("ZK_Users");
            this.Property(t => t.Enroll_ID).HasColumnName("Enroll_ID");
            this.Property(t => t.User_Name).HasColumnName("User_Name");
            this.Property(t => t.User_Level).HasColumnName("User_Level");
            this.Property(t => t.User_Status).HasColumnName("User_Status");
            this.Property(t => t.User_Pin).HasColumnName("User_Pin");
            this.Property(t => t.Device_ID).HasColumnName("Device_ID");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.ZK_Users_ID).HasColumnName("ZK_Users_ID");
        }
    }
}
