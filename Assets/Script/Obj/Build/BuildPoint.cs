using System;
using Script.Manager.ResourceMgr;
using Script.Manager.TableMgr;
using Script.Manager.UIMgr;
using Script.Parameter.Enum;
using Script.Parameter.Struct;
using Script.TableParser;
using Script.UI.Popup_MessageBox;
using UnityEngine;

namespace Script.Obj.Build
{
    public class BuildPoint : BuildingModelBase
    {
        [Header("# Point GameObject")]
        [SerializeField] protected GameObject m_Flag = null;

        protected override void OnClick()
        {
            UIManager.Instance.OpenMessageBox<Popup_MessageBox>(EAddressableID.Popup_MessageBox,
                box => 
                {
                    box.Show(EMessageBoxBtnFlag.OK_Cancel,
                        new MessageBoxParameter(TableManager.Instance.GetTableString("BuildConfirm", TableManager.Instance.GetTableString("ArcherTower"))),
                        Test_BuildBuilding, null);
                });
        }

        private void Test_BuildBuilding()
        {
            var _t = TableManager.Instance.GetTable<AddressableTable>().GetData(EAddressableID.ArcherTowerLV1);
            ResourceManager.LoadGOAsync(_t.Path, x =>
            {
                x.transform.SetParent(this.transform);
                x.transform.localPosition = new Vector3(0f, 0.5f, 0f);
                x.SetActive(true);
            });
        }

        protected override Transform GetTweenTarget() => m_Flag.transform;
    }
}