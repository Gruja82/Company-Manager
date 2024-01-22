using SharedProject.Dtos;

namespace BizApi.Utility
{
    // This static generic class contains one method for returning paginated result
    public static class PaginationUtility<T> where T : class
    {
        // This method takes 3 parameters. First represents list od data, second is page number
        // and third represents number of items shown per page
        public static Pagination<T> GetPaginatedResult(in List<T> dataList, int currentPage, int pageSize)
        {
            // Define new Pagination<T> object
            Pagination<T> pagination = new();

            // Using LINQ, query the input data list
            pagination.DataList = (from c in dataList select c)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Calculate number of pages based on number of items shown per page
            double pageCount = (double)dataList.Count / pageSize;

            // Using Math.Ceiling method, which returns smallest integer grater than
            // or equal to specified double value, assign TotalPages property
            pagination.TotalPages = (int)Math.Ceiling(pageCount);

            // Assign currentPage to PageIndex property
            pagination.PageIndex = currentPage;

            // Assign pageSize to PageSize property
            pagination.PageSize = pageSize;

            // Return Pagination<T> object
            return pagination;
        }
    }
}
