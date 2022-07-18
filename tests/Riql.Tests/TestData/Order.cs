using System;

namespace Riql.Tests.TestData;

public class Order
{
    public int Id { get; set; }
    public string? Requirement { get; set; }
    public Guid? ManagerId { get; set; }
    public Employee? Manager { get; set; }
    public DateTime? Started { get; set; }
    public decimal? Price { get; set; }
    public bool? Accepted { get; set; }
    public OrderFlags? Flags { get; set; }
}