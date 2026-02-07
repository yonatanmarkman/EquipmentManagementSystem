using API.DTOs;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EquipmentController(IEquipmentRepository equipmentService): ControllerBase
    {
        // GET: api/Equipment?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<EquipmentDto>>> GetAllEquipment(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (pageNumber < 1)
                    return BadRequest("Page number must be greater than 0.");

                if (pageSize < 1 || pageSize > 100)
                    return BadRequest("Page size must be between 1 and 100.");

                PagedResultDto<EquipmentDto> result
                     = await equipmentService.GetEquipmentItemsAsync(pageNumber, pageSize);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while retrieving equipment.", details = ex.Message });
            }
        }

        // GET: api/Equipment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EquipmentDto>> GetEquipmentById(int id)
        {
            try
            {
                EquipmentDto? equipment = await equipmentService.GetEquipmentByIdAsync(id);

                if (equipment == null)
                    return NotFound(new { message = $"Equipment with ID {id} not found." });

                return Ok(equipment);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while retrieving equipment.", details = ex.Message });
            }
        }

        // POST: api/Equipment/search
        [HttpPost("search")]
        public async Task<ActionResult<PagedResultDto<EquipmentDto>>> SearchEquipment([FromBody] EquipmentSearchDto searchDto)
        {
            try
            {
                if (searchDto.PageNumber < 1)
                    return BadRequest("Page number must be greater than 0.");

                if (searchDto.PageSize < 1 || searchDto.PageSize > 100)
                    return BadRequest("Page size must be between 1 and 100.");

                PagedResultDto<EquipmentDto> result
                     = await equipmentService.SearchEquipmentAsync(searchDto);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while searching equipment.", details = ex.Message });
            }
        }

        // POST: api/Equipment
        [HttpPost]
        public async Task<ActionResult<EquipmentDto>> CreateEquipment([FromBody] CreateEquipmentDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result
                     = await equipmentService.CreateEquipmentAsync(createDto);

                if (!result.IsSuccess)
                    return BadRequest(new { message = result.ErrorMessage });

                return CreatedAtAction(
                    nameof(GetEquipmentById),
                    new { 
                        id = result.Data!.Id 
                    },
                    result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while creating equipment.", details = ex.Message });
            }
        }

        // PUT: api/Equipment/5
        [HttpPut("{id}")]
        public async Task<ActionResult<EquipmentDto>> UpdateEquipment(int id, [FromBody] UpdateEquipmentDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await equipmentService.UpdateEquipmentAsync(id, updateDto);

                if (!result.IsSuccess)
                {
                    // Check if it's a "not found" vs validation error
                    if (result.ErrorMessage.Contains("not found"))
                        return NotFound(new { message = result.ErrorMessage });
                    
                    return BadRequest(new { message = result.ErrorMessage });
                }

                return Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while updating equipment.", details = ex.Message });
            }
        }

        // DELETE: api/Equipment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipment(int id)
        {
            try
            {
                bool result = await equipmentService.DeleteEquipmentAsync(id);

                if (!result)
                    return NotFound(new { message = $"Equipment with ID {id} not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while deleting equipment.", details = ex.Message });
            }
        }

        // GET: api/Equipment/category/5?pageNumber=1&pageSize=10
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<PagedResultDto<EquipmentDto>>> GetEquipmentByCategory(
            int categoryId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (pageNumber < 1)
                    return BadRequest("Page number must be greater than 0.");

                if (pageSize < 1 || pageSize > 100)
                    return BadRequest("Page size must be between 1 and 100.");

                var result = await equipmentService.GetEquipmentByCategoryAsync(categoryId, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while retrieving equipment by category.", details = ex.Message });
            }
        }
    }
}
