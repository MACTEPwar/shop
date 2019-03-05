using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_api.Data;
using Shop_api.Models;

namespace Shop_api.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        dataContext _context;
        public ProductController(dataContext ctx)
        {
            _context = ctx;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var res = _context.Products.ToList();
            return Ok(res);
        }

        [HttpGet("Groups")]
        public async Task<IActionResult> GetGroups()
        {
            //var res = from item in _context.Groups.Include(x => x.Products).ToList()
            //          select new
            //          {
            //              item.Title,item.Discription,
            //              products = from el in item.Products
            //                         select new {
            //                             el.Code,
            //                             el.Title,
            //                             el.Count,
            //                             el.Measure,
            //                             el.Price
            //                         }
            //          };
            //return Ok(res);

            //var res = await _context.Groups.FirstAsync();
            //_context.Entry(res).Collection(g => g.Products).Load();

            //var res = _context.Products.Include(x => x.Group).ToList();

            //var res = from item in _context.Groups.ToList()
            //          select new {
            //              item.Id,item.Title,item.Discription,
            //              Products = _context.Products.Where(p => p.GroupId == item.Id)
            //          };

            var res = _context.Groups.ForEachAsync(x => x.Products = _context.Products.Where(p => p.GroupId == x.Id).ToList());

            return Ok(res);
            //return Ok();
        }

        [HttpGet("ProductsByGroup/{id}")]
        public async Task<IActionResult> GetProductsByGroup(int id)
        {
            var products = await _context.Products.Where(p => p.GroupId == id).ToListAsync();
            return Ok(products);
        }

        //[HttpGet("Groups/CreateDB")]
        //public async Task<IActionResult> CreateDB()
        //{
        //    _context.Products.Add(new Product() { Code = "11111111", Count=10, GroupId = 1, Measure = MeasureList.Метр, Title = "Простыня" , Price = 10.25});
        //    _context.SaveChanges();
        //    return Ok();
        //}

        [HttpGet("Test")]
        public async Task<IActionResult> Test()
        {
            return Ok(await _context.Groups.Include(g => g.Products).ToListAsync());
        }
    }
}