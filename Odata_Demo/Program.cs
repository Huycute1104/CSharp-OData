using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Odata_Demo.DependencyInjection;
using Repository.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register your dependencies
builder.Services.AddDatabase();
builder.Services.addUnitOfWork();

builder.Services.AddControllers().AddOData(opt => opt.Select().Expand().Filter().OrderBy().SetMaxTop(100).Count()
    .AddRouteComponents("odata", GetEdmModel()));
builder.Services.AddDbContext<FStoreDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDB")));

builder.Services.AddHealthChecks();
var app = builder.Build();

app.MapHealthChecks("/user");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Method to build the EDM model
static IEdmModel GetEdmModel()
{
    var odataBuilder = new ODataConventionModelBuilder();
    odataBuilder.EntitySet<Product>("Products");
    odataBuilder.EntitySet<Member>("Members");
    odataBuilder.EntitySet<Order>("Orders");
    odataBuilder.EntitySet<Category>("Categories");
/*    odataBuilder.EntitySet<OrderDetail>("OrderDetails");*/


    return odataBuilder.GetEdmModel();
}
