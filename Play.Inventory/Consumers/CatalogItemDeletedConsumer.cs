using MassTransit;
using Play.Common.Events;
using Play.Common.Abstractions.Repositories;
using Play.Inventory.Entities;

namespace Play.Inventory.Consumers;
public class CatalogItemDeletedConsumer : IConsumer<CatalogItemDeleted>
{
    private readonly IBaseRepository<CatalogItem> _catalogItemRepository;

    public CatalogItemDeletedConsumer(IBaseRepository<CatalogItem> catalogItemRepository)
    {
        _catalogItemRepository = catalogItemRepository;
    }
    public async Task Consume(ConsumeContext<CatalogItemDeleted> context)
    {
        var message = context.Message;
        var item = await _catalogItemRepository.Get(message.ItemId);
        if (item == null) return; //not exist in database

        await _catalogItemRepository.Delete(message.ItemId);
    }
}
