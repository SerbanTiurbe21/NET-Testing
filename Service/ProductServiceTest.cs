
using Moq;
using WebApplication1.Exceptions;
using WebApplication1.Models;
using WebApplication1.Repository;
using WebApplication1.Service;

namespace WebApplication.UnitTests.Service
{
    public class ProductServiceTest
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly ProductService _productService;

        public ProductServiceTest()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _productService = new ProductService(_mockProductRepository.Object);
        }

        [Fact]
        public async Task CreateProductAsync_ShouldThrowDuplicateProductException_WhenProductAlreadyExists()
        {
            _mockProductRepository.Setup(repo => repo.ProductExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

            var exception = await Assert.ThrowsAsync<DuplicateProductException>(() => _productService.CreateProductAsync(new Product { Name = "Test Product" }));
            Assert.Equal("Product already exists", exception.Message);
        }

        [Fact]
        public async Task CreateProductAsync_ShouldCreateProduct_WhenProductDoesNotExist() 
        {
            _mockProductRepository.Setup(repo => repo.ProductExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _mockProductRepository.Setup(repo => repo.CreateProductAsync(It.IsAny<Product>())).ReturnsAsync(new Product { Id = Guid.NewGuid(), Name = "Test Product" });

            var result = await _productService.CreateProductAsync(new Product { Name = "Test Product" });

            Assert.NotNull(result);
            Assert.Equal("Test Product", result.Name);
            _mockProductRepository.Verify(repo => repo.CreateProductAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldThrowException_WhenProductNotFound() 
        {
            _mockProductRepository.Setup(repo => repo.GetProductByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Product?)null);

            var exception = await Assert.ThrowsAsync<Exception>(() => _productService.GetProductByIdAsync(Guid.NewGuid()));
            Assert.Equal("Product not found", exception.Message);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists() 
        {
            var productId = Guid.NewGuid();
            
            _mockProductRepository.Setup(repo => repo.GetProductByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Product { Id = productId, Name = "Test Product" });

            var result = await _productService.GetProductByIdAsync(productId);

            Assert.NotNull(result);
            Assert.Equal(productId, result.Id);
            Assert.Equal("Test Product", result.Name);
            _mockProductRepository.Verify(repo => repo.GetProductByIdAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldThrowException_WhenProductNotFound() 
        {
            _mockProductRepository.Setup(repo => repo.GetProductByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Product?)null);

            var exception = await Assert.ThrowsAsync<Exception>(() => _productService.UpdateProductAsync(new Product { Id = Guid.NewGuid(), Name = "Updated Product" }));
            Assert.Equal("Product not found", exception.Message);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldUpdateProduct_WhenProductExists() 
        {
            var productId = Guid.NewGuid();
            
            _mockProductRepository.Setup(repo => repo.GetProductByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Product { Id = productId, Name = "Test Product" });
            _mockProductRepository.Setup(repo => repo.UpdateProductAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

            await _productService.UpdateProductAsync(new Product { Id = productId, Name = "Updated Product" });

            _mockProductRepository.Verify(repo => repo.UpdateProductAsync(It.IsAny<Product>()), Times.Once);
        }
    }
}
