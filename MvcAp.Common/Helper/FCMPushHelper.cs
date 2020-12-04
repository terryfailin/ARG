using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace MvcAp.Common.Helper
{
    public  class FCMPushHelper
    {
        public FCMPushHelper()
        {
            // TODO: Add constructor logic here
        }

        public bool Successful
        {
            get;
            set;
        }

        public string Response
        {
            get;
            set;
        }
        public Exception Error
        {
            get;
            set;
        }



        public FCMPushHelper SendNotification(int articleId, string message, List<string> deviceIds)
        {
            FCMPushHelper result = new FCMPushHelper();
            try
            {
                result.Successful = true;
                result.Error = null;
                // var value = message;
                var requestUri = "https://fcm.googleapis.com/fcm/send";

                WebRequest webRequest = WebRequest.Create(requestUri);
                webRequest.Method = "POST";
                webRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAPOhCrpQ:APA91bFvbDJyuQIAYCLk7_SrI7md6EpWldIgPBgOMCiZTZxGCwDdjxMqfQFBxxJiYFdxIVMhNCr5o5F6RPxsOmiSM7MJ0lnViQe0C4DZnETLORL8htA8MRI8u626hNErg8DjNWQ6_zIX"));

                webRequest.ContentType = "application/json";

                var msg = new
                {
                    // to = YOUR_FCM_DEVICE_ID, // Uncoment this if you want to test for single device
                    registration_ids = deviceIds, // this is for topic 
                    data = new
                    {
                        message = message,
                        articleId = articleId
                    }
                };
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(msg);

                Byte[] byteArray = Encoding.UTF8.GetBytes(json);

                webRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);

                    using (WebResponse webResponse = webRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = webResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                result.Response = sResponseFromServer;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                result.Successful = false;
                result.Response = null;
                result.Error = ex;
            }
            return result;
        }
    }
}
