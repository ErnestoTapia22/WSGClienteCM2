namespace WSGClienteCM.Models
{
    public class WebHookPayloadModel
    {
        public long Timestamp { get; set; }
        public string WebhookEvent { get; set; }
        public string issue_event_type_name { get; set; }

    }
}
