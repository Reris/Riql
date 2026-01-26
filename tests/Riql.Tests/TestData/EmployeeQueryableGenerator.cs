using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Riql.Tests.TestData;

public class EmployeeQueryableGenerator : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { TestSamples.GetEmployees().AsQueryable(), new DisposableMock() };

        var (efQueryable, efConnection) = BuildEmployeesEfContext();
        yield return new object[] { efQueryable, efConnection };

        var (linq2dbQueryable, linq2dbConnection) = BuildEmployeesLinq2DbContext();
        yield return new object[] { linq2dbQueryable, linq2dbConnection };
    }

    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    private static (IQueryable<Employee>, IDisposable) BuildEmployeesEfContext()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        try
        {
            connection.Open();
            var options = new DbContextOptionsBuilder<EmployeeDbContext>()
                          .UseSqlite(connection)
                          .Options;
            var context = new EmployeeDbContext(options);
            var databaseCreator = (RelationalDatabaseCreator)context.Database.GetService<IDatabaseCreator>();
            databaseCreator.CreateTables();
            context.Employees.AddRange(TestSamples.GetEmployees());
            context.SaveChanges();
            return (context.Employees, connection);
        }
        catch
        {
            connection.Dispose();
            throw;
        }
    }

    private static (IQueryable<Employee>, IDisposable) BuildEmployeesLinq2DbContext()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        try
        {
            connection.Open();

            var linq2Db = new EmployeeLinq2Db(connection);
            linq2Db.CreateTable<Employee>();
            foreach (var employee in TestSamples.GetEmployees())
            {
                linq2Db.Insert(employee);
            }

            return (linq2Db.Employees, connection);
        }
        catch
        {
            connection.Dispose();
            throw;
        }
    }

    public class EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : DbContext(options)
    {
        public DbSet<Employee> Employees { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Fluent API
            modelBuilder.Entity<Employee>().HasIndex(a => a.Id);
            modelBuilder.Entity<Employee>().Property(a => a.Salary).HasConversion<double>();
        }
    }

    public class EmployeeLinq2Db(DbConnection connection)
        : DataConnection(new DataOptions().UseConnection(SQLiteTools.GetDataProvider(SQLiteProvider.Microsoft), connection))
    {
        public ITable<Employee> Employees => this.GetTable<Employee>();
    }
}