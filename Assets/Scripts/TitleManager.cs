using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// タイトルを管理するクラス
/// </summary>
public class TitleManager : MonoBehaviour
{
    GameManager gameManager = null;

    private InputAction _pressAnyKeyAction =
        new InputAction(type: InputActionType.PassThrough, binding: "*/<Button>", interactions: "Press");

    private void OnEnable() => _pressAnyKeyAction.Enable();
    private void OnDisable() => _pressAnyKeyAction.Disable();
    void Awake()
    {
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        gameManager.game_State = GameManager.GameState.Title;
    }

    void FixedUpdate()
    {
        if (_pressAnyKeyAction.triggered)
        {
            gameManager.game_State = GameManager.GameState.Select;
            SceneManager.LoadScene("MainScene");
        }
    }
}
