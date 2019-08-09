using System;
using System.Linq;
using FluentAssertions;
using Riql.Tests.TestData;
using Riql.Transpiler;
using Riql.Transpiler.Rsql;
using Xunit;

namespace Riql.Tests
{
    public class RiqlInvalid_Tests
    {
        private Unit[] GetUnits()
        {
            return new[]
            {
                new Unit(1, false),
                new Unit(2, true),
                new Unit(3, false),
                new Unit(4, true),
                new Unit(5, false)
            };
        }

        [Theory]
        [InlineData("$w=Int32==X")]
        [InlineData("$w=Int32N==X")]
        [InlineData("$w=UInt32==X")]
        [InlineData("$w=UInt32N==X")]
        [InlineData("$w=Int64==X")]
        [InlineData("$w=Int64N==X")]
        [InlineData("$w=UInt64==X")]
        [InlineData("$w=UInt64N==X")]
        [InlineData("$w=Int16==X")]
        [InlineData("$w=Int16N==X")]
        [InlineData("$w=UInt16==X")]
        [InlineData("$w=UInt16N==X")]
        [InlineData("$w=Int8==X")]
        [InlineData("$w=Int8N==X")]
        [InlineData("$w=UInt8==X")]
        [InlineData("$w=UInt8N==X")]
        [InlineData("$w=Float32==X")]
        [InlineData("$w=Float32N==X")]
        [InlineData("$w=Float64==X")]
        [InlineData("$w=Float64N==X")]
        [InlineData("$w=Decimal==X")]
        [InlineData("$w=DecimalN==X")]
        [InlineData("$w=Char==XX")]
        [InlineData("$w=CharN==XX")]
        [InlineData("$w=Guid==X")]
        [InlineData("$w=GuidN==X")]
        [InlineData("$w=DateTime==X")]
        [InlineData("$w=DateTimeN==X")]
        [InlineData("$w=DateTimeOffset==X")]
        [InlineData("$w=DateTimeOffsetN==X")]
        [InlineData("$w=Enum==X")]
        [InlineData("$w=EnumN==X")]
        public void ApplyRiql_WhereInvalidType_ShouldThrowInvalidConversionException(string riql)
        {
            // Arrange
            var units = this.GetUnits().AsQueryable();

            // Act
            Action act = () => units.ApplyRiql(riql);

            // Assert
            act.Should().Throw<InvalidConversionException>();
        }

        [Theory]
        [InlineData("$w=Int32=5")]
        [InlineData("$w=Int32===5")]
        [InlineData("$w=Int32=l=5")]
        public void ApplyRiql_WhereInvalidComparator_ShouldThrowUnknownComparatorException(string riql)
        {
            // Arrange
            var units = this.GetUnits().AsQueryable();

            // Act
            Action act = () => units.ApplyRiql(riql);

            // Assert
            act.Should().Throw<UnknownComparatorException>();
        }

        [Theory]
        [InlineData("$w=Int32==(5, 5)")]
        [InlineData("$w=Int32N==(5, 5)")]
        [InlineData("$w=UInt32==(5, 5)")]
        [InlineData("$w=UInt32N==(5, 5)")]
        [InlineData("$w=Int64==(5, 5)")]
        [InlineData("$w=Int64N==(5, 5)")]
        [InlineData("$w=UInt64==(5, 5)")]
        [InlineData("$w=UInt64N==(5, 5)")]
        [InlineData("$w=Int16==(5, 5)")]
        [InlineData("$w=Int16N==(5, 5)")]
        [InlineData("$w=UInt16==(5, 5)")]
        [InlineData("$w=UInt16N==(5, 5)")]
        [InlineData("$w=Int8==(5, 5)")]
        [InlineData("$w=Int8N==(5, 5)")]
        [InlineData("$w=UInt8==(5, 5)")]
        [InlineData("$w=UInt8N==(5, 5)")]
        [InlineData("$w=Float32==(5.05, 5.05)")]
        [InlineData("$w=Float32N==(5.05, 5.05)")]
        [InlineData("$w=Float64==(5.05, 5.05)")]
        [InlineData("$w=Float64N==(5.05, 5.05)")]
        [InlineData("$w=Decimal==(5.05, 5.05)")]
        [InlineData("$w=DecimalN==(5.05, 5.05)")]
        [InlineData("$w=Char==(f, f)")]
        [InlineData("$w=CharN==(f, f)")]
        [InlineData("$w=Guid==(00000005-0005-0005-0505-050505050505, 00000005-0005-0005-0505-050505050505)")]
        [InlineData("$w=GuidN==(00000005-0005-0005-0505-050505050505, 00000005-0005-0005-0505-050505050505)")]
        [InlineData("$w=DateTime==(2005-5-13, 2005-5-13)")]
        [InlineData("$w=DateTimeN==(2005-5-13, 2005-5-13)")]
        [InlineData("$w=DateTimeOffset==(2005-5-13+2, 2005-5-13+2)")]
        [InlineData("$w=DateTimeOffsetN==(2005-5-13+2, 2005-5-13+2)")]
        [InlineData("$w=Enum==(5, 5)")]
        [InlineData("$w=EnumN==(5, 5)")]
        [InlineData("$w=String==('I am 5', 'I am 5')")]
        public void ApplyRiql_WhereInvalidMultiples_ShouldThrowTooManyArgumentsException(string riql)
        {
            // Arrange
            var units = this.GetUnits().AsQueryable();

            // Act
            Action act = () => units.ApplyRiql(riql);

            // Assert
            act.Should().Throw<TooManyArgumentsException>();
        }

