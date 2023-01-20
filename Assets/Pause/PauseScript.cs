﻿using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

//ポーズ機能を呼び出すクラス
//常にアクティブなオブジェクトにアタッチして
public class PauseScript : MonoBehaviour
{
    //インプットシステム
    InputSystem _inputSystem;

    //ポーズしたときに表示するUIプレハブ
    [SerializeField] private GameObject _pauseUI = default;

    //ポーズしたとき選択されているボタン
    [SerializeField] private GameObject _selectedButton = default;

    private void Awake()
    {
        _inputSystem = new InputSystem();
        _inputSystem.Enable();

        //タイムスケールの初期化
        Time.timeScale = 1f;
    }
    //ポーズ中は処理を受け付けない処理を書いてはいけない
    void Update()
    {
        if (_inputSystem.Player.Pause.triggered)
        {
            OnPouse();
        }

        if (_inputSystem.Player.Test.triggered)
        {
            _pauseUI.SetActive(!_pauseUI.activeSelf);
        }
    }
    /// <summary>
    /// ポーズUIの表示とゲーム状態の変更
    /// </summary>
    public void OnPouse()
    {
        if (!(GameManager.instance.game_State == GameManager.GameState.GameNow || GameManager.instance.game_State == GameManager.GameState.Pause))
        {
            return;
        }
        //ポーズUIのアクティブを切り替え
        _pauseUI.SetActive(!_pauseUI.activeSelf);

        //ポーズUIが表示されている時は停止
        if (_pauseUI.activeSelf)
        {
            Time.timeScale = 0f;
            GameManager.instance.game_State = GameManager.GameState.Pause;
            EventSystem.current.SetSelectedGameObject(_selectedButton);
        }
        else
        {
            Time.timeScale = 1f;
            GameManager.instance.game_State = GameManager.GameState.GameNow;
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
