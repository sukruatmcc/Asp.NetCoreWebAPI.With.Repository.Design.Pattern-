using api.Dtos.Comment;
using api.Dtos.Stock;
using api.Models;

namespace api.Mappers
{
    public static class CommentMappers
    {
        public static CommentDto ToCommentDto(this Comment commentModel)
        {
           if(commentModel == null)
           {
                throw new ArgumentNullException(nameof(commentModel), "Comment model cannot be null");
           }

            return new CommentDto
            {
                Id = commentModel.Id,
                Title = commentModel.Title,
                Content = commentModel.Content,
                CreatedOn = commentModel.CreatedOn,
                CreatedBy = commentModel.AppUser.UserName,
                StockId = commentModel.StockId
            };
        }

        public static Comment ToCommentFromCreateDTO(this CreateCommentRequestDto commentDto, int stockId)
        {
            return new Comment
            {
                 Title = commentDto.Title,
                 Content = commentDto.Content,
                 StockId = stockId
            };
        }

        public static Comment ToCommentFromUpdateDTO(this UpdateCommentRequestDto commentDto)
        {
            return new Comment
            {
                 Title = commentDto.Title,
                 Content = commentDto.Content,
            };
        }
    }
}
