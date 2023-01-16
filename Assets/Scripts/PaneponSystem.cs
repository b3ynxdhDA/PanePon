using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// パネポンのフィールド全体を管理するクラス
/// </summary>
public class PaneponSystem : MonoBehaviour
{
    #region 変数
    //インプットシステム
    InputSystem _inputSystem;

    //PanePonUI   @スクリプトがMonoBehaviourを継承しているとnewが使えない
    [SerializeField] private PanePonUI _panePonUI = null;

    //GameManager@staticを使うと必要ない
    //GameManager gameManager = null;

    const int FIELD_SIZE_X = 6;
    const int FIELD_SIZE_Y_BASE = 12;
    const int FIELD_SIZE_Y = 15;

    //パネルのプレハブのもと
    [SerializeField] private PaneponPanel _panelPurefab = null;

    //パネルのテクスチャを設定する用
    [SerializeField] private List<Texture> _panelTextureList = new List<Texture>();

    //パネル全体の位置を制御するやつ
    [SerializeField] private GameObject _panelRoot = null;

    //カーソルのプレハブ
    [SerializeField] private GameObject _cursorPrefab = null;

    //パネルの色
    public enum PanelColor
    {
        Red,
        Blue,
        SkyBlue,
        Purple,
        Yellow,
        Green,

        Max
    };

    /// <summary>
    /// パネルのステート
    /// None:パネル無し Stable:停止中 Swap:入れ替え中 Flash:発光中 Erase:消滅中 Fall:落下中
    /// </summary>
    public enum PanelState
    {
        None,    //パネル無し
        Stable,  //停止中
        Swap,    //入れ替え中
        Flash,   //発光中
        Erase,   //消滅中
        Fall,    //落下中

        Max
    };

    //パネルのpurefabのリスト
    private List<PaneponPanel> _panelPrefabList = new List<PaneponPanel>((int)PanelColor.Max);

    //マテリアルのリスト
    private List<Material> _panelMaterialList = new List<Material>((int)PanelColor.Max);

    //フィールド上にあるパネルの状態
    //private PanelState[,] _fieldPanelsState = new PanelState[FIELD_SIZE_Y,FIELD_SIZE_X];

    //影響しているパネルへの参照
    private PaneponPanel[,] _fieldPanels = new PaneponPanel[FIELD_SIZE_Y, FIELD_SIZE_X];

    //スクロールスピード
    private float _scrollSpeed = 0.3f;
    private float _scrollRaio = 0;

    //直前のスピードを保持
    private float _scrollSpeedTmp = 0;

    //スクロールボタンが押されているか
    private bool _onScrollButton = false;

    //スクロールの加速倍率
    const float FAST_SCROLL = 1f;

    //連鎖カウントを加算していいか
    private bool isIncreaseChainCount = false;

    //カーソル
    private GameObject _cursolL = null;
    private GameObject _cursolR = null;

    //カーソル位置
    private int _corsorPosX = 0;
    private int _corsorPosY = 0;

    //パネルを消せる最小数
    const int MIN_ERASE_COUNT = 3;

    //判定しない最下段の数
    const int _BOTTOM_ROW = 1;

    #endregion
    private void Awake()
    {
        //gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        _inputSystem = new InputSystem();
        _inputSystem.Enable();
    }

    void Start()
    {
        //ゲームのStateをGameRedyに設定@
        GameManager.instance.game_State = GameManager.GameState.GameRedy;

        //ゲームのハイスコアを初期化
        GameManager.instance._highScore = 0;

        //プレハブとマテリアルを用意する
        for (int i = 0; i < _panelTextureList.Count; i++)
        {
            _panelPrefabList.Add(GameObject.Instantiate<PaneponPanel>(_panelPurefab));
            _panelMaterialList.Add(GameObject.Instantiate<Material>(_panelPrefabList[i].meshRenderer.material));
            _panelPrefabList[i].gameObject.SetActive(false);
            _panelPrefabList[i].meshRenderer.material = _panelMaterialList[i];
            _panelMaterialList[i].mainTexture = _panelTextureList[i];
        }

        //初期状態のパネルの配置
        for (int i = 0; i < FIELD_SIZE_Y_BASE / 2; i++)
        {
            for (int j = 0; j < FIELD_SIZE_X; j++)
            {
                //パネルの実体
                PanelColor color = (PanelColor)Random.Range(0, _panelPrefabList.Count);
                PaneponPanel newPanel = GameObject.Instantiate<PaneponPanel>(_panelPrefabList[(int)color]);
                newPanel.transform.SetParent(_panelRoot.transform);
                newPanel.Init(this);
                newPanel.gameObject.SetActive(true);
                newPanel.SetPosition(j, i);
                newPanel.SetColor(color);
                _fieldPanels[i, j] = newPanel;
            }
        }

        //カーソルの用意
        _cursolL = GameObject.Instantiate<GameObject>(_cursorPrefab);
        _cursolL.transform.SetParent(_panelRoot.transform);
        _cursolR = GameObject.Instantiate<GameObject>(_cursorPrefab);
        _cursolR.transform.SetParent(_panelRoot.transform);

        //カーソルの初期位置を設定
        _corsorPosX = 2;
        _corsorPosY = 4;

        _panePonUI.StartCountCoroutine();
    }

