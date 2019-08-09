using System.Linq;
using FluentAssertions;
using Riql.Tests.TestData;
using Xunit;

namespace Riql.Tests
{
    public class RiqlWhere_Tests
    {
        private Unit[] GetUnits()
        {
            return new[]
            {
                new Unit(1, false),
                new Unit(2, true),
                new Unit(3, false)
                {
                    Inner = new Unit(13, false)
                },
                new Unit(4, true)
                {
                    Inner = new Unit(14, true)
                },
                new Unit(5, false)
                {
                    Inner = new Unit(15, false)
                    {
                        Inner = new Unit(25, false)
                    }
                },
                new Unit(6, true)
                {
                    Inner = new Unit(16, true)
                    {
                        Inner = new Unit(26, true)
                    }
                },
                new Unit(7, false)
                {
                    Inner = new Unit(17, false)
                    {
                        Inner = new Unit(27, false)
                        {
                            Inner = new Unit(37, false)
                        }
                    }
                },
                new Unit(8, true)
                {
                    Inner = new Unit(18, true)
                    {
                        Inner = new Unit(28, true)
                        {
                            Inner = new Unit(38, true)
                        }
                    }
                }
            };
        }

        private Unit[] GetBoolUnits()
        {
            return new[]
            {
                new Unit(1, true)
                {
                    Bool = true,
                    BoolN = true,
                    Inner = new Unit(11, true)
                    {
                        Bool = true,
                        BoolN = true,
                        Inner = new Unit(21, true)
                        {
                            Bool = true,
                            BoolN = true
                        }
                    }
                },
                new Unit(2, true)
                {
                    Bool = false,
                    BoolN = false,
                    Inner = new Unit(12, true)
                    {
                        Bool = false,
                        BoolN = false,
                        Inner = new Unit(22, true)
                        {
                            Bool = false,
                            BoolN = false,
                            Inner = new Unit(32, true)
                            {
                                Bool = false,
                                BoolN = false
                            }
                        }
                    }
                },
                new Unit(3, true)
                {
                    Bool = false,
                    BoolN = null,
                    Inner = new Unit(13, true)
                    {
                        Bool = false,
                        BoolN = null,
                        Inner = new Unit(23, true)
                        {
                            Bool = false,
                            BoolN = null,
                            Inner = new Unit(33, true)
                            {
                                Bool = false,
                                BoolN = null
                            }
                        }
                    }
                }
            };
        }

        [Theory]
        [InlineData("$w=Int32==5", 5)]
        [InlineData("$w=Int32N==5", 5)]
        [InlineData("$w=UInt32==5", 5)]
        [InlineData("$w=UInt32N==5", 5)]
        [InlineData("$w=Int64==5", 5)]
        [InlineData("$w=Int64N==5", 5)]
        [InlineData("$w=UInt64==5", 5)]
        [InlineData("$w=UInt64N==5", 5)]
        [InlineData("$w=Int16==5", 5)]
        [InlineData("$w=Int16N==5", 5)]
        [InlineData("$w=UInt16==5", 5)]
        [InlineData("$w=UInt16N==5", 5)]
        [InlineData("$w=Int8==5", 5)]
        [InlineData("$w=Int8N==5", 5)]
        [InlineData("$w=UInt8==5", 5)]
        [InlineData("$w=UInt8N==5", 5)]
        [InlineData("$w=Float32==5.05", 5)]
        [InlineData("$w=Float32N==5.05", 5)]
        [InlineData("$w=Float64==5.05", 5)]
        [InlineData("$w=Float64N==5.05", 5)]
        [InlineData("$w=Decimal==5.05", 5)]
        [InlineData("$w=DecimalN==5.05", 5)]
        [InlineData("$w=Char==f", 5)]
        [InlineData("$w=CharN==f", 5)]
        [InlineData("$w=Guid==00000005-0005-0005-0505-050505050505", 5)]
        [InlineData("$w=GuidN==00000005-0005-0005-0505-050505050505", 5)]
        [InlineData("$w=DateTime==2005-5-13", 5)]
        [InlineData("$w=DateTimeN==2005-5-13", 5)]
        [InlineData("$w=DateTimeOffset==2005-5-13+2", 5)]
        [InlineData("$w=DateTimeOffsetN==2005-5-13+2", 5)]
        [InlineData("$w=Enum==5", 5)]
        [InlineData("$w=EnumN==5", 5)]
        [InlineData("$w=String=='I am 5'", 5)]
        public void ApplyRiql_Equal_ShouldSelectItem(string riql, int expectedId)
        {
            // Arrange
            var units = this.GetUnits().AsQueryable();
            var expected = units.Single(u => u.Int32 == expectedId);

            // Act
            var result = units.ApplyRiql(riql).Single();

            // Assert
            result.Should().BeSameAs(expected);
        }

