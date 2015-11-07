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
    public struct IssuePanelOptions
    {

        public bool inPanel;
        public bool showOrg;
        public bool showCat;
        public bool showTopic;
        public bool showUser;
        public bool userVoted;
    }

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

    public abstract class CountCol : ICollection
    {
        protected List<CountDay> cArray = new List<CountDay>();

        public CountCol() { }

        public CountDay this[int index]
        {
            get { return (CountDay)cArray[index]; }
        }

        public void CopyTo(Array a, int index)
        {
            cArray.CopyTo((CountDay[])a, index);
        }

        public int Count
        {
            get { return cArray.Count; }
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
            return cArray.GetEnumerator();
        }

        public void Add(CountDay newVCW)
        {
            cArray.Add(newVCW);
        }

        public abstract int Get_Value(DateTime day);

        public bool isCurrent()
        {
            if (cArray[cArray.Count - 1].date.Date == DateTime.UtcNow.Date)
                return true;
            else
                return false;
        }
    }

    
    public class CountDay
    {
        public int count;
        public DateTime date;
        public CountDay() { }
        public CountDay(DateTime d, int c)
        {
            count = c;
            date = d;
        }

        public void updateCount()
        {
            count++;
        }
    }

    public class ViewCountCol : CountCol    
    {
        //public override List<ViewCountDay> vcArray = new List<ViewCountDay>();

        public ViewCountCol() {}

        /// <summary>
        /// Use to update the view count for an issue. When called checks to see if the last entry(the last time it was viewed) is the current day. If it is then it updates the view count for that day. If it is a new day it creates a new ViewCountDay and appends to the end of the collection.
        /// </summary>
        public void Update()
        {
            if (cArray.Count > 0 && isCurrent())
                cArray[cArray.Count - 1].updateCount();
            else
                cArray.Add(new CountDay(DateTime.Today, 1));
            
        }

        /// <summary>
        /// Returns the number of new views from the passed in day.
        /// </summary>
        /// <param name="day">The day that is being checked for number of new views.</param>
        /// <returns>The number of new views of the passed in day.</returns>
        public override int Get_Value(DateTime day)
        {
            foreach (CountDay v in cArray)
            {
                if (v.date.Date == day.Date)
                    return v.count;
            }
            return 0;
        }
    }

 

    //================================= VoteCounts helper data structures =========================================================================//


    public class VoteCountCol : CountCol
    {


        public VoteCountCol() {  }

        /// <summary>
        /// Used to increment the vote count for an issue. When called checks to see if the last entry(the last time it was voted for) is the current day. If it is then it updates the vote count for that day. If it is a new day it creates a new CountDay and appends to the end of the collection.
        /// </summary>
        public void Update()
        {
            if (cArray.Count > 0 && isCurrent())
                cArray[cArray.Count - 1].updateCount();
            else
                cArray.Add(new CountDay(DateTime.Today, 1));

        }

        /// <summary>
        /// Used to decrement the vote count for an issue. When called checks to see if the last entry(the last time it was voted for) is the current day. If it is then it updates the vote count for that day. If it is a new day it creates a new CountDay and appends to the end of the collection.
        /// </summary>
        public void Remove_Vote()
        {
            if (cArray.Count > 0 && isCurrent())
                cArray[cArray.Count - 1].count--;
            else
                cArray.Add(new CountDay(DateTime.Today, -1));
        }


        /// <summary>
        /// Returns the number of new votes from the passed in day.
        /// </summary>
        /// <param name="day">The day that is being checked for number of new votes.</param>
        /// <returns>The number of new votes of the passed in day.</returns>
        public override int Get_Value(DateTime day)
        {
            foreach(CountDay v in cArray)
            {
                if (v.date.Date == day.Date)
                    return v.count;
            }
            return 0;
        }
    
    }


    //================================= SubCounts helper data structures =========================================================================//


    public class SubCountCol : CountCol
    {

        /// <summary>
        /// Used to increment the Sub count for an issue. When called checks to see if the last entry(the last time a user added the sub) is the current day. If it is then it updates the sub count for that day. If it is a new day it creates a new SubCountDay and appends to the end of the collection.
        /// </summary>
        public void Update()
        {
            if (cArray.Count > 0 && isCurrent())
                cArray[cArray.Count - 1].updateCount();
            else
                cArray.Add(new CountDay(DateTime.Today, 1));

        }

        /// <summary>
        /// Used to decrement the vote count for an issue. When called checks to see if the last entry(the last time it was voted for) is the current day. If it is then it updates the vote count for that day. If it is a new day it creates a new SubCountDay and appends to the end of the collection.
        /// </summary>
        public void Remove_Sub()
        {
            if (cArray.Count > 0 && isCurrent())
                cArray[cArray.Count - 1].count--;
            else
                cArray.Add(new CountDay(DateTime.Today, -1));
        }


        /// <summary>
        /// Returns the number of new subscriptions from the passed in day.
        /// </summary>
        /// <param name="day">The day that is being checked for number of new subscriptions.</param>
        /// <returns>The number of new subscritions of the passed in day.</returns>
        public override int Get_Value(DateTime day)
        {
            foreach (CountDay s in cArray)
            {
                if (s.date.Date == day.Date)
                    return s.count;
            }
            return 0;
        }

    }

    //================================= CommentCounts helper data structures =========================================================================//


    public class CommentCountCol : CountCol
    {
        public CommentCountCol() { }

        /// <summary>
        /// Use to update the view count for an issue. When called checks to see if the last entry(the last time it was viewed) is the current day. If it is then it updates the view count for that day. If it is a new day it creates a new CommentCountDay and appends to the end of the collection.
        /// </summary>
        public void Update()
        {
            if (cArray.Count > 0 && isCurrent())
                cArray[cArray.Count - 1].updateCount();
            else
                cArray.Add(new CountDay(DateTime.Today, 1));

        }

     
        /// <summary>
        /// Returns the number of new comments from the passed in day.
        /// </summary>
        /// <param name="day">The day that is being checked for number of new comments.</param>
        /// <returns>The number of new comments of the passed in day.</returns>
        public override int Get_Value(DateTime day)
        {
            foreach (CountDay c in cArray)
            {
                if (c.date.Date == day.Date)
                    return c.count;
            }
            return 0;
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

        public void Add_Vote(int commentId, int issueID, int orgID)
        {
            uvArray.Add(new UserVote(issueID, orgID, commentId));
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
                if (uv.IssueID == issueID && uv.OrgID == orgID && uv.CommentId == 0)
                {
                    uvArray.Remove(uv);
                    return true;
                }
            }
            return false;
        }

        public bool Delete_Vote(int commentID, int issueID, int orgID)
        {
            foreach (UserVote uv in uvArray)
            {
                if (uv.IssueID == issueID && uv.OrgID == orgID && uv.CommentId == commentID)
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
                if (uv.IssueID == issueID && uv.OrgID == orgID && uv.CommentId == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Check_Vote(int commentId, int issueID, int orgID)
        {
            foreach (UserVote uv in uvArray)
            {
                if (uv.IssueID == issueID && uv.OrgID == orgID && uv.CommentId == commentId)
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
        public int CommentId;

        public UserVote() { }
        public UserVote(int issueID, int orgID)
        {
            IssueID = issueID;
            OrgID = orgID;
            CommentId = 0;
        }
        public UserVote(int issueId, int orgId, int commentId)
        {
            IssueID = issueId;
            OrgID = orgId;
            CommentId = commentId;
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