    void Update()
    {
        if(GameManager.instance.game_State == GameManager.GameState.Pause
           || GameManager.instance.game_State == GameManager.GameState.GameOver)
        {
            return;
        }

        //カーソル移動処理
        int deltaY = 0;
        int deltaX = 0;

        if (_inputSystem.Player.Up.triggered)
        {
            //カーソル移動SE
            GameManager.instance._seManager.OnMoveCorsor_SE();

            deltaY++;
        }
        if (_inputSystem.Player.Down.triggered)
        {
            //カーソル移動SE
            GameManager.instance._seManager.OnMoveCorsor_SE();

            deltaY--;
        }
        if (_inputSystem.Player.Right.triggered)
        {
            //カーソル移動SE
            GameManager.instance._seManager.OnMoveCorsor_SE();

            deltaX++;
        }
        if (_inputSystem.Player.Left.triggered)
        {
            //カーソル移動SE
            GameManager.instance._seManager.OnMoveCorsor_SE();

            deltaX--;
        }
        _corsorPosX = Mathf.Clamp(_corsorPosX + deltaX, 0, 4);
        _corsorPosY = Mathf.Clamp(_corsorPosY + deltaY, _BOTTOM_ROW, FIELD_SIZE_Y_BASE);
        MoveCursor(_corsorPosX, _corsorPosY);

        //ゲーム状態がRedyならカーソル移動以外の操作を行わない
        if (GameManager.instance.game_State != GameManager.GameState.GameRedy)
        {
            //パネル入れ替え処理
            if (_inputSystem.Player.Swap.triggered && IsSwapable())
            {
                if (_fieldPanels[_corsorPosY, _corsorPosX])
                {
                    _fieldPanels[_corsorPosY, _corsorPosX].Swap(_corsorPosX + 1, _corsorPosY);
                }
                if (_fieldPanels[_corsorPosY, _corsorPosX + 1])
                {
                    _fieldPanels[_corsorPosY, _corsorPosX + 1].Swap(_corsorPosX, _corsorPosY);
                }

                //入れ替え
                PaneponPanel tmp = _fieldPanels[_corsorPosY, _corsorPosX];
                _fieldPanels[_corsorPosY, _corsorPosX] = _fieldPanels[_corsorPosY, _corsorPosX + 1];
                _fieldPanels[_corsorPosY, _corsorPosX + 1] = tmp;

                //入れ替えSE
                GameManager.instance._seManager.OnSwap_SE();
            }

            //スクロールupが押されたら
            if (_inputSystem.Player.ScrolUp.triggered && !_onScrollButton)
            {
                _scrollSpeedTmp = _scrollSpeed;
                _scrollSpeed += FAST_SCROLL;
                _onScrollButton = true;
            }
            //スクロールupを離したら
            else if (_inputSystem.Player.ScrolUp.triggered && _onScrollButton)
            {
                _scrollSpeed = _scrollSpeedTmp;
                _onScrollButton = false;
            }
        }

        //パネルについている連鎖フラグを切る
        ResetAllStablePanelChainTargetFlag();

        if (_inputSystem.Player.Test.triggered)
        {
            Test();
        }

    }

