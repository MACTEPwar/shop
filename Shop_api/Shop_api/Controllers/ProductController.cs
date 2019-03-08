using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
            var groups = await _context.Groups.ToListAsync();
            return Ok(groups);
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

        //[HttpGet("Test")]
        //public async Task<IActionResult> Test()
        //{
        //    return Ok(await _context.Groups.Include(g => g.Products).ToListAsync());
        //}

        //
        [HttpGet("Test")]
        [Authorize]
        public IActionResult TestPost()
        {
            return Ok(new List<string> { "val 1", "val 2", "val 3" });
        }
    }
}