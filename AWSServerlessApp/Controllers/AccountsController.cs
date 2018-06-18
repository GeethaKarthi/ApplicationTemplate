using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using AWSServerlessApp.CustomModels;
using AWSServerlessApp.DbContext;
using AWSServerlessApp.LogProvider;
using AWSServerlessApp.Models;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace AWSServerlessApp.Controllers
{
    [Route("api/[controller]")]
    // [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    public class AccountsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signinManager;
        private readonly JWTSettings _options;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public IConfiguration _iconfiguration;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController()
        {

        }

        public AccountsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signinManager,
                                   IOptions<JWTSettings> optionsAccessor, RoleManager<ApplicationRole> roleManager,
                                   IConfiguration iconfiguration, ILogger<AccountsController> logger)
        {
            _userManager = userManager;
            _signinManager = signinManager;
            _roleManager = roleManager;
            _options = optionsAccessor.Value;
            _iconfiguration = iconfiguration;
            _logger = logger;
        }


     
        ///<summary>
        /// Get all user
        ///</summary>
        ///<remarks>
        /// Get all user List - a sample api AWS Lambda method 
        ///</remarks>
        [HttpGet]
        public APIGatewayProxyResponse GetUsersSampleAPI()
        {
            AspNetUserPinsDbContext dbContext = new AspNetUserPinsDbContext(_iconfiguration, _logger);
            IEnumerable<AspNetUserPinModel> list = dbContext.GetAspNetUserPins();
            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(list),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };

            return response;
        }
        ///<summary>
        /// Get user pin details
        ///</summary>
        ///<remarks>
        /// Get list of user pin details
        ///</remarks>
        [HttpGet]
        [Route("getaspnetuserpins")]
        public async Task<IEnumerable<AspNetUserPinModel>> GetAspNetUserPins()
        {
            _logger.LogInformation((int)LoggingEvents.GET_ITEM, "Get the list of users in AspNetUserPins");
            AspNetUserPinsDbContext dbContext = new AspNetUserPinsDbContext(_iconfiguration, _logger);
            IEnumerable<AspNetUserPinModel> list = dbContext.GetAspNetUserPins();
            return list;

        }

       
    }
}
