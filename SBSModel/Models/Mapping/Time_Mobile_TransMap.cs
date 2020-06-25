using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Time_Mobile_TransMap : EntityTypeConfiguration<Time_Mobile_Trans>
    {
        public Time_Mobile_TransMap()
        {
            // Primary Key
            this.HasKey(t => t.Trans_ID);

            // Properties
            this.Property(t => t.UUID)
                .HasMaxLength(30);

            this.Property(t => t.Note)
                .HasMaxLength(500);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Time_Mobile_Trans");
            this.Property(t => t.Trans_ID).HasColumnName("Trans_ID");
            this.Property(t => t.Map_ID).HasColumnName("Map_ID");
            this.Property(t => t.UUID).HasColumnName("UUID");
            this.Property(t => t.Trans_Date).HasColumnName("Trans_Date");
            this.Property(t => t.Clock_In).HasColumnName("Clock_In");
            this.Property(t => t.Clock_Out).HasColumnName("Clock_Out");
            this.Property(t => t.Note).HasColumnName("Note");
            this.Property(t => t.Duration).HasColumnName("Duration");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");

            // Relationships
            this.HasOptional(t => t.Time_Mobile_Map)
                .WithMany(t => t.Time_Mobile_Trans)
                .HasForeignKey(d => d.Map_ID);

        }
    }
}
