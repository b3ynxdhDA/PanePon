using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaneponPanel : MonoBehaviour
{
    #region 変数
    //エフェクト用のオブジェクトのリスト
    [SerializeField] private List<GameObject> _effectObjectList = new List<GameObject>();

    //エフェクト点滅用の変数
    private int _effectFlashCounter = 0;

    //メインのMeshRenderer
    [SerializeField] private MeshRenderer _meshRenderer = null;

    public MeshRenderer meshRenderer { get { return _meshRenderer; } set { _meshRenderer = value; } }

    //パネルの状態
    private PaneponSystem.PanelState _state = PaneponSystem.PanelState.Stable;
    public PaneponSystem.PanelState state { get { return _state; } }

    //パネルの色
    private PaneponSystem.PanelColor _color = PaneponSystem.PanelColor.Max;
    public PaneponSystem.PanelColor color { get { return _color; } }

    //移動元の位置
    int _posX = 0;
    int _posY = 0;
    
    //移動先の位置
    int _moveDestX = 0;
    int _moveDestY = 0;

    //移動割合
    float _moveRatio = 0f;

    //ステート経過時間
    float _stateTimer = 0f;

    //入れ替えにかかる時間(4フレーム)
    const float SWAP_TIME = 4.0f / 60.0f;

    //発光時間(1秒)
    const float FLASH_TIME = 1f;

    //消滅時間(0.1秒)
    const float ERASE_TIME = 0.1f;

    #endregion
    void Start()
    {
        //ステートのデフォルトは静止状態
        _state = PaneponSystem.PanelState.Stable;

        //デフォルトはエフェクトOFF
        SetEffectVisible(false);
    }


    void Update()
    {
        switch (_state)
        {
            case PaneponSystem.PanelState.Swap:
                //割合を加算
                _moveRatio += Time.deltaTime / SWAP_TIME;

                //割合が1以上になったら終了
                if (_moveRatio >= 1.0f)
                {
                    //位置を直接設定
                    SetPosition(_moveDestX, _moveDestY);
                }
                else
                {
                    //移動処理
                    transform.localPosition = new Vector3(_posX, _posY, 0f) * (1f - _moveRatio) + new Vector3(_moveDestX, _moveDestY, 0f) * _moveRatio;
                }
                break;
            case PaneponSystem.PanelState.Flash:
                //時間経過でエフェクトを点滅させる
                SetEffectVisible((_effectFlashCounter++ / 2) % 2 == 0);
                //時間経過でEraseに移行
                _stateTimer += Time.deltaTime;
                if(_stateTimer >= FLASH_TIME)
                {
                    //ステート遷移 
                    _state = PaneponSystem.PanelState.Erase;
                    _stateTimer = 0f;
                }
                break;
            case PaneponSystem.PanelState.Erase:
                //時間経過でエフェクトを四方八方に散らす
                SetEffectVisible(true);
                SetEffectDivision(0f);

                //時間経過でパネル消滅
                _stateTimer += Time.deltaTime;
                if (_stateTimer >= ERASE_TIME)
                {
                    //エフェクトも消す
                    SetEffectVisible(false);
                    //MeshRendererをOFF
                    _meshRenderer.enabled = false;
                    //ステート遷移 
                    _state = PaneponSystem.PanelState.None;
                    _stateTimer = 0f;
                }
                break;
        }
    }

    #region メソッド
    /// <summary>
    /// パネル位置を直接設定
    /// </summary>
    /// <param name="destX"></param>
    /// <param name="destY"></param>
    public void SetPosition(int destX, int destY)
    {
        _posX = destX;
        _posY = destY;
        _moveDestX = destX;
        _moveDestY = destY;
        _moveRatio = 0f;

        //移動処理
        transform.localPosition = new Vector3(_posX, _posY, 0f);

        //@todo 移動先の床が無かったら落ちる処理に
        _state = PaneponSystem.PanelState.Stable;
    }
    /// <summary>
    /// 色を設定
    /// </summary>
    /// <param name="color"></param>
    public void SetColor(PaneponSystem.PanelColor color)
    {
        _color = color;
    }
    /// <summary>
    /// 移動後の位置を設定
    /// </summary>
    /// <param name="destX"></param>
    /// <param name="destY"></param>
    public void Swap(int destX, int destY)
    {
        _moveDestX = destX;
        _moveDestY = destY;

        //ステート遷移
        _state = PaneponSystem.PanelState.Swap;
    }
    /// <summary>
    /// ブロックを消す処理を開始する
    /// </summary>
    public void StartErase()
    {
        //Stableでない場合は行わない
        if(_state != PaneponSystem.PanelState.Stable)
        {
            return;
        }
        //ステート遷移
        _state = PaneponSystem.PanelState.Flash;

        //@test 仮処理
        print("フラッシュ");
    }
    /// <summary>
    /// エフェクトの可視性を設定
    /// </summary>
    /// <param name="flag"></param>
    private void SetEffectVisible(bool flag)
    {
        for (int i = 0; i < _effectObjectList.Count; i++)
        {
            _effectObjectList[i].SetActive(flag);
        }
    }
    private void SetEffectDivision(float _ratio)
    {
        for (int i = 0; i < _effectObjectList.Count; i++)
        {
            _effectObjectList[i].transform.localPosition *= 1.1f;   //@仮
        }
    }
    #endregion
}
