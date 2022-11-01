using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaneponSystem : MonoBehaviour
{
    #region 変数
    const int FIELD_SIZE_X = 6;
    const int FIELD_SIZE_Y_BASE = 12;
    const int FIELD_SIZE_Y = 24;

    //パネルのプレハブ
    [SerializeField] private List<GameObject> _panelPrefabList = new List<GameObject>();

    //パネルの色
    enum PanelColor
    {
        Red,
        Blue,
        SkyBlue,
        Purple,
        Yellow,
        Green,

        Max
    };

    //パネルの状態
    enum PanelState
    {
        Stable,  //停止中
        Swap,    //入れ替え中
        Chain,   //連鎖中
        Fall,    //落下中

        Max
    };

    #endregion
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
