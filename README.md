*pandx.Wheel* 是一个开源的WEB应用快速开发框架，它基于领域驱动设计（Domain Driven Design，DDD）开发，构建于最新的.NET 8.0之上。当前版本，*pandx.Wheel* 是单体单租户的，但我们有计划在不久的将来，开源单体多租户及微服务多租户版本。在快速开发框架之外，*pandx.Wheel* 提供了一个健壮的启动模板。启动模板是前后端分离的，其中前端基于Vue3（JavaScript）技术栈。启动模板包含了WEB应用开发常用的模块且在持续进化中，当前包含用户、角色、权限、部门、维护、日志、定时任务等。使用 *pandx.Wheel* 提供的启动模版，您可以在不改动任何模板代码的情况下，快速开始编写您的业务代码。

下面我们给出一个图书管理的代码节选，从中可以看到 *pandx.Wheel* 给您带来的便利。

```c#
//领域层
public class Book : AuditedEntity<long>
{
    [StringLength(256)] public string Title { get; set; } = default!;

    [StringLength(256)] public string Author { get; set; } = default!;
}
```

```c#
//应用层
public class BookAppService:SampleAppServiceBase, IBookAppService
{
    private readonly IRepository<Book, long> _bookRepository;
    private readonly ILogger<BookAppService> _logger;

    public BookAppService(IRepository<Book, long> bookRepository,ILogger<BookAppService> logger)
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
        query=query.Where(b=>b.CreationTime>=request.StartTime && b.CreationTime<=request.EndTime)
           .WhereIf(!string.IsNullOrWhiteSpace(request.Filter),b=>b.Title.Contains(request.Filter!)||b.Author.Contains(request.Filter!));
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
```

```c#
//分发层
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
    [NeedPermission(SamplePermissions.Resources.Books, SamplePermissions.Actions.Browse)]
    [HttpPost(Name = nameof(GetPagedBooks))]
    [NoAudited]
    public async Task<PagedResponse<BookDto>> GetPagedBooks(GetBooksRequest request)
    {
        return await _bookAppService.GetPagedBooksAsync(request);
    }
}
```

上面的代码，快速实现了图书馆的增、删、改、查以及为不同的`EndPoint`赋予不同的权限和审计策略等功能，但这不是 *pandx.Wheel* 的全部，*pandx.Wheel* 能做的远远多于上面的演示。

目前，*pandx.Wheel* 已经实现了以下的基础功能：

* 依赖注入：*pandx.Wheel* 基于内置的依赖注入系统，实现了基于约定的服务自动注册及内置依赖系统不支持的属性注入；
* 仓储：*pandx.Wheel* 会为每个实体创建一个默认的仓储，仓储是对数据库管理系统（DBMS）及对象关系映射（ORM）的封装，它提供了许多实用的方法，可以大大降低数据访问逻辑；
* 工作单元：*pandx.Wheel* 中，每个请求调用的方法均被认为是一个工作单元，工作单元内所有的操作均是原子的，它在方法开始的时候创建事务，在没有异常的情况下，方法完成的时候提交事务，提交事务的时候，实体上的所有更改都会自动保存；
* 验证：*pandx.Wheel* 会根据数据注释及基于FlunentValidation库自定义验证规则验证接收的请求，如果请求无效，它会返回固定格式的验证异常，以便在客户端优雅地处理它们；
* 授权：*pandx.Wheel* 基于角色及声明授权，未授权的用户将无法访问对应的方法；
* 审计日志：*pandx.Wheel* 基于约定和配置，会为每个请求自动保存用户、浏览器、IP地址、控制器、方法、调用时间、持续时间、输入参数等信息，无论请求成功或失败；
* 异常处理：*pandx.Wheel* 提供了全局异常处理，任何异常均会以固定的格式返回，以便在客户端优雅地处理它们；
* 自动映射：*pandx.Wheel* 基于AutoMapper库实现了对象映射，基于自定义的映射规则，可以轻松将一个对象映射为另一个对象；
* 日志记录：*pandx.Wheel* 使用Serilog日志记录提供程序，得益于Serilog库的强大功能，*pandx.Wheel* 可以优雅地处理运行日志；
* 缓冲：*pandx.Wheel* 默认使用MemoryCache，通过配置，可以快速切换到Redis缓冲；

除了上面介绍的这些功能以外，*pandx.Wheel* 还为后台任务，数据过滤、领域事件、实时消息等提供了强大的基础设施和开发模型，重要的是，*pandx.Wheel* 还提供了一个健壮的启动模板。您专注于您的业务代码，无需重复造轮子，因为 *pandx.Wheel* 就是您最好的一个轮子。

关于 *pandx.Wheel* 的更多信息及如何快速开始请访问我们的网站：[http://www.pandx.com.cn](http://www.pandx.com.cn)

直达启动模板演示请访问：[http://120.53.120.184:8082](http://120.53.120.184:8082)

GitHub：[https://www.github.com/pandx-hua/pandx-wheel-server](https://www.github.com/pandx-hua/pandx-wheel-server)（后端） [https://www.github.com/pandx-hua/pandx-wheel-client](https://www.github.com/pandx-hua/pandx-wheel-client)（前端）

Gitee：[https://www.gitee.com/pandx-hua/pandx-wheel-server](https://www.gitee.com/pandx-hua/pandx-wheel-server)（后端）[https://www.gitee.com/pandx-hua/pandx-wheel-client](https://www.gitee.com/pandx-hua/pandx-wheel-client)（前端）

QQ群：972528993