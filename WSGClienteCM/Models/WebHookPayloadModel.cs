namespace WSGClienteCM.Models
{
    public class WebHookPayloadModel
    {
        public long Timestamp { get; set; }
        public string WebhookEvent { get; set; }
        public string issue_event_type_name { get; set; }
        public WebHookIssue issue { get; set; }

    }
    public class WebHookIssue
    {
        public string id { get; set; }
        public string self { get; set; }
        public string key { get; set; }
        public WebHookField fields { get; set; }

    }
    public class WebHookField
    {
        public string self { get; set; }
        public string name { get; set; }

        public string id { get; set; }
        public WebHookStatus status { get; set; }

    }
    public class WebHookStatus
    { 
        public string self { get; set; }
        public string name { get; set; }
       
        public string id { get; set; }

    }
   
}
