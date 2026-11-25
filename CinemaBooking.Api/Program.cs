using CinemaBooking.Api.Data;
using CinemaBooking.Api.Services;
using Microsoft.EntityFrameworkCore;
using CinemaBooking.Api.Models;
using CinemaBooking.Api.Interfaces;
namespace CinemaBooking.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            
            builder.Services.AddScoped<IMovieService, MovieService>();
            builder.Services.AddScoped<IShowtimeService, ShowtimeService>();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var connectionString = builder.Configuration.GetConnectionString("CinemaDb");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Missing CinemaDb connection string");
            }
            builder.Services.AddDbContext<CinemaDbContext>(options =>
                options.UseMySQL(connectionString));

            var app = builder.Build();

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
}

