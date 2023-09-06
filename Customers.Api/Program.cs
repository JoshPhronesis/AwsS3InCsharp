using Amazon.S3;
using Customers.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAmazonS3, AmazonS3Client>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.Configure<StorageConfig>(builder.Configuration.GetSection("StorageConfig"));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();