using System;

namespace Riql.Tests.TestData
{
    public static class TestSamples
    {
        public static Employee[] GetEmployees()
        {
            return new[]
            {
                new Employee
                {
                    Id = Guid.Parse("71704121-0E9E-4780-BF82-8E1276CAD607"),
                    FirstName = "John",
                    LastName = "Doe",
                    Probation = false,
                    RoomNr = 101,
                    Birthday = new DateTime(1980, 1, 1),
                    Gender = Gender.Male,
                    Weight = 72.5f,
                    Height = 1.8,
                    Salary = 5500
                },

                new Employee
                {
                    Id = Guid.Parse("2BBC0062-F2EF-43A4-923C-867A4D1575E4"),
                    FirstName = "Jane",
                    LastName = "Doe",
                    Probation = false,
                    RoomNr = 105,
                    Birthday = new DateTime(1972, 12, 11),
                    Gender = Gender.Female,
                    Weight = 64f,
                    Height = 1.6,
                    Salary = 5600
                },
                new Employee
                {
                    Id = Guid.Parse("4F83FD3D-4242-4262-A36B-8E2098C9AA55"),
                    FirstName = "Alice",
                    LastName = "Smith",
                    Probation = false,
                    RoomNr = 101,
                    Birthday = new DateTime(1985, 6, 17),
                    Gender = Gender.Female,
                    Weight = 66.0f,
                    Height = 1.72,
                    Salary = 8500
                },

                new Employee
                {
                    Id = Guid.Parse("8E3672B0-9ED8-49AA-BD63-C75EC8C236A0"),
                    FirstName = "Bob",
                    LastName = "Martin",
                    Probation = false,
                    RoomNr = 105,
                    Birthday = new DateTime(1966, 5, 3),
                    Gender = Gender.Male,
                    Weight = 88f,
                    Height = 1.82,
                    Salary = 10600
                },
                new Employee
                {
                    Id = Guid.Parse("2D84F5FC-AB8F-4370-86C1-CED250D89381"),
                    FirstName = "Alex",
                    LastName = "Moore",
                    Probation = false,
                    RoomNr = 105,
                    Birthday = new DateTime(1990, 3, 28),
                    Gender = Gender.Other,
                    Weight = 66f,
                    Height = 1.73,
                    Salary = 5700
                },
                new Employee
                {
                    Id = Guid.Parse("340C71D8-13CB-473A-B57A-A3AD0D75F47C"),
                    FirstName = "Jack",
                    LastName = "Vane",
                    Probation = true,
                    RoomNr = 101,
                    Birthday = new DateTime(2001, 8, 12),
                    Gender = Gender.Male,
                    Weight = 90f,
                    Height = 1.9,
                    Salary = 3500
                },

                new Employee
                {
                    Id = Guid.Parse("5AB4ECB7-1456-42E5-9222-3D3E710B51DE"),
                    FirstName = "Lisa",
                    LastName = "Lane",
                    Probation = true,
                    RoomNr = 105,
                    Birthday = new DateTime(2002, 4, 4),
                    Gender = Gender.Female,
                    Weight = 56f,
                    Height = 1.65,
                    Salary = 3500
                },
                new Employee
                {
                    Id = Guid.Parse("681B16EB-B9B3-47A0-A622-A5810AC99A0F"),
                    FirstName = "Charles",
                    LastName = "Smith",
                    Probation = true,
                    RoomNr = 101,
                    Birthday = new DateTime(2002, 3, 23),
                    Gender = Gender.Male,
                    Weight = 82f,
                    Height = 1.86,
                    Salary = 3600
                },

                new Employee
                {
                    Id = Guid.Parse("8EAAE17C-7425-4EE8-B240-DAB87F5A3BCC"),
                    FirstName = "Amy",
                    LastName = "Addams",
                    Probation = true,
                    RoomNr = 105,
                    Birthday = new DateTime(2004, 2, 7),
                    Gender = Gender.Female,
                    Weight = 55f,
                    Height = 1.66,
                    Salary = 3700
                },

                new Employee
                {
                    Id = Guid.Parse("20951B83-BA5B-4D33-9410-D16EBF7DB91D"),
                    FirstName = "Ray",
                    LastName = "Parker",
                    Probation = false,
                    RoomNr = 102,
                    Birthday = new DateTime(1982, 6, 25),
                    Gender = Gender.Male,
                    Weight = 75f,
                    Height = 1.82,
                    Salary = 7600
                }
            };
        }

        public static Order[] GetOrders()
        {
            var employees = TestSamples.GetEmployees();
            return new[]
            {
                new Order
                {
                    Id = 1,
                    Requirement = "MUST do 1",
                    ManagerId = employees[1].Id,
                    Started = new DateTime(2019, 7, 24, 17, 50, 33),
                    Price = 50,
                    Accepted = true,
                    Flags = OrderFlags.Important
                },
                new Order
                {
                    Id = 2,
                    Requirement = null,
                    ManagerId = null,
                    Started = null,
                    Price = null,
                    Accepted = null,
                    Flags = null
                },
                new Order
                {
                    Id = 3,
                    Requirement = "MUST do 3",
                    ManagerId = employees[3].Id,
                    Started = new DateTime(2013, 3, 4, 13, 37, 33),
                    Price = 3,
                    Accepted = false,
                    Flags = OrderFlags.Deprecated | OrderFlags.Reconcider
                },
                new Order
                {
                    Id = 4,
                    Requirement = "MUST do 4",
                    ManagerId = employees[1].Id,
                    Started = new DateTime(2014, 4, 5, 14, 57, 0),
                    Price = 300,
                    Accepted = false,
                    Flags = OrderFlags.Escalated | OrderFlags.Reconcider
                },
                new Order
                {
                    Id = 5,
                    Requirement = "MUST do 5",
                    ManagerId = employees[5].Id,
                    Started = null,
                    Price = 500,
                    Accepted = null,
                    Flags = null
                },
                new Order
                {
                    Id = 6,
                    Requirement = "MUST do 6",
                    ManagerId = employees[2].Id,
                    Started = null,
                    Price = 606,
                    Accepted = true,
                    Flags = null
                },
                new Order
                {
                    Id = 7,
                    Requirement = "MUST do 7",
                    ManagerId = employees[1].Id,
                    Started = null,
                    Price = 42,
                    Accepted = null,
                    Flags = null
                },
                new Order
                {
                    Id = 8,
                    Requirement = "MUST do 8",
                    ManagerId = employees[5].Id,
                    Started = null,
                    Price = 56,
                    Accepted = false,
                    Flags = null
                },
                new Order
                {
                    Id = 9,
                    Requirement = "MUST do 9",
                    ManagerId = employees[4].Id,
                    Started = new DateTime(2018, 8, 1),
                    Price = -100,
                    Accepted = true,
                    Flags = OrderFlags.Escalated
                },
                new Order
                {
                    Id = 10,
                    Requirement = "MUST do 10",
                    ManagerId = employees[3].Id,
                    Started = null,
                    Price = 500,
                    Accepted = null,
                    Flags = null
                },
                new Order
                {
                    Id = 11,
                    Requirement = "MUST do 11",
                    ManagerId = null,
                    Started = null,
                    Price = -5.5m,
                    Accepted = null,
                    Flags = null
                },
                new Order
                {
                    Id = 12,
                    Requirement = "MUST do 12",
                    ManagerId = employees[4].Id,
                    Started = null,
                    Price = null,
                    Accepted = null,
                    Flags = null
                }
            };
        }
    }
}