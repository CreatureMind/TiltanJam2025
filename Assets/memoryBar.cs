using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class memoryBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        TaskManager.Get().OnMemoryChanged += UpdateMemoryData;
        UpdateMemoryData();
    }

    void UpdateMemoryData()
    {
        float range01 = TaskManager.Get().memoryCapacity / TaskManager.Get().maxMemoryCapacity;
        slider.value = range01;

        text.text = $"{TaskManager.Get().memoryCapacity}/{TaskManager.Get().maxMemoryCapacity}MB";
    }
}
