using TMPro;
using UnityEngine;

namespace Feature.AboutMenu
{
    public class CreditsInfoHandler : MonoBehaviour
    {
        [SerializeField] private CreditsInfoCollection creditsInfo;
        [SerializeField] private TMP_Text leftColumn;
        [SerializeField] private TMP_Text rightColumn;

        private void Start()
        {
            leftColumn.text = "";
            rightColumn.text = "";
            
            foreach (var info in creditsInfo)
            {
                leftColumn.text += $"{info.role}\n";
                rightColumn.text += $"{info.names[0]}\n";

                for (var i = 1; i < info.names.Length; i++)
                {
                    leftColumn.text += "\n";
                    rightColumn.text += $"{info.names[i]}\n";
                }

                leftColumn.text += "\n";
                rightColumn.text += "\n";
            }
            
            var rectTransform = GetComponent<RectTransform>();
            var size = rectTransform.sizeDelta;
            size.y = rightColumn.preferredHeight;
            rectTransform.sizeDelta = size;
        }
    }
}