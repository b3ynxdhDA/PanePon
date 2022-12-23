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
    private GameManager gameManager = null;

    private PanePonUI instance = null;
    public PanePonUI Instance { get { return instance; } set { instance = value; } }

    //連鎖数
    public int _chainCount = 0;

    //パネルが連載中かどうか
    public bool _isSomePanelErasing = false;

    //ゲームスタートのカウント
    [SerializeField] private Text _startCountText = default;

    //ゲームオーバーテキスト
    [SerializeField] private Text _gameOverText = null;

    //連鎖カウントテキスト
    [SerializeField] private Text _chainCountText = default;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
    }
    private void Update()
    {
        switch (gameManager.game_State)
        {
            case GameManager.GameState.GameRedy:
                StartCoroutine(CountdownCoroutine());
                break;
            case GameManager.GameState.GameNow:
                _chainCountText.gameObject.SetActive(_isSomePanelErasing);
                _chainCountText.text = "" + _chainCount;
                break;
            case GameManager.GameState.GameOver:

                break;
        }

    }
    IEnumerator CountdownCoroutine()
    {
        //_imageMask.gameObject.SetActive(true);
        _startCountText.gameObject.SetActive(true);

        _startCountText.text = "3";
        yield return new WaitForSeconds(1.0f);

        _startCountText.text = "2";
        yield return new WaitForSeconds(1.0f);

        _startCountText.text = "1";
        yield return new WaitForSeconds(1.0f);

        _startCountText.text = "GO!";
        yield return new WaitForSeconds(1.0f);

        _startCountText.text = "";
        _startCountText.gameObject.SetActive(false);
        //_imageMask.gameObject.SetActive(false);
        gameManager.game_State = GameManager.GameState.GameNow;
    }
}
