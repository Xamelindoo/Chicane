using Chicane.Backend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Redis cache
var redis = builder.Configuration["Redis:ConnectionString"] ?? "redis:6379";
builder.Services.AddStackExchangeRedisCache(o => { o.Configuration = redis; o.InstanceName = "chicane_"; });

// HTTP clients + services
builder.Services.AddHttpClient<OpenF1Service>();
builder.Services.AddHttpClient<OpenWeatherService>();
builder.Services.AddHttpClient<Rss2JsonService>();

// CORS
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.MapControllers();

app.Run();
