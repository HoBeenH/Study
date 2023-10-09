using System;
using Script.Manager.ResourceMgr;
using Script.Manager.TableMgr;
using Script.Obj;
using Script.Obj.Build;
using Script.Parameter.Enum;
using Script.TableParser;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Script.Map.Parameters
{
    [System.Serializable]
    public class BuildSlot
    {
        [field: SerializeField] public int SlotID { get; private set; } = -1;
        [field: SerializeField] public EBuildState State { get; private set; } = EBuildState.Empty;
        [field: SerializeField] public Transform Root { get; private set; } = null;
        
        [field: SerializeField] public BuildingModelBase BuildingModelObject { get; private set; } = null;
        [field: SerializeField] public BuildPoint BuildPoint { get; private set; } = null;
        
        public bool IsBuild => this.State is not EBuildState.Empty;

        public void Init(int id, BuildingModelBase obj, Transform tr)
        {
            this.SlotID = id;
            this.BuildingModelObject = obj;

            if (Root == null)
            {
                Root = new GameObject("Root").transform;
                Root.SetParent(tr);
                Root.localPosition = Vector3.zero;
            }
            if (BuildPoint == null)
            {
                BuildPoint = tr.GetComponent<BuildPoint>();
                if (BuildPoint == null)
                {
                    var _buildPointData = TableManager.Instance.GetTable<AddressableTable>()
                        .GetData(EAddressableID.BuildPoint);
                    ResourceManager.LoadCompAsync<BuildPoint>(_buildPointData.Path, _point =>
                    {
                        BuildingModelObject = _point;
                    }, new InstantiationParameters(Root, false));
                }
            }
            
            SetState(EBuildState.Empty);
        }

        public void Clear()
        {
            this.SlotID = -1;
            this.State = EBuildState.Empty;
            if (BuildingModelObject != null)
            {
                BuildingModelObject.Clear();
                ResourceManager.ReleaseGO(BuildingModelObject);
                BuildingModelObject = null;
            }
            
            BuildPoint.Clear();
            ResourceManager.ReleaseGO(BuildPoint);
        }

        private void SetState(EBuildState state)
        {
            switch (state)
            {
                case EBuildState.Empty:
                    if (BuildingModelObject != null)
                    {
                        ResourceManager.ReleaseGO(BuildingModelObject);
                        BuildingModelObject = null;
                    }
                    BuildPoint.gameObject.SetActive(true);
                    break;
                case EBuildState.Idle:
                    break;
                case EBuildState.Construct:
                    break;
                case EBuildState.Upgrade:
                    break;
                case EBuildState.Destroy:
                    break;
            }
        }
    }
}
