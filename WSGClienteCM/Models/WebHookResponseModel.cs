using System.Collections.Generic;

namespace WSGClienteCM.Models
{

    public class WebHookResponseModel
    {
        public string id { get; set; }
        public List<Field> fields { get; set; }
    }
  
    public class Field
    {
        public string id { get; set; }
        public dynamic value { get; set; }
    }
}
