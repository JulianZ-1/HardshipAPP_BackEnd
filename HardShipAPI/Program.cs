using HardshipAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

builder.Services.AddSingleton<IHostEnvironment>(builder.Environment);

// Register SQLite service and repositories
builder.Services.AddSingleton<ISQLiteService, SQLiteService>();
builder.Services.AddScoped<IHardshipService, HardshipService>();
builder.Services.AddScoped<IDebtService, DebtService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Initialize database immediately
var sqliteService = app.Services.GetRequiredService<ISQLiteService>();

// Middleware pipeline
app.UseCors("AllowReactApp");
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hardship API v1"));
app.MapControllers();
app.Run();