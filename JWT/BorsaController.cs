using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

[ApiController]
[Route("api/[controller]")]
public class BorsaController : ControllerBase
{
    [Authorize] // Sadece token sahipleri görebilir
    [HttpGet("gizli-veriler")]
    public IActionResult GetSecretData()
    {
        return Ok("Bu veriyi sadece giriş yapmış kullanıcılar görebilir!");
    }
}