using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Employment_HistoryMap : EntityTypeConfiguration<Employment_History>
    {
        public Employment_HistoryMap()
        {
            // Primary Key
            this.HasKey(t => t.History_ID);

            // Properties
            this.Property(t => t.Record_Status)
                .HasMaxLength(50);

            this.Property(t => t.Basic_Salary)
                .HasMaxLength(150);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.CPF_AC_No)
                .HasMaxLength(150);

            this.Property(t => t.Notice_Period_Unit)
                .HasMaxLength(50);

            this.Property(t => t.Basic_Salary_Unit)
                .HasMaxLength(50);

            this.Property(t => t.Other_Branch)
                .IsFixedLength()
                .HasMaxLength(200);

            this.Property(t => t.Other_Department)
                .IsFixedLength()
                .HasMaxLength(200);

            this.Property(t => t.Other_Designation)
                .IsFixedLength()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Employment_History");
            this.Property(t => t.History_ID).HasColumnName("History_ID");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Department_ID).HasColumnName("Department_ID");
            this.Property(t => t.Designation_ID).HasColumnName("Designation_ID");
            this.Property(t => t.Currency_ID).HasColumnName("Currency_ID");
            this.Property(t => t.Record_Status).HasColumnName("Record_Status");
            this.Property(t => t.Effective_Date).HasColumnName("Effective_Date");
            this.Property(t => t.Supervisor).HasColumnName("Supervisor");
            this.Property(t => t.Branch_ID).HasColumnName("Branch_ID");
            this.Property(t => t.Confirm_Date).HasColumnName("Confirm_Date");
            this.Property(t => t.Terminate_Date).HasColumnName("Terminate_Date");
            this.Property(t => t.Basic_Salary).HasColumnName("Basic_Salary");
            this.Property(t => t.Payment_Type).HasColumnName("Payment_Type");
            this.Property(t => t.Employee_Type).HasColumnName("Employee_Type");
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
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Hired_Date).HasColumnName("Hired_Date");
            this.Property(t => t.Bonus_Factor).HasColumnName("Bonus_Factor");
            this.Property(t => t.Days_Worked_WK).HasColumnName("Days_Worked_WK");
            this.Property(t => t.Hours_Worked_YR).HasColumnName("Hours_Worked_YR");
            this.Property(t => t.CPF_AC_No).HasColumnName("CPF_AC_No");
            this.Property(t => t.Daily_Rate).HasColumnName("Daily_Rate");
            this.Property(t => t.CPF_Type).HasColumnName("CPF_Type");
            this.Property(t => t.CL_The).HasColumnName("CL_The");
            this.Property(t => t.NPL_Daily).HasColumnName("NPL_Daily");
            this.Property(t => t.Notice_Period_Amount).HasColumnName("Notice_Period_Amount");
            this.Property(t => t.Notice_Period_Unit).HasColumnName("Notice_Period_Unit");
            this.Property(t => t.Contract_Staff).HasColumnName("Contract_Staff");
            this.Property(t => t.Contract_Start_Date).HasColumnName("Contract_Start_Date");
            this.Property(t => t.Contract_End_Date).HasColumnName("Contract_End_Date");
            this.Property(t => t.Basic_Salary_Unit).HasColumnName("Basic_Salary_Unit");
            this.Property(t => t.Days).HasColumnName("Days");
            this.Property(t => t.Employee_Grading_ID).HasColumnName("Employee_Grading_ID");
            this.Property(t => t.Other_Branch).HasColumnName("Other_Branch");
            this.Property(t => t.Other_Department).HasColumnName("Other_Department");
            this.Property(t => t.Other_Designation).HasColumnName("Other_Designation");
            this.Property(t => t.No_Approval_WF).HasColumnName("No_Approval_WF");
            this.Property(t => t.CL_Lunch).HasColumnName("CL_Lunch");
            this.Property(t => t.ST_Lunch_Time).HasColumnName("ST_Lunch_Time");
            this.Property(t => t.ET_Lunch_Time).HasColumnName("ET_Lunch_Time");
            this.Property(t => t.Hour_Rate).HasColumnName("Hour_Rate");

            // Relationships
            this.HasOptional(t => t.Branch)
                .WithMany(t => t.Employment_History)
                .HasForeignKey(d => d.Branch_ID);
            this.HasOptional(t => t.Currency)
                .WithMany(t => t.Employment_History)
                .HasForeignKey(d => d.Currency_ID);
            this.HasOptional(t => t.Department)
                .WithMany(t => t.Employment_History)
                .HasForeignKey(d => d.Department_ID);
            this.HasOptional(t => t.Designation)
                .WithMany(t => t.Employment_History)
                .HasForeignKey(d => d.Designation_ID);
            this.HasRequired(t => t.Employee_Profile)
                .WithMany(t => t.Employment_History)
                .HasForeignKey(d => d.Employee_Profile_ID);
            this.HasOptional(t => t.Employee_Profile1)
                .WithMany(t => t.Employment_History1)
                .HasForeignKey(d => d.Supervisor);
            this.HasOptional(t => t.Global_Lookup_Data)
                .WithMany(t => t.Employment_History)
                .HasForeignKey(d => d.Payment_Type);

        }
    }
}
