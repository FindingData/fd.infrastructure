using fd.infrastructure.entity.Enums;
using fd.infrastructure.entity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.entity
{
     
    public class WebResponseContent
    {
        public bool status { get; set; }   

        public string code { get; set; }

        public string message { get; set; } = "操作成功";

        public dynamic data { get; set; }


        public WebResponseContent()
        {

        }

       

        public WebResponseContent OK(dynamic data = null)
        {
            this.status = true;            
            this.data = data;
            return this;
        }

        public WebResponseContent OK(string message = "操作成功", dynamic data = null)
        {
            this.status = true;
            this.message = message;
            this.data = data;
            return this;
        }
        public WebResponseContent WithType(ResponseType responseType)
        {
            return Set(responseType,string.Empty, true);
        }

        public WebResponseContent Error(string message = null)
        {
            this.status = false;
            this.message = message;
            return this;
        }

        public WebResponseContent Error(ResponseType responseType)
        {
            return Set(responseType, string.Empty, null);
        }
 
        public WebResponseContent Set(ResponseType responseType, bool? status)
        {
            return this.Set(responseType, null, status);
        }

        public WebResponseContent Set(ResponseType responseType, string msg, bool? status)
        {
            if (status != null)
            {
                this.status = (bool)status;
            }
            this.code = ((int)responseType).ToString();
            if (!string.IsNullOrEmpty(msg))
            {
                message = msg;
                return this;
            }
            message = responseType.GetMsg();
            return this;
        }

     
    }
}
