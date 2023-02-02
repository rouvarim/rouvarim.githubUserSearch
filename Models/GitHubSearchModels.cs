using Newtonsoft.Json;
using System.ComponentModel;
using System.Linq;

namespace CodeChallenge.Models
{
    public class GitHubSearchResults
    {
        [JsonProperty("total_count")]
        public int TotalCount { get; set; }
        [JsonProperty("items")]
        public List<GitHubUser> Users { get; set; }
        public int ResultsPerPage { get; set; }
        public int PageNumber { get; set; }
        public bool HasNextPage => (PageNumber * ResultsPerPage) < TotalCount;
        public bool HasPreviousPage => PageNumber > 1;

        public GitHubSearchResults()
        {
            Users = new List<GitHubUser>();
            ResultsPerPage = 5;
            PageNumber = 1;
        }
    }

    public class GitHubUser
    {
        [JsonProperty("login")]
        public string Login { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("html_url")]
        public string HtmlUrl { get; set; }
        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }
        public GitHubUserDetails UserDetails { get; set; }

        public GitHubUser()
        {
            UserDetails = new GitHubUserDetails();
        }
    }

    public class GitHubUserDetails
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue("")]
        public string Name { get; set; } = "";
        [JsonProperty("company", NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue("")]
        public string Company { get; set; } = "";

        [JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue("")]
        public string Location { get; set; } = "";
        [JsonProperty("bio", NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue("")]
        public string Bio { get; set; } = "";
        [JsonProperty("followers")]
        public int FollowerCount { get; set; } = 0;
        [JsonProperty("following")]
        public int FollowingCount { get; set; } = 0;
        public int StarCount { get; set; } = 0;
    }
}
