using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class OpenHyperlinks : MonoBehaviour, IPointerClickHandler {
        [SerializeField]
        private TMP_Text TMP_text;

        public void OnPointerClick(PointerEventData eventData) {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(TMP_text, Input.mousePosition, Camera.current);
            if( linkIndex != -1 ) { // was a link clicked?
                TMP_LinkInfo linkInfo = TMP_text.textInfo.linkInfo[linkIndex];

                // open the link id as a url, which is the metadata we added in the text field
                Application.OpenURL(linkInfo.GetLinkID());
            }
        }
    }
}