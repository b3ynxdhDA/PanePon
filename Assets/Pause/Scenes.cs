using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// UIのボタンによるシーン遷移などを呼び出すクラス
/// </summary>
public class Scenes : MonoBehaviour
{
    //EventSystemのFirstSelectedに
    //呼び出した時に選択状態にするボタンオブジェクトをアタッチして
    public void OnClose()
    {
        //ポーズUIのアクティブを切り替え
        gameObject.SetActive(!gameObject.activeSelf);

        //ポーズUIが表示されている時は停止
        if (gameObject.activeSelf)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }   
    
    public void OnCheckPoint()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnMain()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void OnTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void OnExit()
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
