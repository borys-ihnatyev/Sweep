namespace Messaging;

record GenericMessage
{
    public String type { get; init; }

    public GenericMessage(String type)
    {
        this.type = type;
    }
}

record MessageWithPayload<T>
{
    public String type { get; init; }
    public T payload { get; init; }

    public MessageWithPayload(String type, T payload)
    {
        this.type = type;
        this.payload = payload;
    }
}

record ErrorPayload
{
    public string type { get; init; }
    public string message { get; init; }

    public ErrorPayload(Exception exception)
    {
        type = exception.GetType().Name;
        message = exception.Message;
    }
}

record ErrorMessage: MessageWithPayload<ErrorPayload>
{
    public ErrorMessage(Exception exception): base("error", new ErrorPayload(exception)) { }
}