using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ClassPlanner.Data;

public partial class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Teacher> Teacher { get; set; }
    public DbSet<Subject> Subject { get; set; }
    public DbSet<Classroom> Classroom { get; set; }

    public async Task CreateDatabaseAsync()
    {
        string createTablesCommandText = @"-- Criação da tabela Classroom
CREATE TABLE IF NOT EXISTS Classroom (
    ClassroomId INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL
);

-- Criação da tabela Teacher
CREATE TABLE IF NOT EXISTS Teacher (
    TeacherId INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL
);

-- Criação da tabela Subject
CREATE TABLE IF NOT EXISTS Subject (
    SubjectId INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    PeriodsPerWeek INTEGER NOT NULL,
    ClassroomId INTEGER NOT NULL,
    TeacherId INTEGER,
    FOREIGN KEY (ClassroomId) REFERENCES Classroom(ClassroomId) ON DELETE CASCADE,
    FOREIGN KEY (TeacherId) REFERENCES Teacher(TeacherId) ON DELETE SET NULL
);
";


        await Database.ExecuteSqlRawAsync(createTablesCommandText);
    }
}
