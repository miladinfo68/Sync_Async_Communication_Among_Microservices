using Play.Inventory.Dtos;
using Play.Inventory.Entities;

namespace Play.Inventory.Mapping;
public static class Mapper
{
    // public static InventoryItemDto AsDto(this InventoryItem item)
    // {
    //     return new InventoryItemDto(item.CatalogItemId, item.Quantity, item.AccuiredDate);
    // }

    public static InventoryItemDto AsDto(this InventoryItem item, string name, string description)
    {
        return new InventoryItemDto(item.CatalogItemId, name, description, item.Quantity, item.AccuiredDate);
    }

    public static InventoryItem AsInvetoryItem(this GrantItemsDto item)
    {
        return new InventoryItem
        {
            CatalogItemId = item.CatalogItemId,
            UserId = item.UserId,
            Quantity = item.Quantity,
            AccuiredDate = DateTimeOffset.UtcNow
        };
    }
}
