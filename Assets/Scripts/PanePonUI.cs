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

    //ゲームスタートのカウント
    [SerializeField] private Text _startCountText = default;

    //リザルトテキスト
    [SerializeField] private GameObject _resultText = default;

    //リザルトテキストの初期選択ボタン
    [SerializeField] private GameObject _resultSelectedButton = default;

    //連鎖カウントテキスト
    [SerializeField] private Text _chainCountText = default;

    //ハイスコアテキスト
    [SerializeField] private Text _highScoreText = default;

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
        _highScoreText.text = "" + GameManager.instance._highScore;
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
