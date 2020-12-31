using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Services;

namespace Shop.Controllers
{
    [Route("v1/users")]
    public class UserController : ControllerBase
    {

        [HttpGet]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get(
            [FromServices] DataContext context
        )
        {
            var users = await context
                .Users
                .AsNoTracking()
                .ToListAsync();

            return Ok(users);
        }

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        // [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Post(
            [FromServices] DataContext context,
            [FromBody] User model)
        {
            //Verifica se os dados são válidos
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Força o usuário a ser sempre "funcionário"
                model.Role = "employee";

                context.Users.Add(model);
                await context.SaveChangesAsync();

                // Esconde a senha
                model.Password = "";

                return model;
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível criar o usuário" });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Put(
            [FromServices] DataContext context,
            int id,
            [FromBody] User model
        )
        {
            // Verifica se os dados são válidos
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verifica se o ID informado é o mesmo do modelo
            if (id != model.Id)
                return NotFound(new { message = "Usuário não encontrado" });

            try
            {
                context.Entry(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return model;
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível criar o usuário" });
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate(
            [FromServices] DataContext context,
            [FromBody] User model
        )
        {
            // Única maneira que eu encontrei de forçar case sensitive na query, foi usando Collate com essa string
            // https://docs.microsoft.com/en-us/ef/core/miscellaneous/collations-and-case-sensitivity
            var user = await context.Users
                .AsNoTracking()
                .Where(
                    x =>
                    EF.Functions.Collate(x.Username, "SQL_Latin1_General_CP1_CS_AS") == model.Username
                    &&
                    EF.Functions.Collate(x.Password, "SQL_Latin1_General_CP1_CS_AS") == model.Password
                )
                .FirstOrDefaultAsync();
            if (user == null)
                return NotFound(new { message = "Usuário ou senha inválidos" });

            var token = TokenService.GenerateToken(user);

            // Esconde a senha
            user.Password = "";
            return new
            {
                user = user,
                token = token
            };
        }
    }
}