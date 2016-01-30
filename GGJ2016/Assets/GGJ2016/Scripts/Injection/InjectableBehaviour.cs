using UnityEngine;
using Zenject;

namespace Sense.Injection
{
    public abstract class InjectableBehaviour : MonoBehaviour
    {
        [PostInject]
        private void OnEmitPostInject()
        {
            OnPostInject();
        }

        public void Destroy()
        {
            OnDestroyed();
            Destroy(gameObject);
        }

        protected virtual void OnPostInject()
        {

        }

        protected virtual void OnDestroyed()
        {

        }
    }
}
