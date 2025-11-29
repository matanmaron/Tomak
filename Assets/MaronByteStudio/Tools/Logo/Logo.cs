using UnityEngine;
using UnityEngine.UI;

namespace MaronByteStudio.Othello
{
    public class Logo : MonoBehaviour
    {
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => Application.OpenURL("https://matanmaron.wixsite.com/home/maronbytestudio"));
        }
    }
}
