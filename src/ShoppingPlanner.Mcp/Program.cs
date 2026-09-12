using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConsole(consoleLogOptions =>
{
    // all logs — в stderr
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();
await builder.Build().RunAsync();

[McpServerToolType]
public static class ShoppingListTools
    {
    [McpServerTool(Name = "get_lists"), Description("Returns all shopping lists of the current user with their id and name.")]
    public static string GetLists()
        => "1: Weekend shopping (3 items)\n2: Drogerie (5 items)";
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