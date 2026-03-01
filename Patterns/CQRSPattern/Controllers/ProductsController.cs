using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRSPattern.Commands;
using CQRSPattern.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CQRSPattern.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;   
        public ProductsController(IMediator _mediator)
        {
            this._mediator = _mediator;
        }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return CreatedAtAction(nameof(GetAll), null);
    }
  [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? kategori = null)
    {
        var result = await _mediator.Send(new GetAllProductsQuery(kategori));
        return Ok(result);
    }
    }
}