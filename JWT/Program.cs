using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// JWT Doğrulama Ayarları
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true, // Süresi dolmuşsa reddet
            ValidateIssuerSigningKey = true,
            ValidIssuer = "borsa.com",
            ValidAudience = "borsa-istemcileri",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Bu_Cok_Gizli_Ve_Uzun_Bir_Anahtar_Olmali_123!"))
        };
    });

builder.Services.AddAuthorization();
var app = builder.Build();

app.UseAuthentication(); // 👈 Kimsin?
app.UseAuthorization();  // 👈 Yetkin var mı?