        [Theory]
        [InlineData("$w=Bool==true", 1)]
        [InlineData("$w=BoolN==true", 1)]
        public void ApplyRiql_EqualBool_ShouldFilterItems(string riql, int expectedId)
        {
            // Arrange
            var units = this.GetBoolUnits().AsQueryable();
            var expected = units.Single(u => u.Int32 == expectedId);

            // Act
            var result = units.ApplyRiql(riql).ToList();

            // Assert
            result.Should().Equal(expected);
        }

        [Theory]
        [InlineData("$w=Int32!=5", 5)]
        [InlineData("$w=Int32N!=5", 5)]
        [InlineData("$w=UInt32!=5", 5)]
        [InlineData("$w=UInt32N!=5", 5)]
        [InlineData("$w=Int64!=5", 5)]
        [InlineData("$w=Int64N!=5", 5)]
        [InlineData("$w=UInt64!=5", 5)]
        [InlineData("$w=UInt64N!=5", 5)]
        [InlineData("$w=Int16!=5", 5)]
        [InlineData("$w=Int16N!=5", 5)]
        [InlineData("$w=UInt16!=5", 5)]
        [InlineData("$w=UInt16N!=5", 5)]
        [InlineData("$w=Int8!=5", 5)]
        [InlineData("$w=Int8N!=5", 5)]
        [InlineData("$w=UInt8!=5", 5)]
        [InlineData("$w=UInt8N!=5", 5)]
        [InlineData("$w=Float32!=5.05", 5)]
        [InlineData("$w=Float32N!=5.05", 5)]
        [InlineData("$w=Float64!=5.05", 5)]
        [InlineData("$w=Float64N!=5.05", 5)]
        [InlineData("$w=Decimal!=5.05", 5)]
        [InlineData("$w=DecimalN!=5.05", 5)]
        [InlineData("$w=Char!=f", 5)]
        [InlineData("$w=CharN!=f", 5)]
        [InlineData("$w=Guid!=00000005-0005-0005-0505-050505050505", 5)]
        [InlineData("$w=GuidN!=00000005-0005-0005-0505-050505050505", 5)]
        [InlineData("$w=DateTime!=2005-5-13", 5)]
        [InlineData("$w=DateTimeN!=2005-5-13", 5)]
        [InlineData("$w=DateTimeOffset!=2005-5-13+2", 5)]
        [InlineData("$w=DateTimeOffsetN!=2005-5-13+2", 5)]
        [InlineData("$w=Enum!=5", 5)]
        [InlineData("$w=EnumN!=5", 5)]
        [InlineData("$w=String!='I am 5'", 5)]
        public void ApplyRiql_NotEqual_ShouldSelectAllOtherItems(string riql, int excludedId)
        {
            // Arrange
            var units = this.GetUnits().AsQueryable();
            var expected = units.Where(u => u.Int32 != excludedId).ToList();

            // Act
            var result = units.ApplyRiql(riql).ToList();

            // Assert
            result.Select(i => i.Int32).Should().Equal(expected.Select(i => i.Int32));
        }

        [Theory]
        [InlineData("$w=Bool!=true", 1)]
        [InlineData("$w=BoolN!=true", 1)]
        public void ApplyRiql_NotEqualBool_ShouldFilterItems(string riql, int excludedId)
        {
            // Arrange
            var units = this.GetBoolUnits().AsQueryable();
            var expected = units.Where(u => u.Int32 != excludedId).ToList();

            // Act
            var result = units.ApplyRiql(riql).ToList();

            // Assert
            result.Should().Equal(expected);
        }


