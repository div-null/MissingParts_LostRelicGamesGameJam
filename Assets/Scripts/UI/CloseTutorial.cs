using UnityEngine;

namespace UI
{
    public class CloseTutorial : MonoBehaviour
    {
        public void CloseWindow()
        {
            transform.parent.gameObject.SetActive(false);
        } 
    }
}
