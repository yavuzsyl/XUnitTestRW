using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using XUnitTest.WEB.Controllers;
using XUnitTest.WEB.Models;
using XUnitTest.WEB.Repository;
using XUnitTestRW.TEST.MockModel;

namespace XUnitTestRW.TEST
{
    public class ProductApiControllerTest
    {
        private readonly ProductsApiController apiController;
        private readonly Mock<IRepository<Product>> mockRepo;
        private List<Product> products;

        public ProductApiControllerTest()
        {
            mockRepo = new Mock<IRepository<Product>>();
            apiController = new ProductsApiController(mockRepo.Object);
            products = MockModels.GetProducts();

        }

        #region Get Products
        [Fact]
        public async void Should_GetProducts_Returns_Ok_If_EntitiesExist()
        {
            mockRepo.Setup(x => x.GetEntities()).Returns(Task.FromResult<IEnumerable<Product>>(products));
            var result = await apiController.GetProducts();
            var viewRes = Assert.IsType<OkObjectResult>(result);
            //Assert.NotNull(viewRes.Value);
            //Assert.IsAssignableFrom<IEnumerable<Product>>(viewRes.Value);
        }
        [Fact]
        public async void Should_GetProducts_Returns_NotFound_If_EntitiesNotExist()
        {
            mockRepo.Setup(x => x.GetEntities()).Returns(Task.FromResult<IEnumerable<Product>>(null));
            var result = await apiController.GetProducts();
            Assert.IsType<NotFoundResult>(result);
        }
        #endregion
        #region Get Product
        [Theory]
        [InlineData(1)]
        public async void Should_GetProduct_Returns_Ok_If_IdExist(int id)
        {
            mockRepo.Setup(x => x.GetEntity(id)).Returns(Task.FromResult(products.FirstOrDefault(x => x.Id == id)));
            var result = await apiController.GetProduct(id);
            var viewRes = Assert.IsType<OkObjectResult>(result);
            //Assert.NotNull(viewRes.Value);
            //Assert.IsType<Product>(viewRes.Value);
        }
        [Theory]
        [InlineData(5)]
        public async void Should_GetProduct_Returns_NotFound_If_IdNotExist(int id)
        {
            mockRepo.Setup(x => x.GetEntity(id)).Returns(Task.FromResult<Product>(null));
            var result = await apiController.GetProduct(id);
            Assert.IsType<NotFoundResult>(result);

        }
        #endregion  
        #region Put Product
        [Theory]
        [InlineData(1, 2)]
        public void Should_PutProduct_Returns_BadRequest_If_RouteId_IsNotEqual_ObjectId(int id, int id2)
        {
            var result = apiController.PutProduct(id, products.FirstOrDefault(x => x.Id == id2));
            var viewRes = Assert.IsType<BadRequestResult>(result);
        }
        [Theory]
        [InlineData(4)]
        [InlineData(3)]//burada kaç adet inline varsa test o kadar çalışır
        public void Should_PutProduct_Returns_500_CouldntUpdate(int id)
        {
            var product = products.FirstOrDefault(x => x.Id == id);
            mockRepo.Setup(x => x.Update(product)).Returns(false);
            var result = apiController.PutProduct(id, product);
            var vieRest = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, vieRest.StatusCode);

        }
        [Theory]
        [InlineData(3)]
        public void Should_PutProduct_Returns_Ok_If_ObjectUpdated(int id)
        {
            var product = products.FirstOrDefault(x => x.Id == id);
            product.Name = "newName";
            mockRepo.Setup(x => x.Update(product)).Returns(true);
            var result = apiController.PutProduct(id, product);
            mockRepo.Verify(x => x.Update(product), times: Times.Once);
            var vieRest = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, vieRest.StatusCode);

        }
        #endregion   
        #region Post Product
        [Fact]
        public async void Should_PostProduct_Returns_Returns_500_CouldntCreate()
        {
            var product = new Product() { Id = 5, Name = "nw", Price = 25, Stock = 10 };
            mockRepo.Setup(x => x.Create(product)).Returns(Task.FromResult(false));
            var result = await apiController.PostProduct(product);
            var vieRest = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, vieRest.StatusCode);

        }
        [Fact]
        public async void Should_PostProduct_Returns_CreatedAction()
        {
            var product = new Product() { Id = 5, Name = "nw", Price = 25, Stock = 10 };
            mockRepo.Setup(x => x.Create(product)).Returns(Task.FromResult(true));
            var result = await apiController.PostProduct(product);
            var vieRest = Assert.IsType<CreatedAtActionResult>(result);
            mockRepo.Verify(x => x.Create(product), times: Times.Once);
            Assert.Equal("GetProduct", vieRest.ActionName);

        }

        #endregion

        #region Delete

        [Theory]
        [InlineData(5)]
        public async void Should_DeleteProduct_Returns_NotFound_If_IdNotExist(int id)
        {
            mockRepo.Setup(x => x.GetEntity(id)).Returns(Task.FromResult<Product>(null));
            var result = await apiController.DeleteProduct(id);
            Assert.IsType<NotFoundResult>(result);

        }
        [Theory]
        [InlineData(3)]
        public async void Should_DeleteProduct_Returns_500_IfCouldntDelete(int id)
        {
            var product = products.FirstOrDefault(x=> x.Id == id);
            mockRepo.Setup(x => x.GetEntity(id)).Returns(Task.FromResult<Product>(product));
            mockRepo.Setup(x => x.Delete(product)).Returns(false);
            var result = await apiController.DeleteProduct(id);
            mockRepo.Verify(x => x.Delete(product), times: Times.Once);
            var vieRest = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, vieRest.StatusCode);

        }  
        [Theory]
        [InlineData(4)]
        public async void Should_DeleteProduct_ReturnsOk(int id)
        {
            var product = products.FirstOrDefault(x=> x.Id == id);
            mockRepo.Setup(x => x.GetEntity(id)).Returns(Task.FromResult<Product>(product));
            mockRepo.Setup(x => x.Delete(product)).Returns(true);
            var result = await apiController.DeleteProduct(id);
            mockRepo.Verify(x => x.Delete(product), times: Times.Once);
            var vieRest = Assert.IsType<OkResult>(result);
            Assert.Equal(200, vieRest.StatusCode);

        }
        #endregion
    }
}
