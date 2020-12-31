using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("v1")]
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Get([FromServices] DataContext context)
        {
            var employee = new User { Id = 1, Username = "Stanley", Role = "employee", Password = "employee427" };
            var manager = new User { Id = 2, Username = "Narrator", Role = "manager", Password = "StanleyEnteredTheDoorOnHisLeft" };
            var category = new Category { Id = 1, Title = "Broom Closet" };
            var product = new Product { Id = 1, Category = category, Title = "Best Ending", Price = 1, Description = "OH, DID U GET THE BROOM CLOSET ENDING? THE BROOM CLOSET ENDING WAS MY FAVRITE!1 XD" };

            context.Users.Add(employee);
            context.Users.Add(manager);
            context.Categories.Add(category);
            context.Products.Add(product);

            await context.SaveChangesAsync();

            return Ok(
                new
                {
                    message = "Dados configurados"
                }
            );
        }
    }
}