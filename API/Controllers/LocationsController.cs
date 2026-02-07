using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationsController(AppDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Location>>> GetAllLocations()
        {
            try
            {
                var locations = await context.Locations
                                        .OrderBy(l => l.LocationName)
                                        .ToListAsync();

                return Ok(locations);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while retrieving locations.", details = ex.Message });
            }
        }
    }
}
