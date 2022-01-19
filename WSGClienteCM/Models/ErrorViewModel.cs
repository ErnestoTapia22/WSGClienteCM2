using Newtonsoft.Json;

namespace WSGClienteCM.Models
{
    public class ErrorViewModel
    {
        public int EventLogId { get; private set; }
        public string Message { get; private set; }
        public ErrorViewModel(int eventLogId, string message)
        {
            EventLogId = eventLogId;
            Message = message;
        }
        public string GetErrorAsJson()
        {
            return JsonConvert.SerializeObject(this);
        }
        public override string ToString()
        {
            return EventLogId == 0 ? Message : string.Format("{0} - Error Id: {1}", Message, EventLogId);
        }
    }
}
