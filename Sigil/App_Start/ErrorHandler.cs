using System;
using System.IO;
using System.Web.Hosting;

namespace Sigil
{
    public static class ErrorHandler 
    {
        
        public static void Log_Error(Object source, Exception e)
        {
            string path = HostingEnvironment.MapPath(@"~/App_Data/");
            string error = "\r\n"+  DateTime.Now.ToLongTimeString() +" "+ DateTime.Now.ToLongDateString() + " -- Error From " + source.ToString() + " -- " + e.Message;
            StreamWriter w = File.AppendText(path + "/Error_Log.txt");
            w.WriteLine(error);
            w.Close();
        }
    }
}
