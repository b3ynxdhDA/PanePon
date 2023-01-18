using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// パネポンのゲーム本編で使用されるUI
/// を管理するクラス
/// </summary>
public class PanePonUI : MonoBehaviour
{
    private PanePonUI instance = null;
    public PanePonUI Instance { get { return instance; } set { instance = value; } }

    //連鎖数
    public int _chainCount = 0;

    //パネルが連載中かどうか
    public bool _isSomePanelErasing = false;

    //タイマー
    private float _timerCount = 0;

    const int ONE_MINUTES = 60;

    //ゲームスタートのカウント
    [SerializeField] private Text _startCountText = default;

    //リザルトテキスト
    [SerializeField] private GameObject _resultText = default;

    //リザルトテキストの初期選択ボタン
    [SerializeField] private GameObject _resultSelectedButton = default;

    //連鎖カウントテキスト
    [SerializeField] private Text _chainCountText = default;

    //ハイスコアテキスト
    [SerializeField] private Text _scoreCountText = default;

    //タイマーテキスト
    [SerializeField] private Text _timerCountText = default;

    private void Awake()
    {
        //リザルトを非表示に
        _resultText.gameObject.SetActive(false);
    }
    private void Update()
    {
        //連鎖中しか表示しない
        _chainCountText.gameObject.SetActive(_isSomePanelErasing);

        _chainCountText.text = "" + _chainCount;

        //ハイスコアの更新
        _scoreCountText.text = "" + GameManager.instance._highScore;

        if (GameManager.instance.game_State == GameManager.GameState.GameNow)
        {
            //タイマーの更新
            _timerCount += Time.deltaTime;
            _timerCountText.text = "" + ((int)_timerCount / ONE_MINUTES).ToString("00") + " : " + ((int)_timerCount % ONE_MINUTES).ToString("00");
        }
    }
    /// <summary>
    /// カウントダウンコルーチンを開始
    /// </summary>
    public void StartCountCoroutine()
    {
        StartCoroutine(CountdownCoroutine());
    }
    IEnumerator CountdownCoroutine()
    {
        //_imageMask.gameObject.SetActive(true);
        _startCountText.gameObject.SetActive(true);

        _startCountText.text = "3";
        GameManager.instance._seManager.OnStartCount3_SE();
        yield return new WaitForSeconds(1.0f);

        _startCountText.text = "2";
        GameManager.instance._seManager.OnStartCount3_SE();
        yield return new WaitForSeconds(1.0f);

        _startCountText.text = "1";
        GameManager.instance._seManager.OnStartCount3_SE();
        yield return new WaitForSeconds(1.0f);

        _startCountText.text = "GO!";
        GameManager.instance._seManager.OnStartCountGo_SE();
        yield return new WaitForSeconds(1.0f);

        _startCountText.text = "";
        _startCountText.gameObject.SetActive(false);
        //_imageMask.gameObject.SetActive(false);
        GameManager.instance.game_State = GameManager.GameState.GameNow;
    }
    /// <summary>
    /// リザルトを表示
    /// </summary>
    public void ResultUI()
    {
        _resultText.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_resultSelectedButton);
    }
}
