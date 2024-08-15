using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDBContext _context;
        public CommentRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        
        public async Task<List<Comment>> GetAllAsync()
        {
            return await _context.Comments.Include(a => a.AppUser).ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            var commentModel = await _context.Comments.Include(a => a.AppUser).FirstOrDefaultAsync(c => c.Id == id);

            return commentModel;
        }

        public async Task<Comment> CreateAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            return comment;
        }

        public async Task<Comment?> UpdateAsync(int id, Comment comment)
        {
            var commentModel = await _context.Comments.FindAsync(id);

            if(commentModel == null)
            {
                return null;
            }
            commentModel.Title = comment.Title;
            commentModel.Content = comment.Content;
            await _context.SaveChangesAsync();

            return comment;
        }

        public async Task<Comment?> DeleteAsync(int id)
        {
            var existingCommentModel = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
            
            if(existingCommentModel == null)
            {
                return null;
            }

            _context.Comments.Remove(existingCommentModel);

            await _context.SaveChangesAsync();

            return existingCommentModel;
        }
    }
}
