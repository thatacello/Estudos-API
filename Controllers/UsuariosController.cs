using System.Collections.Generic;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Estudos_API.Data;
using Estudos_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Estudos_API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _database; 
        public UsuariosController(ApplicationDbContext database)
        {
            _database = database;
        }

        [HttpPost("registro")]
        public IActionResult Registro([FromBody] Usuario usuario)
        {
            _database.Add(usuario);
            _database.SaveChanges();
            return Ok(new {msg = "Usuário cadastrado com sucesso"});
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] Usuario credenciais)
        {
            try
            {
                // verifica se o usuário está logado
                Usuario usuario = _database.Usuarios.First(user => user.Email.Equals(credenciais.Email));
                if(usuario != null)
                {
                    if(usuario.Senha.Equals(credenciais.Senha))
                    {
                        // a senha está correta
                        // gera o Token JWT
                        // chave de segurança
                        string chaveDeSeguranca = "a_thais_eh_uma_guerreira_sayajin!";
                        var chaveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveDeSeguranca));
                        var credenciaisAcesso = new SigningCredentials(chaveSimetrica, SecurityAlgorithms.HmacSha256Signature);

                        // claims
                        var claims = new List<Claim>();
                        claims.Add(new Claim("id", usuario.Id.ToString()));
                        claims.Add(new Claim("email", usuario.Email)); // ATENÇÃO -> NUNCA COLOQUE INFORMAÇÕES SENSÍVEIS NO TOKEN
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                        
                        var Jwt = new JwtSecurityToken(
                            issuer: "thaismarket.com", // quem fornece o jwt para o usuário
                            expires: DateTime.Now.AddHours(1),
                            audience: "usuario_comum",
                            signingCredentials: credenciaisAcesso,
                            claims : claims
                        );
                        return Ok(new JwtSecurityTokenHandler().WriteToken(Jwt));
                    }
                    else
                    {
                        Response.StatusCode = 401; // não autorizado
                        return new ObjectResult("");
                    }
                }
                else
                {
                    Response.StatusCode = 401; // não autorizado
                    return new ObjectResult("");
                }
            }
            catch(Exception)
            {
                Response.StatusCode = 401; // não existe nenhum usuário com esse e-mail
                return new ObjectResult("");
            }
            
        }
    }
}