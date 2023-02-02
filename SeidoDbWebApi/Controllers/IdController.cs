using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

using DbModelsLib;
using DbCRUDReposLib;
using SeidoDbWebApi.Logger;

namespace DbAppWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IdController : ControllerBase
    {
        private ICustomerRepository _repo;
        private ILogger<LogController> _logger;

        //GET /Log
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<DbInfo> Get([FromServices] ILoggerProvider myLogger)
        {
            var info = await _repo.ReadDbInfoAsync();
            info.dBConnection = AppConfig.CurrentDbConnection;
            return info;
        }
        public IdController(ICustomerRepository repo, ILogger<LogController> logger)
        {
            _repo = repo;
            _logger = logger;
            _logger.LogInformation($"IdController started: {AppConfig.CurrentDbConnection}");
        }
    }
}
