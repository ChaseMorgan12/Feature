using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance {  get { return instance; } }

    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private GameObject arrowPrefab;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance.gameObject);
        }
    }

    public void ShowArrow(RectTransform t)
    {

    }

    public void UpdateArrow(RectTransform t)
    {

    }
}
