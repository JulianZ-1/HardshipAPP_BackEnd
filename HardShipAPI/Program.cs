using HardshipAPI.Services;

// Initialize the web application builder with default settings
var builder = WebApplication.CreateBuilder(args);

// Add controllers for handling API endpoints
builder.Services.AddControllers();

// Configure environment access (for path resolution)
builder.Services.AddSingleton<IHostEnvironment>(builder.Environment);

// SQLite database service (singleton for shared connection management)
builder.Services.AddSingleton<ISQLiteService, SQLiteService>();

// Scoped services (new instance per HTTP request)
builder.Services.AddScoped<IHardshipService, HardshipService>();
builder.Services.AddScoped<IDebtService, DebtService>();

// Allow cross-origin requests from React development server
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Enable API documentation generation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Build the application
var app = builder.Build();

// Force database creation
var sqliteService = app.Services.GetRequiredService<ISQLiteService>();


app.UseCors("AllowReactApp");

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hardship API v1"));

//Route controllers
app.MapControllers();

// Start the application
app.Run();