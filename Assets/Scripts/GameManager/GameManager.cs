using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // 定义存储的键名，防止拼写错误
    private const string PREF_POS_X = "CheckpointX";
    private const string PREF_POS_Y = "CheckpointY";
    private const string PREF_POS_Z = "CheckpointZ";
    private const string PREF_HAS_CHECKPOINT = "HasCheckpoint";

    private void Awake()
    {
        // 单例模式：保证全局只有一个GameManager
        if (Instance == null)
        {
            Instance = this;
            // 如果希望跨关卡保留数据，可以取消下面这行的注释
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;
        // 场景加载开始时，尝试恢复玩家位置
        LoadCheckpoint();
    }

    // --- 1. 保存存档点 ---
    public void SaveCheckpoint(Vector3 position)
    {
        PlayerPrefs.SetFloat(PREF_POS_X, position.x);
        PlayerPrefs.SetFloat(PREF_POS_Y, position.y);
        PlayerPrefs.SetFloat(PREF_POS_Z, position.z);
        
        // 标记我们已经有了存档
        PlayerPrefs.SetInt(PREF_HAS_CHECKPOINT, 1);
        
        // 强制写入磁盘
        PlayerPrefs.Save();
        Debug.Log("存档成功：坐标 " + position);
    }

    // --- 2. 加载并恢复玩家位置
    public void LoadCheckpoint()
    {
        // 1. 检查是否有存档
        if (PlayerPrefs.GetInt("HasCheckpoint", 0) == 1)
        {
            float x = PlayerPrefs.GetFloat("CheckpointX");
            float y = PlayerPrefs.GetFloat("CheckpointY");
            float z = PlayerPrefs.GetFloat("CheckpointZ");
            Vector3 spawnPos = new Vector3(x, y, z);

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            
            if (player != null)
            {
                // 获取 CharacterController 组件
                CharacterController cc = player.GetComponent<CharacterController>();

                if (cc != null)
                {
                    // 【关键步骤】
                    // 1. 必须先禁用 CharacterController，否则它会覆盖你的坐标修改
                    cc.enabled = false;

                    // 2. 修改位置
                    player.transform.position = spawnPos;

                    // 3. 重新启用 CharacterController
                    cc.enabled = true;
                }
                else
                {
                    // 如果没找到组件，则回退到普通移动方式
                    player.transform.position = spawnPos;
                }

                Debug.Log("玩家已复活，位置：" + spawnPos);
            }
        }
    }

    // --- 3. 供UI调用的“继续游戏”方法 ---
    public void ReloadScene()
    {
        // 获取当前场景名字并重新加载
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // (可选) 新游戏开始时清除存档
    public void ClearCheckpoint()
    {
        PlayerPrefs.DeleteAll();
    }
}