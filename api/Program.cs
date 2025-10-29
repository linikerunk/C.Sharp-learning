using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World! 4");
app.MapGet("/user", () => new { Name = "Liniker", Age = 26 });
app.MapGet("/AddHeader", (HttpResponse response) => {
    response.Headers.Append("Teste", "MyCustomHeaderValue");
    return new { Name = "Liniker", Age = 26 };
});


app.MapPost("/saveproduct", (Product product) =>
{
    ProductRepository.Add(product);
});


//api.app.com/users?datastart={date}&dateend={date}

app.MapGet("/getproduct", ([FromQuery] string dateStart, [FromQuery] string dateEnd) =>
{
    return dateStart + " - " + dateEnd;
});

//api.app.com/users/{code}
app.MapGet("/getproduct/{code}", ([FromRoute] string code) =>
{
    var product = ProductRepository.GetBy(code);
    return product;
});


app.MapGet("/getproductbyheader", (HttpRequest request) =>
{
    return request.Headers["product-code"].ToString();
});

app.Run();


public static class ProductRepository
{
    public static List<Product> Products { get; set; }

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
    
}

public class Product
{
    public string Code { get; set; }

    public string Name { get; set; }
}