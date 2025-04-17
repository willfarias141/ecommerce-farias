using Amazon;
using Amazon.S3;
using EcommerceFarias.Data;
using EcommerceFarias.Middlewares;
using EcommerceFarias.Model;
using EcommerceFarias.Model.Discount;
using EcommerceFarias.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "E-commerce Farias API",
        Description = "Uma API REST para gerenciamento de um e-commerce, inicialmente será usada para manter o catálogo de produtos.",
        Contact = new OpenApiContact
        {
            Email = "willian.farias141@gmail.com"
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<AwsSettings>(builder.Configuration.GetSection("AWS"));
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var config = sp.GetRequiredService<IOptions<AwsSettings>>().Value;
    var awsConfig = new AmazonS3Config
    {
        RegionEndpoint = RegionEndpoint.GetBySystemName(config.Region)
    };
    return new AmazonS3Client(config.AccessKey, config.SecretKey, awsConfig);
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddScoped<IDiscountStrategyFactory, DiscountStrategyFactory>();
builder.Services.AddScoped<ElectronicsDiscount>();
builder.Services.AddScoped<ClothingDiscount>();
builder.Services.AddScoped<NoDiscount>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();
