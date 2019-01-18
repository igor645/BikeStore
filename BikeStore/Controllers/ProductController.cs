﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BikeStore.core.Domain;
using BikeStore.core.Repositories;
using BikeStore.Infrastructure.Commands;
using BikeStore.ViewModels;
using BikeStore.ViewsModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BikeStore.Controllers {
  public class ProductController : BikeStoreControllerBaseController {

    private readonly IProductsRepository mProductsRepository;
   
    public int mPageSize = 3;
    public ProductController(ICommandDispatcher xCommandDispatcher, IProductsRepository xProductsRepository) 
      : base(xCommandDispatcher) {
      mProductsRepository = xProductsRepository;
    }

    public async Task<IActionResult> ListProduct(string category, int productpage = 1) 
      => View(new ProductsListVM {
        Products = mProductsRepository.Product
                    .Where(p => category == null || p.Category == category)
                    .OrderBy(p => p.ProductID)
                    .Skip((productpage - 1) * mPageSize)
                    .Take(mPageSize),
        PagingInfo = new PagingInfo {
          CurrentPage = productpage,
          ItemsPerPage = mPageSize,
          TotalItems = category == null ?
                        mProductsRepository.Product.Count() :
                        mProductsRepository.Product.Where(e =>
                            e.Category == category).Count()
        },
        CurrentCategory = category
      });
    [HttpGet]
    public IActionResult AddProduct()
      => View();
    [HttpPost]
    public async Task<IActionResult> AddProduct(Product xProduct) {

      await mProductsRepository.AddProductAsync(xProduct);

     return RedirectToAction("ListProduct");

    }

    [HttpGet]
    public async Task<IActionResult> EditProduct(int xId) {

      Product pProduct = await mProductsRepository.Product.SingleOrDefaultAsync(x => x.ProductID == xId);

      return View(pProduct);

    }

    [HttpPost]
    public async Task<IActionResult>EditProduct(Product xProduct) {

      await mProductsRepository.EditProductAsync(xProduct);

      return RedirectToAction("ListProduct");
    }

    [HttpGet]
    public async Task<IActionResult>DeleteProduct(int xId)    {

      Product pProduct = await mProductsRepository.Product.SingleOrDefaultAsync(x => x.ProductID == xId);
       await mProductsRepository.DeleteProductAsync(pProduct);

      return RedirectToAction("ListProduct");

    }

  }
}