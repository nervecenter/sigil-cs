using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;
using Sigil.Repository;
using System.Xml.Linq;

namespace Sigil.Services
{
    public enum CountDataType
    {
        View, Vote, Subscription, Comment
    }

    //The operations we want to expose to the controllers;
    public interface ICountService
    {
        void CreateOrgCountData(int orgId);//ViewCount viewC, SubCount subC);
        void CreateIssueCountData(string userId, int orgId, int issueId);//ViewCount viewC, VoteCount voteC, CommentCount comC);
        void SaveOrgCountData();
        void SaveIssueCountData();

        void UpdateIssueViewCountData(Issue issue);
        void UpdateIssueVoteCountData(Issue issue, bool upVote = true);
        void SaveIssueDataChanges();

        /// <summary>
        /// Updated Orgs Subscription Data
        /// </summary>
        /// <param name="orgId">The orgs Id</param>
        /// <param name="subed">True(default) if adding a subscription, False if removing a subscription</param>
        void UpdateOrgSubscriptionCount(int orgId, bool subed = true);
        void SaveOrgDataChanges();

        void SaveCountChanges(CountCol count, CountDataType t, int orgId);
        void SaveCountChanges(CountCol count, CountDataType t, int orgid, int issueId);


        //I changed the order of the parameters half way through refactoring. Need to double check all calls to make sure parameters are in right order!!!!
        ViewCountCol GetIssueViewCountCol(int orgId, int issueId);
        VoteCountCol GetIssueVoteCountCol(int orgId, int issueId);
        CommentCountCol GetIssueCommentCountCol(int orgId, int issueId);

        IEnumerable<ViewCountCol> GetOrgViewCountCols(int orgId);
        IEnumerable<VoteCountCol> GetOrgVoteCountCols(int orgId);
        SubCountCol GetOrgSubscriptionCount(int orgId);
        IEnumerable<CommentCountCol> GetOrgCommentCountCols(int orgId);

        //UserVoteCol GetUserVotes(string userId);

    }

    public class CountService : ICountService
    {
        private readonly IOrgRepository OrgsRepository;
        private readonly IVoteCountRepository voteCountRepository;
        private readonly IViewCountRepository viewCountRepository;
        private readonly ISubscriptionCountRepository subCountRepository;
        private readonly ICommentCountRepository commCountRepository;
        private readonly IUnitOfWork unitOfWork;

        public CountService(IUnitOfWork unit, IVoteCountRepository voteRepo, IViewCountRepository viewRepo, ISubscriptionCountRepository subRepo, ICommentCountRepository commRepo, IOrgRepository orgRepo)
        {
            unitOfWork = unit;
            OrgsRepository = orgRepo;

            voteCountRepository = voteRepo;
            viewCountRepository = viewRepo;
            subCountRepository = subRepo;
            commCountRepository = commRepo;

        }

        public void CreateOrgCountData(int orgId)
        {
            SubCount newSubs = new SubCount();
            newSubs.OrgId = orgId;
            newSubs.count = CountXML<SubCountCol>.DATAtoXML(new SubCountCol()).ToString();

            subCountRepository.Add(newSubs);

            ViewCount newVCount = new ViewCount();
            newVCount.OrgId = orgId;
            newVCount.IssueId = 0;
            newVCount.count = CountXML<ViewCountCol>.DATAtoXML(new ViewCountCol()).ToString();

            viewCountRepository.Add(newVCount);

            SaveOrgCountData();
        }

        public void CreateIssueCountData(string userId, int orgId, int issueId)
        {
            //Create Vote count table entry

            VoteCount newVote = new VoteCount();
            newVote.IssueId = issueId;
            newVote.OrgId = orgId;

            VoteCountCol newVoteCol = new VoteCountCol();
            newVoteCol.Update();
            newVote.count = CountXML<VoteCountCol>.DATAtoXML(newVoteCol).ToString();

            voteCountRepository.Add(newVote);

            ////Create Comment count table entry
            CommentCount newCom = new CommentCount();
            newCom.IssueId = issueId;
            newCom.OrgId = orgId;
            newCom.count = CountXML<CommentCountCol>.DATAtoXML(new CommentCountCol()).ToString();

            commCountRepository.Add(newCom);

            //Create View count table entry
            ViewCount newView = new ViewCount();
            newView.IssueId = issueId;
            newView.OrgId = orgId;

            ViewCountCol newViewCol = new ViewCountCol();
            newViewCol.Update();
            newView.count = CountXML<ViewCountCol>.DATAtoXML(newViewCol).ToString();

            viewCountRepository.Add(newView);

            SaveIssueCountData();

        }

        public void SaveOrgCountData()
        {
            unitOfWork.Commit();
        }

        public void SaveIssueCountData()
        {
            unitOfWork.Commit();
        }

        public void UpdateIssueViewCountData(Issue issue)
        {
            ViewCount viewC = viewCountRepository.GetIssueViewCount(issue.Category.OrgId, issue.Id);

            ViewCountCol viewCol = CountXML<ViewCountCol>.XMLtoDATA(XElement.Parse(viewC.count));
            viewCol.Update();
            viewC.count = CountXML<ViewCountCol>.DATAtoXML(viewCol).ToString();

            viewCountRepository.Update(viewC);
        }

