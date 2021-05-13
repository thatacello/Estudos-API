using System.Net;
using System.Runtime.ExceptionServices;
using System;
using System.Linq;
using Estudos_API.Data;
using Estudos_API.Models;
using Microsoft.AspNetCore.Mvc;
using Estudos_API.Hateoas;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Estudos_API.Controllers
{
    [Route("api/v1/[controller]")] // v1 -> versao 1 da minha API 
    // posso criar outras versões e a v1 continuar existindo
    // versão legada - versão sem suporte
    [ApiController]
    [Authorize(Roles = "Admin")] // só permite entrar em produtos quem estiver autorizado
    public class ProdutosController : ControllerBase // controller voltado para API, não possui html ou views
    {
        private readonly ApplicationDbContext database;
        private Hateoas.Hateoas Hateoas;
        public ProdutosController(ApplicationDbContext database)
        {
            this.database = database;
            Hateoas = new Hateoas.Hateoas("localhost:5001/api/v1/produtos");
            Hateoas.AddAction("GET_INFO", "GET");
            Hateoas.AddAction("DELETE_PRODUCT", "DELETE");
            Hateoas.AddAction("EDIT_PRODUCT", "PATCH");
        }
        [HttpGet("teste")]
        public IActionResult TesteClaims()
        {
            // StringComparison.InvariantCultureIgnoreCase -> compara duas string ignorando letras maiusculas, minusculas, etc
            return Ok(HttpContext.User.Claims.First(claim => claim.Type.ToString().Equals("id", StringComparison.InvariantCultureIgnoreCase)).Value);
        }
        [HttpGet]
        public IActionResult Get()
        {
            // return Ok("Tudo ok!"); // status code = 200

            // retorna um Json
            var produtos = database.Produtos.ToList();
            List<ProdutoContainer> produtosHateoas = new List<ProdutoContainer>();
            foreach(var prod in produtos)
            {
                ProdutoContainer produtoHateoas = new ProdutoContainer();
                produtoHateoas.produto = prod;
                produtoHateoas.links = Hateoas.GetActions(prod.Id.ToString());
                produtosHateoas.Add(produtoHateoas);
            }
            return Ok(produtos);
            // return Ok(new { nome = "Thais Cardoso", signo = "capricórnio"});
        }
        [HttpGet("{id}")]
        public IActionResult Get(int id) // se eu digitar um numero na rota, automaticamente vem para esse método
        {
            try
            {
                Produto produto = database.Produtos.First(p => p.Id == id);
                ProdutoContainer produtoHateoas = new ProdutoContainer();
                produtoHateoas.produto = produto;
                produtoHateoas.links = Hateoas.GetActions(produto.Id.ToString());
                return Ok(produtoHateoas);
            }
            catch(Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult(new { msg = "Id inválido!" });

                // return BadRequest(new { msg = "Id inválido: " + e });
            }
        }
        [HttpPost]
        public IActionResult Post([FromBody] ProdutoTemp pTemp){

            // validação
            if(pTemp.Preco <= 0)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "O preço do produto não pode ser negativo"});
            }

            if(pTemp.Nome.Length <= 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "O nome precisa de mais caracteres"});
            }

            Produto p = new Produto();
            p.Nome = pTemp.Nome;
            p.Preco = pTemp.Preco;

            database.Produtos.Add(p);
            database.SaveChanges();

            // retorna um código de estado
            Response.StatusCode = 201;
            return new ObjectResult(new {msg = "Produto criado com sucesso!"}); // é melhor não mandar mensagem nenhuma , pois o status 201 já estará sendo enviado

            // return Ok(new {info = "Você criou um novo produto", produto = p}); -> StatusCode = 200;
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Produto produto = database.Produtos.First(p => p.Id == id);
                database.Produtos.Remove(produto);
                database.SaveChanges();
                return Ok("Produto deletado");
            }
            catch(Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult(new { msg = "Id inválido!" });

                // return BadRequest(new { msg = "Id inválido: " + e });
            }
        }
        // patch -> permite editar parcialmente
        [HttpPatch]
        public IActionResult Patch([FromBody] Produto produto)
        {
            if(produto.Id < 0)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "Id inválido!" });
            }
            else
            {
                try
                {
                    var p = database.Produtos.First(pTemp => pTemp.Id == produto.Id);

                    if( p != null)
                    {   
                        p.Nome = produto.Nome != null ? produto.Nome : p.Nome;
                        p.Preco = produto.Preco != 0 ? produto.Preco : p.Preco;

                        database.SaveChanges();

                        return Ok();
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult(new { msg = "Produto não encontrado!" });
                    }
                }
                catch
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new { msg = "Produto não encontrado!" });
                }
            }
        }
        // SWAGGER -> gera uma página html com a documentação da API
        public class ProdutoTemp{
            public string Nome { get; set; }
            public float Preco { get; set; }
        }
        public class ProdutoContainer
        {
            public Produto produto;
            public Link[] links;
        }
    }
}