using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using SBSWorkFlowAPI.Models.Mapping;

namespace SBSWorkFlowAPI.Models
{
    public partial class WorkflowDBContext : DbContext
    {
        static WorkflowDBContext()
        {
            Database.SetInitializer<WorkflowDBContext>(null);
        }

        public WorkflowDBContext()
            : base("Name=WorkflowDBContext")
        {
        }

        public DbSet<Applicable_Employee> Applicable_Employee { get; set; }
        public DbSet<Approval_Flow> Approval_Flow { get; set; }
        public DbSet<Approver> Approvers { get; set; }
        public DbSet<Condition> Conditions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<History> Histories { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Reviewer> Reviewers { get; set; }
        public DbSet<Task_Assignment> Task_Assignment { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new Applicable_EmployeeMap());
            modelBuilder.Configurations.Add(new Approval_FlowMap());
            modelBuilder.Configurations.Add(new ApproverMap());
            modelBuilder.Configurations.Add(new ConditionMap());
            modelBuilder.Configurations.Add(new DepartmentMap());
            modelBuilder.Configurations.Add(new HistoryMap());
            modelBuilder.Configurations.Add(new RequestMap());
            modelBuilder.Configurations.Add(new ReviewerMap());
            modelBuilder.Configurations.Add(new Task_AssignmentMap());
        }
    }
}
