using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class FileGenerator : MonoBehaviour
{
    public FileHandler filePrefab;
    public Transform spawnPoint;

    public List<FileHandler> FilesToGenerate { get; private set; } = new();
    public int maxFilesToGenerate = 10;
    public int filesGenerated = 0;
    public float timeToGenerate = 10;
    public List<FileHandler> GeneratedFiles { get; private set; } = new();
    
    public event UnityAction<FileHandler> OnNewFilePooled;

    Coroutine spawnPool;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
    
    private IEnumerator GenerateFileSlowly()
    {
        while (true)
        {
            //Debug.Log(timeToGenerate * Mathf.Pow(Mathf.Pow(0.5f, 1f / 10f), TaskManager.Get().WireCompleteCount));
            yield return new WaitForSeconds(timeToGenerate * Mathf.Pow(Mathf.Pow(0.5f, 1f / 10f), TaskManager.Get().WireCompleteCount));
            InstantiateFile();
        }
    }

    IEnumerator AutoReleaseFromPool(FileHandler file)
    {
        //Debug.Log(timeToGenerate * Mathf.Pow(Mathf.Pow(0.75f, 1f / 10f), TaskManager.Get().WireCompleteCount));
        yield return new WaitForSeconds(UnityEngine.Random.Range(1, timeToGenerate * Mathf.Pow(Mathf.Pow(0.75f, 1f / 10f), TaskManager.Get().WireCompleteCount)));
        SpawnFile(file);
    }
}
