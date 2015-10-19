using System;
using System.IO;
using System.Web.Hosting;
using Sigil.Models;
namespace Sigil
{
    /// <summary>
    /// Class that handles error logging for sigil
    /// </summary>
    public static class ErrorHandler 
    {
        /// <summary>
        /// Logs the passed exception and object into the db as well as to a text file on the server
        /// </summary>
        /// <param name="source">Object that was created and caused an error</param>
        /// <param name="e">The associated exception</param>
        /// <param name="dc">Callers database instance</param>
        public static void Log_Error(Object source, Exception e, SigilDBDataContext dc)
        {
            Log_Error_File(source, e);
            Log_Error_DB(dc, source, e);

        }

        /// <summary>
        /// Logs the passed string as an error. Can be used to log custom errors
        /// </summary>
        /// <param name="source">Object that was created and caused an error</param>
        /// <param name="e">The associated custom error</param>
        /// <param name="dc">Callers database instance</param>
        public static void Log_Error(Object source, String e, SigilDBDataContext dc)
        {
            Log_Error_File(source, e);
            Log_Error_DB(dc, source, e);

        }

        /// <summary>
        /// Function that takes care of logging an error to the error text file
        /// </summary>
        /// <param name="source">Object that was created that is associated with the error</param>
        /// <param name="e">The passed exception</param>
        private static void Log_Error_File(Object source, Exception e)
        {
            string path = HostingEnvironment.MapPath(@"~/App_Data/");
            string error = "\r\n" + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString() + " -- Error From " + source.ToString() + " -- " + e.Message;
            StreamWriter w = File.AppendText(path + "/Error_Log.txt");
            w.WriteLine(error);
            w.Close();
        }

        /// <summary>
        /// Function that handles logging the error to the database so Sigil members can see them remotely
        /// </summary>
        /// <param name="dc">Database reference associated with the source</param>
        /// <param name="source">The object that was created and caused an error</param>
        /// <param name="e">The associated exception </param>
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

        /// <summary>
        /// Same as function above but overloaded to work with a passed string
        /// </summary>
        /// <param name="source"> Object that was created and caused an issue</param>
        /// <param name="e">Passed string used for error</param>
        private static void Log_Error_File(Object source, String e)
        {
            string path = HostingEnvironment.MapPath(@"~/App_Data/");
            string error = "\r\n" + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString() + " -- Error From " + source.ToString() + " -- " + e;
            StreamWriter w = File.AppendText(path + "/Error_Log.txt");
            w.WriteLine(error);
            w.Close();
        }

        /// <summary>
        /// Same as function above but overloaded to work with a passed string
        /// </summary>
        /// <param name="dc">Database reference associated with object</param>
        /// <param name="source">Object that was created and caused error</param>
        /// <param name="e">Passed string assicated with error</param>
        private static void Log_Error_DB(SigilDBDataContext dc, Object source, String e)
        {
            Error newError = new Error();
            newError.error_date = DateTime.UtcNow;
            newError.error_object = source.ToString();
            newError.error_exception = e;

            try
            {
                dc.Errors.InsertOnSubmit(newError);
                dc.SubmitChanges();
            }
            catch (Exception ee)
            {
                Log_Error_File(newError, ee.InnerException);
            }

        }
    }
}
