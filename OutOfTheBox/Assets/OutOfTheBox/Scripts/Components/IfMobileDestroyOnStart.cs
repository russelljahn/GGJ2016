using UnityEngine;

namespace Assets.OutOfTheBox.Scripts.Components
{
    public class IfMobileDestroyOnStart : MonoBehaviour {
        void Start ()
        {
            if (Application.isMobilePlatform)
            {
                Destroy(gameObject);
            }
        }
    }
}
