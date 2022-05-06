namespace Play.Inventory.Dtos;

public record GrantItemsDto(Guid UserId, Guid CatalogItemId, int Quantity);
// public record InventoryItemDto(Guid CatalogItemId, int Quantity, DateTimeOffset AccuiredDate);
public record InventoryItemDto(Guid CatalogItemId, string Name, string Description, int Quantity, DateTimeOffset AccuiredDate);
public record CatalogItemDto(Guid Id, string Name, string Description);

