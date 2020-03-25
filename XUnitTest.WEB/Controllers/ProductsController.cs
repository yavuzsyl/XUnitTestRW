﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using XUnitTest.WEB.Data;
using XUnitTest.WEB.Models;
using XUnitTest.WEB.Repository;

namespace XUnitTest.WEB.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IRepository<Product> repository;

        public ProductsController(IRepository<Product> repository)
        {
            this.repository = repository;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await repository.GetEntities());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //1
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }
            //2
            var product = await repository.GetEntity(id.Value);
            if (product == null)
            {
                return NotFound();
            }
            //3
            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Stock")] Product product)
        {
            if (ModelState.IsValid)
            {
                await repository.Create(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await repository.GetEntity(id.Value);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Price,Stock")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    repository.Update(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await repository.GetEntity(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await repository.GetEntity(id);
            repository.Delete(product);
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return repository.GetEntity(id).Result != null;
        }
    }
}
