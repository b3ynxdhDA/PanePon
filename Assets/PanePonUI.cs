using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// パネポンのゲーム本編で使用されるUI
/// を管理するクラス
/// </summary>
public class PanePonUI : MonoBehaviour
{
    private PanePonUI instance = null;
    public PanePonUI Instance { get { return instance; } }

    //連鎖数
    public int _chainCount = 0;

    //パネルが連載中かどうか
    public bool _isSomePanelErasing = false;

    //ゲームオーバーテキスト
    [SerializeField] private Text _gameOverText = null;

    //連鎖カウントテキスト
    [SerializeField] private Text _chainCountText = default;

    void Start()
    {
        
    }

    void Update()
    {
        _chainCountText.gameObject.SetActive(_isSomePanelErasing);
        _chainCountText.text = "" + _chainCount;
    }
}
