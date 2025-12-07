using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 1. 必须导入这个命名空间来管理场景
using UnityEngine.SceneManagement; 

public class SceneLoader : MonoBehaviour
{
    // 2. 我们将创建一个公共函数，这样 Unity 的按钮才能“看”到它
    public void LoadTargetScene(string sceneName)
    {
        // 3. 使用 SceneManager 来加载你指定的场景
        // 这里的 sceneName 将从 Unity 编辑器中传入
        SceneManager.LoadScene(sceneName);
    }
}