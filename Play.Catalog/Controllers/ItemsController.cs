using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Dtos;
using Play.Catalog.Entities;
using Play.Catalog.Mapping;
using Play.Common.Abstractions.Repositories;
using Play.Common.Abstractions.Services;
using Play.Common.Events;

namespace Play.Catalog.Controllers;


public class ItemsController : RichedBaseController<ItemsController, Item>
{
    private static int requestCounter = 0;
    public ItemsController(IBaseRepository<Item> store,
    IPublishEndpoint buss,
    ILogger<ItemsController> logger)
        : base(store, buss, logger)
    {
    }


    //action method to simulating of partial/temperal failure when this service is out of access for another external service

    [HttpGet("GetAllWithSimulatingTemperalFailure")]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetAllWithSimulatingTemperalFailure()
    {
        requestCounter++;
        System.Console.WriteLine($"Request : {requestCounter} starting ....");
        if (requestCounter < 3)
        {
            System.Console.WriteLine($"Request : {requestCounter} Delaying ....");
            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        if (requestCounter < 5)
        {
            System.Console.WriteLine($"Request : {requestCounter} Internal Server Error (Status 500) ....");
            return StatusCode(500);
        }


        var items = (await _store.GetAll()).Select(item => item.AsItemDto());
        System.Console.WriteLine($"Request : {requestCounter} Ended (Status 200) ....");
        return Ok(items);
    }


    [HttpGet("GetAll")]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetAll()
    {
        var items = (await _store.GetAll()).Select(item => item.AsItemDto());
        return Ok(items);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetById(Guid id)
    {
        var item = await _store.Get(id);
        if (item is null) return NotFound();
        return Ok(item);
    }


    [HttpPost]
    public async Task<ActionResult<ItemDto>> Post(CreateItemDto item)
    {
        var newItem = item.AsItem();
        await _store.Create(newItem);
        await _bus.Publish(newItem.AsAddCatalogEvent());
        return CreatedAtAction(nameof(GetById), new { id = newItem.Id }, newItem);
    }

    [HttpPut]
    public async Task<ActionResult> Put(Guid id, UpdateItemDto item)
    {
        var existItem = await _store.Get(id);
        if (existItem is null) return NotFound();
        var updated = item.AsEditItem(id);

        await _store.Update(updated);
        await _bus.Publish(updated.AsEditCatalogEvent());
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var existItem = await _store.Get(id);
        if (existItem is null) return NotFound();

        await _store.Delete(id);
        await _bus.Publish(new CatalogItemDeleted(id));
        return NoContent();
    }


}
