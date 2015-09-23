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

        public ViewCountCol() {}

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

        /// <summary>
        /// Use to update the view count for an issue. When called checks to see if the last entry(the last time it was viewed) is the current day. If it is then it updates the view count for that day. If it is a new day it creates a new ViewCountDay and appends to the end of the collection.
        /// </summary>
        public void Update()
        {
            if (vcArray.Count > 0 && isCurrent())
                vcArray[vcArray.Count - 1].updateCount();
            else
                vcArray.Add(new ViewCountDay(DateTime.Today, 1));
            
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

        public VoteCountCol() {  }

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

        /// <summary>
        /// Used to increment the vote count for an issue. When called checks to see if the last entry(the last time it was voted for) is the current day. If it is then it updates the vote count for that day. If it is a new day it creates a new VoteCountDay and appends to the end of the collection.
        /// </summary>
        public void Update()
        {
            if (vcArray.Count > 0 && isCurrent())
                vcArray[vcArray.Count - 1].updateCount();
            else
                vcArray.Add(new VoteCountDay(DateTime.Today, 1));

        }

        /// <summary>
        /// Used to decrement the vote count for an issue. When called checks to see if the last entry(the last time it was voted for) is the current day. If it is then it updates the vote count for that day. If it is a new day it creates a new VoteCountDay and appends to the end of the collection.
        /// </summary>
        public void Remove_Vote()
        {
            if (vcArray.Count > 0 && isCurrent())
                vcArray[vcArray.Count - 1].count--;
            else
                vcArray.Add(new VoteCountDay(DateTime.Today, -1));
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


    //================================= SubCounts helper data structures =========================================================================//


    public class SubCountCol : ICollection
    {
        private List<SubCountDay> scArray = new List<SubCountDay>();

        public SubCountCol() { }

        public SubCountDay this[int index]
        {
            get { return (SubCountDay)scArray[index]; }
        }

        public void CopyTo(Array a, int index)
        {
            scArray.CopyTo((SubCountDay[])a, index);
        }

        public int Count
        {
            get { return scArray.Count; }
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
            return scArray.GetEnumerator();
        }

        public void Add(SubCountDay newVCW)
        {
            scArray.Add(newVCW);
        }

        /// <summary>
        /// Used to increment the Sub count for an issue. When called checks to see if the last entry(the last time a user added the sub) is the current day. If it is then it updates the sub count for that day. If it is a new day it creates a new SubCountDay and appends to the end of the collection.
        /// </summary>
        public void Update()
        {
            if (scArray.Count > 0 && isCurrent())
                scArray[scArray.Count - 1].updateCount();
            else
                scArray.Add(new SubCountDay(DateTime.Today, 1));

        }

        /// <summary>
        /// Used to decrement the vote count for an issue. When called checks to see if the last entry(the last time it was voted for) is the current day. If it is then it updates the vote count for that day. If it is a new day it creates a new SubCountDay and appends to the end of the collection.
        /// </summary>
        public void Remove_Sub()
        {
            if (scArray.Count > 0 && isCurrent())
                scArray[scArray.Count - 1].count--;
            else
                scArray.Add(new SubCountDay(DateTime.Today, -1));
        }

        public bool isCurrent()
        {
            if (scArray[scArray.Count - 1].date.Date == DateTime.UtcNow.Date)
                return true;
            else
                return false;
        }

        public int Get_Subs(DateTime day)
        {
            foreach (SubCountDay s in scArray)
            {
                if (s.date.Date == day.Date)
                    return s.count;
            }
            return 0;
        }

    }

    /// <summary>
    /// Class model Voteccount data for issues and orgs.
    /// </summary>
    public class SubCountDay
    {
        public int count;
        public DateTime date;
        public SubCountDay() { }
        public SubCountDay(DateTime d, int c)
        {
            count = c;
            date = d;
        }

        public void updateCount()
        {
            count++;
        }

    }

    //================================= CommentCounts helper data structures =========================================================================//


    public class CommentCountCol : ICollection
    {
        private List<CommentCountDay> ccArray = new List<CommentCountDay>();

        public CommentCountCol() { }

        public CommentCountDay this[int index]
        {
            get { return (CommentCountDay)ccArray[index]; }
        }

        public void CopyTo(Array a, int index)
        {
            ccArray.CopyTo((CommentCountDay[])a, index);
        }

        public int Count
        {
            get { return ccArray.Count; }
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
            return ccArray.GetEnumerator();
        }

        public void Add(CommentCountDay newVCW)
        {
            ccArray.Add(newVCW);
        }

        /// <summary>
        /// Use to update the view count for an issue. When called checks to see if the last entry(the last time it was viewed) is the current day. If it is then it updates the view count for that day. If it is a new day it creates a new CommentCountDay and appends to the end of the collection.
        /// </summary>
        public void Update()
        {
            if (ccArray.Count > 0 && isCurrent())
                ccArray[ccArray.Count - 1].updateCount();
            else
                ccArray.Add(new CommentCountDay(DateTime.Today, 1));

        }

        public bool isCurrent()
        {
            if (ccArray[ccArray.Count - 1].date.Date == DateTime.UtcNow.Date)
                return true;
            else
                return false;
        }

        public int Get_Comments(DateTime day)
        {
            foreach (CommentCountDay c in ccArray)
            {
                if (c.date.Date == day.Date)
                    return c.count;
            }
            return 0;
        }
    }

    /// <summary>
    /// Class model Viewccount data for issues and orgs.
    /// </summary>
    public class CommentCountDay
    {
        public int count;
        public DateTime date;
        public CommentCountDay() { }
        public CommentCountDay(DateTime d, int c)
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


    //================================= UserIDs helper data structures =========================================================================//


    public class UserIDCol : ICollection
    {
        private List<string> uvArray = new List<string>();

        public UserIDCol() { }
        public UserIDCol(List<string> usV)
        {
            uvArray = usV;
        }
        public UserIDCol(string userid)
        {
            Add_User(userid);
        }

        public string this[int index]
        {
            get { return (string)uvArray[index]; }
        }

        public void CopyTo(Array a, int index)
        {
            uvArray.CopyTo((string[])a, index);
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

        public void Add(string newVCW)
        {
            uvArray.Add(newVCW);
        }


        public void Add_User(string userid)
        {
            uvArray.Add(userid);
        }


        public bool Delete_User(string userid)
        {
            foreach (string uv in uvArray)
            {
                if (uv == userid)
                {
                    uvArray.Remove(uv);
                    return true;
                }
            }
            return false;
        }

        public List<string> Get_Users()
        {
            return uvArray;
        }

        public bool Check_User(string userid)
        {
            foreach (string uv in uvArray)
            {
                if (uv == userid)
                {
                    return true;
                }
            }
            return false;
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
