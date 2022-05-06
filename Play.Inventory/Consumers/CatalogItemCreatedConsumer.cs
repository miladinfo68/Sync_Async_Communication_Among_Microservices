using MassTransit;
using Play.Common.Events;
using Play.Common.Abstractions.Repositories;
using Play.Inventory.Entities;

namespace Play.Inventory.Consumers;
public class CatalogItemCreatedConsumer : IConsumer<CatalogItemCreated>
{
    private readonly IBaseRepository<CatalogItem> _catalogItemRepository;
    public CatalogItemCreatedConsumer(IBaseRepository<CatalogItem> catalogItemRepository)
    {
        _catalogItemRepository = catalogItemRepository;
    }
    public async Task Consume(ConsumeContext<CatalogItemCreated> context)
    {
        var message = context.Message;
        var item = await _catalogItemRepository.Get(message.ItemId);
        if (item != null) return; //it is already exist in database
        item = new CatalogItem
        {
            Id = message.ItemId,
            Name = message.Name,
            Description = message.Description
        };
        await _catalogItemRepository.Create(item);
    }
}
