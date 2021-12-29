using FilesStorage.DAO.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilesStorage.DAO.DbContext
{
    /// <summary> Класс с контекстом для паспортов </summary>
    internal class SqlContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<Passport> Passports { get; set; }
        public SqlContext(DbContextOptions<SqlContext> options) : base(options) => Database.EnsureCreated();
    }
}