        [Theory]
        [InlineData("$w=(Int32==5")]
        [InlineData("$w=(Int32N==5")]
        [InlineData("$w=(UInt32==5")]
        [InlineData("$w=(UInt32N==5")]
        [InlineData("$w=(Int64==5")]
        [InlineData("$w=(Int64N==5")]
        [InlineData("$w=(UInt64==5")]
        [InlineData("$w=(UInt64N==5")]
        [InlineData("$w=(Int16==5")]
        [InlineData("$w=(Int16N==5")]
        [InlineData("$w=(UInt16==5")]
        [InlineData("$w=(UInt16N==5")]
        [InlineData("$w=(Int8==5")]
        [InlineData("$w=(Int8N==5")]
        [InlineData("$w=(UInt8==5")]
        [InlineData("$w=(UInt8N==5")]
        [InlineData("$w=(Float32==5.05")]
        [InlineData("$w=(Float32N==5.05")]
        [InlineData("$w=(Float64==5.05")]
        [InlineData("$w=(Float64N==5.05")]
        [InlineData("$w=(Decimal==5.05")]
        [InlineData("$w=(DecimalN==5.05")]
        [InlineData("$w=(Char==f")]
        [InlineData("$w=(CharN==f")]
        [InlineData("$w=(Guid==00000005-0005-0005-0505-050505050505")]
        [InlineData("$w=(GuidN==00000005-0005-0005-0505-050505050505")]
        [InlineData("$w=(DateTime==2005-5-13")]
        [InlineData("$w=(DateTimeN==2005-5-13")]
        [InlineData("$w=(DateTimeOffset==2005-5-13+2")]
        [InlineData("$w=(DateTimeOffsetN==2005-5-13+2")]
        [InlineData("$w=(Enum==5")]
        [InlineData("$w=(EnumN==5")]
        [InlineData("$w=(String=='I am 5'")]
        public void ApplyRiql_WhereGroupNotClosed_ShouldThrowInvalidGroupException(string riql)
        {
            // Arrange
            var units = this.GetUnits().AsQueryable();

            // Act
            Action act = () => units.ApplyRiql(riql);

            // Assert
            act.Should().Throw<InvalidGroupException>();
        }

        [Theory]
        [InlineData("$w=Int32==")]
        [InlineData("$w=Int32N==")]
        [InlineData("$w=UInt32==")]
        [InlineData("$w=UInt32N==")]
        [InlineData("$w=Int64==")]
        [InlineData("$w=Int64N==")]
        [InlineData("$w=UInt64==")]
        [InlineData("$w=UInt64N==")]
        [InlineData("$w=Int16==")]
        [InlineData("$w=Int16N==")]
        [InlineData("$w=UInt16==")]
        [InlineData("$w=UInt16N==")]
        [InlineData("$w=Int8==")]
        [InlineData("$w=Int8N==")]
        [InlineData("$w=UInt8==")]
        [InlineData("$w=UInt8N==")]
        [InlineData("$w=Float32==")]
        [InlineData("$w=Float32N==")]
        [InlineData("$w=Float64==")]
        [InlineData("$w=Float64N==")]
        [InlineData("$w=Decimal==")]
        [InlineData("$w=DecimalN==")]
        [InlineData("$w=Char==")]
        [InlineData("$w=CharN==")]
        [InlineData("$w=Guid==")]
        [InlineData("$w=GuidN==")]
        [InlineData("$w=DateTime==")]
        [InlineData("$w=DateTimeN==")]
        [InlineData("$w=DateTimeOffset==")]
        [InlineData("$w=DateTimeOffsetN==")]
        [InlineData("$w=Enum==")]
        [InlineData("$w=EnumN==")]
        [InlineData("$w=String==")]
        [InlineData("$w=Int32<=5,Int32>=")]
        [InlineData("$w=Int32<=,Int32>=3")]
        public void ApplyRiql_Where_ShouldThrowUnknownComparatorException(string riql)
        {
            // Arrange
            var units = this.GetUnits().AsQueryable();

            // Act
            Action act = () => units.ApplyRiql(riql);

            // Assert
            act.Should().Throw<UnknownComparatorException>();
        }

