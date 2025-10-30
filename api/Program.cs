using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);

app.MapGet("/", () => "Hello World! 4");
app.MapGet("/user", () => new { Name = "Liniker", Age = 26 });
app.MapGet("/AddHeader", (HttpResponse response) => {
    response.Headers.Append("Teste", "MyCustomHeaderValue");
    return new { Name = "Liniker", Age = 26 };
});


app.MapPost("/products", (Product product) =>
{
    ProductRepository.Add(product);
    return Results.Created($"/products/{product.Code}", product.Code);
});


//api.app.com/users?datastart={date}&dateend={date}

app.MapGet("/products", ([FromQuery] string dateStart, [FromQuery] string dateEnd) =>
{
    return dateStart + " - " + dateEnd;
});

//api.app.com/users/{code}
app.MapGet("/products/{code}", ([FromRoute] string code) =>
{
    var product = ProductRepository.GetBy(code);
    if (product != null)
        return Results.Ok(product);

    return Results.NotFound();
});


app.MapPut("/products", (Product product) =>
{
    var productSaved = ProductRepository.GetBy(product.Code);
    productSaved.Name = product.Name;
    return Results.Ok(productSaved);
});

app.MapDelete("/products/{code}", ([FromRoute] string code) =>
{
    var productSaved = ProductRepository.GetBy(code);
    ProductRepository.Remove(productSaved);
    return Results.Ok(productSaved);
});
app.MapGet("/configuration/database", (IConfiguration configuration) =>
{
   return Results.Ok($"{configuration["database:connection"]}/{configuration["database:port"]}");
});

// app.MapGet("/getproductbyheader", (HttpRequest request) =>
// {
//     return request.Headers["product-code"].ToString();
// });


app.Run();


public static class ProductRepository
{
    public static List<Product> Products { get; set; } = Products = new List<Product>();

    public static void Init(IConfiguration configuration)
    {
        var products = configuration.GetSection("Products").Get<List<Product>>();
        Products = products;
    }

    public static void Add(Product product)
    {
        if (Products == null)
            Products = new List<Product>();

        Products.Add(product);
    }

    public static Product GetBy(string code)
    {
        if (Products == null)
            return null;

        return Products.FirstOrDefault(p => p.Code == code);
    }

    public static void Remove(Product product)
    {
        Products.Remove(product);
    }
}

public class Product
{
    public string Code { get; set; }

    public string Name { get; set; }
}