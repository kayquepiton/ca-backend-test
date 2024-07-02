using System.Reflection;
using Ca.Backend.Test.API.Middlewares;
using Ca.Backend.Test.Application.Mappings;
using Ca.Backend.Test.Infra.IoC;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Add services to the container.
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "API para Gerenciamento de Faturamento de Clientes .NET8 (Base)", 
        Version = "v1",
        Description = "Este projeto é uma API REST desenvolvida em .NET 8.0 para gerenciar o faturamento de clientes.",
        Contact = new OpenApiContact {
            Name = "Kayque Almeida Piton",
            Email = "kayquepiton@gmail.com",
            Url = new Uri("https://github.com/kayquepiton")
        }
    });

    // Configure XML Comments to Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

});

builder.Services.ConfigureAppDependencies(builder.Configuration);

// Add HttpClient to the container
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.RoutePrefix = string.Empty;
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API para Gerenciamento de Faturamento de Clientes .NET8 (Base)");
    });
}

// Adicione o middleware de exceção personalizado
app.UseMiddleware<ExceptionMiddleware>();

// Adicione outros middlewares aqui, se necessário
app.UseRouting();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
