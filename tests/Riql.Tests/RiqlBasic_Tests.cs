using System;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Riql.Tests.TestData;
using Riql.Transpiler;
using Xunit;

namespace Riql.Tests
{
    public class RiqlBasic_Tests
    {
        private EquivalencyAssertionOptions<Employee> WithSameOrder(EquivalencyAssertionOptions<Employee> o)
        {
            return o.WithStrictOrdering();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(" \t \n ")]
        public void Apply_NullOrEmpty_ShouldFilterItems(string riql)
        {
            // Arrange
            var employees = TestSamples.GetEmployees().AsQueryable();

            // Act
            var result = employees.ApplyRiql(riql);

            // Assert
            result.Should().BeSameAs(employees);
        }

        [Theory]
        [ClassData(typeof(EmployeeQueryableGenerator))]
        public void Apply_Reduce_ShouldReduceData(IQueryable<Employee> employees, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var expected = (from e in employees select new Employee {FirstName = e.FirstName, LastName = e.LastName}).ToList();

                // Act
                var result = employees.ApplyRiql("$reduce=FirstName,LastName").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(EmployeeQueryableGenerator))]
        public void Apply_Skip_ShouldFilterItems(IQueryable<Employee> employees, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                const int skip = 3;
                var expected = (from e in employees select e).Skip(skip).ToList();

                // Act
                var result = employees.ApplyRiql($"$skip={skip}").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(EmployeeQueryableGenerator))]
        public void Apply_SkipTake_ShouldFilterItems(IQueryable<Employee> employees, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                const int skip = 3;
                const int take = 2;
                var expected = (from e in employees select e).Skip(skip).Take(take).ToList();

                // Act
                var result = employees.ApplyRiql($"$skip={skip}$take={take}").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(EmployeeQueryableGenerator))]
        public void Apply_Take_ShouldFilterItems(IQueryable<Employee> employees, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                const int take = 2;
                var expected = (from e in employees select e).Take(take).ToList();

                // Act
                var result = employees.ApplyRiql($"$take={take}").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData("foo")]
        [InlineData("0")]
        public void Apply_TakeNotAnPositiveInteger_ShouldThrowRequiresIntegerExeptions(string take)
        {
            // Arrange
            var employees = TestSamples.GetEmployees().AsQueryable();

            // Act
            Action act = () => employees.ApplyRiql($"$take={take}");

            // Assert
            act.Should().Throw<RequiresIntegerExeption>();
        }

        [Theory]
        [InlineData("")]
        [InlineData("foo")]
        [InlineData("-1")]
        public void Apply_SkipNotAnIntegerGreaterEqualZero_ShouldThrowRequiresIntegerExeptions(string skip)
        {
            // Arrange
            var employees = TestSamples.GetEmployees().AsQueryable();

            // Act
            Action act = () => employees.ApplyRiql($"$skip={skip}");

            // Assert
            act.Should().Throw<RequiresIntegerExeption>();
        }

        [Theory]
        [ClassData(typeof(EmployeeQueryableGenerator))]
        public void Apply_MaxTakeButNoTakeDefined_ShouldFilterItems(IQueryable<Employee> employees, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                const int maxTake = 3;
                var expected = (from e in employees select e).Skip(1).Take(maxTake).ToList();

                // Act
                var result = employees.ApplyRiql("$skip=1", maxTake).ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(EmployeeQueryableGenerator))]
        public void Apply_MaxTakeLimitsHigherTake_ShouldFilterItems(IQueryable<Employee> employees, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                const int maxTake = 4;
                var expected = (from e in employees select e).Take(maxTake).ToList();

                // Act
                var result = employees.ApplyRiql("$take=1337", maxTake).ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(EmployeeQueryableGenerator))]
        public void Apply_MaxTakeAllowsLowerTake_ShouldFilterItems(IQueryable<Employee> employees, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                const int maxTake = 4;
                var expected = (from e in employees select e).Take(2).ToList();

                // Act
                var result = employees.ApplyRiql("$take=2", maxTake).ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(EmployeeQueryableGenerator))]
        public void Apply_MaxTakeLimitsMissingTake_ShouldFilterItems(IQueryable<Employee> employees, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                const int maxTake = 4;
                var expected = (from e in employees select e).Take(maxTake).ToList();

                // Act
                var result = employees.ApplyRiql(null, maxTake).ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(EmployeeQueryableGenerator))]
        public void Apply_OrderBy_ShouldOrderItems(IQueryable<Employee> employees, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var expected = (from e in employees orderby e.RoomNr, e.Probation, e.Birthday select e).ToList();

                // Act
                var result = employees.ApplyRiql("$orderby=RoomNr,Probation,Birthday asc").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected, this.WithSameOrder);
            }
        }

        [Theory]
        [ClassData(typeof(EmployeeQueryableGenerator))]
        public void Apply_OrderByDesc_ShouldOrderItems(IQueryable<Employee> employees, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var expected = (from e in employees orderby e.RoomNr descending, e.Probation descending, e.Birthday descending select e).ToList();

                // Act
                var result = employees.ApplyRiql("$orderby=RoomNr desc,Probation desc,Birthday desc").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected, this.WithSameOrder);
            }
        }

        [Theory]
        [ClassData(typeof(EmployeeQueryableGenerator))]
        public void Apply_WhereAnd_ShouldFilterItems(IQueryable<Employee> employees, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var random = employees.Random();
                var expected = (from e in employees where e.FirstName == random.FirstName && e.LastName == random.LastName select e).ToList();

                // Act
                var result = employees.ApplyRiql($"$where=FirstName=={random.FirstName};LastName=={random.LastName}").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(EmployeeQueryableGenerator))]
        public void Apply_WhereBoolean_ShouldFilterItems(IQueryable<Employee> employees, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var expected = (from e in employees where e.Probation select e).ToList();

                // Act
                var result = employees.ApplyRiql("$where=Probation==true").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(EmployeeQueryableGenerator))]
        public void Apply_WhereDateTimeGreaterEqual_ShouldFilterItems(IQueryable<Employee> employees, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var dateTime = new DateTime(2001, 2, 3);
                var expected = (from e in employees where e.Birthday >= dateTime select e).ToList();

                // Act
                var result = employees.ApplyRiql($"$where=Birthday>={dateTime:o}").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(EmployeeQueryableGenerator))]
        public void Apply_WhereDecimalGreaterEqual_ShouldFilterItems(IQueryable<Employee> employees, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var expected = (from e in employees where e.Salary >= 10000 select e).ToList();

                // Act
                var result = employees.ApplyRiql($"$where=Salary>={10000}").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(EmployeeQueryableGenerator))]
        public void Apply_WhereDoubleLessEqual_ShouldFilterItems(IQueryable<Employee> employees, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var expected = (from e in employees where e.Height <= 1.82 select e).ToList();

                // Act
                var result = employees.ApplyRiql($"$where=Height<={1.82}").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(EmployeeQueryableGenerator))]
        public void Apply_WhereEnum_ShouldFilterItems(IQueryable<Employee> employees, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var expected = (from e in employees where e.Gender == Gender.Female select e).ToList();

                // Act
                var result = employees.ApplyRiql($"$where=Gender=={Gender.Female}").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(EmployeeQueryableGenerator))]
        public void Apply_WhereFloatLessEqual_ShouldFilterItems(IQueryable<Employee> employees, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var expected = (from e in employees where e.Weight <= 76 select e).ToList();

                // Act
                var result = employees.ApplyRiql($"$where=Weight<={76}").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(EmployeeQueryableGenerator))]
        public void Apply_WhereGuidEqual_ShouldFilterItems(IQueryable<Employee> employees, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var random = employees.Random();
                var expected = (from e in employees where e.Id == random.Id select e).ToList();

                // Act
                var result = employees.ApplyRiql($"$where=Id=={random.Id}").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(EmployeeQueryableGenerator))]
        public void Apply_WhereInt_ShouldFilterItems(IQueryable<Employee> employees, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var expected = (from e in employees where e.RoomNr == 105 select e).ToList();

                // Act
                var result = employees.ApplyRiql($"$where=RoomNr=={105}").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(EmployeeQueryableGenerator))]
        public void Apply_WhereOr_ShouldFilterItems(IQueryable<Employee> employees, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var random = employees.Random();
                var expected = (from e in employees where e.FirstName == random.FirstName || e.LastName == random.LastName select e).ToList();

                // Act
                var result = employees.ApplyRiql($"$where=FirstName=={random.FirstName},LastName=={random.LastName}").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(EmployeeQueryableGenerator))]
        public void Apply_WhereStringContains_ShouldFilterItems(IQueryable<Employee> employees, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var expected = (from e in employees where e.FirstName.Contains("o") select e).ToList();

                // Act
                var result = employees.ApplyRiql("$where=FirstName==*o*").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(EmployeeQueryableGenerator))]
        public void Apply_WhereStringEndsWith_ShouldFilterItems(IQueryable<Employee> employees, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var expected = (from e in employees where e.FirstName.EndsWith("e") select e).ToList();

                // Act
                var result = employees.ApplyRiql("$where=FirstName==*e").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory]
        [ClassData(typeof(EmployeeQueryableGenerator))]
        public void Apply_WhereStringStartsWith_ShouldFilterItems(IQueryable<Employee> employees, IDisposable lifetime)
        {
            using (lifetime)
            {
                // Arrange
                var expected = (from e in employees where e.FirstName.StartsWith("J") select e).ToList();

                // Act
                var result = employees.ApplyRiql("$where=FirstName==J*").ToList();

                // Assert
                result.Should().NotBeEmpty();
                result.Should().HaveSameCount(expected);
                result.Should().BeEquivalentTo(expected);
            }
        }
    }
}