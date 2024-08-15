using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockRepository _stockRepository;
        public StockController(IStockRepository stockRepository)
        {
             _stockRepository = stockRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject queryObject)//[From Query] = form query input açıldı.
        {
            var stocks = await _stockRepository.GetAllAsync(queryObject);  

            var stockDto = stocks.Select(s => s.ToStockDto()).ToList();

            return Ok(stockDto);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
             var stock = await _stockRepository.GetByIdAsync(id);
             
             if(stock == null)
             {
                return NotFound();
             }

             return Ok(stock.ToStockDto());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto stockDto)
        {
            var stockModel = stockDto.ToStockFromCreateDTO();
            await _stockRepository.CreateAsync(stockModel);

            return CreatedAtAction(nameof(GetById), new {id = stockModel.Id}, stockModel.ToStockDto());
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto stockDto)
        {
            var stockModel = await _stockRepository.UpdateAsync(id,stockDto);
            if(stockModel == null)
            {
                return NotFound();
            }

            return Ok(stockModel);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var stockModel = await _stockRepository.AsyncDelete(id);
            if(stockModel == null)
            {
                return NotFound();
            }

            return NoContent();//NoContent genellikle bir işlemin başarılı olduğunu fakat yanıt olarak veri iletmeye gerek olmadığını belirlemek için kullanılır. Bu, istemci tarafında işlem sonucunu anlamak için yeterli bilgi sağlar.
        }
    }
}
