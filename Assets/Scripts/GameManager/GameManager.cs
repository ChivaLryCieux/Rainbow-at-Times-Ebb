using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // --- Key 定义 ---
    private const string PREF_TOTAL_COUNT = "TotalCheckpoints";
    private const string PREF_CURRENT_INDEX = "CurrentIndex";
    private const string PREF_BASE_X = "CP_{0}_X";
    private const string PREF_BASE_Y = "CP_{0}_Y";
    private const string PREF_BASE_Z = "CP_{0}_Z";

    public int CurrentIndex { get; private set; } = 0;
    public int MaxIndex { get; private set; } = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        //PlayerPrefs.DeleteAll(); // 测试清档，请注释掉
        Time.timeScale = 1f;

        // 1. 全新游戏初始化
        if (!PlayerPrefs.HasKey(PREF_TOTAL_COUNT))
        {
            Debug.Log("初始化新游戏数据...");
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                SavePosition(0, player.transform.position);
                UpdateRuntimeIndices(0, 0);
                SaveMeta(1, 0); // Total=1, Current=0
            }
        }
        else
        {
            // 2. 读取旧数据
            MaxIndex = PlayerPrefs.GetInt(PREF_TOTAL_COUNT, 0) - 1;

            // 容错处理：万一 MaxIndex 读出来小于 0
            if (MaxIndex < 0) MaxIndex = 0;

            if (PlayerPrefs.HasKey(PREF_CURRENT_INDEX))
            {
                CurrentIndex = PlayerPrefs.GetInt(PREF_CURRENT_INDEX);
            }
            else
            {
                CurrentIndex = MaxIndex;
            }

            // 恢复位置
            LoadCheckpoint(CurrentIndex);
        }
    }

    // --- 智能添加存档点 ---
    public void AddCheckpoint(Vector3 position)
    {
        // A. 防重检查：遍历所有已存在的存档点
        // 如果当前触发的位置和之前存过的某个点距离很近，就认为是同一个点
        for (int i = 0; i <= MaxIndex; i++)
        {
            Vector3 savedPos = GetSavedPosition(i);

            // 如果距离小于 2.0f (根据你的触发器大小调整)，视为同一个点
            if (Vector3.Distance(position, savedPos) < 2.0f)
            {
                Debug.Log($"检测到已存在的存档点 (Index: {i})，仅更新当前进度，不新增。");

                // 仅更新当前所在位置
                if (CurrentIndex != i)
                {
                    CurrentIndex = i;
                    PlayerPrefs.SetInt(PREF_CURRENT_INDEX, CurrentIndex);
                    PlayerPrefs.Save();
                }
                return; // 【关键】直接退出，不执行后面的新增代码
            }
        }

        // B. 如果是全新的位置，才执行新增逻辑
        int newIndex = MaxIndex + 1;
        SavePosition(newIndex, position);
        UpdateRuntimeIndices(newIndex, newIndex);
        SaveMeta(MaxIndex + 1, CurrentIndex);

        Debug.Log($"<color=green>新区域解锁！存档点 {newIndex} 已保存</color>");
    }

    public void LoadCheckpoint(int index)
    {
        if (index < 0 || index > MaxIndex) return;

        Vector3 spawnPos = GetSavedPosition(index);
        TeleportPlayer(spawnPos);

        CurrentIndex = index;
        PlayerPrefs.SetInt(PREF_CURRENT_INDEX, CurrentIndex);
        PlayerPrefs.Save();

        Debug.Log(index == 0 ? "已回到出生点" : $"已加载存档点 {index}");
    }

    // --- 辅助方法：读取指定 Index 的坐标 ---
    private Vector3 GetSavedPosition(int index)
    {
        string keyX = string.Format(PREF_BASE_X, index);
        string keyY = string.Format(PREF_BASE_Y, index);
        string keyZ = string.Format(PREF_BASE_Z, index);

        float x = PlayerPrefs.GetFloat(keyX);
        float y = PlayerPrefs.GetFloat(keyY);
        float z = PlayerPrefs.GetFloat(keyZ);

        return new Vector3(x, y, z);
    }

    private void TeleportPlayer(Vector3 targetPos)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            player.transform.position = targetPos;
            if (cc != null) cc.enabled = true;
        }
    }

    private void SavePosition(int index, Vector3 pos)
    {
        PlayerPrefs.SetFloat(string.Format(PREF_BASE_X, index), pos.x);
        PlayerPrefs.SetFloat(string.Format(PREF_BASE_Y, index), pos.y);
        PlayerPrefs.SetFloat(string.Format(PREF_BASE_Z, index), pos.z);
    }

    private void UpdateRuntimeIndices(int max, int current)
    {
        MaxIndex = max;
        CurrentIndex = current;
    }

    private void SaveMeta(int totalCount, int currentIndex)
    {
        PlayerPrefs.SetInt(PREF_TOTAL_COUNT, totalCount);
        PlayerPrefs.SetInt(PREF_CURRENT_INDEX, currentIndex);
        PlayerPrefs.Save();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void ReloadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    [ContextMenu("清除存档")]
    public void DeleteSave()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("存档已清空");
    }
}