        [Theory]
        [InlineData("$w=Inner==5")]
        [InlineData("$w=Inner<5")]
        [InlineData("$w=Char<f")]
        [InlineData("$w=CharN<f")]
        [InlineData("$w=Guid<00000005-0005-0005-0505-050505050505")]
        [InlineData("$w=GuidN<00000005-0005-0005-0505-050505050505")]
        [InlineData("$w=Enum<5")]
        [InlineData("$w=EnumN<5")]
        [InlineData("$w=String<'I am 5'")]
        public void ApplyRiql_WhereInvalidComparison_ShouldThrowInvalidComparatorException(string riql)
        {
            // Arrange
            var units = this.GetUnits().AsQueryable();

            // Act
            Action act = () => units.ApplyRiql(riql);

            // Assert
            act.Should().Throw<InvalidComparatorException>();
        }

        [Theory]
        [InlineData("$w=Inner.IntX32==5")]
        [InlineData("$w=InnXer.Int32==5")]
        [InlineData("$w=IntX32==5")]
        public void ApplyRiql_WhereInvalidProperty_ShouldThrowPropertyNotFoundException(string riql)
        {
            // Arrange
            var units = this.GetUnits().AsQueryable();

            // Act
            Action act = () => units.ApplyRiql(riql);

            // Assert
            act.Should().Throw<PropertyNotFoundException>();
        }

        [Theory]
        [InlineData("$o=Inner.IntX32")]
        [InlineData("$o=InnXer.Int32")]
        [InlineData("$o=IntX32")]
        public void ApplyRiql_OrderByInvalidProperty_ShouldThrowPropertyNotFoundException(string riql)
        {
            // Arrange
            var units = this.GetUnits().AsQueryable();

            // Act
            Action act = () => units.ApplyRiql(riql);

            // Assert
            act.Should().Throw<PropertyNotFoundException>();
        }

        [Theory]
        [InlineData("$r=InnXer")]
        [InlineData("$r=IntX32")]
        public void ApplyRiql_ReduceInvalidProperty_ShouldThrowPropertyNotFoundException(string riql)
        {
            // Arrange
            var units = this.GetUnits().AsQueryable();

            // Act
            Action act = () => units.ApplyRiql(riql);

            // Assert
            act.Should().Throw<PropertyNotFoundException>();
        }

        [Theory]
        [InlineData("$o=Int32 ask")]
        [InlineData("$o=Int32, Int16 de")]
        public void ApplyRiql_OrderByInvalidDirection_ShouldThrowErrorNodeException(string riql)
        {
            // Arrange
            var units = this.GetUnits().AsQueryable();

            // Act
            Action act = () => units.ApplyRiql(riql);

            // Assert
            act.Should().Throw<ErrorNodeException>();
        }

        [Theory]
        [InlineData("$s=a")]
        [InlineData("$s=(1)")]
        public void ApplyRiql_SkipInvalidDigit_ShouldThrowRequiresIntegerExeption(string riql)
        {
            // Arrange
            var units = this.GetUnits().AsQueryable();

            // Act
            Action act = () => units.ApplyRiql(riql);

            // Assert
            act.Should().Throw<RequiresIntegerExeption>();
        }

        [Theory]
        [InlineData("$s=1.5")]
        [InlineData("$s=1f")]
        public void ApplyRiql_SkipInvalidDigit_ShouldThrowErrorNodeException(string riql)
        {
            // Arrange
            var units = this.GetUnits().AsQueryable();

            // Act
            Action act = () => units.ApplyRiql(riql);

            // Assert
            act.Should().Throw<ErrorNodeException>();
        }

        [Theory]
        [InlineData("$t=a")]
        [InlineData("$t=(1)")]
        public void ApplyRiql_TakeInvalidDigit_ShouldThrowRequiresIntegerExeption(string riql)
        {
            // Arrange
            var units = this.GetUnits().AsQueryable();

            // Act
            Action act = () => units.ApplyRiql(riql);

            // Assert
            act.Should().Throw<RequiresIntegerExeption>();
        }

        [Theory]
        [InlineData("$t=1.5")]
        [InlineData("$t=1f")]
        public void ApplyRiql_TakeInvalidDigit_ShouldThrowErrorNodeException(string riql)
        {
            // Arrange
            var units = this.GetUnits().AsQueryable();

            // Act
            Action act = () => units.ApplyRiql(riql);

            // Assert
            act.Should().Throw<ErrorNodeException>();
        }

        [Theory]
        [InlineData("$a=a")]
        [InlineData("$b=(1)")]
        [InlineData("w=Int32==5")]
        public void ApplyRiql_InvalidKey_ShouldThrowErrorNodeException(string riql)
        {
            // Arrange
            var units = this.GetUnits().AsQueryable();

            // Act
            Action act = () => units.ApplyRiql(riql);

            // Assert
            act.Should().Throw<ErrorNodeException>();
        }
    }
}