using FGA_PoC_Login_Token.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FGA_PoC_Login_Token.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll([FromServices] IPermitAuthorizationService permit)
        {
            var userId = GetUserId();

            if (string.IsNullOrEmpty(userId))
            {
                return Forbid();
            }

            var allowed = await permit.IsAllowedAsync(
                userId, 
                "read", 
                "invoice"
            );

            if (!allowed)
            {
                return Forbid();
            }

            return Ok(new List<string> { "invoice_1", "invoice_2", "invoice_3", "invoice_4", "invoice_5" });
        }

        [Authorize]
        [HttpPost("approve")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Approve([FromServices] IPermitAuthorizationService permit)
        {
            var userId = GetUserId();

            if (string.IsNullOrEmpty(userId))
            {
                return Forbid();
            }

            var allowed = await permit.IsAllowedAsync(
                userId,
                "approve",
                "invoice"
            );

            if (!allowed)
            {
                return Forbid();
            }

            return NoContent();
        }

        private string? GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
