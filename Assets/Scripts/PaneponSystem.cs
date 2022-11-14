using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PaneponSystem : MonoBehaviour
{
    #region 変数
    //インプットシステム
    InputSystem _inputSystem;

    const int FIELD_SIZE_X = 6;
    const int FIELD_SIZE_Y_BASE = 12;
    const int FIELD_SIZE_Y = 24;

    //パネルのプレハブのもと
    [SerializeField] private PaneponPanel _panelPurefab = null;

    //パネルのテクスチャを設定する用
    [SerializeField] private List<Texture> _panelTextureList = new List<Texture>();

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
    private PaneponPanel[,] _fieldPanels = new PaneponPanel[FIELD_SIZE_Y,FIELD_SIZE_X];

    //カーソル
    private GameObject _cursolL = null;
    private GameObject _cursolR = null;

    //カーソル位置
    private int _corsorPosX = 0;
    private int _corsorPosY = 0;

    //パネルを消せる最小数
    const int MIN_ERASE_COUNT = 3;

    #endregion
    private void Awake()
    {
        _inputSystem = new InputSystem();
        _inputSystem.Enable();
    }

    void Start()
    {
        //プレハブとマテリアルを用意する
        for(int i = 0; i < _panelTextureList.Count; i++)
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
            for(int j = 0;j < FIELD_SIZE_X; j++)
            {
                //パネルの実体
                PanelColor color = (PanelColor)Random.Range(0, _panelPrefabList.Count);
                PaneponPanel newPanel = GameObject.Instantiate<PaneponPanel>(_panelPrefabList[(int)color]);
                //newPanel.transform.localPosition = new Vector3(j, i, 0f); 下のメソッドで置き換え
                newPanel.Init(this);
                newPanel.gameObject.SetActive(true);
                newPanel.SetPosition(j, i);
                newPanel.SetColor(color);
                _fieldPanels[i, j] = newPanel;
            }
        }

        //カーソルの用意
        _cursolL = GameObject.Instantiate<GameObject>(_cursorPrefab);
        _cursolR = GameObject.Instantiate<GameObject>(_cursorPrefab);

        //カーソルの初期位置を設定
        MoveCursor(4, 4);
    }

    void Update()
    {
        //カーソル移動処理
        int deltaY = 0;
        int deltaX = 0;

        if (_inputSystem.Player.Up.triggered)
        {
            deltaY++;
        }
        if(_inputSystem.Player.Down.triggered)
        {
            deltaY--;
        }
        if(_inputSystem.Player.Right.triggered)
        {
            deltaX++;
        }
        if(_inputSystem.Player.Left.triggered)
        {
            deltaX--;
        }
        _corsorPosX = Mathf.Clamp(_corsorPosX + deltaX, 0, 4);
        _corsorPosY = Mathf.Clamp(_corsorPosY + deltaY, 0, 12);
        MoveCursor(_corsorPosX, _corsorPosY);

        //パネル入れ替え処理
        if (_inputSystem.Player.Swap.triggered)
        {
            if(!(_fieldPanels[_corsorPosY, _corsorPosX].state == PanelState.Stable ||
               _fieldPanels[_corsorPosY, _corsorPosX].state == PanelState.None))
            {
                return;
            }
            if(!(_fieldPanels[_corsorPosY, _corsorPosX + 1].state == PanelState.Stable ||
               _fieldPanels[_corsorPosY, _corsorPosX + 1].state == PanelState.None))
            {
                return;
            }
            _fieldPanels[_corsorPosY, _corsorPosX].Swap(_corsorPosX + 1, _corsorPosY);
            _fieldPanels[_corsorPosY, _corsorPosX + 1].Swap(_corsorPosX, _corsorPosY);

            //入れ替え
            PaneponPanel tmp = _fieldPanels[_corsorPosY, _corsorPosX];
            _fieldPanels[_corsorPosY, _corsorPosX] = _fieldPanels[_corsorPosY, _corsorPosX + 1];
            _fieldPanels[_corsorPosY, _corsorPosX + 1] = tmp;
        }


        //パネルがそろっているかどうかの判定
        CheckErase();

        //消滅したパネルを消す処理

        /*//パネルの色を確認するテスト用
        if (_inputSystem.Player.Up.triggered)
        {
            //配列を出力するテスト
            print("Field---------------------------");
            for (int y = 0; y < FIELD_SIZE_Y; y++)
            {

                for (int x = 0; x < FIELD_SIZE_X; x++)
                {
                    print(_fieldPanels[y, x].color);
                }
                print("\n");
            }
            print("Field---------------------------");
        }*/

    }

    #region メソッド
    /// <summary>
    /// カーソル移動
    /// </summary>    
    void MoveCursor(int leftX, int leftY)
    {
        _cursolL.transform.localPosition = new Vector3(leftX, leftY, 0f);
        _cursolR.transform.localPosition = new Vector3(leftX + 1, leftY, 0f);
    }
    /// <summary>
    /// パネルがそろっているかの判定
    /// </summary>
    void CheckErase()
    {

        for (int i = 0; i < FIELD_SIZE_Y / 2; i++)
        {
            for (int j = 0; j < FIELD_SIZE_X; j++)
            {
                //パネルが消せる場合の処理(X方向)
                SameEraseHorixontal(i, j, CheckSameColorHorizontal(j, i));

                //パネルが消せる場合の処理(Y方向)
                SameEraseVartical(i, j, CheckSameColorVartical(j, i));
            }
        }
    }
    /// <summary>
    /// 横方向に何個そろっているか
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_y"></param>
    /// <returns>そろっている数</returns>
    int CheckSameColorHorizontal(int _x, int _y)
    {
        PanelColor baseColor = GetColor(_x, _y);
        PanelState baseState = GetState(_x, _y);
        if(baseColor == PanelColor.Max || baseState != PanelState.Stable)
        {
            return 0;
        }
        int n = 0;
        for(int x = _x; x < FIELD_SIZE_X; x++)
        {
            if(baseColor != GetColor(x, _y) || baseState != GetState(x, _y))
            {
                break;
            }
            n++;
        }
        return n;
    } 
    /// <summary>
    /// 縦方向に何個そろっているか
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_y"></param>
    /// <returns>そろっている数</returns>
    int CheckSameColorVartical(int _x, int _y)
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
    /// @消したパネルがColorを持っているため消えたパネル同士でも色が揃えば消えてしまう
    /// </summary>
    /// <param name="y"></param>
    /// <param name="x"></param>
    /// <param name="n"></param>
    private void SameEraseHorixontal(int y, int x, int n)
    {
        if (n >= MIN_ERASE_COUNT)
        {
            for (int k = 0; k < n; k++)
            {
                _fieldPanels[y, x + k].StartErase();
            }
        }
    }
    /// <summary>
    /// 縦方向にパネルを何個消すか
    /// @消したパネルがColorを持っているため消えたパネル同士でも色が揃えば消えてしまう
    /// </summary>
    /// <param name="y"></param>
    /// <param name="x"></param>
    /// <param name="n"></param>
    private void SameEraseVartical(int y, int x, int n)
    {
        if (n >= MIN_ERASE_COUNT)
        {
            for (int k = 0; k < n; k++)
            {
                _fieldPanels[y + k, x].StartErase();
            }
        }
    }
    /// <summary>
    /// パネルの色を取得する
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_y"></param>
    /// <returns></returns>
    PanelColor GetColor(int _x, int _y)
    {
        if(_x < 0 || FIELD_SIZE_X <= _x || _y < 0 || FIELD_SIZE_Y <= _y)
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
    PanelState GetState(int _x, int _y)
    {
        if (_x < 0 || FIELD_SIZE_X <= _x || _y < 0 || FIELD_SIZE_Y <= _y)
        {
            return PanelState.Max;
        }
        return (_fieldPanels[_y, _x] ? _fieldPanels[_y, _x].state : PanelState.Max);
    }
    /// <summary>
    /// パネルの状態を取得
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_y"></param>
    /// <returns></returns>
    public PanelState GetPanelState(int _x, int _y)
    {
        if(_y < 0 || _y >= FIELD_SIZE_Y)
        {
            return PaneponSystem.PanelState.Stable;
        }
        return _fieldPanels[_y, _x].state;
    }
    /// <summary>
    /// パネルを配列内でも落下させる
    /// </summary>
    public void FallPanel(int _x, int _y)
    {
        //入れ替え
        PaneponPanel tmp = _fieldPanels[_x, _y];
        _fieldPanels[_x, _y] = _fieldPanels[_x, _y - 1];
        _fieldPanels[_x, _y - 1] = tmp;
    }
    #endregion
}