        public void UpdateIssueVoteCountData(Issue issue, bool upVote = true)
        {
            VoteCount voteC = voteCountRepository.GetIssueVoteCount(issue.Category.OrgId, issue.Id);

            VoteCountCol voteCol = CountXML<VoteCountCol>.XMLtoDATA(XElement.Parse(voteC.count));

            if (upVote)
            {
                voteCol.Update();
            }
            else
            {
                voteCol.Remove_Vote();
            }

            voteC.count = CountXML<VoteCountCol>.DATAtoXML(voteCol).ToString();

            voteCountRepository.Update(voteC);
        }

        public void SaveIssueDataChanges(CountDataType type)
        {
            unitOfWork.Commit();
        }

        public void UpdateOrgSubscriptionCount(int orgId, bool subed = true)
        {
            SubCount subC = subCountRepository.GetOrgsSubscriptionCount(orgId);

            SubCountCol subCol = CountXML<SubCountCol>.XMLtoDATA(XElement.Parse(subC.count));

            if (subed)
            {
                subCol.Update();
            }
            else
            {
                subCol.Remove_Sub();
            }

            subC.count = CountXML<SubCountCol>.DATAtoXML(subCol).ToString();

            subCountRepository.Update(subC);

        }

        public void SaveOrgDataChanges()
        {
            unitOfWork.Commit();
        }



        public ViewCountCol GetIssueViewCountCol(int orgId, int issueId)
        {
            ViewCount viewC = viewCountRepository.GetIssueViewCount(orgId, issueId);

            return CountXML<ViewCountCol>.XMLtoDATA(XElement.Parse(viewC.count));

        }

        public VoteCountCol GetIssueVoteCountCol(int orgId, int issueId)
        {
            VoteCount voteC = voteCountRepository.GetIssueVoteCount(orgId, issueId);

            return CountXML<VoteCountCol>.XMLtoDATA(XElement.Parse(voteC.count));
        }

        public CommentCountCol GetIssueCommentCountCol(int orgId, int issueId)
        {
            CommentCount comC = commCountRepository.GetIssueCommentCount(orgId, issueId);

            return CountXML<CommentCountCol>.XMLtoDATA(XElement.Parse(comC.count));
        }

        public IEnumerable<ViewCountCol> GetOrgViewCountCols(int orgId)
        {
            var viewC = viewCountRepository.GetOrgsViewCounts(orgId);

            IEnumerable<ViewCountCol> orgViewsCol = viewC.Select(vc => CountXML<ViewCountCol>.XMLtoDATA(XElement.Parse(vc.count)));

            return orgViewsCol;
        }

        public IEnumerable<VoteCountCol> GetOrgVoteCountCols(int orgId)
        {
            var voteC = voteCountRepository.GetOrgsVoteCounts(orgId);

            IEnumerable<VoteCountCol> orgVotesCol = voteC.Select(vc => CountXML<VoteCountCol>.XMLtoDATA(XElement.Parse(vc.count)));
            return orgVotesCol;
        }

        public SubCountCol GetOrgSubscriptionCountCol(int orgId)
        {
            return CountXML<SubCountCol>.XMLtoDATA(XElement.Parse(subCountRepository.GetOrgsSubscriptionCount(orgId).count));
        }

        public IEnumerable<CommentCountCol> GetOrgCommentCountCols(int orgId)
        {
            var commC = commCountRepository.GetOrgsCommentCounts(orgId);

            IEnumerable<CommentCountCol> orgCommCol = commC.Select(cc => CountXML<CommentCountCol>.XMLtoDATA(XElement.Parse(cc.count)));

            return orgCommCol;
        }

        public void SaveIssueDataChanges()
        {
            unitOfWork.Commit();
        }

        public SubCountCol GetOrgSubscriptionCount(int orgId)
        {
            var sub = subCountRepository.GetOrgsSubscriptionCount(orgId);
            return CountXML<SubCountCol>.XMLtoDATA(XElement.Parse(sub.count));
        }

        public void SaveCountChanges(CountCol count, CountDataType t, int orgId)
        {
            switch (t)
            {
                case CountDataType.Subscription:
                    {
                        var col = count as SubCountCol;
                        var commC = subCountRepository.GetOrgsSubscriptionCount(orgId);
                        commC.count = CountXML<SubCountCol>.DATAtoXML(col).ToString();
                        subCountRepository.Update(commC);
                        break;
                    }
                case CountDataType.View:
                    {
                        var col = count as ViewCountCol;
                        var viewC = viewCountRepository.GetOrgsViewCount(orgId);
                        viewC.count = CountXML<ViewCountCol>.DATAtoXML(col).ToString();
                        viewCountRepository.Update(viewC);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        public void SaveCountChanges(CountCol count, CountDataType t, int orgid, int issueId)
        {
            switch (t)
            {
                case CountDataType.Vote:
                    {
                        var col = count as SubCountCol;
                        var voteC = voteCountRepository.GetIssueVoteCount(orgid, issueId);
                        voteC.count = CountXML<SubCountCol>.DATAtoXML(col).ToString();
                        voteCountRepository.Update(voteC);
                        break;
                    }
                case CountDataType.View:
                    {
                        var col = count as ViewCountCol;
                        var viewC = viewCountRepository.GetIssueViewCount(orgid, issueId);
                        viewC.count = CountXML<ViewCountCol>.DATAtoXML(col).ToString();
                        viewCountRepository.Update(viewC);
                        break;
                    }
                case CountDataType.Comment:
                    {
                        var col = count as CommentCountCol;
                        var comC = commCountRepository.GetIssueCommentCount(orgid, issueId);
                        comC.count = CountXML<CommentCountCol>.DATAtoXML(col).ToString();
                        commCountRepository.Update(comC);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

    }
}