using Play.Common.Entities;

namespace Play.Inventory.Entities;
public class InventoryItem : IEntity
{
    public Guid Id { get; init; }
    public Guid UserId { get; set; }
    public Guid CatalogItemId { get; set; }
    public int Quantity { get; set; }
    public DateTimeOffset AccuiredDate { get; set; }
}
