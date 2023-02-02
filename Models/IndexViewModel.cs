using CodeChallenge.Models;

namespace Rouvari.CodeChallenge.Models
{
    public class IndexViewModel
    {
        public int UserCount { get; set; }

        public string? SearchInput { get; set; }

        public GitHubSearchResults? SearchResults { get; set; }
    }
}