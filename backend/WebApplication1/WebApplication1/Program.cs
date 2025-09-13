using AutoMapper;
using Database;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using WebApplication1.Mapping;

namespace WebApplication1;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        
        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        
        ContextDb.AddDbContext(builder.Services, builder.Configuration.GetConnectionString("DefaultConnection"));
        builder.Services.AddAutoMapper(cfg => AutoMappingProfile.UseAutoMappingProfile(cfg));
        
        var app = builder.Build();

        ContextDb.Seed(app.Services).GetAwaiter().GetResult();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}