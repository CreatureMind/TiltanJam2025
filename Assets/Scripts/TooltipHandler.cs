using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;

public class TooltipHandler : MonoBehaviour
{
    public Canvas canvas;
    public TMP_Text ipText;
    public TMP_Text sizeText;
    public Animator animator;
    public ITipable tipable;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tipable = ValidateParent(transform.parent.gameObject) ? transform.parent.gameObject.GetComponent<ITipable>() : null;
        canvas.worldCamera = Camera.main;
        gameObject.SetActive(false);
        animator.speed = 2;
    }

    public void ShowTooltip()
    {
        if(tipable == null)
            return;

        if (tipable.GetIp() == null)
            ipText.gameObject.SetActive(false);
        else
            ipText.text = tipable.GetIp();
        if (tipable.GetSize() == null)
            sizeText.gameObject.SetActive(false);
        else
            sizeText.text = $"File size: {tipable.GetSize()} mb";
        
        gameObject.SetActive(true);
        animator.SetBool("Show", true);
        animator.SetBool("Hide", false);
    }
    
    public void HideTooltip()
    {
        animator.SetBool("Hide", true);
        animator.SetBool("Show", false);
    }
    
    private bool ValidateParent(GameObject go)
    {
        if (!go) return false;
        return go.GetComponent<ITipable>() != null;
    }
}
