using DatingApp.API.Base;

namespace DatingApp.API.ResourceParameters;

public class MessagesParameters : EntityPagination
{
    /// <summary>
    /// Message sender or the recipient Id.
    /// </summary>
    public string? MemberId { get; set; }

    public string Container { get; set; } = MessagesContainer.INBOX;
}

public static class MessagesContainer
{
    /// <summary>
    /// Contains all messages received by the member (messages sent by others to the member).
    /// </summary>
    public const string INBOX = "inbox";

    /// <summary>
    /// Contains all messages sent by the member (messages the member sent to others).
    /// </summary>
    public const string OUTBOX = "outbox";
}
