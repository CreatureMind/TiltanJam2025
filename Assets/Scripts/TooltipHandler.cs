using TMPro;
using UnityEngine;

public class TooltipHandler : MonoBehaviour
{
    public Canvas canvas;
    public TMP_Text ipText;
    public TMP_Text sizeText;
    public Animator animator;
    public FileHandler file;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvas.worldCamera = Camera.main;
        gameObject.SetActive(false);
        animator.speed = 2;
    }

    public void ShowTooltip()
    {
        ipText.text = file.IP;
        sizeText.text = $"File size: {file.file.size} mb";
        gameObject.SetActive(true);
        animator.SetBool("Show", true);
        animator.SetBool("Hide", false);
    }
    
    public void HideTooltip()
    {
        animator.SetBool("Hide", true);
        animator.SetBool("Show", false);
    }
}
