using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class memoryBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Gradient fillColor;

    private void Start()
    {
        TaskManager.Get().OnMemoryChanged += UpdateMemoryData;
        UpdateMemoryData();
    }

    void UpdateMemoryData()
    {
        float range01 = TaskManager.Get().memoryCapacity / TaskManager.Get().maxMemoryCapacity;
        slider.value = range01;

        slider.fillRect.GetComponent<Image>().color = fillColor.Evaluate(TaskManager.Get().memoryCapacity / TaskManager.Get().maxMemoryCapacity);
        text.color = fillColor.Evaluate(TaskManager.Get().memoryCapacity / TaskManager.Get().maxMemoryCapacity);

        text.text = $"{TaskManager.Get().memoryCapacity}/{TaskManager.Get().maxMemoryCapacity}MB";
    }
}
