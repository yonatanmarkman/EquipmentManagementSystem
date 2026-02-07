using System.Text.Json;
using API.Entities;
using API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataSeeder
    {
        private readonly AppDbContext _context;
        private readonly string _seedDataPath;

        public DataSeeder(AppDbContext context)
        {
            _context = context;
            _seedDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData");
        }

        public async Task SeedAllAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            // Seed in order
            await SeedCategoriesAsync();
            await SeedLocationsAsync();
            await SeedEquipmentAsync();
        }

        private async Task SeedCategoriesAsync()
        {
            if (await _context.Categories.AnyAsync())
            {
                Console.WriteLine("Categories already seeded.");
                return;
            }

            string jsonPath = Path.Combine(_seedDataPath, "categories.json");
            if (!File.Exists(jsonPath))
            {
                Console.WriteLine($"Seed file not found: {jsonPath}");
                return;
            }

            string json = await File.ReadAllTextAsync(jsonPath);
            var categories = JsonSerializer.Deserialize<List<Category>>(json);

            if (categories != null && categories.Count != 0)
            {
                await _context.Categories.AddRangeAsync(categories);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Seeded {categories.Count} categories.");
            }
        }

        private async Task SeedLocationsAsync()
        {
            if (await _context.Locations.AnyAsync())
            {
                Console.WriteLine("Locations already seeded.");
                return;
            }

            string jsonPath = Path.Combine(_seedDataPath, "locations.json");
            if (!File.Exists(jsonPath))
            {
                Console.WriteLine($"Seed file not found: {jsonPath}");
                return;
            }

            var json = await File.ReadAllTextAsync(jsonPath);
            var locations = JsonSerializer.Deserialize<List<Location>>(json);

            if (locations != null && locations.Count != 0)
            {
                await _context.Locations.AddRangeAsync(locations);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Seeded {locations.Count} locations.");
            }
        }

        private async Task SeedEquipmentAsync()
        {
            try
            {
                if (await _context.EquipmentItems.AnyAsync())
                {
                    Console.WriteLine("Equipment already seeded.");
                    return;
                }

                string jsonPath = Path.Combine(_seedDataPath, "equipment.json");
                if (!File.Exists(jsonPath))
                {
                    Console.WriteLine($"Seed file not found: {jsonPath}");
                    return;
                }

                var json = await File.ReadAllTextAsync(jsonPath);
                var equipmentDtos = JsonSerializer.Deserialize<List<EquipmentSeedDto>>(json);

                if (equipmentDtos == null || equipmentDtos.Count == 0)
                {
                    return;
                }

                // Load all categories and locations into memory
                List<Category> categories = await _context.Categories.ToListAsync();
                List<Location> locations = await _context.Locations.ToListAsync();

                var equipmentItems = new List<Equipment>();

                foreach (var dto in equipmentDtos)
                {
                    Category? category = categories.FirstOrDefault(c => c.CategoryName == dto.CategoryName);
                    Location? location = locations.FirstOrDefault(l => l.LocationName == dto.LocationName);

                    if (category == null)
                    {
                        Console.WriteLine($"Warning: Category '{dto.CategoryName}' not found for equipment '{dto.EquipmentName}'");
                        continue;
                    }

                    if (location == null)
                    {
                        Console.WriteLine($"Warning: Location '{dto.LocationName}' not found for equipment '{dto.EquipmentName}'");
                        continue;
                    }

                    equipmentItems.Add(new Equipment
                    {
                        EquipmentName = dto.EquipmentName,
                        SerialNumber = dto.SerialNumber,
                        CategoryId = category.Id,
                        LocationId = location.Id,
                        PurchaseDate = dto.PurchaseDate,
                        Status = Enum.Parse<EquipmentStatus>(dto.Status)
                    });
                }

                if (equipmentItems.Count != 0)
                {
                    await _context.EquipmentItems.AddRangeAsync(equipmentItems);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Seeded {equipmentItems.Count} equipment items.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred. Ex: " + ex.Message);
            }
        }
    }
}