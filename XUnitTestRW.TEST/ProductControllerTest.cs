using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using XUnitTest.WEB.Controllers;
using XUnitTest.WEB.Models;
using XUnitTest.WEB.Repository;
using XUnitTestRW.TEST.MockModel;

namespace XUnitTestRW.TEST
{
    public class ProductControllerTest
    {
        //tst edilecek classýn dependaant olduðu servisler mocklanýr
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsController controller;
        private List<Product> products;
        public ProductControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            controller = new ProductsController(_mockRepo.Object);
            products = MockModels.GetProducts();
        }

        [Fact]
        public async void Should_IndexAction_Returns_View()
        {
            var result = await controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        /// <summary>
        /// index action daki irepository.GetAll mocklanacak
        /// </summary>
        [Fact]
        public async void Should_IndexAction_Returns_ProductList()
        {
            _mockRepo.Setup(x => x.GetEntities()).ReturnsAsync(products);

            var result = await controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);
            Assert.True(productList.Any());
        }

        [Fact]
        public async void Should_DetailsAction_ReturnsRedirecToIndexAction_If_Id_IsNull()
        {
            var result = await controller.Details(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(Index), redirect.ActionName);

        }

        [Theory]
        [InlineData(4)]
        public async void Should_DetailsAction_ReturnsNotFound_If_IdDoesNotExists(int productId)
        {
            _mockRepo.Setup(x => x.GetEntity(productId)).Returns(Task.FromResult<Product>(null));
            var result = await controller.Details(productId);
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, redirect.StatusCode);

        }

        [Theory]
        [InlineData(4)]
        public async void Should_DetailsAction_Returns_Product_If_IdExist(int productId)
        {
            _mockRepo.Setup(x => x.GetEntity(productId)).Returns(Task.FromResult(products.FirstOrDefault(x => x.Id == productId)));
            var result = await controller.Details(4);
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultProduct = Assert.IsType<Product>(viewResult.Model);

            Assert.Equal(productId, resultProduct.Id);

        }
    }
}
