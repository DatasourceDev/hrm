using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class BranchMap : EntityTypeConfiguration<Branch>
    {
        public BranchMap()
        {
            // Primary Key
            this.HasKey(t => t.Branch_ID);

            // Properties
            this.Property(t => t.Branch_Code)
                .HasMaxLength(50);

            this.Property(t => t.Branch_Name)
                .HasMaxLength(300);

            this.Property(t => t.Branch_Desc)
                .HasMaxLength(500);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.Record_Status)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Branch");
            this.Property(t => t.Branch_ID).HasColumnName("Branch_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Branch_Code).HasColumnName("Branch_Code");
            this.Property(t => t.Branch_Name).HasColumnName("Branch_Name");
            this.Property(t => t.Branch_Desc).HasColumnName("Branch_Desc");
            this.Property(t => t.ST_Sun_Time).HasColumnName("ST_Sun_Time");
            this.Property(t => t.ST_Mon_Time).HasColumnName("ST_Mon_Time");
            this.Property(t => t.ST_Tue_Time).HasColumnName("ST_Tue_Time");
            this.Property(t => t.ST_Wed_Time).HasColumnName("ST_Wed_Time");
            this.Property(t => t.ST_Thu_Time).HasColumnName("ST_Thu_Time");
            this.Property(t => t.ST_Fri_Time).HasColumnName("ST_Fri_Time");
            this.Property(t => t.ST_Sat_Time).HasColumnName("ST_Sat_Time");
            this.Property(t => t.ET_Sun_Time).HasColumnName("ET_Sun_Time");
            this.Property(t => t.ET_Mon_Time).HasColumnName("ET_Mon_Time");
            this.Property(t => t.ET_Tue_Time).HasColumnName("ET_Tue_Time");
            this.Property(t => t.ET_Wed_Time).HasColumnName("ET_Wed_Time");
            this.Property(t => t.ET_Thu_Time).HasColumnName("ET_Thu_Time");
            this.Property(t => t.ET_Fri_Time).HasColumnName("ET_Fri_Time");
            this.Property(t => t.ET_Sat_Time).HasColumnName("ET_Sat_Time");
            this.Property(t => t.CL_Sun).HasColumnName("CL_Sun");
            this.Property(t => t.CL_Mon).HasColumnName("CL_Mon");
            this.Property(t => t.CL_Tue).HasColumnName("CL_Tue");
            this.Property(t => t.CL_Wed).HasColumnName("CL_Wed");
            this.Property(t => t.CL_Thu).HasColumnName("CL_Thu");
            this.Property(t => t.CL_Fri).HasColumnName("CL_Fri");
            this.Property(t => t.CL_Sat).HasColumnName("CL_Sat");
            this.Property(t => t.ST_Same).HasColumnName("ST_Same");
            this.Property(t => t.ET_Same).HasColumnName("ET_Same");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.ET_Time).HasColumnName("ET_Time");
            this.Property(t => t.ST_Time).HasColumnName("ST_Time");
            this.Property(t => t.Record_Status).HasColumnName("Record_Status");

            // Relationships
            this.HasRequired(t => t.Company)
                .WithMany(t => t.Branches)
                .HasForeignKey(d => d.Company_ID);

        }
    }
}
