using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaneponPanel : MonoBehaviour
{
    #region 変数
    //メインのMeshRenderer
    [SerializeField] private MeshRenderer _meshRenderer = null;

    public MeshRenderer meshRenderer { get { return _meshRenderer; } set { _meshRenderer = value; } }

    //パネルの状態
    private PaneponSystem.PanelState _state = PaneponSystem.PanelState.Stable;

    //移動元の位置
    int _posX = 0;
    int _posY = 0;
    
    //移動先の位置
    int _moveDestX = 0;
    int _moveDestY = 0;

    //移動割合
    float _moveRatio = 0f;

    //入れ替えにかかる時間
    const float SWAP_TIME = 4.0f / 60.0f;

    #endregion
    void Start()
    {
        
    }


    void Update()
    {
        if(_state == PaneponSystem.PanelState.Swap)
        {
            _moveRatio += Time.deltaTime / SWAP_TIME;
            if(_moveRatio >= 1.0f)
            {
                //@todo 移動先の床が無かったら落ちる処理に
                _state = PaneponSystem.PanelState.Stable;
            }
        }
    }

    #region メソッド
    public void Swap(int destX, int destY)
    {
        _moveDestX = destX;
        _moveDestY = destY;
    }
    #endregion
}
