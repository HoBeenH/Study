using UnityEngine;

namespace Script.Base.Pool
{
    public interface IPool
    {
        void Clear();
        void Release(Object obj);
    }
}