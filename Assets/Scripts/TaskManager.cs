using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
    [SerializeField] private float maxMemoryCapacity;
    [SerializeField, ReadOnly] private float memoryCapacity;

    [SerializeField] private TextMeshProUGUI bgText;

    [SerializeField] GameObject wirePrefab;
    [SerializeField] private WeightedList<InternetWireScriptableObject> possibleWires;
    [SerializeField] private FileGenerator generator;

    [SerializeField] private GameObject gameOverCanvas;

    private bool isDead;

    public event UnityAction OnGameOver;

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

        if (maxMemoryCapacity <= memoryCapacity && !isDead)
        {
            isDead = true;
            OnGameOver?.Invoke();
            gameOverCanvas.SetActive(true);
        }
    }

    void OnFilePooled(FileHandler file)
    {
        if (file.file.isVirus) return;

        var wire = Instantiate(wirePrefab).GetComponent<InternetWire>();
        wire.SetupWithSettings(possibleWires.ChooseRandom());
        wire.requiredIPs.Add(file.IP);
        wire.transform.position = new Vector3(Random.Range(-7.5f, 7.5f), 8.85f, 0);
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
        SceneManager.LoadScene(1);
    }
}
