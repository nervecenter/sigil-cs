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

    public static class CountXML<T>
    {
        /// <summary>
        /// Converts a ____CountCol object which keeps a list of Views per week for an issue into XML format to store in ___Counts.count field
        /// </summary>
        /// <param name="vcData">Already Existing ____Count.count data coverted from XML into ____CountCol</param>
        /// <returns>XML data to insert into DB field</returns>
        public static XElement DATAtoXML(T vcData)
        {
            using (var memStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memStream))
                {
                    var xmlSerial = new XmlSerializer(typeof(T));
                    xmlSerial.Serialize(streamWriter, vcData);
                    return XElement.Parse(Encoding.ASCII.GetString(memStream.ToArray()));
                }
            }
        }

        /// <summary>
        /// Used to unserialize Xml data into a ____CountCol collection
        /// </summary>
        /// <param name="xmlVC"></param>
        /// <returns>T data collection so that it can be updated </returns>
        public static T XMLtoDATA(XElement xmlVC)
        {

                var xmlSerial = new XmlSerializer(typeof(T));
                return (T)xmlSerial.Deserialize(xmlVC.CreateReader());

        }
    }


    //================================= ViewCounts helper data structures =========================================================================//

    
    public class ViewCountCol : ICollection
    {
        private List<ViewCountDay> vcArray = new List<ViewCountDay>();

        public ViewCountCol() { Update(); }

        public ViewCountDay this[int index]
        {
            get { return (ViewCountDay)vcArray[index]; }
        }

        public void CopyTo(Array a, int index)
        {
            vcArray.CopyTo((ViewCountDay[])a, index);
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

        public void Add(ViewCountDay newVCW)
        {
            vcArray.Add(newVCW);
        }

        public void Update()
        {
            if (vcArray.Count > 0 && isCurrent())
                vcArray[vcArray.Count - 1].updateCount();
            else
                vcArray.Add(new ViewCountDay(DateTime.UtcNow, 1));
            
        }

        public bool isCurrent()
        {
            if (vcArray[vcArray.Count - 1].date.Date == DateTime.UtcNow.Date)
                return true;
            else
                return false;
        }

        public int Get_Views(DateTime day)
        {
            foreach (ViewCountDay v in vcArray)
            {
                if (v.date.Date == day.Date)
                    return v.count;
            }
            return 0;
        }
    }

    /// <summary>
    /// Class model Viewccount data for issues and orgs.
    /// </summary>
    public class ViewCountDay
    {
        public int count;
        public DateTime date;
        public ViewCountDay() { }
        public ViewCountDay(DateTime d, int c)
        {
            count = c;
            date = d;
        }

        public void updateCount()
        {
            count++;
        }
        
    }


    //================================= VoteCounts helper data structures =========================================================================//


    public class VoteCountCol : ICollection
    {
        private List<VoteCountDay> vcArray = new List<VoteCountDay>();

        public VoteCountCol() { Update(); }

        public VoteCountDay this[int index]
        {
            get { return (VoteCountDay)vcArray[index]; }
        }

        public void CopyTo(Array a, int index)
        {
            vcArray.CopyTo((VoteCountDay[])a, index);
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

        public void Add(VoteCountDay newVCW)
        {
            vcArray.Add(newVCW);
        }

        public void Update()
        {
            if (vcArray.Count > 0 && isCurrent())
                vcArray[vcArray.Count - 1].updateCount();
            else
                vcArray.Add(new VoteCountDay(DateTime.UtcNow, 1));

        }

        public bool isCurrent()
        {
            if (vcArray[vcArray.Count - 1].date.Date == DateTime.UtcNow.Date)
                return true;
            else
                return false;
        }

        public int Get_Votes(DateTime day)
        {
            foreach(VoteCountDay v in vcArray)
            {
                if (v.date.Date == day.Date)
                    return v.count;
            }
            return 0;
        }

        public void Delete_Vote()
        {
            vcArray[vcArray.Count - 1].count--;
        }        
    }

    /// <summary>
    /// Class model Voteccount data for issues and orgs.
    /// </summary>
    public class VoteCountDay
    {
        public int count;
        public DateTime date;
        public VoteCountDay() { }
        public VoteCountDay(DateTime d, int c)
        {
            count = c;
            date = d;
        }

        public void updateCount()
        {
            count++;
        }

    }


    //================================= UserVotes helper data structures =========================================================================//


    public class UserVoteCol : ICollection
    {
        private List<UserVote> uvArray = new List<UserVote>();

        public UserVoteCol() { }
        public UserVoteCol(List<UserVote> usV)
        {
            uvArray = usV;
        }
        public UserVoteCol(int issueid, int orgid)
        {
            Add_Vote(issueid, orgid);
        }

        public UserVote this[int index]
        {
            get { return (UserVote)uvArray[index]; }
        }

        public void CopyTo(Array a, int index)
        {
            uvArray.CopyTo((UserVote[])a, index);
        }

        public int Count
        {
            get { return uvArray.Count; }
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
            return uvArray.GetEnumerator();
        }

        public void Add(UserVote newVCW)
        {
            uvArray.Add(newVCW);
        }

        /// <summary>
        /// Adds a vote to the List of users votes
        /// </summary>
        /// <param name="issueID">Id of the issue that is being voted on.</param>
        /// <param name="orgID">ID of the Org the issues is associated with.</param>
        public void Add_Vote(int issueID, int orgID)
        {
            uvArray.Add(new UserVote(issueID, orgID));
        }
        
        /// <summary>
        /// Deletes a vote of the user.
        /// </summary>
        /// <param name="issueID">Id f the issue the votes is being deleted from</param>
        /// <param name="orgID">Id of the org the issue is associated with</param>
        /// <returns>True if found and deleted, False if not found in list.</returns>
        public bool Delete_Vote(int issueID, int orgID)
        {
            foreach(UserVote uv in uvArray)
            {
                if (uv.IssueID == issueID && uv.OrgID == orgID)
                {
                    uvArray.Remove(uv);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the users votes
        /// </summary>
        /// <returns>A list of UserVote objects.</returns>
        public List<UserVote> Get_Votes()
        {
            return uvArray;
        }

        public bool Check_Vote(int issueID, int orgID)
        {
            foreach (UserVote uv in uvArray)
            {
                if (uv.IssueID == issueID && uv.OrgID == orgID)
                {
                    return true;
                }
            }
            return false;
        }

    }

    /// <summary>
    /// Class model UserVote data for issues and orgs.
    /// </summary>
    public class UserVote
    {
        public int IssueID;
        public int OrgID;

        public UserVote() { }
        public UserVote(int issueID, int orgID)
        {
            IssueID = issueID;
            OrgID = orgID;
        }
    }





    //============================= HighChart Helper Datastructure ===============================================================================//

    public class GraphData
    {
        public DateTime date;
        public int voteCount;
        public int viewCount;

        public GraphData(DateTime d, int voteC, int viewC)
        {
            date = d;
            voteCount = voteC;
            viewCount = viewC;
        }
    }
}
