using System;
using System.Collections.Generic;
using System.Text;
using XUnitTest.WEB.Models;

namespace XUnitTestRW.TEST.MockModel
{
    public static class MockModels
    {
        public static List<Product> GetProducts()
        {
            return new List<Product>()
            {
                new Product(){Id = 1,Name = "Smart watch", Price = 350, Stock = 45},
                new Product(){Id = 2,Name = "Smart Tv", Price = 1500, Stock = 45},
                new Product(){Id = 3,Name = "Smart Phone", Price = 1100, Stock = 45},
                new Product(){Id = 4,Name = "Laptop", Price = 2550, Stock = 45},
            };
        }
    }
}
