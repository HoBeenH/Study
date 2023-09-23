using Script.Manager.ResourceMgr;
using UnityEngine;
using UnityEngine.Pool;

namespace Script.Base.Pool
{
    public abstract class PoolMonoObj : MonoBehaviour
    {
        private IPool ReleaseHandle;
        
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

            if (gameObject.transform.parent != ResourceManager.GetPoolRoot())
                gameObject.transform.SetParent(ResourceManager.GetPoolRoot());
            
            OnClear();
        }

        public void SetHandler(IPool pool) => ReleaseHandle = pool;

        public void Release()
        {
            OnRelease();
            ReleaseHandle.Release(this);
        }
        
        protected virtual void OnRelease(){}
        protected abstract void OnGet();
        protected abstract void OnClear();
    }
}