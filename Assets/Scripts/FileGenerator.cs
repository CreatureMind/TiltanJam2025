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
        
    }

    private void GenerateFile()
    {
        Instantiate(filePrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
