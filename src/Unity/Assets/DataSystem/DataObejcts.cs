using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BuffSystem;
namespace DataSystem
{
    public class DataObject : MonoBehaviour
    {

        public Dictionary<string, float> dataDict;

        void Awake() //这里用Start会出bug，因为Start在第一帧刷新前才调用
        {
            dataDict = new Dictionary<string, float>();
            this.dataDict.Add("热度", 1000);
            this.dataDict.Add("心理压力", 10);
            this.dataDict.Add("取证进度", 0);
            this.dataDict.Add("热度增速", -0.05f);
            this.dataDict.Add("热度等级", 0);
            this.dataDict.Add("压力增速", -0.05f);
            this.dataDict.Add("精力", 100);
            this.dataDict.Add("上热搜了", 0);
            this.dataDict.Add("死亡方式", -1);
        }
        public void Refresh()
        {
            this.dataDict["热度"] *= 1 + this.dataDict["热度增速"];
            // 压力上限100 超出有buff
            this.dataDict["心理压力"] = this.dataDict["心理压力"] + this.dataDict["压力增速"];
            // 每回合恢复100精力,上限100
            this.dataDict["精力"] = Mathf.Min(this.dataDict["精力"] + 100, 100);
        }

        // 获取指定键的数据值
        public float GetData(string key)
        {
            if (dataDict.ContainsKey(key))
            {
                return dataDict[key];
            }
            else
            {
                return 0;
            }
        }

        // 设置指定键的数据值
        public void SetData(string key, float value)
        {
            if (dataDict.ContainsKey(key))
            {
                dataDict[key] = value;
            }
            else
            {
                dataDict.Add(key, value);
            }
        }
        public void Reset()
        {
            this.Awake();
        }
    }
}