using System;
using System.IO;
using System.Web.Hosting;
using Sigil.Models;
namespace Sigil
{
    public static class ErrorHandler 
    {
        public static void Log_Error(Object source, Exception e, SigilDBDataContext dc)
        {
            Log_Error_File(source, e);
            Log_Error_DB(dc, source, e);

        }

        private static void Log_Error_File(Object source, Exception e)
        {
            string path = HostingEnvironment.MapPath(@"~/App_Data/");
            string error = "\r\n" + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString() + " -- Error From " + source.ToString() + " -- " + e.Message;
            StreamWriter w = File.AppendText(path + "/Error_Log.txt");
            w.WriteLine(error);
            w.Close();
        }

        private static void Log_Error_DB(SigilDBDataContext dc, Object source, Exception e)
        {
            Error newError = new Error();
            newError.error_date = DateTime.UtcNow;
            newError.error_object = source.ToString();
            newError.error_exception = e.Message;

            try
            {
                dc.Errors.InsertOnSubmit(newError);
                dc.SubmitChanges();
            }
            catch(Exception ee)
            {
                Log_Error_File(newError, ee.InnerException);
            }

        }
    }
}
