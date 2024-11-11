using DocumentManagement.API.Configuration;
using DocumentManagement.API.Controllers;
using DocumentManagement.API.Middleware;
using DocumentManagement.Core.Validation;
using DocumentManagement.Domain;
using DocumentManagement.Domain.Documents.Command;
using DocumentManagement.Infrastructure;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateDocumentCommand>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApiConfiguration(builder.Configuration);
builder.Services.AddApiVersionConfiguration<BaseController>();
builder.Services.RegisterApplicationServices(builder.Configuration);
builder.Services.RegisterInfrastructureServices(builder.Configuration);
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddTransient<TraceMiddleware>();
builder.Services.AddAutoMapperConfiguration();
builder.Services.AddSingleton<IContextConfiguration, DataContextConfiguration>();

builder.Services.AddHealthChecks()
               .AddCheck("Health Check", () => HealthCheckResult.Healthy());

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(typeof(CreateDocumentCommand).Assembly);
});

var app = builder.Build();

using var scope = app.Services.CreateScope();
await using var dbContext = scope.ServiceProvider.GetRequiredService<DocumentContext>();
await dbContext.Database.MigrateAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.AddOpenApiConfiguration(app.Configuration);
}

app.UseHttpsRedirection();
app.RegisterApplicationServices();

app.UseMiddleware<TraceMiddleware>();
app.UseCustomExceptionMiddleware();

app.MapHealthChecks("api/health");
app.MapControllers();

app.Run();

namespace DocumentManagement.API
{
    public partial class Program { }
}
