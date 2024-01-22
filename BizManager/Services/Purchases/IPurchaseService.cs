using SharedProject.Dtos;

namespace BizManager.Services.Purchases
{
    // Interface that defines methods for communicating with Server
    public interface IPurchaseService
    {
        // Return paginated filtered list of PurchaseDto objects
        Task<Pagination<PurchaseDto>> GetPurchasesAsync(string? searchText, string? purchaseDate, int supplierId, int materialId, int pageIndex, int pageSize);
        // Return single PurchaseDto object
        Task<PurchaseDto> GetSinglePurchaseAsync(int id);
        // Create new Purchase
        Task<object> CreateNewPurchaseAsync(PurchaseDto purchaseDto);
        // Edit selected Purchase
        Task<object> EditPurchaseAsync(PurchaseDto purchaseDto);
        // Delete selected Purchase
        Task<int> DeletePurchaseAsync(int id);
        // Return lis of Purchase dates
        Task<List<string>> GetPurchaseDatesAsync();
    }
}
