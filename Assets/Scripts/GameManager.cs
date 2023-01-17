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

    //SEマネージャー
    public SEManager _seManager = default;

    //private GameManager _instance = null;
    public static GameManager instance { get; set; }

    //ゲームの状態
    private GameState _game_State = GameState.Title;
    public GameState game_State { get { return _game_State; } set { _game_State = value; } }
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

    //ハイスコアの変数
    public int _highScore = 0;

    #endregion
    private void Awake()
    {
        _inputSystem = new InputSystem();
        _inputSystem.Enable();

        //SEマネージャーを外部から参照しやすく
        _seManager = transform.GetComponent<SEManager>();

        if (instance == null)
        {
            instance = this;
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
