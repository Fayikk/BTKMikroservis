using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogSidecar.Services;
using Microsoft.AspNetCore.Mvc;

namespace LogSidecar.Controllers
{
    [ApiController]
    [Route("logs")]
    public class LogsController : ControllerBase
    {
         private readonly LogReader _reader;
    public LogsController(LogReader reader) => _reader = reader;

    [HttpGet]
    public IActionResult GetAll()
        => Ok(_reader.ReadAll());
    }
}