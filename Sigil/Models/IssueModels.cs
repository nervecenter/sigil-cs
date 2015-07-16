using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Sigil.Models
{
    public class IssueModels
    {
        public int Id { get; set; }
        public string OrgId { get; set; }
        public string UserId { get; set; }
        public DateTime createTime { get; set; }
        public DateTime editTime { get; set; }
        public string title { get; set; }
        public string text { get; set; }
        public bool responded { get; set; }
        public int officialComment { get; set; }
        public Int64 votes { get; set; }
        public DateTime lastVoted { get; set; }
    }
}