using Workflow.Infrastructure;        // AddInfrastructure/AddApplication
using Workflow.Infrastructure.Data;
using Demo.Workflow.Endpoints;         // your endpoint mapping ext
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:5173")));

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
