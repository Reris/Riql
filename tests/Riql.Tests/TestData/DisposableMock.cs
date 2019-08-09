using System;

namespace Riql.Tests.TestData
{
    public struct DisposableMock : IDisposable
    {
        public void Dispose()
        {
        }
    }
}