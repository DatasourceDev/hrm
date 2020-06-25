using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Holiday_ConfigMap : EntityTypeConfiguration<Holiday_Config>
    {
        public Holiday_ConfigMap()
        {
            // Primary Key
            this.HasKey(t => t.Holiday_ID);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(150);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.Record_Status)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Holiday_Config");
            this.Property(t => t.Holiday_ID).HasColumnName("Holiday_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Holiday_Lenght).HasColumnName("Holiday_Lenght");
            this.Property(t => t.Start_Date).HasColumnName("Start_Date");
            this.Property(t => t.End_Date).HasColumnName("End_Date");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Record_Status).HasColumnName("Record_Status");

            // Relationships
            this.HasOptional(t => t.Company)
                .WithMany(t => t.Holiday_Config)
                .HasForeignKey(d => d.Company_ID);
            this.HasOptional(t => t.Global_Lookup_Data)
                .WithMany(t => t.Holiday_Config)
                .HasForeignKey(d => d.Holiday_Lenght);

        }
    }
}
