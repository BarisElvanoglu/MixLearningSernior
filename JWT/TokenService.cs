using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenService
{
    public string CreateToken(string username)
    {
        // 1. Payload: Token içinde taşınacak veriler (Claims)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim("CustomData", "BorsaUygulamasi")
        };

        // 2. Key: Token'ı imzalamak için gizli bir anahtar
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Bu_Cok_Gizli_Ve_Uzun_Bir_Anahtar_Olmali_123!"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 3. Token Ayarları
        var tokenOptions = new JwtSecurityToken(
            issuer: "borsa.com",              // Kim yayınladı
            audience: "borsa-istemcileri",    // Kimler kullanabilir
            claims: claims,                   // Veriler
            expires: DateTime.Now.AddMinutes(15), // Ne kadar geçerli (Örn: 15 dk)
            signingCredentials: creds         // İmza
        );

        // 4. Token'ı string formatına çevir
        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }
}