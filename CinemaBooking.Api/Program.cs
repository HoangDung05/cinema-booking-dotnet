using CinemaBooking.Api.Data;
using CinemaBooking.Api.Services;
using Microsoft.EntityFrameworkCore;
using CinemaBooking.Api.Models;
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
                app.MapGet("/dev/db-health", async (CinemaDbContext context) =>
                {
                    bool connected = await context.Database.CanConnectAsync();
                    if (connected)
                        return Results.Ok("Database is healthy");
                    else
                        return Results.StatusCode(503);
                });

                app.MapGet("/dev/movies-db", async (CinemaDbContext context) =>
                {
                    var movies = await context.Movies.AsNoTracking().ToListAsync();
                    return Results.Ok(movies);
                });

                app.MapGet("/dev/movies-db/{id:int}", async (int id, CinemaDbContext context) =>
                {
                    var movie = await context.Movies.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
                    if (movie == null)
                        return Results.NotFound();
                    return Results.Ok(movie);
                });

                app.MapGet("/dev/movies-db/long", async (CinemaDbContext context) =>
                {
                    var movies = await context.Movies.AsNoTracking().Where(b => b.Duration > 120).ToListAsync();
                    return Results.Ok(movies);
                });

                app.MapGet("/dev/movies-db/now-showing", async (CinemaDbContext context) =>
                {
                    var movies = await context.Movies.AsNoTracking().Where(b => b.Status == MovieStatus.NOW_SHOWING).ToListAsync();
                    return Results.Ok(movies);
                });

                app.MapGet("/dev/movies-db/coming-soon", async (CinemaDbContext context) =>
                {
                    var movies = await context.Movies.AsNoTracking().Where(b => b.Status == MovieStatus.COMING_SOON).ToListAsync();
                    return Results.Ok(movies);
                });

                app.MapGet("/dev/movies-db/search", async (string? keyword, CinemaDbContext context) =>
                {
                    if (string.IsNullOrWhiteSpace(keyword))
                        return Results.BadRequest();
                    keyword = keyword.Trim();
                    var movies = await context.Movies.AsNoTracking().Where(b => b.Title.Contains(keyword)).ToListAsync();
                    return Results.Ok(movies);
                });

                app.MapGet("/dev/showtimes-count", async (CinemaDbContext db) =>
                {
                    var count = await db.Showtimes.CountAsync();
                    return Results.Ok(new { count });
                });
            }


            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

