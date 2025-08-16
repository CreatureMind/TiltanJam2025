using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AdressColor
{
    public string ip;
    public Color color = Color.white;
}
public class FileGenerator : MonoBehaviour
{
    public FileHandler filePrefab;
    public Transform spawnPoint;
    public AdressColor[] colors;

    public List<FileHandler> FilesToGenerate { get; private set; } = new();
    public int maxFilesToGenerate = 10;
    public int filesGenerated = 0;

    // Baseline (early-game) interval, seconds
    public float timeToGenerate = 10f;

    // New pacing controls
    [Header("Pacing")]
    [Tooltip("Minimum interval the game asymptotically approaches (seconds).")]
    public float minInterval = 3.0f;

    [Tooltip("Wires cleared needed to halve the interval (higher = gentler ramp).")]
    public float halfLifeWires = 20f;

    // Optional: light slowdown as pool fills (0 = off, 1 = strong)
    [Range(0f, 1f)]
    public float poolPressure = 0.25f;

    public List<FileHandler> GeneratedFiles { get; private set; } = new();
    
    public event UnityAction<FileHandler> OnNewFilePooled;

    Coroutine spawnPool;
    
    void Start()
    {
        spawnPool = StartCoroutine(GenerateFileSlowly());
        TaskManager.Get().OnGameOver += () => StopCoroutine(spawnPool);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out FileHandler file))
            file.OnEntered();
    }

    private void InstantiateFile()
    {
        if (FilesToGenerate.Count >= maxFilesToGenerate) return;
        filesGenerated++;
        var file = Instantiate(filePrefab);
        file.gameObject.SetActive(false);
        FilesToGenerate.Add(file);
        file.SetupForRandomFile();
        file.SetColorAdress(colors[UnityEngine.Random.Range(0, colors.Length)]);

        OnNewFilePooled?.Invoke(file);

        TaskManager.Get().WriteToConsole($"receiving new packet...");

        StartCoroutine(AutoReleaseFromPool(file));
    }

    private void SpawnFile(FileHandler file)
    {
        if (FilesToGenerate.Count == 0 || !FilesToGenerate.Contains(file)) return;
        
        file.gameObject.SetActive(true);
        GeneratedFiles.Add(file);
        FilesToGenerate.Remove(file);
        
        file.transform.position = spawnPoint.position;
        file.transform.rotation = spawnPoint.rotation;
        file.moveDirection = (transform.position - spawnPoint.position).normalized;

        TaskManager.Get().WriteToConsole($"Packet '{file.file.fileName}' recieved!");
    }

    // New: smoothly decaying interval with a floor and optional pool pressure
    private float GetBaseInterval()
    {
        int wires = TaskManager.Get().WireCompleteCount;

        // Exponential decay with half-life on wires: interval = L + (T0 - L) * exp(-k * wires)
        float k = Mathf.Log(2f) / Mathf.Max(1f, halfLifeWires);
        float interval = minInterval + (timeToGenerate - minInterval) * Mathf.Exp(-k * wires);

        if (poolPressure > 0f && maxFilesToGenerate > 0)
        {
            // As pool fills, slow down slightly to prevent runaway pile-ups
            float fill = Mathf.Clamp01((float)FilesToGenerate.Count / maxFilesToGenerate);
            float pressure = 1f + poolPressure * fill; // 1..1+poolPressure
            interval *= pressure;
        }

        return Mathf.Max(0.1f, interval); // safety clamp
    }
    
    private IEnumerator GenerateFileSlowly()
    {
        while (true)
        {
            float wait = GetBaseInterval();
            yield return new WaitForSeconds(wait);
            InstantiateFile();
        }
    }

    IEnumerator AutoReleaseFromPool(FileHandler file)
    {
        // Release within [0.5x .. 1.0x] of the current base interval for a natural stagger
        float baseInterval = GetBaseInterval();
        float releaseDelay = Mathf.Lerp(0.5f * baseInterval, baseInterval, UnityEngine.Random.value);
        yield return new WaitForSeconds(releaseDelay);
        SpawnFile(file);
    }
}