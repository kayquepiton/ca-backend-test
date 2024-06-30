using Ca.Backend.Test.API.Middlewares;
using Ca.Backend.Test.Application.Mappings;
using Ca.Backend.Test.Infra.IoC;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

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
builder.Services.AddSwaggerGen();

builder.Services.ConfigureAppDependencies(builder.Configuration);

// Add HttpClient to the container
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Adicione o middleware de exceção personalizado
app.UseMiddleware<ExceptionMiddleware>();

// Adicione outros middlewares aqui, se necessário
app.UseRouting();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
