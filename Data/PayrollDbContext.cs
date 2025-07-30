using Microsoft.EntityFrameworkCore;
using PayrollandOnsiteExpenses.Models;
using PayrollandOnsiteExpenses.Models.ReportsPage;

namespace PayrollandOnsiteExpenses.Data
{
    public class PayrollDbContext : DbContext
    {
        public PayrollDbContext(DbContextOptions<PayrollDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeQualification> EmployeeQualifications { get; set; }

        public DbSet<ProjectTable> ProjectTable { get; set; }

        public DbSet<AssignedProjectEmployee> AssignedProjectEmployees { get; set; }

        public DbSet<PayrollModel> PayrollTable { get; set; }

        public DbSet<ExpensesClaimReport> ExpensesClaimReports { get; set; }

        public DbSet<ExpenseApprovalSummary> ExpenseApprovalSummary { get; set; }

        public DbSet<PFRegistration> PFRegistrations { get; set; }

        public DbSet<EmployeePayslipModel> EmployeePayslipModels { get; set; }

        public DbSet<FullPayrollDetailsModel> FullPayrollDetails { get; set; }

        public DbSet<PayslipGenerateModel> PayslipGenerate { get; set; }

        public DbSet<PendingApprovalsModel> PendingApprovals { get; set; }

    }
}
