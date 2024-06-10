using pandx.Wheel.Application.Dto;
using pandx.Wheel.Application.Services;
using pandx.Wheel.DependencyInjection;
using pandx.Wheel.Models;
using Sample.Application.Books.Dto;

namespace Sample.Application.Books;

public interface IBookAppService:IApplicationService
{
    Task CreateBookAsync(CreateOrUpdateBookRequest request);
    Task UpdateBookAsync(CreateOrUpdateBookRequest request);
    Task DeleteBookAsync(EntityDto<long> request);
    Task<PagedResponse<BookDto>> GetPagedBooksAsync(GetBooksRequest request);
}