using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Notification Service", Version = "v1" });
});
builder.Services.AddSingleton<NotificationRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Notification Service v1"));
}

app.UseHttpsRedirection();

// ── Notifications ─────────────────────────────────────────────────────────────

app.MapGet("/notifications", (NotificationRepository repo) => repo.GetAll())
    .WithName("GetNotifications");

app.MapGet("/notifications/{id:guid}", (Guid id, NotificationRepository repo) =>
{
    var notification = repo.GetById(id);
    return notification is null ? Results.NotFound() : Results.Ok(notification);
})
.WithName("GetNotificationById");

app.MapPost("/notifications", ([FromBody] CreateNotificationRequest request, NotificationRepository repo) =>
{
    if (string.IsNullOrWhiteSpace(request.Recipient) || string.IsNullOrWhiteSpace(request.Message))
        return Results.BadRequest("Recipient and Message are required.");

    var notification = new Notification
    {
        Id = Guid.NewGuid(),
        Type = request.Type,
        Message = request.Message,
        Recipient = request.Recipient,
        CreatedAt = DateTime.UtcNow,
        IsRead = false
    };

    repo.Add(notification);

    // Futuramente: este endpoint será chamado pelo consumer do broker de mensagens
    return Results.Created($"/notifications/{notification.Id}", notification);
})
.WithName("CreateNotification");

app.MapPatch("/notifications/{id:guid}/read", (Guid id, NotificationRepository repo) =>
{
    var notification = repo.GetById(id);
    if (notification is null) return Results.NotFound();

    notification.IsRead = true;
    return Results.Ok(notification);
})
.WithName("MarkNotificationAsRead");

app.Run();

// ── Models ───────────────────────────────────────────────────────────────────

public class Notification
{
    public Guid Id { get; set; }
    public NotificationType Type { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}

public enum NotificationType { OrderCreated, OrderStatusChanged, OrderCancelled, General }

public record CreateNotificationRequest(NotificationType Type, string Message, string Recipient);

// ── Repository ───────────────────────────────────────────────────────────────

public class NotificationRepository
{
    private readonly List<Notification> _notifications = [];

    public IEnumerable<Notification> GetAll() => _notifications;
    public Notification? GetById(Guid id) => _notifications.FirstOrDefault(n => n.Id == id);
    public void Add(Notification notification) => _notifications.Add(notification);
}
