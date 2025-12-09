using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Egypt_EInvoice_Api.Models;
using Egypt_EInvoice_Api.Repos;

namespace Egypt_EInvoice_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepos<User> authRepos;
        public AuthController(IAuthRepos<User> authRepos)
        {
            this.authRepos = authRepos;
        }

        [HttpGet]
        [Route("Login")]
        public User Login(string userName, string password)
        {
            return this.authRepos.Login(userName, password);
        }
    }
}
