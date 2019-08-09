using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Riql.Tests.TestData
{
    public class OrderQueryableGenerator : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var employees = TestSamples.GetEmployees();
            yield return new object[]
            {
                TestSamples.GetOrders().Select(
                    (o) =>
                    {
                        o.Manager = employees.FirstOrDefault(e => e.Id == o.ManagerId);
                        return o;
                    }).AsQueryable(),
                new DisposableMock()
            };

            var (efQueryable, efConnection) = OrderQueryableGenerator.BuildOrdersEfContext();
            yield return new object[] {efQueryable, efConnection};

            var (linq2dbQueryable, linq2dbConnection) = OrderQueryableGenerator.BuildOrdersLinq2DbContext();
            yield return new object[] {linq2dbQueryable, linq2dbConnection};
        }

        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private static (DbSet<Order>, IDisposable) BuildOrdersEfContext()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            try
            {
                connection.Open();
                var options = new DbContextOptionsBuilder<OrdersDbContext>()
                              .UseSqlite(connection)
                              .Options;
                var context = new OrdersDbContext(options);
                var databaseCreator = (RelationalDatabaseCreator) context.Database.GetService<IDatabaseCreator>();
                databaseCreator.CreateTables();
                context.Employees.AddRange(TestSamples.GetEmployees());
                context.Orders.AddRange(TestSamples.GetOrders());
                context.SaveChanges();
                return (context.Orders, connection);
            }
            catch
            {
                connection.Dispose();
                throw;
            }
        }

        private static (IQueryable<Order>, IDisposable) BuildOrdersLinq2DbContext()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            try
            {
                connection.Open();

                var linq2Db = new OrderLinq2Db(connection);
                linq2Db.CreateTable<Employee>();
                foreach (var employee in TestSamples.GetEmployees())
                {
                    linq2Db.Insert(employee);
                }

                linq2Db.CreateTable<Order>();
                foreach (var order in TestSamples.GetOrders())
                {
                    linq2Db.Insert(order);
                }

                return (linq2Db.Orders, connection);
            }
            catch
            {
                connection.Dispose();
                throw;
            }
        }

        public class OrdersDbContext : DbContext
        {
            public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
                : base(options)
            {
            }

            public DbSet<Order> Orders { get; set; }
            public DbSet<Employee> Employees { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                // Fluent API
                modelBuilder.Entity<Order>().HasIndex(a => a.Id);
                modelBuilder.Entity<Employee>().HasIndex(a => a.Id);
            }
        }

        public class OrderLinq2Db : DataConnection
        {
            public OrderLinq2Db(IDbConnection connection)
                : base(new SQLiteDataProvider(), connection)
            {
                this.MappingSchema.GetFluentMappingBuilder().Entity<Order>().Association(o => o.Manager, (o, e) => o.ManagerId == e.Id);
            }

            public ITable<Order> Orders => this.GetTable<Order>().LoadWith(o => o.Manager);
            public ITable<Employee> Employees => this.GetTable<Employee>();
        }
    }
}