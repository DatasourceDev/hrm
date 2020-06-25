using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Ctl_Table_SynMap : EntityTypeConfiguration<Ctl_Table_Syn>
    {
        public Ctl_Table_SynMap()
        {
            // Primary Key
            this.HasKey(t => t.Table_Name);

            // Properties
            this.Property(t => t.Table_Name)
                .IsRequired()
                .HasMaxLength(300);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Ctl_Table_Syn");
            this.Property(t => t.Table_Name).HasColumnName("Table_Name");
            this.Property(t => t.Index).HasColumnName("Index");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
        }
    }
}
