using MediaService.src.Application.Commands.Concretes;
using MediaService.Src.Application.Commands.Interfaces;
using MediaService.Src.Application.Interfaces;
using MediaService.Src.Infrastructure.Configuration;
using MediaService.Src.Infrastructure.Services;
using MediaService.src.WebApi.Controllers.Filters;

var builder = WebApplication.CreateBuilder(args);

builder
    .Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile(
        $"appsettings.{builder.Environment.EnvironmentName}.json",
        optional: true,
        reloadOnChange: true
    )
    .AddEnvironmentVariables();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});
builder.Services.AddScoped<IStorageService, BackblazeB2StorageService>();
builder.Services.AddScoped<IUploadImageCommand, UploadImageCommand>();
builder.Services.AddScoped<IDeleteImageCommand, DeleteImageCommand>();
builder.Services.AddScoped<IUploadMultipleImagesCommand, UploadMultipleImagesCommand>();
builder.Services.AddScoped<IGetImageUrlCommand, GetImageUrlCommand>();

builder.Services.Configure<StorageSettings>(builder.Configuration.GetSection("StorageSettings"));
builder.Services.Configure<BackBlazeCredentials>(
    builder.Configuration.GetSection("BackBlazeCredentials")
);

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowFrontend",
        policy =>
        {
            if (allowedOrigins != null && allowedOrigins.Length > 0)
            {
                policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }
            else
            {
                policy
                    .WithOrigins("http://localhost:3000", "https://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }
        });
});

var app = builder.Build();
app.MapControllers();
app.UseCors("AllowFrontEnd");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();
