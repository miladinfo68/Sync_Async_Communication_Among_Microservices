using Microsoft.AspNetCore.Mvc;
using Play.Inventory.Dtos;
using Play.Inventory.Entities;
using Play.Inventory.Mapping;
using Play.Inventory.Client;
using Play.Common.Abstractions.Repositories;
using Play.Common.Concretes;

namespace Play.Inventory.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly ILogger<InventoryController> _logger;
    private readonly IBaseRepository<InventoryItem> _inventoryRepository;
    private readonly IBaseRepository<CatalogItem> _catalogRepository;
    private readonly CatalogClient _client;


    public InventoryController(
    IBaseRepository<InventoryItem> inventoryRepository,
    IBaseRepository<CatalogItem> catalogRepository,
    CatalogClient client,
    ILogger<InventoryController> logger)
    {
        _logger = logger;
        _client = client;
        _catalogRepository = catalogRepository;
        _inventoryRepository = inventoryRepository;
    }


    // [HttpGet("{userId}")]
    // public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetById(Guid userId)
    // {
    //     if (userId == Guid.Empty) return BadRequest();
    //     var items = (await _inventoryRepository.GetAll(x => x.UserId == userId))
    //                 .Select(a => a.AsDto());
    //     if (items?.Count() == 0) return NotFound();
    //     return Ok(items);
    // }


    [HttpGet("GetDataFromCatalogServiceByHttpClient/{userId}")]
    public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetDataFromCatalogServiceByHttpClient(Guid userId)
    {
        if (userId == Guid.Empty) return BadRequest();
        var inventoryItems = await _inventoryRepository.GetAll(invItem => invItem.UserId == userId);
        var catalogItems = await _client.GetCatalogItemsAsync();
        var inventoryItemsDto = inventoryItems.Select(invItem =>
        {
            var catItem = catalogItems.SingleOrDefault(ci => ci.Id == invItem.CatalogItemId);
            return invItem.AsDto(catItem?.Name ?? "", catItem?.Description ?? "");
        });
        return Ok(inventoryItemsDto);
    }


    [HttpGet("GetDataFromCatalogServiceByRabbitMqBus/{userId}")]
    public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetDataFromCatalogServiceByRabbitMqBus(Guid userId)
    {
        if (userId == Guid.Empty) return BadRequest();
        var inventoryItems = await _inventoryRepository.GetAll(invItem => invItem.UserId == userId);
        var catalogIds = inventoryItems.Select(s => s.CatalogItemId);
        var catalogItems = await _catalogRepository.GetAll(c => catalogIds.Contains(c.Id));
        var inventoryItemsDto = inventoryItems.Select(invItem =>
        {
            var catItem = catalogItems.SingleOrDefault(ci => ci.Id == invItem.CatalogItemId);
            return invItem.AsDto(catItem?.Name ?? "", catItem?.Description ?? "");
        });
        return Ok(inventoryItemsDto);
    }


    [HttpPost]
    public async Task<ActionResult> PostAsync(GrantItemsDto item)
    {
        var inventoryItem = await _inventoryRepository.GetBy(x =>
            x.UserId == item.UserId && x.CatalogItemId == item.CatalogItemId);
        if (inventoryItem is null)
        {
            var newInventoryItem = item.AsInvetoryItem();
            await _inventoryRepository.Create(newInventoryItem);
        }
        else
        {
            inventoryItem.Quantity += item.Quantity;
            await _inventoryRepository.Update(inventoryItem);
        }
        return Ok();
    }

}