        [Theory]
        [InlineData("$w=Int32N=is-null=false", false)]
        [InlineData("$w=Int32N=is-null=true", true)]
        [InlineData("$w=UInt32N=is-null=false", false)]
        [InlineData("$w=UInt32N=is-null=true", true)]
        [InlineData("$w=Int64N=is-null=false", false)]
        [InlineData("$w=Int64N=is-null=true", true)]
        [InlineData("$w=UInt64N=is-null=false", false)]
        [InlineData("$w=UInt64N=is-null=true", true)]
        [InlineData("$w=Int16N=is-null=false", false)]
        [InlineData("$w=Int16N=is-null=true", true)]
        [InlineData("$w=UInt16N=is-null=false", false)]
        [InlineData("$w=UInt16N=is-null=true", true)]
        [InlineData("$w=Int8N=is-null=false", false)]
        [InlineData("$w=Int8N=is-null=true", true)]
        [InlineData("$w=UInt8N=is-null=false", false)]
        [InlineData("$w=UInt8N=is-null=true", true)]
        [InlineData("$w=Float32N=is-null=false", false)]
        [InlineData("$w=Float32N=is-null=true", true)]
        [InlineData("$w=Float64N=is-null=false", false)]
        [InlineData("$w=Float64N=is-null=true", true)]
        [InlineData("$w=DecimalN=is-null=false", false)]
        [InlineData("$w=DecimalN=is-null=true", true)]
        [InlineData("$w=CharN=is-null=false", false)]
        [InlineData("$w=CharN=is-null=true", true)]
        [InlineData("$w=GuidN=is-null=false", false)]
        [InlineData("$w=GuidN=is-null=true", true)]
        [InlineData("$w=DateTimeN=is-null=false", false)]
        [InlineData("$w=DateTimeN=is-null=true", true)]
        [InlineData("$w=DateTimeOffsetN=is-null=false", false)]
        [InlineData("$w=DateTimeOffsetN=is-null=true", true)]
        [InlineData("$w=EnumN=is-null=false", false)]
        [InlineData("$w=EnumN=is-null=true", true)]
        [InlineData("$w=String=is-null=false", false)]
        [InlineData("$w=String=is-null=true", true)]
        [InlineData("$w=BoolN=is-null=false", false)]
        [InlineData("$w=BoolN=is-null=true", true)]
        public void ApplyRiql_IsNull_ShouldSelectItems(string riql, bool nulls)
        {
            // Arrange
            var units = this.GetUnits().AsQueryable();
            var expected = units.Where(u => nulls ? u.Int32N == null : u.Int32N != null).ToList();

            // Act
            var result = units.ApplyRiql(riql).ToList();

            // Assert
            result.Select(i => i.Int32).Should().Equal(expected.Select(i => i.Int32));
        }

        [Theory]
        [InlineData("$w=Int32=is-null=false", false)]
        [InlineData("$w=Int32=is-null=true", true)]
        [InlineData("$w=UInt32=is-null=false", false)]
        [InlineData("$w=UInt32=is-null=true", true)]
        [InlineData("$w=Int64=is-null=false", false)]
        [InlineData("$w=Int64=is-null=true", true)]
        [InlineData("$w=UInt64=is-null=false", false)]
        [InlineData("$w=UInt64=is-null=true", true)]
        [InlineData("$w=Int16=is-null=false", false)]
        [InlineData("$w=Int16=is-null=true", true)]
        [InlineData("$w=UInt16=is-null=false", false)]
        [InlineData("$w=UInt16=is-null=true", true)]
        [InlineData("$w=Int8=is-null=false", false)]
        [InlineData("$w=Int8=is-null=true", true)]
        [InlineData("$w=UInt8=is-null=false", false)]
        [InlineData("$w=UInt8=is-null=true", true)]
        [InlineData("$w=Float32=is-null=false", false)]
        [InlineData("$w=Float32=is-null=true", true)]
        [InlineData("$w=Float64=is-null=false", false)]
        [InlineData("$w=Float64=is-null=true", true)]
        [InlineData("$w=Decimal=is-null=false", false)]
        [InlineData("$w=Decimal=is-null=true", true)]
        [InlineData("$w=Char=is-null=false", false)]
        [InlineData("$w=Char=is-null=true", true)]
        [InlineData("$w=Guid=is-null=false", false)]
        [InlineData("$w=Guid=is-null=true", true)]
        [InlineData("$w=DateTime=is-null=false", false)]
        [InlineData("$w=DateTime=is-null=true", true)]
        [InlineData("$w=DateTimeOffset=is-null=false", false)]
        [InlineData("$w=DateTimeOffset=is-null=true", true)]
        [InlineData("$w=Enum=is-null=false", false)]
        [InlineData("$w=Enum=is-null=true", true)]
        [InlineData("$w=Bool=is-null=false", false)]
        [InlineData("$w=Bool=is-null=true", true)]
        public void ApplyRiql_IsNullNonNullables_ShouldSelectItems(string riql, bool removeAll)
        {
            // Arrange
            var units = this.GetUnits().AsQueryable();
            var expected = units.Where(u => !removeAll).ToList();

            // Act
            var result = units.ApplyRiql(riql).ToList();

            // Assert
            result.Should().Equal(expected);
        }

