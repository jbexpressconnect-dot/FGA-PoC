using FGA_PoC_Login_Token.Common;
using FGA_PoC_Login_Token.Middlewares;
using FGA_PoC_Login_Token.Services;
using FGA_PoC_Login_Token.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Permit (Authorization)
// --------------------
builder.Services.AddSingleton<IPermitAuthorizationService, PermitAuthorizationService>();

// --------------------
// Controllers & Swagger
// --------------------
builder.Services.AddControllers(options => {

    options.Filters.Add<PermitAuthorizeFilter>();
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<PermitAuthorizeFilter>();

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


//Swagger JWT
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

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
app.UsePermitAuthorization();
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
