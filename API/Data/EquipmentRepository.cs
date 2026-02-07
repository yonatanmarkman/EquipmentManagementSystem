using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class EquipmentRepository(AppDbContext context) : IEquipmentRepository
    {
        public async Task<PagedResultDto<EquipmentDto>> GetEquipmentItemsAsync(int pageNumber, int pageSize)
        {
            IQueryable<Equipment> query = context.EquipmentItems
                                                .Include(e => e.Category)
                                                .Include(e => e.Location)
                                                .AsQueryable();

            int totalCount = await query.CountAsync();

            List<EquipmentDto> items = await query
                .OrderBy(e => e.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EquipmentDto
                {
                    Id = e.Id,
                    EquipmentName = e.EquipmentName,
                    SerialNumber = e.SerialNumber,
                    CategoryId = e.CategoryId,
                    CategoryName = e.Category.CategoryName,
                    LocationId = e.LocationId,
                    LocationName = e.Location.LocationName,
                    PurchaseDate = e.PurchaseDate,
                    Status = e.Status.ToString()
                })
                .ToListAsync();

            return new PagedResultDto<EquipmentDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<EquipmentDto?> GetEquipmentByIdAsync(int id)
        {
            Equipment? equipment = await context.EquipmentItems
                                            .Include(e => e.Category)
                                            .Include(e => e.Location)
                                            .FirstOrDefaultAsync(e => e.Id == id);

            if (equipment == null)
                return null;

            return new EquipmentDto
            {
                Id = equipment.Id,
                EquipmentName = equipment.EquipmentName,
                SerialNumber = equipment.SerialNumber,
                CategoryId = equipment.CategoryId,
                CategoryName = equipment.Category.CategoryName,
                LocationId = equipment.LocationId,
                LocationName = equipment.Location.LocationName,
                PurchaseDate = equipment.PurchaseDate,
                Status = equipment.Status.ToString()
            };
        }

        public async Task<PagedResultDto<EquipmentDto>> SearchEquipmentAsync(EquipmentSearchDto searchDto)
        {
            IQueryable<Equipment> query = context.EquipmentItems
                                                .Include(e => e.Category)
                                                .Include(e => e.Location)
                                                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchDto.EquipmentName))
            {
                query = query.Where(e => e.EquipmentName.Contains(searchDto.EquipmentName));
            }

            if (searchDto.PurchaseDate.HasValue)
            {
                query = query.Where(e => e.PurchaseDate.Date == searchDto.PurchaseDate.Value.Date);
            }

            if (searchDto.CategoryId.HasValue)
            {
                query = query.Where(e => e.CategoryId == searchDto.CategoryId.Value);
            }

            int totalCount = await query.CountAsync();

            List<EquipmentDto> items = await query
                .OrderBy(e => e.Id)
                .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .Select(e => new EquipmentDto
                {
                    Id = e.Id,
                    EquipmentName = e.EquipmentName,
                    SerialNumber = e.SerialNumber,
                    CategoryId = e.CategoryId,
                    CategoryName = e.Category.CategoryName,
                    LocationId = e.LocationId,
                    LocationName = e.Location.LocationName,
                    PurchaseDate = e.PurchaseDate,
                    Status = e.Status.ToString()
                })
                .ToListAsync();

            return new PagedResultDto<EquipmentDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = searchDto.PageNumber,
                PageSize = searchDto.PageSize
            };
        }

        public async Task<Result<EquipmentDto>> CreateEquipmentAsync(CreateEquipmentDto createDto)
        {
            // Check if serial number already exists
            Equipment? existingEquipment = await context.EquipmentItems
                                            .FirstOrDefaultAsync(e => e.SerialNumber
                                                                    == createDto.SerialNumber);

            if (existingEquipment != null)
            {
                return Result<EquipmentDto>.Failure($"Equipment with serial number '{createDto.SerialNumber}' already exists.");
            }

            // Verify category exists
            bool categoryExists = await context.Categories.AnyAsync(c => c.Id == createDto.CategoryId);
            if (!categoryExists)
            {
                return Result<EquipmentDto>.Failure($"Category with ID {createDto.CategoryId} does not exist.");
            }

            // Verify location exists
            bool locationExists = await context.Locations.AnyAsync(l => l.Id == createDto.LocationId);
            if (!locationExists)
            {
                return Result<EquipmentDto>.Failure($"Location with ID {createDto.LocationId} does not exist.");
            }

            var equipment = new Equipment
            {
                EquipmentName = createDto.EquipmentName,
                SerialNumber = createDto.SerialNumber,
                CategoryId = createDto.CategoryId,
                LocationId = createDto.LocationId,
                PurchaseDate = createDto.PurchaseDate,
                Status = Enum.Parse<EquipmentStatus>(createDto.Status)  // Parse string to enum
            };

            context.EquipmentItems.Add(equipment);
            await context.SaveChangesAsync();

            EquipmentDto? createdEquipment = await GetEquipmentByIdAsync(equipment.Id);

            if (createdEquipment == null)
            {
                return Result<EquipmentDto>.Failure("Failed to retrieve created equipment.");
            }

            return Result<EquipmentDto>.Success(createdEquipment);
        }

        public async Task<Result<EquipmentDto>> UpdateEquipmentAsync(int id, UpdateEquipmentDto updateDto)
        {
            Equipment? equipment = await context.EquipmentItems.FindAsync(id);

            if (equipment == null)
                return Result<EquipmentDto>.Failure($"Equipment with ID {id} not found.");

            // Check if serial number is being changed and if it already exists
            if (equipment.SerialNumber != updateDto.SerialNumber)
            {
                Equipment? existingEquipment = await context.EquipmentItems
                    .FirstOrDefaultAsync(e => e.SerialNumber == updateDto.SerialNumber && e.Id != id);

                if (existingEquipment != null)
                {
                    return Result<EquipmentDto>.Failure($"Equipment with serial number '{updateDto.SerialNumber}' already exists.");
                }
            }

            // Verify category exists
            bool categoryExists = await context.Categories
                                            .AnyAsync(c => c.Id == updateDto.CategoryId);
            if (!categoryExists)
            {
                 return Result<EquipmentDto>.Failure($"Category with ID {updateDto.CategoryId} does not exist.");
            }

            // Verify location exists
            bool locationExists = await context.Locations
                                            .AnyAsync(l => l.Id == updateDto.LocationId);
            if (!locationExists)
            {
                return Result<EquipmentDto>.Failure($"Location with ID {updateDto.LocationId} does not exist.");
            }

            equipment.EquipmentName = updateDto.EquipmentName;
            equipment.SerialNumber = updateDto.SerialNumber;
            equipment.CategoryId = updateDto.CategoryId;
            equipment.LocationId = updateDto.LocationId;
            equipment.PurchaseDate = updateDto.PurchaseDate;
            equipment.Status = Enum.Parse<EquipmentStatus>(updateDto.Status);  // Parse string to enum

            await context.SaveChangesAsync();

            EquipmentDto? updatedEquipment = await GetEquipmentByIdAsync(id);
            
            if (updatedEquipment == null)
            {
                return Result<EquipmentDto>.Failure("Failed to retrieve updated equipment.");
            }

            return Result<EquipmentDto>.Success(updatedEquipment);
        }

        public async Task<bool> DeleteEquipmentAsync(int id)
        {
            Equipment? equipment = await context.EquipmentItems.FindAsync(id);

            if (equipment == null)
                return false;

            context.EquipmentItems.Remove(equipment);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<PagedResultDto<EquipmentDto>> GetEquipmentByCategoryAsync(int categoryId, int pageNumber, int pageSize)
        {
            IQueryable<Equipment> query = context.EquipmentItems
                                            .Include(e => e.Category)
                                            .Include(e => e.Location)
                                            .Where(e => e.CategoryId == categoryId);

            int totalCount = await query.CountAsync();

            List<EquipmentDto> items = await query
                .OrderBy(e => e.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EquipmentDto
                {
                    Id = e.Id,
                    EquipmentName = e.EquipmentName,
                    SerialNumber = e.SerialNumber,
                    CategoryId = e.CategoryId,
                    CategoryName = e.Category.CategoryName,
                    LocationId = e.LocationId,
                    LocationName = e.Location.LocationName,
                    PurchaseDate = e.PurchaseDate,
                    Status = e.Status.ToString()
                })
                .ToListAsync();

            return new PagedResultDto<EquipmentDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}