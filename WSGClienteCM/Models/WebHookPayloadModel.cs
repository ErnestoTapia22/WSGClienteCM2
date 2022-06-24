﻿using System.Collections.Generic;
using System;

namespace WSGClienteCM.Models
{
    //ini ernesto.tapia@materiagris.pe 16/02/2022 tel 979718300 
    public class WebHookPayloadModel
    {
        public long Timestamp
        {
            get; set;
        }
        public string WebhookEvent
        {
            get; set;
        }
        public string issue_event_type_name
        {
            get; set;
        }
        public WebHookIssue issue
        {
            get; set;
        }

    }
    public class WebHookIssue : BaseWebHook
    {
        public WebHookField fields
        {
            get; set;
        }
    }
    public class WebHookField : BaseWebHook
    {
        public string summary
        {
            get; set;
        }
        public WebHookStatus status
        {
            get; set;
        }
        public FieldObject customfield_12229
        {
            get; set;
        }
        public string customfield_12314
        {
            get; set;
        }
        public string customfield_12301
        {
            get; set;
        }
        public string customfield_12427
        {
            get; set;
        }// cancelado tra
        public string customfield_12319
        {
            get; set;
        }
        public string customfield_12315
        {
            get; set;
        }
        public List<FieldObject> customfield_12308
        {
            get; set;
        }
        public List<SubTask> subtasks
        {
            get; set;
        }
        public Project project
        {
            get; set;
        }
        //DEV CY -- INI
        public string customfield_13400
        {
            get; set;
        }

        public string customfield_13409
        {
            get; set;
        }
        
        public FieldObjectComment comment
        {
            get; set;
        }
        //DEV CY -- FIN
    }
    public class WebHookStatus : BaseWebHook
    {


    }


    public class FieldObject : BaseWebHook
    {
        public string value
        {
            get; set;
        }
    }
    public class Project : BaseWebHook
    {

    }

    public class SubTask : WebHookIssue
    {

    }
    public class BaseWebHook
    {

        public string self
        {
            get; set;
        }
        public string name
        {
            get; set;
        }

        public string id
        {
            get; set;
        }
        public string key
        {
            get; set;
        }
    }

    //DEV CY -- INI

    public class FieldObjectComment : CommentsField
    {
        public List<CommentsField> comments
        {
            get; set;
        }
    }
    public class CommentsField
    {
        
        public string created
        {
            get; set;
        }
        public string updated
        {
            get; set;
        }
        public string body
        {
            get; set;
        }
    }
    //DEV CY -- FIN

    //fin ernesto.tapia@materiagris.pe
}
