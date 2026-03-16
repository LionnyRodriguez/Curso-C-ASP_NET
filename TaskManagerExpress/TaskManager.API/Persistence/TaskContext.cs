using TaskManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Task = TaskManager.Domain.Entities.Task; //To avoid the ambiguous reference between my domain's Task class
                                               //and the Task class from System.Threading.Tasks.


namespace TaskManager.API.Persistence
{
    public sealed class TaskContext : DbContext
    {
        public DbSet<Task> Tasks => Set<Task>();
        public TaskContext() { }
        public TaskContext(string connectionString): base(GetOptions(connectionString)) { }
        public TaskContext(DbContextOptions<TaskContext> options):base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Task>().HasKey(x => x.Id);
        }

        #region Helpers
        private static DbContextOptions GetOptions(string connectionString)
        {
            return new DbContextOptionsBuilder().UseNpgsql(connectionString).Options;
        }
        #endregion
    }
}
