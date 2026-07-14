public class EditRequestService
{
    private string? _logType;
    private int? _id;
    private object? _payload;

    public void SetPending(string logType, int id, object payload)
    {
        _logType = logType;
        _id = id;
        _payload = payload;
    }

    public (int Id, object Payload)? Consume(string logType)
    {
        if (_logType == logType && _id is int id && _payload is not null)
        {
            var result = (id, _payload);
            _logType = null;
            _id = null;
            _payload = null;
            return result;
        }
        return null;
    }
}
