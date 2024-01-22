using BizApi.Extensions;
using BizApi.Modules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BizApi.Data;
using BizApi.Data.Database;

var MyPolicy = "_myPolicy";

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyPolicy, policy =>
    {
        policy.WithOrigins("https://localhost:7196", "http://localhost:5147")
        .AllowAnyMethod()
        .WithHeaders(HeaderNames.ContentType);
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddServicesToContainer(builder.Configuration.GetConnectionString("DefaultConnection")!);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyPolicy);

app.UseHttpsRedirection();

app.MapCategoryEndpoint();
app.MapCustomerEndpoint(Path.Combine(app.Environment.ContentRootPath.Replace("\\BizApi", "\\BizManager"), "wwwroot\\images\\Customers"));
app.MapMaterialEndpoint(Path.Combine(app.Environment.ContentRootPath.Replace("\\BizApi", "\\BizManager"), "wwwroot\\images\\Materials"));
app.MapProductEndpoint(Path.Combine(app.Environment.ContentRootPath.Replace("\\BizApi", "\\BizManager"), "wwwroot\\images\\Products"));
app.MapProductionEndpoint();
app.MapOrderEndpoint();
app.MapSupplierEndpoint(Path.Combine(app.Environment.ContentRootPath.Replace("\\BizApi", "\\BizManager"), "wwwroot\\images\\Suppliers"));
app.MapPurchaseEndpoint();

app.MigrateDatabase();

app.Run();
