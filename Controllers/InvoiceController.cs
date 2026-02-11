using FGA_PoC_Login_Token.Common;
using FGA_PoC_Login_Token.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FGA_PoC_Login_Token.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class InvoiceController : ControllerBase
{

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [PermitAuthorize("read", "invoice")]
    public async Task<IActionResult> GetAll([FromServices] IPermitAuthorizationService permit)
    {
        return Ok(new List<string> { "invoice_1", "invoice_2", "invoice_3", "invoice_4", "invoice_5" });
    }


    [HttpPost("approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [PermitAuthorize("approve", "invoice")]
    public async Task<IActionResult> Approve([FromServices] IPermitAuthorizationService permit)
    {
        return Ok("Approved successfully");
    }
}
