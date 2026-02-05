using DocumentService.Consumers;
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
// Exception Handling
builder.Services.AddExceptionHandler<BuildingBlocks.Middleware.GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// RabbitMQ (MassTransit)
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<DocumentCompletedConsumer>();
    x.AddConsumer<DocumentProcessingConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"], "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"]!);
            h.Password(builder.Configuration["RabbitMQ:Password"]!);
        });
        cfg.ConfigureEndpoints(context);
    });
});

// Options
builder.Services.AddOptions<DocumentService.Options.FileStorageOptions>()
    .Bind(builder.Configuration.GetSection(DocumentService.Options.FileStorageOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddScoped<DocumentService.Services.IFileStorageService, DocumentService.Services.FileStorageService>();
builder.Services.AddScoped<DocumentService.Services.IDocumentService, DocumentService.Services.DocumentService>();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "CogniSearch API v1"); });
}

app.MapControllers();
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.Run();