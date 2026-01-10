using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class SceneLoader : MonoBehaviour
{
    public void LoadTargetScene(string sceneName)
    {
        // 使用 SceneManager 来加载你指定的场景
        // 这里的 sceneName 将从 Unity 编辑器中传入
        SceneManager.LoadScene(sceneName);
    }
    
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}