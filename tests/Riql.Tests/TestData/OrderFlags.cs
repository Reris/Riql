using System;

namespace Riql.Tests.TestData;

[Flags]
public enum OrderFlags
{
    Important = 1 << 0,
    Deprecated = 1 << 1,
    Reconcider = 1 << 2,
    Escalated = 1 << 3
}