using CodeChallenge.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rouvari.CodeChallenge.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace Rouvari.CodeChallenge.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _linkHeaderRegexPattern = @"&page=([\d]+)>; rel=\""last\""";
        private readonly string _queryBaseUrl = "https://api.github.com/search/users?q=";
        private readonly string _queryBase = "type:user";
        private static readonly HttpClient _client = new HttpClient();

        private (string Content, HttpResponseMessage HttpRes) GetContFromReq(string baseUrl, string qryStr)
        {
            HttpRequestMessage ReqMes = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}{HttpUtility.UrlEncodeUnicode(qryStr)}");
            ReqMes.Headers.Add("User-Agent", "request");
            //Populate [token] value and uncomment the next line for authenticated requests, granting higher rate limits
            //ReqMes.Headers.Add("Authorization", "Bearer [token]");

            using HttpResponseMessage RespMes = _client.Send(ReqMes);
            RespMes.EnsureSuccessStatusCode();
            return (RespMes.Content.ReadAsStringAsync().Result, RespMes);
        }

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string Content = GetContFromReq(_queryBaseUrl, _queryBase).Content;
            JObject JsonRes = JObject.Parse(Content);
            ViewBag.userCount = JsonRes.Value<int>("total_count");
            return View();
        }

        public IActionResult ExecuteSearch(string searchText)
        {
            string Content = GetContFromReq(_queryBaseUrl, $"{_queryBase} {searchText}").Content;
            GitHubSearchResults SearchResults = JsonConvert.DeserializeObject<GitHubSearchResults>(Content);
            foreach(GitHubUser user in SearchResults.Users)
            {
                string UserContent = GetContFromReq(user.Url, "").Content;
                user.UserDetails = JsonConvert.DeserializeObject<GitHubUserDetails>(UserContent);
                HttpResponseMessage StarredRespMes = GetContFromReq($"{user.Url}/starred?per_page=1", "").HttpRes;
                if (StarredRespMes.Headers.Any() && StarredRespMes.Headers.Contains("Link"))
                {
                    string LinkHeader = StarredRespMes.Headers.GetValues("Link").FirstOrDefault();
                    int StarCount = 0;
                    if (int.TryParse(Regex.Match(LinkHeader, _linkHeaderRegexPattern).Groups[1].Value, out StarCount))
                    {
                        user.UserDetails.StarCount = StarCount;
                    }
                }
            }
            HttpContext.Session.SetString("SearchResults", JsonConvert.SerializeObject(SearchResults));
            SearchResults.Users = SearchResults.Users.Take(SearchResults.ResultsPerPage).ToList();
            return PartialView("_SearchResultsPartial", SearchResults);
        }

        public IActionResult ChangeResultsPage(int newPageNumber)
        {
            GitHubSearchResults SearchResults = JsonConvert.DeserializeObject<GitHubSearchResults>(HttpContext.Session.GetString("SearchResults"));
            SearchResults.PageNumber = newPageNumber;
            HttpContext.Session.SetString("SearchResults", JsonConvert.SerializeObject(SearchResults));
            SearchResults.Users = SearchResults.Users.Skip((newPageNumber - 1) * SearchResults.ResultsPerPage).Take(SearchResults.ResultsPerPage).ToList();
            return PartialView("_SearchResultsPartial", SearchResults);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}