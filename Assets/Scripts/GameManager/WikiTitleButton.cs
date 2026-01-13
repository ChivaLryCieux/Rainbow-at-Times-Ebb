using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System; // 必须引用这个才能用 Action

public class WikiTitleButton : MonoBehaviour
{
    public Button btn;
    public TextMeshProUGUI titleText;

    // 接收数据，和点击时的回调函数
    public void Setup(WikiEntryData data, Action<WikiEntryData> onClickAction)
    {
        titleText.text = data.title;
        
        // 清除旧事件，添加新事件
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => 
        {
            // 当被点击时，执行传进来的动作，并把自己的数据传回去
            onClickAction(data);
        });
    }
}