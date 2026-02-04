using FGA_PoC_Login_Token.Services;
using FGA_PoC_Login_Token.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Permit (Authorization)
// --------------------
builder.Services.AddSingleton<IPermitAuthorizationService, PermitAuthorizationService>();

// --------------------
// Controllers & Swagger
// --------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --------------------
// Authentication (Okta - JWT)
// --------------------
// TODO ISAAC
var issuer = "ISSUER_URL";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = issuer;
        options.Audience = "api://default";
        options.RequireHttpsMetadata = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// ----------------------------------------------------
// Inline Hook endpoint (intentionally disabled for PoC)
// Requires public URL + deployment
// ----------------------------------------------------

// app.MapPost("/authorization-claims", async (HttpContext context, IPermitAuthorizationService permit) =>
// {
//     var body = await context.Request.ReadFromJsonAsync<dynamic>();

//     string userId = body?.data?.context?.user?.profile?.email
//                     ?? body?.data?.context?.user?.id;

//     if (string.IsNullOrEmpty(userId))
//         return Results.BadRequest(new { error = "User not found" });

//     bool allowed = await permit.CheckAsync(userId, "read", "asset");

//     return Results.Ok(new
//     {
//         entitlements = allowed
//             ? new[] { "asset:read" }
//             : Array.Empty<string>()
//     });
// });

app.UseAuthentication();
app.UseAuthorization();

// --------------------
// HTTP pipeline
// --------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
