using System;

namespace Riql.Tests.TestData
{
    public class Unit
    {
        public Unit()
        {
        }

        public Unit(int i, bool nulls)
        {
            this.Int32 = i;
            this.Int32N = !nulls ? i : (int?) null;

            this.UInt32 = (uint) i;
            this.UInt32N = !nulls ? (uint) i : (uint?) null;

            this.Int64 = i;
            this.Int64N = !nulls ? i : (long?) null;

            this.UInt64 = (ulong) i;
            this.UInt64N = !nulls ? (ulong) i : (ulong?) null;

            this.Int16 = (short) i;
            this.Int16N = !nulls ? (short) i : (short?) null;

            this.UInt16 = (ushort) i;
            this.UInt16N = !nulls ? (ushort) i : (ushort?) null;

            this.Int8 = (sbyte) i;
            this.Int8N = !nulls ? (sbyte) i : (sbyte?) null;

            this.UInt8 = (byte) i;
            this.UInt8N = !nulls ? (byte) i : (byte?) null;

            this.Float32 = i + i * 0.01f;
            this.Float32N = !nulls ? this.Float32 : (float?) null;

            this.Float64 = i + i * 0.01d;
            this.Float64N = !nulls ? this.Float64 : (double?) null;

            this.Decimal = i + i * 0.01m;
            this.DecimalN = !nulls ? this.Decimal : (decimal?) null;

            this.Bool = i % 3 == 0;
            this.BoolN = !nulls ? this.Bool : (bool?) null;

            this.Char = (char) ('a' + i);
            this.CharN = !nulls ? this.Char : (char?) null;

            var b = this.UInt8;
            this.Guid = new Guid(i, this.Int16, this.Int16, b, b, b, b, b, b, b, b);
            this.GuidN = !nulls ? this.Guid : (Guid?) null;

            this.DateTime = new DateTime(2000 + i, 5, 13);
            this.DateTimeN = !nulls ? this.DateTime : (DateTime?) null;

            this.DateTimeOffset = new DateTimeOffset(this.DateTime, TimeSpan.FromHours(2));
            this.DateTimeOffsetN = !nulls ? this.DateTimeOffset : (DateTimeOffset?) null;

            this.Enum = (Gender) i;
            this.EnumN = !nulls ? this.Enum : (Gender?) null;

            this.String = !nulls ? $"I am {i}" : null;
        }

        public int? Int32N { get; set; }
        public int Int32 { get; set; }

        public uint? UInt32N { get; set; }
        public uint UInt32 { get; set; }

        public long? Int64N { get; set; }
        public long Int64 { get; set; }

        public ulong? UInt64N { get; set; }
        public ulong UInt64 { get; set; }

        public short? Int16N { get; set; }
        public short Int16 { get; set; }

        public ushort? UInt16N { get; set; }
        public ushort UInt16 { get; set; }

        public sbyte? Int8N { get; set; }
        public sbyte Int8 { get; set; }

        public byte? UInt8N { get; set; }
        public byte UInt8 { get; set; }

        public float? Float32N { get; set; }
        public float Float32 { get; set; }

        public double? Float64N { get; set; }
        public double Float64 { get; set; }

        public decimal? DecimalN { get; set; }
        public decimal Decimal { get; set; }

        public bool? BoolN { get; set; }
        public bool Bool { get; set; }

        public char? CharN { get; set; }
        public char Char { get; set; }

        public Guid? GuidN { get; set; }
        public Guid Guid { get; set; }

        public DateTime? DateTimeN { get; set; }
        public DateTime DateTime { get; set; }

        public DateTimeOffset? DateTimeOffsetN { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }

        public Gender? EnumN { get; set; }
        public Gender Enum { get; set; }

        public string String { get; set; }

        public Unit Inner { get; set; }
    }
}