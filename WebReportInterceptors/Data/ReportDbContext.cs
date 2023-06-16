using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace WebReport.Data {
    public class ReportItem {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public byte[] LayoutData { get; set; }
    }

    public class ReportDbContext : DbContext {
        public DbSet<ReportItem> Reports { get; set; }
        public ReportDbContext(DbContextOptions<ReportDbContext> options) : base(options) {
        }
        public void InitializeDatabase() {
            Database.EnsureCreated();
        }
    }
}
