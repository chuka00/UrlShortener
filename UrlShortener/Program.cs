
using Microsoft.EntityFrameworkCore;
using UrlShortener.BLL.Shared;
using UrlShortener.DAL.Data;
using UrlShortener.DAL.Entities;

namespace UrlShortener
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConn");
            builder.Services.AddDbContext<AppDbContext>(optionsAction: options => options.UseSqlite(connectionString));

            //builder.Services.AddDbContext<AppDbContext>(options =>
            //options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConn")));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection(); 

            app.MapPost("/shorturl", async (UrlDto url, AppDbContext db, HttpContext ctx) =>
            {
                //Validating the input url
                if (!Uri.TryCreate(url.Url, UriKind.Absolute, out var inputUrl))
                    return Results.BadRequest(error: "Invalid Url has been provided");

                //Creating a short version of the provided url
                var random = new Random();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@#";
                var randomStr = new string(Enumerable.Repeat(chars, 8)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

               
                var sUrl = new UrlManagement()
                {
                    Url = url.Url,
                    ShortUrl = randomStr
                };

                db.Urls.Add(sUrl);
                db.SaveChangesAsync();

                //construct url
                var result = $"{ctx.Request.Scheme}://{ctx.Request.Host}/{sUrl.ShortUrl}";
                return Results.Ok(new UrlShortResponseDto()
                {
                    Url = result
                });
            });

            app.UseAuthorization();


            app.MapControllers();

            app.MapFallback(async (AppDbContext db, HttpContext ctx) =>
            {
                var path = ctx.Request.Path.ToUriComponent().Trim('/');
                var urlMatch = await db.Urls.FirstOrDefaultAsync(x => 
                x.ShortUrl.Trim() == path.Trim());

                if (urlMatch == null)
                    return Results.BadRequest("Invalid request");

                return Results.Redirect(urlMatch.Url);
            });

            app.Run();
        }
    }
}