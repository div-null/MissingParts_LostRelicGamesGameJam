using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Game.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class OpenHyperlinks : MonoBehaviour, IPointerClickHandler {
        [SerializeField]
        private TMP_Text TMP_text;

        public void OnPointerClick(PointerEventData eventData)
        {
            Vector3 mousePosition2 = Mouse.current.position.ReadValue();
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(TMP_text, mousePosition2, null);
            if( linkIndex != -1 ) { // was a link clicked?
                TMP_LinkInfo linkInfo = TMP_text.textInfo.linkInfo[linkIndex];

                // open the link id as a url, which is the metadata we added in the text field
                Application.OpenURL(linkInfo.GetLinkID());
            }
        }
    }
}