        [Theory]
        [InlineData("$w=Int32<5", 5, true)]
        [InlineData("$w=Int32N<5", 5, false)]
        [InlineData("$w=UInt32<5", 5, true)]
        [InlineData("$w=UInt32N<5", 5, false)]
        [InlineData("$w=Int64<5", 5, true)]
        [InlineData("$w=Int64N<5", 5, false)]
        [InlineData("$w=UInt64<5", 5, true)]
        [InlineData("$w=UInt64N<5", 5, false)]
        [InlineData("$w=Int16<5", 5, true)]
        [InlineData("$w=Int16N<5", 5, false)]
        [InlineData("$w=UInt16<5", 5, true)]
        [InlineData("$w=UInt16N<5", 5, false)]
        [InlineData("$w=Int8<5", 5, true)]
        [InlineData("$w=Int8N<5", 5, false)]
        [InlineData("$w=UInt8<5", 5, true)]
        [InlineData("$w=UInt8N<5", 5, false)]
        [InlineData("$w=Float32<5.05", 5, true)]
        [InlineData("$w=Float32N<5.05", 5, false)]
        [InlineData("$w=Float64<5.05", 5, true)]
        [InlineData("$w=Float64N<5.05", 5, false)]
        [InlineData("$w=Decimal<5.05", 5, true)]
        [InlineData("$w=DecimalN<5.05", 5, false)]
        [InlineData("$w=DateTime<2005-5-13", 5, true)]
        [InlineData("$w=DateTimeN<2005-5-13", 5, false)]
        [InlineData("$w=DateTimeOffset<2005-5-13+2", 5, true)]
        [InlineData("$w=DateTimeOffsetN<2005-5-13+2", 5, false)]
        public void ApplyRiql_LessThan_ShouldSelectItem(string riql, int expectedId, bool includeNulls)
        {
            // Arrange
            var units = this.GetUnits().AsQueryable();
            var expected = units.Where(u => u.Int32 < expectedId && (includeNulls || u.Int32N.HasValue)).ToList();

            // Act
            var result = units.ApplyRiql(riql).ToList();

            // Assert
            result.Select(i => i.Int32).Should().Equal(expected.Select(i => i.Int32));
        }

        [Theory]
        [InlineData("$w=Int32<=5", 5, true)]
        [InlineData("$w=Int32N<=5", 5, false)]
        [InlineData("$w=UInt32<=5", 5, true)]
        [InlineData("$w=UInt32N<=5", 5, false)]
        [InlineData("$w=Int64<=5", 5, true)]
        [InlineData("$w=Int64N<=5", 5, false)]
        [InlineData("$w=UInt64<=5", 5, true)]
        [InlineData("$w=UInt64N<=5", 5, false)]
        [InlineData("$w=Int16<=5", 5, true)]
        [InlineData("$w=Int16N<=5", 5, false)]
        [InlineData("$w=UInt16<=5", 5, true)]
        [InlineData("$w=UInt16N<=5", 5, false)]
        [InlineData("$w=Int8<=5", 5, true)]
        [InlineData("$w=Int8N<=5", 5, false)]
        [InlineData("$w=UInt8<=5", 5, true)]
        [InlineData("$w=UInt8N<=5", 5, false)]
        [InlineData("$w=Float32<=5.05", 5, true)]
        [InlineData("$w=Float32N<=5.05", 5, false)]
        [InlineData("$w=Float64<=5.05", 5, true)]
        [InlineData("$w=Float64N<=5.05", 5, false)]
        [InlineData("$w=Decimal<=5.05", 5, true)]
        [InlineData("$w=DecimalN<=5.05", 5, false)]
        [InlineData("$w=DateTime<=2005-5-13", 5, true)]
        [InlineData("$w=DateTimeN<=2005-5-13", 5, false)]
        [InlineData("$w=DateTimeOffset<=2005-5-13+2", 5, true)]
        [InlineData("$w=DateTimeOffsetN<=2005-5-13+2", 5, false)]
        public void ApplyRiql_LessThanEqual_ShouldSelectItem(string riql, int expectedId, bool includeNulls)
        {
            // Arrange
            var units = this.GetUnits().AsQueryable();
            var expected = units.Where(u => u.Int32 <= expectedId && (includeNulls || u.Int32N.HasValue)).ToList();

            // Act
            var result = units.ApplyRiql(riql).ToList();

            // Assert
            result.Select(i => i.Int32).Should().Equal(expected.Select(i => i.Int32));
        }

