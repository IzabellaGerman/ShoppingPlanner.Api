using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using ShoppingPlanner.Mcp;
using System.ComponentModel;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConsole(consoleLogOptions =>
{
    // all logs — в stderr
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.Services.AddSingleton<TokenStore>();

builder.Services.AddHttpClient<ShoppingPlannerClient>(client =>
{
    var baseUrl = builder.Configuration["ShoppingPlanner:BaseUrl"]
        ?? throw new InvalidOperationException("ShoppingPlanner:BaseUrl is not configured");

    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();
await builder.Build().RunAsync();

[McpServerToolType]
public class ShoppingListTools
    {
    private readonly ShoppingPlannerClient _client;

    public ShoppingListTools(ShoppingPlannerClient client) => _client = client;

    [McpServerTool(Name = "search_product"), Description(
        "Searches the product catalogue by part of the product name. " +
        "Returns matching products with their numeric ids, which add_item requires.")]
    public Task<string> SearchProductAsync(
        [Description("Part of the product name, for example 'mleko'.")] string query,
        CancellationToken ct)
        => _client.SearchProductsAsync(query, ct);

    // get_lists и add_item пока оставь как есть, только убери у них static
    }

[McpServerToolType]
public class ProductTools
    {
    [McpServerTool(Name = "search_product"), Description("Searches products by name, returns matching products with their ids.")]
    public string SearchProduct(
        [Description("Part of the product name to search for, e.g. 'milk'")] string query)
        => $"Results for '{query}':\n12: Milk 1l\n34: Milk semi-skimmed 1.5l";
    }

[McpServerToolType]
public static class ItemTools
    {
    [McpServerTool(Name = "add_item"), Description("Adds a product to a shopping list. Use ids returned by search_product and get_lists")]
    public static string AddItem(
        [Description("Id of the shopping list, from get_lists")] int listId,
        [Description("Id of the product, from search_product")] int productId,
        [Description("How many units to add")] int quantity) 
        => $"Added {quantity}× product {productId} to list {listId}";
    }