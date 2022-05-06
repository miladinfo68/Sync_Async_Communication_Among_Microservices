using Play.Inventory.Dtos;

namespace Play.Inventory.Client;

public class CatalogClient
{
    private readonly HttpClient _http;
    public CatalogClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyCollection<CatalogItemDto>> GetCatalogItemsAsync()
    {
        var items = await _http.GetFromJsonAsync<IReadOnlyCollection<CatalogItemDto>>("api/Items");
        return items;
    }
}