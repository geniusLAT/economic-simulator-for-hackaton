using economic_simulator_for_hackaton.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace economic_simulator_for_hackaton.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _service;

        public MessageController(IMessageService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult Post([FromBody] string text)
        {
            var resp = _service.GetResponse(text);
            return Ok(new { response = resp });
        }
    }

}
