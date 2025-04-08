using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication1.Controllers;
using WebApplication1.Models;
using WebApplication1.Service;

namespace WebApplication.UnitTests.Controllers
{
    public class ProductsControllerTest
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly ProductsController _controller;

        public ProductsControllerTest()
        {
            _mockProductService = new Mock<IProductService>();
            _controller = new ProductsController(_mockProductService.Object);
        }

        [Fact]
        public async Task GetProductById_ShouldReturnOk_WhenProductExists()
        { 
            var productId = Guid.NewGuid();
            var product = new Product
            {
                Id = productId,
                Name = "Test Product"
            };

            _mockProductService.Setup(service => service.GetProductByIdAsync(productId)).ReturnsAsync(product);

            var result = await _controller.GetProduct(productId);

            Assert.NotNull(result);
            var actionResult = Assert.IsType<ActionResult<Product>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.IsAssignableFrom<Product>(okResult.Value);
            var returnedProduct = Assert.IsType<Product>(okResult.Value);
            _mockProductService.Verify(service => service.GetProductByIdAsync(productId), Times.Once);
        }

        [Fact]
        public async Task GetProductById_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            var productId = Guid.NewGuid();
            _mockProductService.Setup(service => service.GetProductByIdAsync(productId)).ReturnsAsync((Product?)null);

            var result = await _controller.GetProduct(productId);

            Assert.NotNull(result);
            var actionResult = Assert.IsType<ActionResult<Product>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
            _mockProductService.Verify(service => service.GetProductByIdAsync(productId), Times.Once);
        }

        [Fact]
        public async Task GetProducts_ShouldReturnOk_WhenProductsExist()
        {
            var products = new List<Product>
            {
                new() { Id = Guid.NewGuid(), Name = "Product 1" },
                new() { Id = Guid.NewGuid(), Name = "Product 2" }
            };

            _mockProductService.Setup(service => service.GetAllProductsAsync()).ReturnsAsync(products);

            var result = await _controller.GetProducts();

            var actionResult = Assert.IsType<ActionResult<IEnumerable<Product>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(2, returnedProducts.Count());
            _mockProductService.Verify(service => service.GetAllProductsAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnNoContent_WhenProductIsDeleted()
        {
            var productId = Guid.NewGuid();

            _mockProductService.Setup(service => service.DeleteProductAsync(productId)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteProduct(productId);

            var actionResult = Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<NoContentResult>(actionResult);
            _mockProductService.Verify(service => service.DeleteProductAsync(productId), Times.Once);
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnNoContent_WhenProductIsUpdated()
        {
            var productId = Guid.NewGuid();
            var product = new Product
            {
                Id = productId,
                Name = "Updated Product"
            };

            _mockProductService.Setup(service => service.UpdateProductAsync(product))
                .Returns(Task.CompletedTask);

            var result = await _controller.UpdateProduct(productId, product);

            var actionResult = Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<NoContentResult>(actionResult);
            _mockProductService.Verify(service => service.UpdateProductAsync(product), Times.Once);
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnOk_WhenProductIsCreated()
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "New Product"
            };

            _mockProductService.Setup(service => service.CreateProductAsync(product)).ReturnsAsync(product);

            var result = await _controller.CreateProduct(product);

            var actionResult = Assert.IsType<ActionResult<Product>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedProduct = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(product.Name, returnedProduct.Name);
            _mockProductService.Verify(service => service.CreateProductAsync(product), Times.Once);
        }
    }
}
