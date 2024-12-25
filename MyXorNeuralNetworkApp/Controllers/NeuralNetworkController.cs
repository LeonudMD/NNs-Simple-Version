using Microsoft.AspNetCore.Mvc;
using MyXorNeuralNetworkApp.Services;

namespace MyXorNeuralNetworkApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NeuralNetworkController : ControllerBase
    {
        private readonly NeuralNetworkService _service;

        public NeuralNetworkController(NeuralNetworkService service)
        {
            _service = service;
        }
        
        [HttpGet("Initialize")]
        public IActionResult Initialize(int inputSize = 8, int hiddenNeurons = 3, int outputNeurons = 1, double learningRate = 0.1)
        {
            _service.InitializeNetwork(inputSize, hiddenNeurons, outputNeurons, learningRate);
            return Ok("Сеть инициализирована.");
        }
        
        [HttpPost("Train")]
        public IActionResult Train([FromBody] TrainRequest request)
        {
            if (request == null)
                return BadRequest("Неверный запрос.");

            var result = _service.TrainNetwork(request.Epochs, request.Threshold);
            return Ok(result);
        }
        
        [HttpGet("Test")]
        public IActionResult Test()
        {
            var results = _service.TestNetwork();
            if (results == null)
                return BadRequest("Сеть не инициализирована.");

            return Ok(results);
        }
        
        [HttpGet("Parameters")]
        public IActionResult Parameters()
        {
            var parameters = _service.GetNetworkParameters();
            if (parameters == null)
                return BadRequest("Сеть не инициализирована.");

            return Ok(parameters);
        }
    }
    
    public class TrainRequest
    {
        public int Epochs { get; set; }
        public double Threshold { get; set; }
    }
}
