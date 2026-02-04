using DocumentService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

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
builder.Services.AddDbContext<DocumentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
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