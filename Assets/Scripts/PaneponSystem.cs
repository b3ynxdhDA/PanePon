using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaneponSystem : MonoBehaviour
{
    #region 変数
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

    //パネルの状態
    public enum PanelState
    {
        Stable,  //停止中
        Swap,    //入れ替え中
        Chain,   //連鎖中
        Fall,    //落下中

        Max
    };

    //パネルのpurefabのリスト
    private List<PaneponPanel> _panelPrefabList = new List<PaneponPanel>((int)PanelColor.Max);

    //マテリアルのリスト
    private List<Material> _panelMaterialList = new List<Material>((int)PanelColor.Max);

    //フィールド上にあるパネルの状態
    private PanelState[,] _fieldPanelsState = new PanelState[FIELD_SIZE_Y,FIELD_SIZE_X];

    //影響しているパネルへの参照
    private PaneponPanel[,] _fieldPanels = new PaneponPanel[FIELD_SIZE_Y,FIELD_SIZE_X];

    //カーソル
    private GameObject _cursolL = null;
    private GameObject _cursolR = null;

    //カーソル位置
    private int _corsorPosX = 0;
    private int _corsorPosY = 0;

    #endregion
    void Start()
    {
        //プレハブとマテリアルを用意する
        for(int i = 0; i < _panelTextureList.Count; i++)
        {
            /*
            _panelPrefabList[i] = GameObject.Instantiate<PaneponPanel>(_panelPurefab);
            _panelMaterialList[i] = GameObject.Instantiate<Material>(_panelPrefabList[i].meshRenderer.material);
            _panelPrefabList[i].meshRenderer.material = _panelMaterialList[i];
            _panelMaterialList[i].mainTexture = _panelTextureList[i];
            */
            _panelPrefabList.Add(GameObject.Instantiate<PaneponPanel>(_panelPurefab));
            _panelMaterialList.Add(GameObject.Instantiate<Material>(_panelPrefabList[i].meshRenderer.material));
            _panelPrefabList[i].meshRenderer.material = _panelMaterialList[i];
            _panelMaterialList[i].mainTexture = _panelTextureList[i];
        }

        //初期状態のパネルの配置
        for (int i = 0; i < FIELD_SIZE_Y_BASE / 2; i++)
        {
            for(int j = 0;j < FIELD_SIZE_X; j++)
            {
                //パネルの実体
                PaneponPanel newPanel = GameObject.Instantiate<PaneponPanel>(_panelPrefabList[Random.Range(0,_panelPrefabList.Count)]);
                newPanel.transform.localPosition = new Vector3(j, i, 0f);
                _fieldPanels[i, j] = newPanel;
            }
        }

        //カーソルの用意
        _cursolL = GameController.Instantiate<GameObject>(_cursorPrefab);
        _cursolR = GameController.Instantiate<GameObject>(_cursorPrefab);

        //カーソルの初期位置を設定
        MoveCursor(0, 4);
    }

    void Update()
    {
        //カーソル移動処理
        int deltaY = 0;
        int deltaX = 0;
        //if(Input.GetKeyDown(KeyCode.W))
        if(Input.GetButtonDown("Up"))
        {
            deltaY++;
        }
        if(Input.GetButtonDown("Down"))
        {
            deltaY--;
        }
        if(Input.GetButtonDown("Right"))
        {
            deltaX++;
        }
        if(Input.GetButtonDown("Left"))
        {
            deltaX--;
        }
        _corsorPosX = Mathf.Clamp(_corsorPosX + deltaX, 0, 4);
        _corsorPosY = Mathf.Clamp(_corsorPosY + deltaY, 0, 12);
        MoveCursor(_corsorPosX, _corsorPosY);
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
    #endregion
}
