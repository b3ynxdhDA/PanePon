using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームのSEを管理するクラス
/// </summary>
public class SEManager : MonoBehaviour
{
    private AudioSource _audioSource;

    //決定のSE
    [SerializeField] private AudioClip _onDecision = default;

    //カーソル移動のSE
    [SerializeField] private AudioClip _moveCorsor = default;

    //パネル入れ替えのSE
    [SerializeField] private AudioClip _swap = default;

    //パネルの消滅のSE
    [SerializeField] private AudioClip _panelErase = default;

    //カウントダウンのSE
    [SerializeField] private AudioClip _startCount3 = default;

    //ゲームスタートのSE
    [SerializeField] private AudioClip _startCount_Go = default;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 決定SEを鳴らす
    /// </summary>
    public void OnDecision_SE()
    {
        _audioSource.PlayOneShot(_onDecision);
    }
    /// <summary>
    /// カーソル移動SEを鳴らす
    /// </summary>
    public void OnMoveCorsor_SE()
    {
        _audioSource.PlayOneShot(_moveCorsor);
    }
    /// <summary>
    /// 入れ替えSEを鳴らす
    /// </summary>
    public void OnSwap_SE()
    {
        _audioSource.PlayOneShot(_swap);
    }
    /// <summary>
    /// パネルの消滅SEを鳴らす
    /// </summary>
    public void OnPanelErase_SE()
    {
        _audioSource.PlayOneShot(_panelErase);
    }
    /// <summary>
    /// スタートカウントダウンの三秒のSEを鳴らす
    /// </summary>
    public void OnStartCount3_SE()
    {
        _audioSource.PlayOneShot(_startCount3);
    }
    /// <summary>
    /// スタートカウントダウンの最後のゲーム開始SEを鳴らす
    /// </summary>
    public void OnStartCountGo_SE()
    {
        _audioSource.PlayOneShot(_startCount_Go);
    }
}
