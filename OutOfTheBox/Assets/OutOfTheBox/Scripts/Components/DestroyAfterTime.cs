using Assets.OutOfTheBox.Scripts.Utils;
using Assets.OutOfTheBox.Scripts.Extensions;
using Sense.Extensions;
using Sense.PropertyAttributes;
using UnityEngine;

namespace Assets.OutOfTheBox.Scripts
{
    public class DestroyAfterTime : MonoBehaviour
    {
        public bool ShouldDestroy = false;
        public float TimeUntilDestroy = 5.0f;

        private void Start()
        {
            this.InvokeAfterTime(TimeUntilDestroy, () => {
                if (ShouldDestroy) {
                    Destroy(gameObject);
                }
            });
        }

    }
}
