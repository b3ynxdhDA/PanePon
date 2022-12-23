using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/// <summary>
/// ゲーム全体の状態を管理するクラス
/// </summary>
public class GameManager : MonoBehaviour
{
    #region 変数
    //インプットシステム
    InputSystem _inputSystem;

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

    #endregion
    private void Awake()
    {
        _inputSystem = new InputSystem();
        _inputSystem.Enable();

        if (instance == null)
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
        if(_inputSystem.Player.Esc.triggered)
        {
#if UNITY_EDITOR
            //エディターの時は再生をやめる
            UnityEditor.EditorApplication.isPlaying = false;
#else
            //アプリケーションを終了する
            Application.Quit();
#endif
        }
    }
}
