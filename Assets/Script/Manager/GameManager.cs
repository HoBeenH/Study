using System;
using Script.Base.MonoSingleTone;
using Script.TableParser;
using Unity.Mathematics;
using UnityEngine;

namespace Script.Manager
{
    public class GameManager : MonoSingleTone<GameManager>
    {
        public Grid m_Grid;
        private void Awake()
        {
            Init();
            m_Grid = FindObjectOfType<Grid>();
        }

        protected override void OnInit()
        {
            ResourceManager.Instance.Init();
            TableManager.Instance.Init();
        }

        protected override void OnClose()
        {
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                var _pos = SnapCoordinateToGrid(Vector3.zero);
                var _go = ResourceManager.Instance.LoadGOSync(
                    "Assets/Addressables/Prefabs/Buildings/Building_1A.prefab");
                _go.transform.position = _pos;
                _go.transform.rotation = Quaternion.identity;

                _go.AddComponent<ObjDrag>();
            }
        }

        public static Vector3 GetMouseWorldPos()
        {
            var _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(_ray, out var _hit))
            {
                return _hit.point;
            }

            return Vector3.zero;
        }

        public Vector3 SnapCoordinateToGrid(Vector3 pos)
        {
            var _cellPos = m_Grid.WorldToCell(pos);
            pos = m_Grid.GetCellCenterWorld(_cellPos);
            return pos;
        }
    }
}
