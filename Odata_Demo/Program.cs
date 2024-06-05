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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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
    return odataBuilder.GetEdmModel();
}
