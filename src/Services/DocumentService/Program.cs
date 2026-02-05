using DocumentService.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CogniSearch Enterprise API",
        Version = "v1",
        Description = "Enterprise Document Management System API Documentation"
    });
});
builder.Services.AddOpenApi();

// DbContext
builder.Services.AddDbContext<DocumentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// RabbitMQ (MassTransit)
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("admin");
            h.Password("admin");
        });
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddScoped<DocumentService.Services.IDocumentService, DocumentService.Services.DocumentService>();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "CogniSearch API v1"); });
}

app.MapControllers();
app.UseHttpsRedirection();
app.Run();