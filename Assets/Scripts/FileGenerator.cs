using System;
using UnityEngine;
using UnityEngine.Events;

public class FileGenerator : MonoBehaviour
{
    public FileHandler filePrefab;
    public Transform spawnPoint;
    
    public event UnityAction generateFile;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        generateFile += GenerateFile;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            GenerateFile();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out FileHandler file))
            file.OnEntered();
    }

    private void GenerateFile()
    {
        var file = Instantiate(filePrefab);
        file.transform.position = spawnPoint.position;
        file.transform.rotation = spawnPoint.rotation;
        file.moveDirection = (transform.position - spawnPoint.position).normalized;
    }
}
