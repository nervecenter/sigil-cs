using System;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using System.Xml;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Data.SqlTypes;
using System.Xml.Linq;
namespace Sigil.Models
{

    public static class ViewCountXML
    {

        public static XElement CreateNew()
        {
            ViewCountCol vcol = new ViewCountCol();
            vcol.Update();
            return DATAtoXML(vcol);
        }

        /// <summary>
        /// Used to update the count of a ViewCount.count data field
        /// </summary>
        /// <param name="vcData">XML Data from ViewCount.count field</param>
        /// <returns></returns>
        public static XElement UpdateWeekCount(XElement vcData)
        {
            ViewCountCol vcol = XMLtoDATA(vcData);

            vcol.Update();

            return DATAtoXML(vcol);
        }

        /// <summary>
        /// Converts a ViewCountCol object which keeps a list of Views per week for an issue into XML format to store in Viewcounts.count field
        /// </summary>
        /// <param name="vcData">Already Existing ViewCount.count data coverted from XML into ViewCountCol</param>
        /// <returns></returns>
        public static XElement DATAtoXML(ViewCountCol vcData)
        {
            using (var memStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memStream))
                {
                    var xmlSerial = new XmlSerializer(typeof(ViewCountCol));
                    xmlSerial.Serialize(streamWriter, vcData);
                    return XElement.Parse(Encoding.ASCII.GetString(memStream.ToArray()));
                }
            }
        }

        /// <summary>
        /// Used to unserialize Xml data into a ViewCountCol to update a views for a week
        /// </summary>
        /// <param name="xmlVC"></param>
        /// <returns></returns>
        public static ViewCountCol XMLtoDATA(XElement xmlVC)
        {
            var xmlSerial = new XmlSerializer(typeof(ViewCountCol));
            return (ViewCountCol)xmlSerial.Deserialize(xmlVC.CreateReader());
        }
    }

    public class ViewCountCol : ICollection
    {
        private List<ViewCountWeek> vcArray = new List<ViewCountWeek>();
       

        public ViewCountWeek this[int index]
        {
            get { return (ViewCountWeek)vcArray[index]; }
        }

        public void CopyTo(Array a, int index)
        {
            vcArray.CopyTo((ViewCountWeek[])a, index);
        }

        public int Count
        {
            get { return vcArray.Count; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public IEnumerator GetEnumerator()
        {
            return vcArray.GetEnumerator();
        }

        public void Add(ViewCountWeek newVCW)
        {
            vcArray.Add(newVCW);
        }

        public void Update()
        {
            if (vcArray.Count > 0 && isCurrent())
                vcArray[vcArray.Count - 1].updateCount();
            else
                vcArray.Add(new ViewCountWeek(DateTime.UtcNow, 1));
            
        }

        public bool isCurrent()
        {
            if (WeekCompare.isSame(vcArray[vcArray.Count - 1].weekDate, DateTime.UtcNow))
                return true;
            else
                return false;
        }
    }
    
    public static class WeekCompare
    {
        public static Calendar cal = DateTimeFormatInfo.CurrentInfo.Calendar;

        public static bool isSame(DateTime a, DateTime b)
        {
            return (a.Year == b.Year && cal.GetWeekOfYear(a, DateTimeFormatInfo.CurrentInfo.CalendarWeekRule, DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek) == cal.GetWeekOfYear(b, DateTimeFormatInfo.CurrentInfo.CalendarWeekRule, DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek));
        }
    }

    /// <summary>
    /// Class model Viewccount data for issues and orgs.
    /// </summary>
    public class ViewCountWeek
    {
        public int count;
        public DateTime weekDate;
        public ViewCountWeek() { }
        public ViewCountWeek(DateTime d, int c)
        {
            count = c;
            weekDate = d;
        }

        public void updateCount()
        {
            count++;
        }
        
    }

}
