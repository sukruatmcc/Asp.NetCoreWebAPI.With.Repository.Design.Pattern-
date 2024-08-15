using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDBContext _context;

        public StockRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<Stock>> GetAllAsync(QueryObject queryObject)
        {
            var stocks = _context.Stocks.Include(c => c.Comments).ThenInclude(a => a.AppUser).AsQueryable();

            if(!string.IsNullOrWhiteSpace(queryObject.Symbol))
            {
                stocks = stocks.Where(s => s.Symbol.Contains(queryObject.Symbol));
            }
 
            if(!string.IsNullOrWhiteSpace(queryObject.CompanyName))
            {
                stocks = stocks.Where(s => s.CompanyName.Contains(queryObject.CompanyName));
            }

            if(!string.IsNullOrWhiteSpace(queryObject.SortBy))
            {
                if(queryObject.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
                //queryObject.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase) ifadesi, queryObject.SortBy özelliğinin "Symbol" değerine eşit olup olmadığını kontrol eder.
                //StringComparison.OrdinalIgnoreCase parametresi, karşılaştırmanın büyük/küçük harf duyarlı olmadan yapılmasını sağlar. Yani "symbol", "SYMBOL", "Symbol" gibi yazımların hepsi eşit kabul edilir.
                {
                    stocks = queryObject.IsDescending ? stocks.OrderByDescending(s => s.Symbol) : stocks.OrderBy(s => s.Symbol);//symbol harf sıralamasına göre sıralama işlemi
                }
            }

            var skipNumber = (queryObject.PageNumber - 1) * queryObject.PageSize;//kaç öğenin atlanması gerektiğini hesaplar //(queryObject.PageNumber - 1): Bu, hangi sayfanın olduğunu belirtir (örneğin, 2. sayfa için 1).
            
          return await stocks.Skip(skipNumber).Take(queryObject.PageSize).ToListAsync();
        }

        public async Task<Stock> CreateAsync(Stock stockModel)
        {
            await _context.Stocks.AddAsync(stockModel);
            await _context.SaveChangesAsync();
            return stockModel; 
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
             var stockModel = await _context.Stocks.FirstOrDefaultAsync(s => s.Id == id);
             
             if(stockModel == null)
             {
                return null;
             }

             _context.Stocks.Remove(stockModel);
             await _context.SaveChangesAsync();
             return stockModel;
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            var stockModel = await _context.Stocks.Include(c => c.Comments).FirstOrDefaultAsync(c => c.Id == id);

            return stockModel;
        }

        public async Task<Stock> UpdateAsync(int id, UpdateStockRequestDto stockDto)
        {
            var stockModel = await _context.Stocks.FirstOrDefaultAsync(s => s.Id == id);

            if(stockModel == null)
            {
                return null;
            }
            stockModel.Symbol = stockDto.Symbol;
            stockModel.CompanyName = stockDto.CompanyName;
            stockModel.Purchase = stockDto.Purchase;
            stockModel.Industry = stockDto.Industry;
            stockModel.MarketCap = stockDto.MarketCap;
            await _context.SaveChangesAsync();

            return stockModel;
        }

        public async Task<Stock?> AsyncDelete(int id)
        {
            var stockModel =  await _context.Stocks.FirstOrDefaultAsync(s => s.Id == id);
            
            if(stockModel == null)
            {
                return null;
            }

            _context.Stocks.Remove(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<bool> StockExists(int id)
        {
             return await _context.Stocks.AnyAsync(s => s.Id == id);
        }

        public async Task<Stock?> GetBySymbolAsync(string symbol)
        {
            return await _context.Stocks.FirstOrDefaultAsync(s => s.Symbol == symbol);
        }
    }
}
