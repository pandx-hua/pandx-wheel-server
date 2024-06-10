using Microsoft.Extensions.Logging;
using pandx.Wheel.Events;
using Sample.Domain.Books;

namespace Sample.Application.Books.Events;

public class BookAddedEventHandler : NotificationEventHandler<EntityAddedEvent<Book>>
{
    private readonly ILogger<BookAddedEventHandler> _logger;

    public BookAddedEventHandler(ILogger<BookAddedEventHandler> logger)
    {
        _logger = logger;
    }

    protected override Task Handle(EntityAddedEvent<Book> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"新增了图书：{@event.Entity}");

        return Task.CompletedTask;
    }
}