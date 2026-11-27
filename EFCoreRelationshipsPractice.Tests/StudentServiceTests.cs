using EFCoreRelationshipsPractice.Models;
using EFCoreRelationshipsPractice.Repositories;
using EFCoreRelationshipsPractice.Services;
using Moq;

namespace EFCoreRelationshipsPractice.Tests;

public class StudentServiceTests
{

    [Fact]
    public async Task GetStudentAsync_StudentDoesNotExist_ReturnsNull()
    {
        // Arrange
        var mockRepository = new Mock<IStudentRepository>();

        mockRepository
            .Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((Student?)null);

        var service = new StudentService(mockRepository.Object);

        // Act
        var result = await service.GetStudentAsync(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetStudentAsync_StudentExists_ReturnsStudent()
    {
        // Arrange
        var expectedStudent = new Student
        {
            Id = 1,
            Name = "Thanuja"
        };

        var mockRepository = new Mock<IStudentRepository>();

        mockRepository
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(expectedStudent);

        var service = new StudentService(mockRepository.Object);

        // Act
        var result = await service.GetStudentAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Thanuja", result.Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task GetStudentAsync_InvalidId_ThrowsArgumentException(int id)
    {
        // Arrange
        var mockRepository = new Mock<IStudentRepository>();
        var service = new StudentService(mockRepository.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.GetStudentAsync(id));

        // Verify
        mockRepository.Verify(
            r => r.GetByIdAsync(It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task GetStudentAsync_ValidId_CallsRepositoryOnce()
    {
        // Arrange
        var student = new Student
        {
            Id = 5,
            Name = "Arun"
        };

        var mockRepository = new Mock<IStudentRepository>();

        mockRepository
            .Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(student);

        var service = new StudentService(mockRepository.Object);

        // Act
        await service.GetStudentAsync(5);

        // Assert
        mockRepository.Verify(
            r => r.GetByIdAsync(5),
            Times.Once);
    }

}