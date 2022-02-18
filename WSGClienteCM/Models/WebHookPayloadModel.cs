using System.Collections.Generic;

namespace WSGClienteCM.Models
{
    //ini ernesto.tapia@materiagris.pe 16/02/2022
    public class WebHookPayloadModel
    {
        public long Timestamp { get; set; }
        public string WebhookEvent { get; set; }
        public string issue_event_type_name { get; set; }
        public WebHookIssue issue { get; set; }

    }
    public class WebHookIssue:BaseWebHook
    {
        public WebHookField fields { get; set; }
    }
    public class WebHookField:BaseWebHook
    {
        public string summary { get; set; }
        public WebHookStatus status { get; set; }
        public Field12229 customfield_12229 { get; set; }
        public List<SubTask> subtasks { get; set; }


    }
    public class WebHookStatus:BaseWebHook
    { 
    

    }
    public class Field12229 :BaseWebHook
    {
        public string value { get; set; }
    }

    public class SubTask: WebHookIssue
    { 
    
    }
    public class BaseWebHook {

        public string self { get; set; }
        public string name { get; set; }

        public string id { get; set; }
        public string key { get; set; }
    }

    //fin ernesto.tapia@materiagris.pe
}
