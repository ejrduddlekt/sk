using Data;
using System.Collections.Generic;
using UnityEngine;

public class LotsController : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject lotItemPrefab;

    public List<GameObject> lotItems;

    private void Start()
    {
        lotItems = new List<GameObject>();
    }

    public void SetLotItems(List<Wafer> waferList)
    {
        if (waferList.Count == 0)
            return;

        foreach (GameObject lotItem in lotItems)
            lotItem.SetActive(false);

        for (int i = 0; i < waferList.Count; i++)
        {
            if (lotItems.Count <= i)
            {
                GameObject lotItem = Instantiate(lotItemPrefab, content);
                lotItems.Add(lotItem);
            }

            lotItems[i].SetActive(true);
        }
    }
}
