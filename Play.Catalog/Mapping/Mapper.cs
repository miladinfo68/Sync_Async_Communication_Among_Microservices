using Play.Catalog.Dtos;
using Play.Catalog.Entities;
using Play.Common.Events;

namespace Play.Catalog.Mapping;
public static class Mapper
{
    public static ItemDto AsItemDto(this Item item)
    {
        return new ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
    }

    public static CatalogItemCreated AsAddCatalogEvent(this Item item)
    {
        return new CatalogItemCreated(item.Id, item.Name, item.Description);
    }

    public static CatalogItemUpdated AsEditCatalogEvent(this Item item)
    {
        return new CatalogItemUpdated(item.Id, item.Name, item.Description);
    }

    public static Item AsItem(this CreateItemDto item)
    {
        return new Item
        {
            Name = item.Name,
            Description = item.Description,
            Price = item.Price,
            CreatedDate = DateTimeOffset.UtcNow
        };
    }

    public static Item AsEditItem(this UpdateItemDto item, Guid id)
    {
        return new Item
        {
            Id = id,
            Name = item.Name,
            Description = item.Description,
            Price = item.Price
        };
    }
}
