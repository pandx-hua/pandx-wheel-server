using Microsoft.AspNetCore.Mvc;
using pandx.Wheel.Application.Dto;
using pandx.Wheel.Auditing;
using pandx.Wheel.Authorization.Permissions;
using pandx.Wheel.Controllers;
using pandx.Wheel.Models;
using Sample.Application.Books;
using Sample.Application.Books.Dto;
using Sample.Domain.Authorization.Permissions;

namespace Sample.Host.WebAPI.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class BookController : WheelControllerBase
{
    private readonly IBookAppService _bookAppService;

    public BookController(
        IBookAppService bookAppService)
    {
        _bookAppService = bookAppService;
    }
    
    [NeedPermission(SamplePermissions.Resources.Books, SamplePermissions.Actions.Create)]
    [HttpPost(Name = nameof(CreateBook))]
    public async Task CreateBook(CreateOrUpdateBookRequest request)
    {
        await _bookAppService.CreateBookAsync(request);
    }
    [NeedPermission(SamplePermissions.Resources.Books, SamplePermissions.Actions.Update)]
    [HttpPost(Name = nameof(UpdateBook))]
    public async Task UpdateBook(CreateOrUpdateBookRequest request)
    {
        await _bookAppService.UpdateBookAsync(request);
    }
    [NeedPermission(SamplePermissions.Resources.Books, SamplePermissions.Actions.Delete)]
    [HttpPost(Name = nameof(DeleteBook))]
    public async Task DeleteBook(EntityDto<long> request)
    {
        await _bookAppService.DeleteBookAsync(request);
    }
    [NeedPermission(SamplePermissions.Resources.Books, SamplePermissions.Actions.Search)]
    [HttpPost(Name = nameof(GetPagedBooks))]
    [NoAudited]
    public async Task<PagedResponse<BookDto>> GetPagedBooks(GetBooksRequest request)
    {
        return await _bookAppService.GetPagedBooksAsync(request);
    }
}