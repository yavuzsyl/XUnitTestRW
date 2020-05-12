using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        #region Index
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
        #endregion
        #region Details
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
            //arrange
            _mockRepo.Setup(x => x.GetEntity(productId)).Returns(Task.FromResult<Product>(null));
            //act
            var result = await controller.Details(productId);
           //assert
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
        #endregion
        #region Create
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

            //product deðiþkenine listedeki ilk obje eþitlenecek
            var result = await controller.Create(products.FirstOrDefault());

            _mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Once);
            //method 1 kere  çalýþtýðýný verify ediyoruz

            Assert.Equal(products.FirstOrDefault().Id, product.Id);
        }
        [Fact]//so unnecessary
        public async void Should_PostCreateAction_CreateMethodDoesNotExecute_IF_ModelStateInValid()
        {
            controller.ModelState.AddModelError("Name", "");

            var result = await controller.Create();
            
            _mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()), times: Times.Never);
        }
        #endregion
        #region Edit
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
            _mockRepo.Setup(repo => repo.GetEntity(id.Value)).Returns(Task.FromResult<Product>(products.FirstOrDefault(x => x.Id == id.Value)));

            var result = await controller.Edit(id);
            
            var viewResult = Assert.IsType<ViewResult>(result);
            var product = Assert.IsAssignableFrom<Product>(viewResult.Model);
            Assert.Equal(id.Value, product.Id);
            //tek assert te kullanýlabilir
        }
        [Theory]
        [InlineData(2)]
        public void Should_PostEditAction_Returns_NotFound_IF_QueryId_IsNotEqual_ModelId(int id)
        {
            var result = controller.Edit(4, products.FirstOrDefault(x => x.Id == id));
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(2)]
        public void Should_PostEditAction_ReturnsEditView_IF_ModelStateInValid(int id)
        {
            controller.ModelState.AddModelError("Name", "");
            var result = controller.Edit(id, products.FirstOrDefault(x => x.Id == id));
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Product>(viewResult.Model);
        }

        [Theory]
        [InlineData(2)]
        public void Should_PostEditAction_ReturnsRedirectToIndexAction_IF_ModelStateValid(int id)
        {
            var result = controller.Edit(id, products.FirstOrDefault(x => x.Id == id));
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(Index), viewResult.ActionName);
        }

        [Theory]
        [InlineData(2)]
        public void Should_PostEditAction_ThrowsExceptionOnRepoUpdateMethod(int id)
        {
            _mockRepo.Setup(repo => repo.Update(It.IsAny<Product>())).Throws<DbUpdateConcurrencyException>();
            Assert.Throws<DbUpdateConcurrencyException>(() => controller.Edit(id, products.FirstOrDefault(x => x.Id == id)));
        }

        /// <summary>
        /// dnknow man
        /// </summary>
        /// <param name="id"></param>
        [Theory]
        [InlineData(2)]
        public void Should_PostEditAction_RepoUpdateExcutes_IF_ModelIsValid(int id)
        {
            var product = products.FirstOrDefault(x => x.Id == id);
            _mockRepo.Setup(repo => repo.Update(product));

            controller.Edit(id, product);
            
            _mockRepo.Verify(repo => repo.Update(product), Times.Once);

        }
        //edit metodu exception çözülünce devam edilecek --up > done
        #endregion

        #region Delete
        [Fact]
        public async void Should_DeleteAction_ReturnsNotFound_If_Id_IsNull()
        {
            var result = await controller.Delete(null);
            Assert.IsType<NotFoundResult>(result);

        }

        [Theory]
        [InlineData(4)]
        public async void Should_DeleteAction_ReturnsNotFound_If_IdDoesNotExists(int productId)
        {
            _mockRepo.Setup(x => x.GetEntity(productId)).Returns(Task.FromResult<Product>(null));

            var result = await controller.Delete(productId);
            Assert.IsType<NotFoundResult>(result);

        }

        [Theory]
        [InlineData(4)]
        public async void Should_DeleteAction_Returns_Product_If_IdExist(int productId)
        {
            _mockRepo.Setup(x => x.GetEntity(productId)).Returns(Task.FromResult(products.FirstOrDefault(x => x.Id == productId)));

            var result = await controller.Delete(4);
            
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Product>(viewResult.Model);


        }
        #endregion
        #region DeleteConfirmed
        [Theory]
        [InlineData(4)]
        public async void Should_DeleteConfirmAction_Returns_RedirectToIndex(int productId)
        {
            _mockRepo.Setup(x => x.GetEntity(productId)).Returns(Task.FromResult(products.FirstOrDefault(x => x.Id == productId)));
            _mockRepo.Setup(x => x.Delete(products.FirstOrDefault(x => x.Id == productId)));
            
            var result = await controller.DeleteConfirmed(productId);
            
            Assert.IsType<RedirectToActionResult>(result);
        }
        #endregion


        #region ProductExists
        [Theory]
        [InlineData(4)]
        public  void Should_ProductExists_Return(int productId)
        {
            _mockRepo.Setup(x => x.GetEntity(productId)).Returns(Task.FromResult(products.FirstOrDefault(x => x.Id == productId)));
            var result =  controller.ProductExists(productId);
            Assert.True(result);
        }
        #endregion

    }
}
