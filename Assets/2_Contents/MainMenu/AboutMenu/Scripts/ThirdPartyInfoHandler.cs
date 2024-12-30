using System.Text;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Feature.AboutMenu
{
    [RequireComponent(typeof(TMP_Text))]
    public class ThirdPartyInfoHandler : MonoBehaviour
    {
        [SerializeField] private ThirdPartyInfoCollection thirdPartyInfo;

        private readonly StringBuilder sb = new StringBuilder();
        private TMP_Text text;
        
        private void Awake()
        {
            TryGetComponent(out text);
        }

        private void Start()
        {
            sb.Clear();
            foreach (var tpInfo in thirdPartyInfo)
            {
                sb.AppendLine("====================");
                sb.AppendLine("<b>");
                sb.AppendLine($"{ProcessString(nameof(tpInfo.componentName))}: {tpInfo.componentName}");
                sb.AppendLine($"{ProcessString(nameof(tpInfo.licenseType))}: {tpInfo.licenseType}");
                sb.AppendLine("</b>");
                sb.AppendLine($"{tpInfo.licenseText}");
                sb.AppendLine();
            }

            text.text = sb.ToString();
            
            CalculateSize().Forget();
        }

        private async UniTaskVoid CalculateSize()
        {
            await UniTask.NextFrame(this.GetCancellationTokenOnDestroy());
            var rectTransform = GetComponent<RectTransform>();
            var size = rectTransform.sizeDelta;
            size.y = text.preferredHeight;
            rectTransform.sizeDelta = size;
        }

        private string ProcessString(string original)
        {
            var toUpper = $"{char.ToUpper(original[0])}{original.Substring(1)}";
            return Regex.Replace(toUpper, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
        }
    }
}