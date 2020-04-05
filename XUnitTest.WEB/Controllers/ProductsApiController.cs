using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XUnitTest.WEB.Data;
using XUnitTest.WEB.Models;
using XUnitTest.WEB.Repository;

namespace XUnitTest.WEB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsApiController : ControllerBase
    {
        private readonly IRepository<Product> _repository;

        public ProductsApiController(IRepository<Product> repository)
        {
            _repository = repository;
        }

        // GET: api/ProductsApi
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var entities = await _repository.GetEntities();
            if (entities != null)
                return Ok(entities);

            return NotFound();
        }

        // GET: api/ProductsApi/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _repository.GetEntity(id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // PUT: api/ProductsApi/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public IActionResult PutProduct([FromRoute]int id, [FromBody] Product product)
        {
            if (id != product.Id)
                return BadRequest();

            var result = _repository.Update(product);
            if (!result)
                return StatusCode(500);

            return Ok(product);
        }

        // POST: api/ProductsApi
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<IActionResult> PostProduct(Product product)
        {
            var result = await _repository.Create(product);
            if (!result)
                return StatusCode(500);

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/ProductsApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _repository.GetEntity(id);
            if (product == null)
                return NotFound();

            var result = _repository.Delete(product);
            if (!result)
                return StatusCode(500);

            return Ok();

        }
        private bool ProductExists(int id)
        {
            return _repository.GetEntity(id).Result != null;
        }

        [HttpPost("{a}/{b}")]
        public IActionResult Add(int a, int b)
        {
            return Ok(new Helper.Helper().Add(a, b));
        }
    }
}
