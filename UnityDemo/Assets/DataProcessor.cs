using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
 
 
public class DataProcessor : MonoBehaviour
{
    public ChatPanelManager cpm;
    private int count;
    private Dictionary<string, string> huashuDic = new Dictionary<string, string>();
    void Start()
    {
        cpm.Init();
        InitData();
        
    }
    
    private void InitData()
    {
        huashuDic = new Dictionary<string, string>();
        var a = CSVParser.ConvertCsv(Resources.Load<TextAsset>("huashu").text);
        for (int i = 1; i < a.Length -1; i++)
        {
            var data = a[i].Split(',');
            if (data[7] == "AI")
            {
                huashuDic.Add(data[8], data[1]);
            }
        }
    }

    
    public string LookUpData(string result)
    {
        var a = huashuDic.ToLookup(x => x.Value,
            x => x.Key).Where(x => x.Count() > 1);
        foreach(var item in a)
        {
            if (item.Key == result)
            {
                var random = Random.Range(0, item.Count());
                var count = 0;
                foreach (var h in item)
                {
                    if (count == random)
                    {
                        return h;
                    }
                    count++;
                }
            }
        }

        return null;
    }
    
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     cpm.AddBubble(dialogue[count],Random.Range(0,2)>0);
        //     count++;
        //     if (count > dialogue.Count-1)
        //     {
        //         count = 0;
        //     }
        // }
    }
}
