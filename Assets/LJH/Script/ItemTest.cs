using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemTest : MonoBehaviour
{
    // CSVReader.Read 결과
    private List<Dictionary<string, object>> data;

    // 런타임에 만든 ItemInfo 들을 담을 리스트
    public List<ItemInfo> items = new List<ItemInfo>();

    void Awake() 
    {
        // 1) CSV 로드
        data = CSVReader.Read("Item/ItemCSV");

        // 2) Dictionary → ItemInfo 인스턴스로 변환
        foreach (var row in data)
        {
            var info = ScriptableObject.CreateInstance<ItemInfo>();

            // int 로 파싱
            info.itemIndex = Convert.ToInt32(row["Index"]);

            // string 직접 대입
            info.itemName = row["ItemName"].ToString();
            info.itemDescription = row["Description"].ToString();

            // enum 파싱 (대소문자 구분 없이)
            info.itemRarity = (ItemInfo.ItemRarity)
                Enum.Parse(typeof(ItemInfo.ItemRarity), row["ItemGrade"].ToString(), true);
            info.itemUpgradeType = (ItemInfo.ItemUpgradeType)
                Enum.Parse(typeof(ItemInfo.ItemUpgradeType), row["UpgradeType"].ToString(), true);

            // 만든 인스턴스를 리스트에 추가
            items.Add(info);
        }
    }

    private void Start()
    {
        // 제대로 들어갔는지 로그
        for (int i = 0; i < items.Count; i++)
        {
            var it = items[i];
            Debug.Log($"[{i}] {it.itemName} ({it.itemRarity}) → {it.itemUpgradeType}: {it.itemDescription}");
        }
    }
}