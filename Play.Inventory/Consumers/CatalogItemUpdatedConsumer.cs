using MassTransit;
using Play.Common.Events;
using Play.Common.Abstractions.Repositories;
using Play.Inventory.Entities;

namespace Play.Inventory.Consumers;
public class CatalogItemUpdatedConsumer : IConsumer<CatalogItemUpdated>
{
    private readonly IBaseRepository<CatalogItem> _catalogItemRepository;
    public CatalogItemUpdatedConsumer(IBaseRepository<CatalogItem> catalogItemRepository)
    {
        _catalogItemRepository = catalogItemRepository;
    }
    public async Task Consume(ConsumeContext<CatalogItemUpdated> context)
    {
        var message = context.Message;
        var item = await _catalogItemRepository.Get(message.ItemId);
        if (item == null)
        {
            item = new CatalogItem
            {
                Id = message.ItemId,
                Name = message.Name,
                Description = message.Description
            };
            await _catalogItemRepository.Create(item);
        }
        else
        {
            item.Name = message.Name;
            item.Description = message.Description;
            await _catalogItemRepository.Update(item);
        }
    }
}