        [Theory]
        [InlineData("$w=Int32>5", 5, true)]
        [InlineData("$w=Int32N>5", 5, false)]
        [InlineData("$w=UInt32>5", 5, true)]
        [InlineData("$w=UInt32N>5", 5, false)]
        [InlineData("$w=Int64>5", 5, true)]
        [InlineData("$w=Int64N>5", 5, false)]
        [InlineData("$w=UInt64>5", 5, true)]
        [InlineData("$w=UInt64N>5", 5, false)]
        [InlineData("$w=Int16>5", 5, true)]
        [InlineData("$w=Int16N>5", 5, false)]
        [InlineData("$w=UInt16>5", 5, true)]
        [InlineData("$w=UInt16N>5", 5, false)]
        [InlineData("$w=Int8>5", 5, true)]
        [InlineData("$w=Int8N>5", 5, false)]
        [InlineData("$w=UInt8>5", 5, true)]
        [InlineData("$w=UInt8N>5", 5, false)]
        [InlineData("$w=Float32>5.05", 5, true)]
        [InlineData("$w=Float32N>5.05", 5, false)]
        [InlineData("$w=Float64>5.05", 5, true)]
        [InlineData("$w=Float64N>5.05", 5, false)]
        [InlineData("$w=Decimal>5.05", 5, true)]
        [InlineData("$w=DecimalN>5.05", 5, false)]
        [InlineData("$w=DateTime>2005-5-13", 5, true)]
        [InlineData("$w=DateTimeN>2005-5-13", 5, false)]
        [InlineData("$w=DateTimeOffset>2005-5-13+2", 5, true)]
        [InlineData("$w=DateTimeOffsetN>2005-5-13+2", 5, false)]
        public void ApplyRiql_GreaterThan_ShouldSelectItem(string riql, int expectedId, bool includeNulls)
        {
            // Arrange
            var units = this.GetUnits().AsQueryable();
            var expected = units.Where(u => u.Int32 > expectedId && (includeNulls || u.Int32N.HasValue)).ToList();

            // Act
            var result = units.ApplyRiql(riql).ToList();

            // Assert
            result.Select(i => i.Int32).Should().Equal(expected.Select(i => i.Int32));
        }

        [Theory]
        [InlineData("$w=Int32>=5", 5, true)]
        [InlineData("$w=Int32N>=5", 5, false)]
        [InlineData("$w=UInt32>=5", 5, true)]
        [InlineData("$w=UInt32N>=5", 5, false)]
        [InlineData("$w=Int64>=5", 5, true)]
        [InlineData("$w=Int64N>=5", 5, false)]
        [InlineData("$w=UInt64>=5", 5, true)]
        [InlineData("$w=UInt64N>=5", 5, false)]
        [InlineData("$w=Int16>=5", 5, true)]
        [InlineData("$w=Int16N>=5", 5, false)]
        [InlineData("$w=UInt16>=5", 5, true)]
        [InlineData("$w=UInt16N>=5", 5, false)]
        [InlineData("$w=Int8>=5", 5, true)]
        [InlineData("$w=Int8N>=5", 5, false)]
        [InlineData("$w=UInt8>=5", 5, true)]
        [InlineData("$w=UInt8N>=5", 5, false)]
        [InlineData("$w=Float32>=5.05", 5, true)]
        [InlineData("$w=Float32N>=5.05", 5, false)]
        [InlineData("$w=Float64>=5.05", 5, true)]
        [InlineData("$w=Float64N>=5.05", 5, false)]
        [InlineData("$w=Decimal>=5.05", 5, true)]
        [InlineData("$w=DecimalN>=5.05", 5, false)]
        [InlineData("$w=DateTime>=2005-5-13", 5, true)]
        [InlineData("$w=DateTimeN>=2005-5-13", 5, false)]
        [InlineData("$w=DateTimeOffset>=2005-5-13+2", 5, true)]
        [InlineData("$w=DateTimeOffsetN>=2005-5-13+2", 5, false)]
        public void ApplyRiql_GreaterThanEqual_ShouldSelectItem(string riql, int expectedId, bool includeNulls)
        {
            // Arrange
            var units = this.GetUnits().AsQueryable();
            var expected = units.Where(u => u.Int32 >= expectedId && (includeNulls || u.Int32N.HasValue)).ToList();

            // Act
            var result = units.ApplyRiql(riql).ToList();

            // Assert
            result.Select(i => i.Int32).Should().Equal(expected.Select(i => i.Int32));
        }

