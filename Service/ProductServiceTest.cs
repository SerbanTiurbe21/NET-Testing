

using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq.Expressions;
using WebApplication1.Data;
using WebApplication1.Exceptions;
using WebApplication1.Models;
using WebApplication1.Service;

namespace WebApplication.UnitTests.Service
{
    public class ProductServiceTest
    {
        //private readonly Mock<AppDbContext> _mockContext;
        //private readonly ProductService _productService;
        //private readonly Mock<DbSet<Product>> _mockProductDbSet;

        //public ProductServiceTest()
        //{
        //    _mockContext = new Mock<AppDbContext>();
        //    _mockProductDbSet = new Mock<DbSet<Product>>();
        //    _mockContext.Setup(c => c.Products).Returns(_mockProductDbSet.Object);

        //    _productService = new ProductService(_mockContext.Object);
        //}

        //[Fact]
        //public async Task CreateProductAsync_ProductExists_ThrowsDuplicateProductException()
        //{
        //    var product = new Product { Id = Guid.NewGuid(), Name = "New Product" };
        //    _mockProductDbSet.Setup(m => m.AnyAsync(It.IsAny<Expression<Func<Product, bool>>>(), default)).ReturnsAsync(false);


        //    // Act & Assert
        //    await Assert.ThrowsAsync<DuplicateProductException>(() => _productService.CreateProductAsync(product));

        //    //Assert.NotNull(result);
        //    //Assert.Equal(product.Name, result.Name);
        //    _mockProductDbSet.Verify(m => m.Add(It.IsAny<Product>()), Times.Once);
        //    _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        //}
        private readonly Mock<AppDbContext> _mockContext;
        private readonly ProductService _productService;

        public ProductServiceTest()
        {
            _mockContext = new Mock<AppDbContext>();
            _productService = new ProductService(_mockContext.Object);
        }

        [Fact]
        public async Task CreateProductAsync_ProductExists_ThrowsDuplicateProductException()
        {
            // Arrange
            var product = new Product { Id =Guid.NewGuid(), Name = "Test Product" };

            // Mock the DbContext to simulate that the product already exists
            _mockContext.Setup(c => c.Products.AnyAsync(It.IsAny<Expression<Func<Product, bool>>>(), default))
                        .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<DuplicateProductException>(() => _productService.CreateProductAsync(product));
            Assert.Equal("Product already exists", exception.Message);
        }
    }
}
