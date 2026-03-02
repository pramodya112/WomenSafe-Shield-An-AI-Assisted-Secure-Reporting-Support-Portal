using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace WomenEmpower.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FakeProfileController : ControllerBase
    {
        // POST api/FakeProfile/analyze
        [HttpPost("analyze")]
        public IActionResult Analyze([FromBody] AnalyzeRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.ProfileUrl))
                return BadRequest("profileUrl is required.");

            var url = req.ProfileUrl.Trim().ToLower();
            var result = AnalyzeUrl(url);
            return Ok(result);
        }

        private static ProfileResult AnalyzeUrl(string url)
        {
            var redFlags = new List<string>();
            int score = 0;
            var platform = DetectPlatform(url);
            var username = ExtractUsername(url, platform);

            // ── Rule 1: Unknown / suspicious domain ──────────────────
            var knownDomains = new[] {
                "instagram.com", "facebook.com", "twitter.com", "x.com",
                "tiktok.com", "linkedin.com", "youtube.com", "snapchat.com",
                "pinterest.com", "reddit.com", "telegram.me", "t.me"
            };
            if (!knownDomains.Any(d => url.Contains(d)))
            {
                redFlags.Add("Unknown or unrecognized platform domain");
                score += 35;
            }

            // ── Rule 2: Suspicious username patterns ──────────────────
            if (!string.IsNullOrEmpty(username))
            {
                // Ends with year (e.g. _2024, .2023)
                if (Regex.IsMatch(username, @"(19|20)\d{2}$"))
                {
                    redFlags.Add("Username ends with a year — common in fake/cloned accounts");
                    score += 15;
                }

                // Excessive numbers (more than 4 digits)
                var digitCount = username.Count(char.IsDigit);
                if (digitCount > 4)
                {
                    redFlags.Add($"Username contains {digitCount} digits — may be auto-generated");
                    score += 20;
                }

                // Excessive underscores or dots
                var specialCount = username.Count(c => c == '_' || c == '.');
                if (specialCount >= 3)
                {
                    redFlags.Add("Username has many underscores/dots — pattern seen in fake accounts");
                    score += 15;
                }

                // Very long username
                if (username.Length > 25)
                {
                    redFlags.Add("Unusually long username");
                    score += 10;
                }

                // Random-looking string (low vowel ratio)
                var letters = username.Where(char.IsLetter).ToList();
                if (letters.Count > 5)
                {
                    var vowels = letters.Count(c => "aeiou".Contains(c));
                    var vowelRatio = (double)vowels / letters.Count;
                    if (vowelRatio < 0.15)
                    {
                        redFlags.Add("Username appears randomly generated (few vowels)");
                        score += 20;
                    }
                }

                // Common fake suffixes
                var fakeSuffixes = new[] { "_real", "_official", "_orig", "_actual", "real_", "official_" };
                if (fakeSuffixes.Any(s => username.Contains(s)))
                {
                    redFlags.Add("Uses 'real' or 'official' in username — common impersonation tactic");
                    score += 25;
                }

                // Repeated characters (e.g. aaaa, 1111)
                if (Regex.IsMatch(username, @"(.)\1{3,}"))
                {
                    redFlags.Add("Username contains repeated characters");
                    score += 10;
                }
            }

            // ── Rule 3: URL has query params (e.g. Facebook numeric ID) ──
            if (url.Contains("profile.php?id="))
            {
                redFlags.Add("Uses numeric profile ID instead of a named profile");
                score += 20;
            }

            // ── Rule 4: HTTP instead of HTTPS ────────────────────────
            if (url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                redFlags.Add("URL uses HTTP instead of HTTPS");
                score += 10;
            }

            // ── Rule 5: Subdomain tricks (e.g. instagram.com.fake.net) ──
            if (Regex.IsMatch(url, @"(instagram|facebook|twitter|tiktok|linkedin)\.com\.[a-z]+"))
            {
                redFlags.Add("Domain spoofing detected — URL mimics a real platform");
                score += 40;
            }

            // Cap score at 100
            score = Math.Min(score, 100);

            // ── Determine risk level ──────────────────────────────────
            string riskLevel = score switch
            {
                < 20 => "Low",
                < 50 => "Medium",
                < 75 => "High",
                _ => "Critical"
            };

            // ── Build explanation ─────────────────────────────────────
            string explanation = riskLevel switch
            {
                "Low" => $"This {platform} profile URL appears legitimate. No major red flags were detected in the URL structure or username pattern.",
                "Medium" => $"This {platform} profile has some suspicious patterns that warrant caution. Verify this profile through other means before engaging.",
                "High" => $"This {platform} profile shows multiple red flags commonly associated with fake or impersonation accounts. Proceed with extreme caution.",
                "Critical" => $"This URL shows clear signs of a fake or malicious profile. It may be attempting to impersonate a real platform or person.",
                _ => "Unable to determine risk level."
            };

            string recommendedAction = riskLevel switch
            {
                "Low" => "Appears Safe",
                "Medium" => "Exercise Caution",
                "High" => "Avoid Engaging",
                "Critical" => "Block and Report",
                _ => "Use Caution"
            };

            string actionDetail = riskLevel switch
            {
                "Low" => "This profile URL looks normal. Still verify the person's identity through a trusted channel before sharing personal information.",
                "Medium" => "Do not share personal information. Try to verify this person's identity through mutual contacts or official channels.",
                "High" => "Do not interact with this profile. Block the account and report it to the platform immediately.",
                "Critical" => "This is likely a fake or phishing profile. Do not click any links, share any information, or engage in any way. Report immediately.",
                _ => "Be cautious when interacting with this profile."
            };

            return new ProfileResult
            {
                Platform = platform,
                Username = username,
                RiskLevel = riskLevel,
                RiskScore = score,
                Explanation = explanation,
                RedFlags = redFlags,
                RecommendedAction = recommendedAction,
                ActionDetail = actionDetail
            };
        }

        private static string DetectPlatform(string url)
        {
            if (url.Contains("instagram.com")) return "Instagram";
            if (url.Contains("facebook.com")) return "Facebook";
            if (url.Contains("twitter.com")) return "Twitter/X";
            if (url.Contains("x.com")) return "Twitter/X";
            if (url.Contains("tiktok.com")) return "TikTok";
            if (url.Contains("linkedin.com")) return "LinkedIn";
            if (url.Contains("youtube.com")) return "YouTube";
            if (url.Contains("snapchat.com")) return "Snapchat";
            if (url.Contains("pinterest.com")) return "Pinterest";
            if (url.Contains("reddit.com")) return "Reddit";
            if (url.Contains("t.me") || url.Contains("telegram.me")) return "Telegram";
            return "Unknown Platform";
        }

        private static string ExtractUsername(string url, string platform)
        {
            try
            {
                // Remove protocol and domain
                var path = Regex.Replace(url, @"https?://(www\.)?[^/]+/", "");

                // Remove query strings and fragments
                path = Regex.Split(path, @"[\?#]")[0].Trim('/');

                // For Facebook numeric IDs
                if (url.Contains("profile.php?id="))
                {
                    var match = Regex.Match(url, @"id=(\d+)");
                    return match.Success ? match.Groups[1].Value : path;
                }

                // Take first path segment as username
                var segments = path.Split('/');
                return segments.FirstOrDefault(s => !string.IsNullOrEmpty(s)) ?? path;
            }
            catch
            {
                return "";
            }
        }
    }

    public class AnalyzeRequest
    {
        public string ProfileUrl { get; set; } = string.Empty;
    }

    public class ProfileResult
    {
        public string Platform { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string RiskLevel { get; set; } = string.Empty;
        public int RiskScore { get; set; }
        public string Explanation { get; set; } = string.Empty;
        public List<string> RedFlags { get; set; } = new();
        public string RecommendedAction { get; set; } = string.Empty;
        public string ActionDetail { get; set; } = string.Empty;
    }
}