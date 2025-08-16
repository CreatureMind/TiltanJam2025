using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class TaskManager : MonoBehaviour
{
    private static TaskManager instance;
    protected virtual void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public static TaskManager Get() => instance;

    [SerializeField] private List<FileHandler> allFiles = new();
    public float maxMemoryCapacity;
    [ReadOnly] public float memoryCapacity;

    [SerializeField] private TextMeshProUGUI bgText;

    [SerializeField] GameObject wirePrefab;
    [SerializeField] private WeightedList<InternetWireScriptableObject> possibleWires;
    [SerializeField] private FileGenerator generator;

    [SerializeField] private Volume volume;

    [SerializeField] private GameObject gameOverCanvas;

    private bool isDead;

    public event UnityAction OnGameOver;
    public event UnityAction OnMemoryChanged;

    public int WireCompleteCount { get; private set; }

    private void Start()
    {
        generator.OnNewFilePooled += OnFilePooled;
    }

    public void AddFile(FileHandler file)
    {
        if (allFiles.Contains(file)) return;

        allFiles.Add(file);
        file.OnConsumed += fileConsumed => RemoveFile(fileConsumed);

        OnFilesChanged();
    }

    public void RemoveFile(FileHandler file)
    {
        allFiles.Remove(file);

        OnFilesChanged();
    }

    void OnFilesChanged()
    {
        memoryCapacity = allFiles.Sum(e => e.file.size);

        if (isDead) return;
        OnMemoryChanged?.Invoke();

        if (volume.profile.TryGet(out Vignette vignette)){
            float expoVal = NormalizedExp(memoryCapacity / maxMemoryCapacity, 8);

            vignette.color.Override(Color.Lerp(Color.black, Color.red, expoVal));

            vignette.intensity.Override(Mathf.Lerp(.25f, .4f, expoVal));
        }

        if (maxMemoryCapacity <= memoryCapacity && !isDead)
        {
            isDead = true;
            OnGameOver?.Invoke();
            gameOverCanvas.SetActive(true);
        }
    }

    void OnFilePooled(FileHandler file)
    {
        var wire = Instantiate(wirePrefab).GetComponent<InternetWire>();
        wire.SetupWithSettings(possibleWires.ChooseRandom());
        wire.requiredIPs.Add(file.IP);
        wire.transform.position = new Vector3(Random.Range(-7.5f, 7.5f), 8.85f, 0);
        wire.Outlines.ForEach(outline => outline.color = file.fileOutline.color);
        wire.OnWireComplete += OnWireComplete;
    }

    public void WriteToConsole(object message)
    {
        bgText.text += "[CONSOLE]: " + message.ToString() + '\n';
    }

    void OnWireComplete(InternetWire wire)
    {
        WireCompleteCount++;
    }

    public void ResetGame()
    {
        instance = null;
        SceneManager.LoadScene("Presentation");
    }

    float ExpFunction(float x, float k) => Mathf.Exp(k * (x - 1f));
    float NormalizedExp(float x, float k) => (Mathf.Exp(k * (x - 1f)) - Mathf.Exp(-k)) / (1f - Mathf.Exp(-k));
}