        [Theory]
        [InlineData("$w=Int32=in=(5,6,7)", 5, 6, 7)]
        [InlineData("$w=Int32N=in=(5,6,7)", 5, 7)]
        [InlineData("$w=UInt32=in=(5,6,7)", 5, 6, 7)]
        [InlineData("$w=UInt32N=in=(5,6,7)", 5, 7)]
        [InlineData("$w=Int64=in=(5,6,7)", 5, 6, 7)]
        [InlineData("$w=Int64N=in=(5,6,7)", 5, 7)]
        [InlineData("$w=UInt64=in=(5,6,7)", 5, 6, 7)]
        [InlineData("$w=UInt64N=in=(5,6,7)", 5, 7)]
        [InlineData("$w=Int16=in=(5,6,7)", 5, 6, 7)]
        [InlineData("$w=Int16N=in=(5,6,7)", 5, 7)]
        [InlineData("$w=UInt16=in=(5,6,7)", 5, 6, 7)]
        [InlineData("$w=UInt16N=in=(5,6,7)", 5, 7)]
        [InlineData("$w=Int8=in=(5,6,7)", 5, 6, 7)]
        [InlineData("$w=Int8N=in=(5,6,7)", 5, 7)]
        [InlineData("$w=UInt8=in=(5,6,7)", 5, 6, 7)]
        [InlineData("$w=UInt8N=in=(5,6,7)", 5, 7)]
        [InlineData("$w=Float32=in=(5.05,6.06,7.07))", 5, 6, 7)]
        [InlineData("$w=Float32N=in=(5.05,6.06,7.07))", 5, 7)]
        [InlineData("$w=Float64=in=(5.05,6.06,7.07)", 5, 6, 7)]
        [InlineData("$w=Float64N=in=(5.05,6.06,7.07))", 5, 7)]
        [InlineData("$w=Decimal=in=(5.05,6.06,7.07)", 5, 6, 7)]
        [InlineData("$w=DecimalN=in=(5.05,6.06,7.07)", 5, 7)]
        [InlineData("$w=Char=in=(c,d,f)", 2, 3, 5)]
        [InlineData("$w=CharN=in=(c,d,f)", 3, 5)]
        [InlineData("$w=Guid=in=(00000005-0005-0005-0505-050505050505,00000006-0006-0006-0606-060606060606,00000007-0007-0007-0707-070707070707)", 5, 6, 7)]
        [InlineData("$w=GuidN=in=(00000005-0005-0005-0505-050505050505,00000006-0006-0006-0606-060606060606,00000007-0007-0007-0707-070707070707)", 5, 7)]
        [InlineData("$w=DateTime=in=(2005-5-13,2006-5-13,2007-5-13)", 5, 6, 7)]
        [InlineData("$w=DateTimeN=in=(2005-5-13,2006-5-13,2007-5-13)", 5, 7)]
        [InlineData("$w=DateTimeOffset=in=(2005-5-13+2,2006-5-13+2,2007-5-13+2)", 5, 6, 7)]
        [InlineData("$w=DateTimeOffsetN=in=(2005-5-13+2,2006-5-13+2,2007-5-13+2)", 5, 7)]
        [InlineData("$w=Enum=in=(Female,5,6,7)", 1, 5, 6, 7)]
        [InlineData("$w=EnumN=in=(Female,5,6,7)", 1, 5, 7)]
        [InlineData("$w=String=in=('I am 5','I am 6','I am 7')", 5, 7)]
        [InlineData("$w=Bool=in=(true)", 3, 6)]
        [InlineData("$w=BoolN=in=(true,false)", 1, 3, 5, 7)]
        public void ApplyRiql_In_ShouldSelectItem(string riql, params int[] expectedIds)
        {
            // Arrange
            var units = this.GetUnits().AsQueryable();
            var expected = units.Where(u => expectedIds.Contains(u.Int32)).ToList();

            // Act
            var result = units.ApplyRiql(riql).ToList();

            // Assert
            result.Select(i => i.Int32).Should().Equal(expected.Select(i => i.Int32));
        }

