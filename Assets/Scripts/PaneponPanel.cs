﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// パネル一つ一つの状態を管理するクラス
/// </summary>
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
    private PaneponSystem.PanelState _panel_State = PaneponSystem.PanelState.Stable;
    public PaneponSystem.PanelState panel_State { get { return _panel_State; } }

    //パネルの状態が入れ替え可能かどうか
    public bool _isSwapSble { get { return (_panel_State == PaneponSystem.PanelState.None || 
                                            _panel_State == PaneponSystem.PanelState.Stable); } }

    //パネルの色
    private PaneponSystem.PanelColor _color = PaneponSystem.PanelColor.Max;
    public PaneponSystem.PanelColor color { get { return _color; } }

    //連鎖対象フラグ
    private bool _isChainTarget = false;
    public bool isCahainTarget { get { return _isChainTarget; } set { _isChainTarget = value; } }

    //移動元の位置
    private int _posX = 0;
    private int _posY = 0;
    
    //移動先の位置
    private int _moveDestX = 0;
    private int _moveDestY = 0;

    //移動割合
    private float _moveRatio = 0f;

    //パネルの落下速度
    private float _fallSpeed = 10f;

    //ステート経過時間
    private float _stateTimer = 0f;

    //入れ替えにかかる時間(4フレーム)
    const float SWAP_TIME = 4.0f / 60.0f;

    //落下の最大速度(1秒間に落下するマスの数)
    const float MAX_FALL_SPEED = 60.0f / 4.0f;

    //発光時間(1秒)
    const float FLASH_TIME = 1f;

    //エフェクトが飛び散るスピード
    const float EFFECT_SPEED = 1.1f;

    //消滅時間(0.1秒)
    const float ERASE_TIME = 0.1f;

    //割合の定数
    const float RATIO_ONE = 1f;

    //移動の定数
    const int MOVE_ONE = 1;

    //点滅演出の定数
    const int FLASH_TWO = 2;

    //システムへの参照
    private PaneponSystem _system = null;

    #endregion
    void Start()
    {
        //ステートのデフォルトは静止状態
        _panel_State = PaneponSystem.PanelState.Stable;

        //デフォルトはエフェクトOFF
        SetEffectVisible(false);
    }

    /// <summary>
    /// PaneponSystamと同期したUpdateメソッド
    /// </summary>
    public void  ManualUpdate()
    {
        switch (_panel_State)
        {
            case PaneponSystem.PanelState.Swap:
                //割合を加算
                _moveRatio += Time.deltaTime / SWAP_TIME;

                //割合が1以上になったら終了
                if (_moveRatio >= RATIO_ONE)
                {
                    //位置を直接設定
                    SetPosition(_moveDestX, _moveDestY);
                    //全体のパネルの落下判定
                    _system.CheckAllPanels();
                }
                else
                {
                    //移動処理
                    transform.localPosition = new Vector3(_posX, _posY, 0f) * (MOVE_ONE - _moveRatio) + new Vector3(_moveDestX, _moveDestY, 0f) * _moveRatio;
                }
                break;
            case PaneponSystem.PanelState.Flash:
                //時間経過でエフェクトを点滅させる
                SetEffectVisible(_effectFlashCounter++ % FLASH_TWO == 0);
                //時間経過でEraseに移行
                _stateTimer += Time.deltaTime;
                if (_stateTimer >= FLASH_TIME)
                {
                    //ステート遷移 
                    _panel_State = PaneponSystem.PanelState.Erase;
                    _stateTimer = 0f;
                }
                break;
            case PaneponSystem.PanelState.Erase:
                //時間経過でエフェクトを四方八方に散らす
                SetEffectVisible(true);
                SetEffectDivision(0f);

                //時間経過でパネル完全消滅
                _stateTimer += Time.deltaTime;
                if (_stateTimer >= ERASE_TIME)
                {
                    //エフェクトも消す
                    SetEffectVisible(false);
                    //MeshRendererをOFF
                    _meshRenderer.enabled = false;
                    //ステート遷移 
                    _panel_State = PaneponSystem.PanelState.None;
                    _stateTimer = 0f;

                    //連鎖対象フラグを設定
                    _system.SetChainTarget(_posY, _posX);

                    //全体のパネルの落下判定
                    _system.CheckAllPanels();
                }
                break;
            case PaneponSystem.PanelState.Fall:
                //_fallSpeed = Mathf.Clamp(_fallSpeed + 0.5f, 0f, MAX_FALL_SPEED);速度がずれる
                _moveRatio += _fallSpeed * Time.deltaTime;

                //割合が1以上になったら下のマスに移動
                if (_moveRatio >= RATIO_ONE)
                {
                    //位置を直接設定
                    float moveRatioTmp = _moveRatio;
                    SetPosition(_moveDestX, _moveDestY);
                    if (_panel_State == PaneponSystem.PanelState.Fall)
                    {
                        _moveRatio = moveRatioTmp - RATIO_ONE;
                        //移動処理
                        transform.localPosition = new Vector3(_posX, _posY, 0f) * (RATIO_ONE - _moveRatio) +
                            new Vector3(_moveDestX, _moveDestY, 0f) * _moveRatio;
                    }
                }
                else
                {
                    //移動処理
                    transform.localPosition = new Vector3(_posX, _posY, 0f) * (RATIO_ONE - _moveRatio) +
                        new Vector3(_moveDestX, _moveDestY, 0f) * _moveRatio;
                }
                break;
        }
    }

    #region メソッド
    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Init(PaneponSystem system)
    {
        //システムへの参照を保持
        _system = system;
    }
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

        //移動先の床が無かったら落ちる処理に
        CheckToFall(false);
    }
    /// <summary>
    /// 落下判定
    /// </summary>
    public void CheckToFall(bool isOnlyCheckStable)
    {
        //安定状態でない場合は行わない
        if (isOnlyCheckStable && _panel_State != PaneponSystem.PanelState.Stable)
        {
            return;
        }
        //移動先の床が無い かつ 移動先より下に隙間があったら落ちる処理に
        PaneponSystem.PanelState panelState = _system.GetPanelState(_posX, _posY - MOVE_ONE);
        if (panelState == PaneponSystem.PanelState.None)// || panelState == PaneponSystem.PanelState.Fall
        {
            //落下処理
            Fall();
            return;
        }
        else
        {
            _panel_State = PaneponSystem.PanelState.Stable;
        }
        return;
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
        _panel_State = PaneponSystem.PanelState.Swap;
    }
    /// <summary>
    /// 落下後の位置を設定
    /// </summary>
    /// <param name="desetX"></param>
    /// <param name="desetY"></param>
    public void Fall()
    {
        //配列内での落下処理
        _system.FallPanel(_posX, _posY);

        //目的地は1個下のマス
        _moveDestX = _posX;
        _moveDestY = _posY - 1;

        //ステート遷移
        _panel_State = PaneponSystem.PanelState.Fall;
    }
    /// <summary>
    /// パネルの点滅演出を開始する
    /// </summary>
    public void StartFlash()//float flashTime
    {
        //Stableでない場合は行わない
        if(_panel_State != PaneponSystem.PanelState.Stable)
        {
            return;
        }

        //パネルの消滅にディレイを入れる場合の処理
        //FLASH_TIME = flashTime;

        //ステート遷移
        _panel_State = PaneponSystem.PanelState.Flash;
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
    /// <summary>
    /// エフェクトの動きの割合
    /// </summary>
    /// <param name="_ratio"></param>
    private void SetEffectDivision(float _ratio)
    {
        for (int i = 0; i < _effectObjectList.Count; i++)
        {
            _effectObjectList[i].transform.localPosition *= EFFECT_SPEED;
        }
    }
    /// <summary>
    /// 1マス分上に移動する
    /// </summary>
    public void CarryUp()
    {
        _posY++;
        _moveDestY++;

        transform.localPosition += Vector3.up;
    }
    #endregion
}
