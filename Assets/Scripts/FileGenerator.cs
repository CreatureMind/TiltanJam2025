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
    public bool isGenerating = false;
    public List<FileHandler> GeneratedFiles { get; private set; } = new();
    
    public event UnityAction generateFile;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        generateFile += GenerateFile;
        StartCoroutine(GenerateFileSlowly());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            GenerateFile();
        
        if (isGenerating)
        {
            isGenerating = false;
            SpawnFile();
        }
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
    }
    
    private void SpawnFile()
    {
        if (FilesToGenerate.Count == 0) return;
        
        var file = FilesToGenerate[0];
        file.gameObject.SetActive(true);
        GeneratedFiles.Add(FilesToGenerate[0]);
        FilesToGenerate.RemoveAt(0);
        
        file.transform.position = spawnPoint.position;
        file.transform.rotation = spawnPoint.rotation;
        file.moveDirection = (transform.position - spawnPoint.position).normalized;
        file.SetupForRandomFile();
    }
    
    private IEnumerator GenerateFileSlowly()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeToGenerate);
            InstantiateFile();
        }
    }
    
    private void GenerateFile()
    {
        isGenerating = true;
    }
}