    private void FixedUpdate()
    {
        //PaneponPanelのUpdateを下から行う
        for (int y = 0; y < FIELD_SIZE_Y; y++)
        {
            for (int x = 0; x < FIELD_SIZE_X; x++)
            {
                PaneponPanel panel = _fieldPanels[y, x];
                if (panel)
                {
                    panel.UpdateManual();

                    //消滅してNoneになったパネルを消す
                    DeletEmptyPanel(panel);
                }
            }
        }

        //全てのパネルをチェック
        CheckAllPanels();

        //ゲーム開始カウントダウン
        if(GameManager.instance.game_State == GameManager.GameState.GameRedy)
        {
            return;
        }

        //ゲームオーバー処理
        if (IsGameOverCondition())
        {
            if(GameManager.instance.game_State != GameManager.GameState.GameOver)
            {
                print("ガメオベラ");
                //ゲームの状態をGameOverに設定
                GameManager.instance.game_State = GameManager.GameState.GameOver;

                //リザルトを表示
                _panePonUI.ResultUI();
            }
            return;
        }

        //パネルがそろっているかどうかの判定
        CheckErase();

        //パネルの連鎖対象フラグをOFF
        ResetAllStablePanelChainTargetFlag();

        //スクロール処理
        if (!IsSomePanelErasing(true))
        {
            _scrollRaio += _scrollSpeed * Time.deltaTime;
        }
        if (_scrollRaio >= 1.0f)
        {
            //スクロール割合をリセット
            _scrollRaio = 0f;

            ScrollUp();

            //カーソルを1マス分上に上げる
            _corsorPosY++;

            //パネルをランダムに追加する
            int y = 0;
            for (int x = 0; x < FIELD_SIZE_X; x++)
            {
                //パネルの実体
                PanelColor color = (PanelColor)Random.Range(0, _panelPrefabList.Count);
                PaneponPanel newPanel = GameObject.Instantiate<PaneponPanel>(_panelPrefabList[(int)color]);
                newPanel.transform.SetParent(_panelRoot.transform);
                newPanel.Init(this);
                newPanel.gameObject.SetActive(true);
                newPanel.SetPosition(x, y);
                newPanel.SetColor(color);
                _fieldPanels[y, x] = newPanel;
            }
        }
        _panelRoot.transform.localPosition = new Vector3(0f, _scrollRaio, 0f);

    }

