using System;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Riql.Tests.TestData;
using Xunit;

namespace Riql.Tests
{
    public class RiqlComplex_Tests
    {
        private EquivalencyAssertionOptions<Order> WithSameOrder(EquivalencyAssertionOptions<Order> o)
        {
            return o.WithStrictOrdering();
        }

        [Theory]
        [ClassData(typeof(OrderQueryableGenerator))]
        public void Apply_SkipTakeThenWhere_ShouldFilterItems(IQueryable<Order> orders, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var expected = (from o in orders.Skip(3).Take(5) where o.Accepted == null select o).ToList();

                // Act
                var result = orders.ApplyRiql("$s=3 $t=5 $w=Accepted=is-null=true").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(OrderQueryableGenerator))]
        public void Apply_WhereThenSkipTake_ShouldFilterItems(IQueryable<Order> orders, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var expected = (from o in orders where o.Accepted == null select o).Skip(3).Take(5).ToList();

                // Act
                var result = orders.ApplyRiql("$w=Accepted=is-null=true $s=3 $t=5").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(OrderQueryableGenerator))]
        public void Apply_WhereThenOrderByThenTakeThenWhere_ShouldFilterItems(IQueryable<Order> orders, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var expected = (from o in orders where o.Started != null orderby o.Price descending select o).Take(6).Where(o => o.Accepted == true).ToList();

                // Act
                var result = orders.ApplyRiql("$w=Started=nil=false $o=Price desc $t=6 $w=Accepted==true").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected, this.WithSameOrder);
            }
        }

        [Theory]
        [ClassData(typeof(OrderQueryableGenerator))]
        public void Apply_WhereStartedYear_ShouldFilterItems(IQueryable<Order> orders, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var expected = (from o in orders where o.Started != null && o.Started.Value.Year == 2019 select o).ToList();

                // Act
                var result = orders.ApplyRiql("$w=Started.Year==2019").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(OrderQueryableGenerator))]
        public void Apply_WhereStartedYearIsNull_ShouldFilterItems(IQueryable<Order> orders, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var expected = (from o in orders where o.Started == null select o).ToList();

                // Act
                var result = orders.ApplyRiql("$w=Started.Year=nil=true").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(OrderQueryableGenerator))]
        public void Apply_WhereFlagsEqual_ShouldFilterItems(IQueryable<Order> orders, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var expected = (from o in orders where o.Flags == OrderFlags.Important select o).ToList();

                // Act
                var result = orders.ApplyRiql("$w=Flags==Important").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(OrderQueryableGenerator))]
        public void Apply_WhereManagerRoomNr_ShouldFilterItems(IQueryable<Order> orders, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var expected = (from o in orders where o.Manager != null && o.Manager.RoomNr == 105 select o).ToList();

                // Act
                var result = orders.ApplyRiql("$w=Manager.RoomNr==105").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(OrderQueryableGenerator))]
        public void Apply_WhereGroupFirst_ShouldFilterItems(IQueryable<Order> orders, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var expected = (from o in orders
                                where o.Manager != null && (o.Manager.RoomNr == 101 || o.Manager.Salary >= 5700 && o.Manager.Salary < 10000) && o.Price < 100
                                select o).ToList();

                // Act
                var result = orders.ApplyRiql("$w=(Manager.RoomNr==101,Manager.Salary >= 5700 ; Manager.Salary < 10000);Price < 100").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(OrderQueryableGenerator))]
        public void Apply_WhereGroupInFirst_ShouldFilterItems(IQueryable<Order> orders, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var firstNames = new[] {"Jack", "Jane"};
                var expected = (from o in orders
                                where o.Manager != null && (firstNames.Contains(o.Manager.FirstName) || o.Manager.Salary > 8000) && o.Price > 0
                                select o).ToList();

                // Act
                var result = orders.ApplyRiql("$w=(Manager.FirstName=in=('Jack','Jane'),Manager.Salary > 8000);Price > 0").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(OrderQueryableGenerator))]
        public void Apply_PriorizeAndOverOr_ShouldFilterItems(IQueryable<Order> orders, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var expected = (from o in orders
                                where o.Manager != null && (o.Manager.RoomNr == 101 || o.Manager.Salary >= 5700 && o.Manager.Salary < 10000)
                                select o).ToList();

                // Act
                var result = orders.ApplyRiql("$w=Manager.Salary >= 5700 ; Manager.Salary < 10000, Manager.RoomNr==101").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(OrderQueryableGenerator))]
        public void Apply_WhereManagerRoomNrWithWhitespaces_ShouldFilterItems(IQueryable<Order> orders, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var expected = (from o in orders where o.Manager != null && o.Manager.RoomNr == 105 select o).ToList();

                // Act
                var result = orders.ApplyRiql("$w= Manager . RoomNr == 105").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(OrderQueryableGenerator))]
        public void Apply_WhereManagerFirstNameStartsWith_ShouldFilterItems(IQueryable<Order> orders, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var expected = (from o in orders where o.Manager != null && o.Manager.FirstName.StartsWith("J") select o).ToList();

                // Act
                var result = orders.ApplyRiql("$w=Manager.FirstName==J*").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(OrderQueryableGenerator))]
        public void Apply_WhereMultipleTimes_ShouldFilterItems(IQueryable<Order> orders, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var min = new DateTime(2014, 1, 1);
                var max = new DateTime(2019, 1, 1);
                var expected = (from o in orders where o.Started >= min select o).Where(o => o.Started < max).ToList();

                // Act
                var result = orders.ApplyRiql($"$w=Started>={min:yyyy-MM-dd}$w=Started<{max:o}").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(OrderQueryableGenerator))]
        public void Apply_WhereLike_ShouldRespectWhitespace(IQueryable<Order> orders, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var expected = (from o in orders where o.Requirement != null && o.Requirement.Contains("do 1") select o).ToList();

                // Act
                var result = orders.ApplyRiql("$w=Requirement=='*do 1*'").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(OrderQueryableGenerator))]
        public void Apply_WhereProjectionLevel1_ShouldFilterProjection(IQueryable<Order> orders, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var projection = orders.Select(o => new {o.Id, o.Requirement, o.Manager});
                var expected = (from p in projection where p.Manager != null && p.Manager.RoomNr == 105 select p).ToList();

                // Act
                var result = projection.ApplyRiql("$w=Manager.RoomNr==105").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(OrderQueryableGenerator))]
        public void Apply_WhereProjectionLevel2_ShouldFilterProjection(IQueryable<Order> orders, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var projection = orders.Select(
                    o => new
                    {
                        o.Id,
                        o.Requirement,
                        ManagerInfo = o.Manager != null
                                          ? new {o.Manager.FirstName, Room = o.Manager.RoomNr}
                                          : null
                    });
                var expected = (from p in projection where p.ManagerInfo != null && p.ManagerInfo.Room == 105 select p).ToList();

                // Act
                var result = projection.ApplyRiql("$w=ManagerInfo.Room==105").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }
    }
}