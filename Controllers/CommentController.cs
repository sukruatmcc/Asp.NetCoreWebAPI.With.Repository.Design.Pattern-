using api.Dtos.Stock;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using api.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/comment")]
    [ApiController]

    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IStockRepository _stockRepository;
        private readonly UserManager<AppUser> _userManager;
        public CommentController(ICommentRepository commentRepository, IStockRepository stockRepository, UserManager<AppUser> userManager)
        {
           _commentRepository = commentRepository;
           _stockRepository = stockRepository;
           _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var comments = await _commentRepository.GetAllAsync();

            var commentDto = comments.Select(s => s.ToCommentDto());

            return Ok(commentDto);
        }
         
        [HttpGet("{id}")]  
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if(!ModelState.IsValid)
               return BadRequest(ModelState);
            var comment = await _commentRepository.GetByIdAsync(id);

            if(comment == null)
            {
                return NotFound();
            }
            return Ok(comment.ToCommentDto());
        }
         
        [HttpPost("{stockId:int}")] 
        [Authorize]
        public async Task<IActionResult> Create([FromRoute] int stockId, CreateCommentRequestDto commentDto)
        {
               if(!await _stockRepository.StockExists(stockId))
               {
                  return BadRequest("Stock does not exists");
               }

               var username = User.GetUserName();
               var appUser = await _userManager.FindByNameAsync(username);

               var commentModel = commentDto.ToCommentFromCreateDTO(stockId);
               commentModel.AppUserId = appUser.Id;
               await _commentRepository.CreateAsync(commentModel);

               return CreatedAtAction(nameof(GetById), new {id = commentModel.Id} ,commentModel.ToCommentDto());//commentModel.ToCommentDto() ile yeni oluşturulan yorumun DTO'su döndürülür.
        }
        
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentRequestDto updateDto)
        {
            if(!ModelState.IsValid)
               return BadRequest(ModelState);
            var comment = await _commentRepository.UpdateAsync(id, updateDto.ToCommentFromUpdateDTO());

            if(comment == null)
            {
                return NotFound();
            }

            return Ok(comment.ToCommentDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var commentModel = await _commentRepository.DeleteAsync(id);
            
            if(commentModel == null)
            {
                return NotFound();
            }
            return NoContent();
            //DTO'lar, veri transferini daha güvenli, performanslı ve kontrollü hale getirir. Veritabanı modellerinden bağımsız olarak, istemcilere sadece gerekli verileri sağlayarak veri sızıntılarını ve gereksiz veri transferlerini önler. 
        }
    }
}