    #region メソッド
    /// <summary>
    /// テストのため
    /// 配列の中身を出力する
    /// </summary>
    private void Test()
    {
        print("Field---------------------------");
        for (int y = 0; y < FIELD_SIZE_Y; y++)
        {
            print("\n");
            print("Line : " + y);
            for (int x = 0; x < FIELD_SIZE_X; x++)
            {
                if (_fieldPanels[y, x] == null)
                {
                    print("NULLです");
                }
                else
                {
                    print(_fieldPanels[y, x].panel_State + "  " + _fieldPanels[y, x].color);
                }
            }
        }
        print("Field---------------------------");

    }
    /// <summary>
    /// パネルが入れ替え可能な状態かどうか
    /// </summary>
    /// <returns></returns>
    private bool IsSwapable()
    {
        if (_fieldPanels[_corsorPosY, _corsorPosX] && !_fieldPanels[_corsorPosY, _corsorPosX]._isSwapSble)
        {
            return false;
        }
        if (_fieldPanels[_corsorPosY, _corsorPosX + 1] && !_fieldPanels[_corsorPosY, _corsorPosX + 1]._isSwapSble)
        {
            return false;
        }
        return true;
    }
    /// <summary>
    /// カーソル移動
    /// </summary>    
    private void MoveCursor(int leftX, int leftY)
    {
        _cursolL.transform.localPosition = new Vector3(leftX, leftY, 0f);
        _cursolR.transform.localPosition = new Vector3(leftX + 1, leftY, 0f);
    }
    /// <summary>
    /// パネルがそろっているかの判定
    /// </summary>
    private void CheckErase()
    {
        isIncreaseChainCount = false;
        bool isCrossCheck = false;
        for (int i = _BOTTOM_ROW; i < FIELD_SIZE_Y; i++)
        {
            for (int j = 0; j < FIELD_SIZE_X; j++)
            {
                //パネルが消せる場合の処理(X方向)
                SameEraseHorixontal(j, i, CheckSameColorHorizontal(j, i).n, CheckSameColorHorizontal(j, i).n_left, isCrossCheck);

                //パネルが消せる場合の処理(Y方向)
                SameEraseVartical(j, i, CheckSameColorVartical(j, i), isCrossCheck);
            }
        }

        _panePonUI._isSomePanelErasing = IsSomePanelErasing(false);
        if (isIncreaseChainCount)
        {
            _panePonUI._chainCount++;  //連鎖数を実際に加算
        }
        else if(!IsSomePanelErasing(false))
        { 
            _panePonUI._chainCount = 0;    //連鎖を1からやり直す
        }
    }
    /// <summary>
    /// 右方向に何個そろっているか
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_y"></param>
    /// <returns>そろっている数</returns>
    private (int n, int n_left)CheckSameColorHorizontal(int _x, int _y)
    {
        PanelColor baseColor = GetColor(_x, _y);
        PanelState baseState = GetState(_x, _y);
        if (baseColor == PanelColor.Max || baseState != PanelState.Stable)
        {
            return (0,0);
        }
        //右方向に何個そろっている数
        int n_right = 0;
        for (int x = _x; x < FIELD_SIZE_X; x++)
        {
            if (baseColor != GetColor(x, _y)|| baseState != GetState(x, _y))
            {
                break;
            }
            n_right++;
        }
        //左方向にそろっている数
        int n_left = 0;
        for (int x = _x - 1; x >= 0; x--)
        {
            if (baseColor != GetColor(x, _y)|| baseState != GetState(x, _y))
            {
                break;
            }
            n_left++;
        }
        return (n_right + n_left, n_left);
    }
    /// <summary>
    /// 上方向に何個そろっているか
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_y"></param>
    /// <returns>そろっている数</returns>
    private int CheckSameColorVartical(int _x, int _y)
    {
        PanelColor baseColor = GetColor(_x, _y);
        PanelState baseState = GetState(_x, _y);
        if (baseColor == PanelColor.Max || baseState != PanelState.Stable)
        {
            return 0;
        }
        int n = 0;
        for (int y = _y; y < FIELD_SIZE_Y; y++)
        {
            if (baseColor != GetColor(_x, y) || baseState != GetState(_x, y))
            { 
                break;
            }
            n++;
        }
        return n;
    }
    /// <summary>
    /// 横方向にパネルを何個消すか
    /// </summary>
    /// <param name="y"></param>
    /// <param name="x"></param>
    /// <param name="n">そろっている数</param>
    private void SameEraseHorixontal(int _x, int _y, int n, int n_left, bool isCross)
    {
        if (n >= MIN_ERASE_COUNT)
        {
            for (int k = 0; k < n; k++)
            {
                if (!isCross)
                {
                    SameEraseVartical(_x + k, _y, CheckSameColorVartical(_x + k, _y), true);
                }
                
                if (_fieldPanels[_y, _x + k - n_left].isCahainTarget)
                {
                    isIncreaseChainCount = true;    //連鎖数を加算
                }
                _fieldPanels[_y, _x + k - n_left].StartFlash();

                //消したパネルの数をハイスコアに加算
                GameManager.instance._highScore++;
            }
            
            //カーソル移動SE
            GameManager.instance._seManager.OnPanelErase_SE();
        }
    }
    /// <summary>
    /// 縦方向にパネルを何個消すか
    /// </summary>
    /// <param name="y"></param>
    /// <param name="x"></param>
    /// <param name="n">そろっている数</param>
    private void SameEraseVartical(int _x, int _y, int n, bool isCross)
    {
        if (n >= MIN_ERASE_COUNT)
        {
            for (int k = 0; k < n; k++)
            {
                
                if (!isCross)
                {
                    SameEraseHorixontal(_x, _y + k, CheckSameColorHorizontal(_x, _y + k).n, CheckSameColorHorizontal(_x, _y + k).n_left, true);
                }

                if (_fieldPanels[_y + k, _x].isCahainTarget)
                {
                    isIncreaseChainCount = true;    //連鎖数を加算
                }
                _fieldPanels[_y + k, _x].StartFlash();

                //消したパネルの数をハイスコアに加算
                GameManager.instance._highScore++;
            }

            //カーソル移動SE
            GameManager.instance._seManager.OnPanelErase_SE();
        }
    }
    /// <summary>
    /// パネルの色を取得する
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_y"></param>
    /// <returns></returns>
    private PanelColor GetColor(int _x, int _y)
    {
        if (_x < 0 || FIELD_SIZE_X <= _x || _y < 0 || FIELD_SIZE_Y <= _y)
        {
            return PanelColor.Max;
        }
        return (_fieldPanels[_y, _x] ? _fieldPanels[_y, _x].color : PanelColor.Max);
    }
    /// <summary>
    /// パネルのStateを取得
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_y"></param>
    /// <returns></returns>
    private PanelState GetState(int _x, int _y)
    {
        if (_x < 0 || FIELD_SIZE_X <= _x || _y < 0 || FIELD_SIZE_Y <= _y)
        {
            return PanelState.Max;
        }
        return (_fieldPanels[_y, _x] ? _fieldPanels[_y, _x].panel_State : PanelState.Max);
    }
    /// <summary>
    /// パネルの状態を取得
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_y"></param>
    /// <returns></returns>
    public PanelState GetPanelState(int _x, int _y)
    {
        if (_y < 0 || _y >= FIELD_SIZE_Y)
        {
            return PaneponSystem.PanelState.Stable;
        }
        if (_fieldPanels[_y, _x] == null)
        {
            return PaneponSystem.PanelState.None;
        }
        return _fieldPanels[_y, _x].panel_State;
    }
    /// <summary>
    /// パネルを配列内でも落下させる
    /// </summary>
    public void FallPanel(int _x, int _y)
    {
        //入れ替え
        PaneponPanel tmp = _fieldPanels[_y, _x];
        _fieldPanels[_y, _x] = _fieldPanels[_y - 1, _x];
        _fieldPanels[_y - 1, _x] = tmp;
    }
    /// <summary>
    /// 1マス分のスクロール移動(上のパネルから)
    /// </summary>
    private void ScrollUp()
    {
        for (int y = FIELD_SIZE_Y_BASE; y >= 0; y--)
        {
            for (int x = 0; x < FIELD_SIZE_X; x++)
            {
                _fieldPanels[y + 1, x] = _fieldPanels[y, x];
                if (_fieldPanels[y + 1, x])
                {
                    _fieldPanels[y + 1, x].CarryUp();
                }
            }
        }
    }
    /// <summary>
    /// Noneのパネルを消す
    /// </summary>
    private void DeletEmptyPanel(PaneponPanel _panel)
    {
        if (_panel.panel_State == PanelState.None)
        {
            Destroy(_panel);
            _panel = null;
        }
    }
    /// <summary>jj
    /// 全てのパネルをチェックする
    /// </summary>
    public void CheckAllPanels()
    {
        for (int y = 0; y < FIELD_SIZE_Y; y++)
        {
            for (int x = 0; x < FIELD_SIZE_X; x++)
            {
                if (_fieldPanels[y, x])
                {
                    _fieldPanels[y, x].CheckToFall(true);
                }
            }
        }
    }
    /// <summary>
    /// いずれかのパネルが消えている最中か
    /// </summary>
    /// <returns></returns>
    private bool IsSomePanelErasing(bool isScrollCheck)
    {
        for (int y = 0; y < FIELD_SIZE_Y; y++)
        {
            for (int x = 0; x < FIELD_SIZE_X; x++)
            {
                if (_fieldPanels[y, x] && 
                    (_fieldPanels[y, x].panel_State == PanelState.Flash || 
                     _fieldPanels[y, x].panel_State == PanelState.Erase))
                {
                    return true;
                }
                else if (_fieldPanels[y, x] && !isScrollCheck && (_fieldPanels[y, x].panel_State == PanelState.Fall ))
                {
                    return true;
                }
            }
        }
        return false;
    }
    /// <summary>
    /// ゲームオーバー判定
    /// </summary>
    /// <returns></returns>
    public bool IsGameOverCondition()
    {
        for (int y = FIELD_SIZE_Y_BASE; y < FIELD_SIZE_Y; y++)
        {
            for (int x = 0; x < FIELD_SIZE_X; x++)
            {
                if (_fieldPanels[y, x] && _fieldPanels[y, x].panel_State == PanelState.Stable)
                {
                    return true;
                }
            }
        }
        return false;
    }
    /// <summary>
    /// 連鎖対象フラグを設定
    /// </summary>
    /// <param name="_y"></param>
    /// <param name="_x"></param>
    public void SetChainTarget(int _y, int _x)
    {
        //上に向かってパネルを捜査してStableのパネルのみにフラグを設定
        for (int y = _y + 1; y < FIELD_SIZE_Y; y++)
        {
            if (_fieldPanels[y, _x])
            {
                PanelState state = _fieldPanels[y, _x].panel_State;
                if (state == PanelState.Stable)
                {
                    _fieldPanels[y, _x].isCahainTarget = true;
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }
    /// <summary>
    /// 着地しているパネルの連鎖対象フラグををOFF
    /// </summary>
    void ResetAllStablePanelChainTargetFlag()
    {
        for (int y = _BOTTOM_ROW; y < FIELD_SIZE_Y; y++)
        {
            for (int x = 0; x < FIELD_SIZE_X; x++)
            {
                if (_fieldPanels[y, x] && _fieldPanels[y, x].panel_State == PanelState.Stable)
                {
                    _fieldPanels[y, x].isCahainTarget = false;
                }
            }
        }
    }
    #endregion
}