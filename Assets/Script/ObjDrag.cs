using System;
using Script.Manager;
using UnityEngine;

namespace Script
{
    public class ObjDrag : MonoBehaviour
    {
        private Vector3 m_OffSet;

        private void OnMouseDown()
        {
            m_OffSet = transform.position - GameManager.GetMouseWorldPos();
        }

        private void OnMouseDrag()
        {
            var _pos = GameManager.GetMouseWorldPos() + m_OffSet;
            _pos = GameManager.Instance.SnapCoordinateToGrid(_pos);
            transform.position = new Vector3(_pos.x, 0.5f, _pos.z);
            transform.localScale = Vector3.one / 10f;
        }
    }
}