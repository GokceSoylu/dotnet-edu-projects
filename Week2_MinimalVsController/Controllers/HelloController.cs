using Microsoft.AspNetCore.Mvc;

namespace Week2_MinimalVsController.Controllers;

[ApiController]
[Route("api/[controller]")] // DİKKAT: Burayı "api/[controller]" yaparsan sınıf adındaki 'Controller' kısmını atıp rotayı otomatik 'api/hello' yapar.
public class HelloController : ControllerBase
{
    [HttpGet("hello")] // Eğer yukarıda api/controller yazdıysan burası api/controller/hello olur.
    public IActionResult GetHello()
    {
        return Ok(new { Message = "Hello from Controller-Based API!" });
    }
}