using API.Data;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add the DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            
            // Add services to the container
            builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp",
                    policy => policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod());
            });

            WebApplication app = builder.Build();

            // Seed data on startup
            using (IServiceScope scope = app.Services.CreateScope())
            {
                IServiceProvider services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();
                    var seeder = new DataSeeder(context);

                    await seeder.SeedAllAsync();

                    Console.WriteLine("Data seeding completed successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while seeding data: {ex.Message}");
                }
            }

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            // Use CORS
            app.UseCors("AllowAngularApp");

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}