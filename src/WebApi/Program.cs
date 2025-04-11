using MediaService.src.Application.Commands.Concretes;
using MediaService.Src.Application.Commands.Interfaces;
using MediaService.Src.Application.Interfaces;
using MediaService.Src.Infrastructure.Configuration;
using MediaService.Src.Infrastructure.Services;
using MediaService.src.WebApi.Controllers.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
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
builder.Services.Configure<BackBlazeCredentials>(builder.Configuration.GetSection("BackBlazeCredentials"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();
