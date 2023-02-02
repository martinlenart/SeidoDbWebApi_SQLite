using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using SeidoDbWebApi.Logger;

namespace DbAppWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogController : ControllerBase
    {
        private ILogger<LogController> _logger;

        //GET /Log
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(IEnumerable<LogMessage>))]
        public IEnumerable<LogMessage> Get([FromServices] ILoggerProvider myLogger)
        {
            //Should have recieved my custom logger through DI
            if (myLogger is InMemoryLoggerProvider cl)
            {
                return cl.Messages;
            }
            return null;
        }
        public LogController(ILogger<LogController> logger)
        {
            _logger = logger;
            _logger.LogInformation($"LogController started: {AppConfig.CurrentDbConnection}");
        }
    }
}
