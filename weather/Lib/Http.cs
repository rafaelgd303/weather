using System;
using System.IO;
using System.Net;
using System.Text;

namespace weather.Lib
{
    public class Http
    {
        public static CookieContainer Cc = new CookieContainer();

        public static string Post(string host, string data)
        {
            var request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "POST";
            request.KeepAlive = true;

            string postData = data;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            Stream dataStream = null;

            request.CookieContainer = Cc;

            try
            {
                dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection to server lost with msg: " + ex.Message);
            }


            var responseFromServer = string.Empty;
            StreamReader reader = null;
            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                dataStream = response.GetResponseStream();
                request.CookieContainer.Add(response.Cookies);
                Cc.Add(response.Cookies);

                if (dataStream != null)
                {
                    reader = new StreamReader(dataStream);
                    responseFromServer = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Http.Post method error: {0}", ex.Message);
            }
            finally
            {
                if (reader != null) reader.Close();
                if (dataStream != null) dataStream.Close();
                if (response != null) response.Close();
            }

            return responseFromServer;
        }

        public static string Get(string url)
        {
            var source = string.Empty;

            try
            {
                var req = (HttpWebRequest)WebRequest.Create(url);
                req.CookieContainer = Cc;
                req.Method = "GET";
                req.KeepAlive = true;

                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                Stream dataStream = response.GetResponseStream();

                if (dataStream != null)
                {
                    using (var reader = new StreamReader(dataStream))
                    {
                        source = reader.ReadToEnd();
                    }

                    req.CookieContainer.Add(response.Cookies);
                    Cc.Add(response.Cookies);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Http.Get method error: {0}", ex.Message);
            }

            return source;
        }
    }
}
