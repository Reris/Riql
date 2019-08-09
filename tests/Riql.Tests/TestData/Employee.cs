using System;

namespace Riql.Tests.TestData
{
    public class Employee
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Probation { get; set; }
        public int RoomNr { get; set; }
        public DateTime Birthday { get; set; }
        public Gender Gender { get; set; }
        public float Weight { get; set; }
        public double Height { get; set; }
        public decimal Salary { get; set; }
    }
}