namespace OrderManagement.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;
    public DateTime OrderDate { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();

    private Order() { }

    public Order(Customer customer)
    {
        Id = Guid.NewGuid();
        CustomerId = customer.Id;
        OrderDate = DateTime.UtcNow;
        Status = OrderStatus.Pending;
    }

    public void AddItem(Product product, int quantity)
    {
        var item = new OrderItem(product.Id, product.Price, quantity);
        Items.Add(item);
        TotalAmount += item.Price * item.Quantity;
        product.DecreaseStock(quantity); // упрощённо, лучше через доменное событие
    }

    public void UpdateStatus(OrderStatus newStatus)
    {
        Status = newStatus;
    }
}

public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }

    private OrderItem() { }

    public OrderItem(Guid productId, decimal price, int quantity)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Price = price;
        Quantity = quantity;
    }
}