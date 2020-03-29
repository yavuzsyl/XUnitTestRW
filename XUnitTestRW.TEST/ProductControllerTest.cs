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
            //MockBehavior seçeneklerinden mockun geçtiði yerlerde kullanýmýnýn zorunlu olup olmayacaðýna karar verilir
            _mockRepo = new Mock<IRepository<Product>>(MockBehavior.Default);
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

        [Fact]
        public async void Should_GetCreateAction_Returns_ViewResult()
        {
            var result = await controller.Create();
            Assert.IsType<ViewResult>(result);

        }

        [Fact]
        public async void Should_PostCreateAction_ReturnsCreateView_IF_ModelStateNotValid()
        {
            controller.ModelState.AddModelError("Name", "Name is required");
            var result = await controller.Create(products.FirstOrDefault());
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Product>(viewResult.Model);

        }    
        [Fact]
        public async void Should_PostCreateAction_ReturnsRedirectToIndex_IF_ModelStateValid()
        {
            //bu mock olmasada olur
            _mockRepo.Setup(x => x.Create(products.FirstOrDefault()));
            var result = await controller.Create(products.FirstOrDefault());
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(Index), viewResult.ActionName);
        }    
        [Fact]
        public async void Should_PostCreateAction_CreateMethodExecutes_IF_ModelStateValid()
        {
            //yeni eklenecek ürün
            Product product = null;
            _mockRepo.Setup(repo => repo.Create(It.IsAny<Product>())).Callback<Product>(p => product = p);
            //callback ile belirtilen method simule edildiði zaman alacaðý paramatre product deðiþkenimize eþitlenecek

            //product deðiþkenine listedeki ilke obje eþitlenecek
            var result = await controller.Create(products.FirstOrDefault());

            _mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Once);
            //method 1 kere  çalýþtýðýný verify ediyoruz

            Assert.Equal(products.FirstOrDefault().Id, product.Id);
        } 
        [Fact]//so unnecessary shit
        public async void Should_PostCreateAction_CreateMethodDoesNotExecute_IF_ModelStateUnValid()
        {
            controller.ModelState.AddModelError("Name", "");
            var result = await controller.Create();
            _mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()), times: Times.Never);
        }    
        
        [Theory]
        [InlineData(null)]
        public async void Should_GetEditAction_Returns_RedirectToIndex_IF_idIsNull(int? id)
        {
            var result = await controller.Edit(id);
            //Assert.IsType<RedirectToActionResult>(result); gibi
            Assert.Equal(nameof(Index), ((RedirectToActionResult)result).ActionName);
        }
        [Theory]
        [InlineData(25)]
        public async void Should_GetEditAction_Returns_NotFound_IF_idIsNotExist(int? id)
        {
            _mockRepo.Setup(repo => repo.GetEntity(id.Value)).Returns(Task.FromResult<Product>(null));
            var result = await controller.Edit(id);
            Assert.IsType<NotFoundResult>(result);
            //Assert.Equal(404,((NotFoundResult)result).StatusCode);
        }
        [Theory]
        [InlineData(2)]
        public async void Should_GetEditAction_Returns_ViewWithProduct_IF_idIsExist(int? id)
        {
            _mockRepo.Setup(repo => repo.GetEntity(id.Value)).Returns(Task.FromResult<Product>(products.FirstOrDefault(x=> x.Id == id.Value)));
            var result = await controller.Edit(id);
            var viewResult = Assert.IsType<ViewResult>(result);
            var product =  Assert.IsAssignableFrom<Product>(viewResult.Model);
            Assert.Equal(id.Value, product.Id);
            //tek assert te kullanýlabilir
        }
    }
}