        [Theory]
        [InlineData("$w=Int32=out=(5,6,7)", 5, 6, 7)]
        [InlineData("$w=Int32N=out=(5,6,7)", 5, 7)]
        [InlineData("$w=UInt32=out=(5,6,7)", 5, 6, 7)]
        [InlineData("$w=UInt32N=out=(5,6,7)", 5, 7)]
        [InlineData("$w=Int64=out=(5,6,7)", 5, 6, 7)]
        [InlineData("$w=Int64N=out=(5,6,7)", 5, 7)]
        [InlineData("$w=UInt64=out=(5,6,7)", 5, 6, 7)]
        [InlineData("$w=UInt64N=out=(5,6,7)", 5, 7)]
        [InlineData("$w=Int16=out=(5,6,7)", 5, 6, 7)]
        [InlineData("$w=Int16N=out=(5,6,7)", 5, 7)]
        [InlineData("$w=UInt16=out=(5,6,7)", 5, 6, 7)]
        [InlineData("$w=UInt16N=out=(5,6,7)", 5, 7)]
        [InlineData("$w=Int8=out=(5,6,7)", 5, 6, 7)]
        [InlineData("$w=Int8N=out=(5,6,7)", 5, 7)]
        [InlineData("$w=UInt8=out=(5,6,7)", 5, 6, 7)]
        [InlineData("$w=UInt8N=out=(5,6,7)", 5, 7)]
        [InlineData("$w=Float32=out=(5.05,6.06,7.07))", 5, 6, 7)]
        [InlineData("$w=Float32N=out=(5.05,6.06,7.07))", 5, 7)]
        [InlineData("$w=Float64=out=(5.05,6.06,7.07)", 5, 6, 7)]
        [InlineData("$w=Float64N=out=(5.05,6.06,7.07))", 5, 7)]
        [InlineData("$w=Decimal=out=(5.05,6.06,7.07)", 5, 6, 7)]
        [InlineData("$w=DecimalN=out=(5.05,6.06,7.07)", 5, 7)]
        [InlineData("$w=Char=out=(c,d,f)", 2, 3, 5)]
        [InlineData("$w=CharN=out=(c,d,f)", 3, 5)]
        [InlineData("$w=Guid=out=(00000005-0005-0005-0505-050505050505,00000006-0006-0006-0606-060606060606,00000007-0007-0007-0707-070707070707)", 5, 6, 7)]
        [InlineData("$w=GuidN=out=(00000005-0005-0005-0505-050505050505,00000006-0006-0006-0606-060606060606,00000007-0007-0007-0707-070707070707)", 5, 7)]
        [InlineData("$w=DateTime=out=(2005-5-13,2006-5-13,2007-5-13)", 5, 6, 7)]
        [InlineData("$w=DateTimeN=out=(2005-5-13,2006-5-13,2007-5-13)", 5, 7)]
        [InlineData("$w=DateTimeOffset=out=(2005-5-13+2,2006-5-13+2,2007-5-13+2)", 5, 6, 7)]
        [InlineData("$w=DateTimeOffsetN=out=(2005-5-13+2,2006-5-13+2,2007-5-13+2)", 5, 7)]
        [InlineData("$w=Enum=out=(Female,5,6,7)", 1, 5, 6, 7)]
        [InlineData("$w=EnumN=out=(Female,5,6,7)", 1, 5, 7)]
        [InlineData("$w=String=out=('I am 5','I am 6','I am 7')", 5, 7)]
        [InlineData("$w=Bool=out=(true)", 3, 6)]
        [InlineData("$w=BoolN=out=(true,false)", 1, 3, 5, 7)]
        public void ApplyRiql_Out_ShouldSelectItem(string riql, params int[] exceptIds)
        {
            // Arrange
            var units = this.GetUnits().AsQueryable();
            var expected = units.Where(u => !exceptIds.Contains(u.Int32)).ToList();

            // Act
            var result = units.ApplyRiql(riql).ToList();

            // Assert
            result.Select(i => i.Int32).Should().Equal(expected.Select(i => i.Int32));
        }
    }
}