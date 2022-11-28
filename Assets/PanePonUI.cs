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
    //ゲームオーバーテキスト
    [SerializeField] private Text _gameOverText = default;

    //連鎖カウントテキスト
    [SerializeField] private Text _chainCountText = default;

    void Start()
    {
        
    }

    void Update()
    {

    }
}
