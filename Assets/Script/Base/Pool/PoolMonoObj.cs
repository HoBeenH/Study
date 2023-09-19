using UnityEngine;

namespace Script.Base.Pool
{
    public abstract class PoolMonoObj : MonoBehaviour
    {
        public void Get()
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            OnGet();
        }

        public void Clear()
        {
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
            
            OnClear();
        }
        
        protected abstract void OnGet();
        protected abstract void OnClear();
    }
}