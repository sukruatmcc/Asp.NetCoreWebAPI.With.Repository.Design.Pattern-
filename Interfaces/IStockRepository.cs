using api.Dtos.Stock;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    public interface IStockRepository
    {
        Task<List<Stock>> GetAllAsync(QueryObject queryObject);
        Task<Stock?> GetByIdAsync(int id);//firstordefault can be null
        Task<Stock?> GetBySymbolAsync(string symbol);
        Task<Stock> CreateAsync(Stock stockModel);
        Task<Stock> UpdateAsync(int id, UpdateStockRequestDto stockDto);
        Task<Stock?> AsyncDelete(int id);
        Task<bool> StockExists(int id);
    }
}
