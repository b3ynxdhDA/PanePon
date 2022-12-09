using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲーム全体の状態を管理するクラス
/// </summary>
public class GameManager : MonoBehaviour
{
    private GameManager _instance = null;
    public GameManager instance { get { return _instance; } set { _instance = value; } }

    //ゲームの状態
    private GameManager.GameState _game_State = GameManager.GameState.Title;
    public GameManager.GameState game_State { get { return _game_State; } set { _game_State = value; } }
    public enum GameState
    {
        Title,
        Select,
        GameRedy,
        GameNow,
        GameOver,
        Result,
        Pause
    };

    private void Awake()
    {
        if(instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        switch (_game_State)
        {
            case GameState.Title:
                break;
            case GameState.Select:
                break;
            case GameState.GameRedy:
                break;
            case GameState.GameNow:
                break;
            case GameState.GameOver:
                break;
            case GameState.Result:
                break;
        }
    }
}
