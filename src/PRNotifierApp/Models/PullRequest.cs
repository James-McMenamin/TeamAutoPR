using System;
using System.Collections.Generic;

namespace PRNotifierApp.Models
{
    public class PullRequest
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Url { get; set; }
        public required string Status { get; set; }
        public bool HasComments { get; set; }
        public bool HasPendingComments { get; set; }
        public required List<string> WorkItemIds { get; set; }
        public required List<Reviewer> Reviewers { get; set; }
    }

    public class Reviewer
    {
        public required string DisplayName { get; set; }
        public required string AvatarUrl { get; set; }
        public required string VoteStatus { get; set; }
    }
}