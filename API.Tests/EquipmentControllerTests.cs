using API.Controllers;
using API.DTOs;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace API.Tests
{
    public class EquipmentControllerTests
    {
        private readonly Mock<IEquipmentRepository> _mockService;
        private readonly EquipmentController _controller;

        public EquipmentControllerTests()
        {
            _mockService = new Mock<IEquipmentRepository>();
            _controller = new EquipmentController(_mockService.Object);
        }

        [Fact]
        public async Task GetEquipmentById_ExistingId_ReturnsOkWithEquipment()
        {
            // Arrange
            var equipmentDto = new EquipmentDto
            {
                Id = 1,
                EquipmentName = "Test Equipment",
                SerialNumber = "TEST-001",
                CategoryId = 1,
                CategoryName = "Computers",
                LocationId = 1,
                LocationName = "Main Office",
                PurchaseDate = DateTime.Now,
                Status = "Active"
            };

            _mockService
                .Setup(s => s.GetEquipmentByIdAsync(1))
                .ReturnsAsync(equipmentDto);

            // Act
            var result = await _controller.GetEquipmentById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedEquipment = Assert.IsType<EquipmentDto>(okResult.Value);
            Assert.Equal("Test Equipment", returnedEquipment.EquipmentName);
        }

        [Fact]
        public async Task GetEquipmentById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetEquipmentByIdAsync(999))
                .ReturnsAsync((EquipmentDto?)null);

            // Act
            var result = await _controller.GetEquipmentById(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.NotNull(notFoundResult.Value);
        }

        [Fact]
        public async Task CreateEquipment_ValidData_ReturnsCreated()
        {
            // Arrange
            var createDto = new CreateEquipmentDto
            {
                EquipmentName = "New Equipment",
                SerialNumber = "NEW-001",
                CategoryId = 1,
                LocationId = 1,
                PurchaseDate = DateTime.Now,
                Status = "Active"
            };

            var createdEquipment = new EquipmentDto
            {
                Id = 1,
                EquipmentName = "New Equipment",
                SerialNumber = "NEW-001",
                CategoryId = 1,
                CategoryName = "Computers",
                LocationId = 1,
                LocationName = "Main Office",
                PurchaseDate = DateTime.Now,
                Status = "Active"
            };

            _mockService
                .Setup(s => s.CreateEquipmentAsync(It.IsAny<CreateEquipmentDto>()))
                .ReturnsAsync(Result<EquipmentDto>.Success(createdEquipment));

            // Act
            var result = await _controller.CreateEquipment(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedEquipment = Assert.IsType<EquipmentDto>(createdResult.Value);
            Assert.Equal("New Equipment", returnedEquipment.EquipmentName);
        }

        [Fact]
        public async Task CreateEquipment_DuplicateSerialNumber_ReturnsBadRequest()
        {
            // Arrange
            var createDto = new CreateEquipmentDto
            {
                EquipmentName = "Duplicate Equipment",
                SerialNumber = "DUPLICATE-001",
                CategoryId = 1,
                LocationId = 1,
                PurchaseDate = DateTime.Now,
                Status = "Active"
            };

            _mockService
                .Setup(s => s.CreateEquipmentAsync(It.IsAny<CreateEquipmentDto>()))
                .ReturnsAsync(Result<EquipmentDto>.Failure("Equipment with serial number 'DUPLICATE-001' already exists."));

            // Act
            var result = await _controller.CreateEquipment(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteEquipment_ExistingId_ReturnsNoContent()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteEquipmentAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteEquipment(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteEquipment_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteEquipmentAsync(999))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteEquipment(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
        }

        [Fact]
        public async Task UpdateEquipment_ValidData_ReturnsOk()
        {
            // Arrange
            var updateDto = new UpdateEquipmentDto
            {
                EquipmentName = "Updated Equipment",
                SerialNumber = "UPD-001",
                CategoryId = 1,
                LocationId = 1,
                PurchaseDate = DateTime.Now,
                Status = "InMaintenance"
            };

            var updatedEquipment = new EquipmentDto
            {
                Id = 1,
                EquipmentName = "Updated Equipment",
                SerialNumber = "UPD-001",
                CategoryId = 1,
                CategoryName = "Computers",
                LocationId = 1,
                LocationName = "Main Office",
                PurchaseDate = DateTime.Now,
                Status = "InMaintenance"
            };

            _mockService
                .Setup(s => s.UpdateEquipmentAsync(1, It.IsAny<UpdateEquipmentDto>()))
                .ReturnsAsync(Result<EquipmentDto>.Success(updatedEquipment));

            // Act
            var result = await _controller.UpdateEquipment(1, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedEquipment = Assert.IsType<EquipmentDto>(okResult.Value);
            Assert.Equal("Updated Equipment", returnedEquipment.EquipmentName);
            Assert.Equal("InMaintenance", returnedEquipment.Status);
        }
    }
}