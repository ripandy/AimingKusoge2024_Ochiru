using Soar.Commands;
using UnityEngine;
using UnityEngine.Networking;

namespace Feature.Twitter
{
    [CreateAssetMenu(fileName = "TweetHandler", menuName = "Kusoge/TweetHandler")]
    public class TweetCommand : Command<int>
    {
        [SerializeField] private string tweetText;
        [SerializeField] private string tweetUrl;
        [SerializeField] private string[] tweetHashtags;

        private const string BaseUrl = "https://twitter.com/intent/tweet?=";
        private const string AppNameWildcard = "[AppName]";
        private const string ScoreWildcard = "[Score]";

        public override void Execute(int scoreValue)
        {
            var appName = Application.productName;
            var textParam = FormatText(tweetText, appName, scoreValue);
            var urlParam = string.IsNullOrEmpty(tweetUrl) ? "" : $"&url={UnityWebRequest.EscapeURL(tweetUrl)}";
            var hashtagString = "";
            for (var i = 0; i < tweetHashtags.Length; i++)
            {
                hashtagString += $"{(i == 0 ? "" : ",")}{tweetHashtags[i]}";
            }

            var hashtagParam = string.IsNullOrEmpty(hashtagString)
                ? ""
                : $"&hashtags={UnityWebRequest.EscapeURL(hashtagString)}";
            var url = $"{BaseUrl}{urlParam}{textParam}{hashtagParam}";
            Debug.Log($"Attempt tweet App {appName} with score value of {scoreValue}");
            Debug.Log($"{BaseUrl}{urlParam}{textParam}{hashtagParam}");
            Debug.Log($"{url}");
            Application.OpenURL(url);
        }

        private string FormatText(string originalText, string appName, int scoreValue)
        {
            if (string.IsNullOrEmpty(originalText)) return "";
            
            var text = originalText;
            text = text.Replace(AppNameWildcard, $"{appName}");
            text = text.Replace(ScoreWildcard, $"{scoreValue}");
            text += "\n\n";
            return $"&text={UnityWebRequest.EscapeURL(text)}";
        }
    }
}