using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using pandx.Wheel.Application.Dto;
using pandx.Wheel.Domain.Repositories;
using pandx.Wheel.Extensions;
using pandx.Wheel.Models;
using Sample.Application.Books.Dto;
using Sample.Domain.Books;

namespace Sample.Application.Books;

public class BookAppService : SampleAppServiceBase, IBookAppService
{
    private readonly IRepository<Book, long> _bookRepository;
    private readonly ILogger<BookAppService> _logger;

    public BookAppService(IRepository<Book, long> bookRepository, ILogger<BookAppService> logger)
    {
        _bookRepository = bookRepository;
        _logger = logger;
    }


    public async Task CreateBookAsync(CreateOrUpdateBookRequest request)
    {
        var book = Mapper.Map<Book>(request.Book);
        await _bookRepository.InsertAsync(book);
    }

    public async Task UpdateBookAsync(CreateOrUpdateBookRequest request)
    {
        var book = await _bookRepository.GetAsync(request.Book.Id!.Value);
        Mapper.Map(request.Book, book);
        await _bookRepository.UpdateAsync(book);
    }

    public async Task DeleteBookAsync(EntityDto<long> request)
    {
        await _bookRepository.DeleteAsync(request.Id);
    }

    public async Task<PagedResponse<BookDto>> GetPagedBooksAsync(GetBooksRequest request)
    {
        var query = await _bookRepository.GetAllAsync();
        query = query.Where(b => b.CreationTime >= request.StartTime && b.CreationTime <= request.EndTime)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Filter),
                b => b.Title.Contains(request.Filter!) || b.Author.Contains(request.Filter!));
        var totalCount = await query.CountAsync();
        var books = await query.OrderBy(request.Sorting!).PageBy(request).ToListAsync();
        var dtos = books.Select(item =>
        {
            var dto = Mapper.Map<BookDto>(item);
            return dto;
        }).ToList();
        return new PagedResponse<BookDto>(totalCount, dtos);
    }
}