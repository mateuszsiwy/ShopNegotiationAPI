using Moq;
using Xunit;
using ShopNegotiationAPI.Application.Exceptions;
using ShopNegotiationAPI.Application.Interfaces.Repositories;
using ShopNegotiationAPI.Application.Services;
using ShopNegotiationAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopNegotiationAPI.Test.Application.Services
{
    public class NegotiationServiceTests
    {
        private readonly Mock<INegotiationRepository> _mockRepository;
        private readonly NegotiationService _service;

        public NegotiationServiceTests()
        {
            _mockRepository = new Mock<INegotiationRepository>();
            _service = new NegotiationService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetNegotiationByIdAsync_ExistingNegotiation_ReturnsNegotiation()
        {
            // Arrange
            var expectedNegotiation = new Negotiation { Id = 1 };
            _mockRepository.Setup(r => r.GetNegotiationByIdAsync(1))
                .ReturnsAsync(expectedNegotiation);

            // Act
            var result = await _service.GetNegotiationByIdAsync(1);

            // Assert
            Assert.Equal(expectedNegotiation, result);
            _mockRepository.Verify(r => r.GetNegotiationByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetNegotiationByIdAsync_NonExistingNegotiation_ThrowsException()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetNegotiationByIdAsync(999))
                .ThrowsAsync(new NegotiationNotFoundException(999));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<NegotiationNotFoundException>(
                async () => await _service.GetNegotiationByIdAsync(999));
            Assert.Contains("999", ex.Message);
        }

        [Fact]
        public async Task FinalizeNegotiationAsync_WhenAccepted_SetsStatusToAccepted()
        {
            // Arrange
            var negotiation = new Negotiation
            {
                Id = 1,
                Status = NegotiationStatus.Pending
            };

            _mockRepository.Setup(r => r.GetNegotiationByIdAsync(1))
                .ReturnsAsync(negotiation);

            _mockRepository.Setup(r => r.UpdateNegotiationAsync(It.IsAny<Negotiation>()))
                .ReturnsAsync((Negotiation n) => n);

            // Act
            var result = await _service.FinalizeNegotiationAsync(1, true);

            // Assert
            Assert.Equal(NegotiationStatus.Accepted, result.Status);
            _mockRepository.Verify(r => r.UpdateNegotiationAsync(It.IsAny<Negotiation>()), Times.Once);
        }

        [Fact]
        public async Task FinalizeNegotiationAsync_WhenRejected_SetsStatusToRejected()
        {
            // Arrange
            var negotiation = new Negotiation
            {
                Id = 1,
                Status = NegotiationStatus.Pending
            };

            _mockRepository.Setup(r => r.GetNegotiationByIdAsync(1))
                .ReturnsAsync(negotiation);

            _mockRepository.Setup(r => r.UpdateNegotiationAsync(It.IsAny<Negotiation>()))
                .ReturnsAsync((Negotiation n) => n);

            // Act
            var result = await _service.FinalizeNegotiationAsync(1, false);

            // Assert
            Assert.Equal(NegotiationStatus.Rejected, result.Status);
            Assert.True(result.ExpirationDate > DateTime.UtcNow);
        }

        [Fact]
        public async Task FinalizeNegotiationAsync_NonPendingNegotiation_ThrowsInvalidOperationException()
        {
            // Arrange
            var negotiation = new Negotiation
            {
                Id = 1,
                Status = NegotiationStatus.Accepted
            };

            _mockRepository.Setup(r => r.GetNegotiationByIdAsync(1))
                .ReturnsAsync(negotiation);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _service.FinalizeNegotiationAsync(1, true));
            Assert.Contains("pending", ex.Message);
        }

        [Fact]
        public async Task ProcessNegotiationAsync_ExcessiveAttempts_ClosesNegotiation()
        {
            // Arrange
            var negotiation = new Negotiation
            {
                Id = 1,
                AttemptsCount = 4,
                Status = NegotiationStatus.Pending
            };

            _mockRepository.Setup(r => r.GetNegotiationByIdAsync(1))
                .ReturnsAsync(negotiation);

            // Act
            var result = await _service.ProcessNegotiationAsync(1, 100m);

            // Assert
            Assert.Equal(NegotiationStatus.Closed, result.Status);
        }

        [Fact]
        public async Task GetPendingNegotiationsAsync_ReturnsOnlyPendingNegotiations()
        {
            // Arrange
            var negotiations = new List<Negotiation>
            {
                new Negotiation { Id = 1, Status = NegotiationStatus.Pending },
                new Negotiation { Id = 2, Status = NegotiationStatus.Accepted },
                new Negotiation { Id = 3, Status = NegotiationStatus.Pending },
                new Negotiation { Id = 4, Status = NegotiationStatus.Rejected }
            };

            _mockRepository.Setup(r => r.GetAllNegotiationsAsync())
                .ReturnsAsync(negotiations);

            // Act
            var result = await _service.GetPendingNegotiationsAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.True(result.All(n => n.Status == NegotiationStatus.Pending));
        }
    }
}