using Workflow.Infrastructure;        // AddInfrastructure/AddApplication
using Workflow.Infrastructure.Data;
using Demo.Workflow.Endpoints;         // your endpoint mapping ext
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173", "http://127.0.0.1:3000")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure JSON serialization to handle string enums and circular references
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

// Layered DI
builder.Services.AddApplication()
                .AddInfrastructure(); // null => InMemory

var app = builder.Build();
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

// call seed method (place it in Infrastructure or Api as a small helper)
await SeedData.EnsureSeededAsync(app.Services);

app.MapRequestEndpoints();
app.Run();
