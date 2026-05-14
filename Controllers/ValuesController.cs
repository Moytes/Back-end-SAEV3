using Microsoft.AspNetCore.Mvc;

namespace Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    // GET: api/<ValuesController>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new string[] { "value1", "value2" });
    }

    // GET api/<ValuesController>/5
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        return Ok("value");
    }
}
