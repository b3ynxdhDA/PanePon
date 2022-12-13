using UnityEngine;
using UnityEngine.InputSystem;

//ポーズ機能を呼び出すクラス
//常にアクティブなオブジェクトにアタッチして
public class PauseScript : MonoBehaviour
{
    //インプットシステム
    InputSystem _inputSystem;

    //ポーズしたときに表示するUIプレハブ
    [SerializeField] private GameObject pauseUI = default;

    private void Awake()
    {
        _inputSystem = new InputSystem();
        _inputSystem.Enable();
        Time.timeScale = 1f;
    }
    //ポーズ中は処理を受け付けない処理を書いてはいけない
    void Update()
    {
        GameManager gameManager = GetComponent<GameManager>();
        //print(GetComponent<GameManager>().game_State);
        if (_inputSystem.Player.Pause.triggered)
        {
            //ポーズUIのアクティブを切り替え
            pauseUI.SetActive(!pauseUI.activeSelf);

            //ポーズUIが表示されている時は停止
            if (pauseUI.activeSelf)
            {
                Time.timeScale = 0f;
                gameManager.game_State = GameManager.GameState.Pause;
            }
            else
            {
                Time.timeScale = 1f;
                gameManager.game_State = GameManager.GameState.GameNow;
            }

        }
    }
}
