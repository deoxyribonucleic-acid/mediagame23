using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BuffSystem;
namespace DataSystem
{
    public class DataObject : MonoBehaviour
    {

        public Dictionary<string, float> _data;
        public Dictionary<int, BuffObject> _buffs;
        public List<int> _buffsToRemove = new List<int>();


        void Awake() //这里用Start会出bug，因为Start在第一帧刷新前才调用
        {
            _data = new Dictionary<string, float>();
            _buffs = new Dictionary<int, BuffObject>();
            this._data.Add("热度", 1000);
            this._data.Add("心理压力", 10);
            this._data.Add("取证进度", 0);
            this._data.Add("热度增速", -0.05f);
            this._data.Add("热度等级", 0);
            this._data.Add("压力增速", -0.05f);
            this._data.Add("精力", 100);
            this._data.Add("上热搜了", 0);
            this._data.Add("死亡方式", -1);
        }
        public void Refresh()
        {
            if (_buffs.Count > 0)
            {
                foreach (KeyValuePair<int, BuffObject> kvp in _buffs)
                {
                    kvp.Value.OnUpdate();
                    if (kvp.Value.timer > kvp.Value.duration)
                    {
                        _buffsToRemove.Add(kvp.Key);
                    }
                }
                foreach (int buffID in _buffsToRemove)
                {
                    _buffs.Remove(buffID);
                }
                _buffsToRemove.Clear();
            }
            this._data["热度"] *= 1 + this._data["热度增速"];
            // 压力上限100 超出有buff
            this._data["心理压力"] = this._data["心理压力"] + this._data["压力增速"];
            // 每回合恢复100精力,上限100
            this._data["精力"] = Mathf.Min(this._data["精力"] + 100, 100);
        }

        public string PrintBuff()
        {
            string buffStr = "";
            foreach (KeyValuePair<int, BuffObject> kvp in _buffs)
            {
                buffStr += kvp.Value.buffName + ":" + kvp.Value.timer + "/" + kvp.Value.duration + "\n";
                foreach (KeyValuePair<string, float> kvp2 in kvp.Value.target_effects)
                {
                    buffStr += kvp2.Key + ":";
                    if (kvp.Value.m_buffType == BuffObject.buffType.Addition)
                     {
                        buffStr += "增加" + kvp2.Value + "\n";
                    }
                    else if (kvp.Value.m_buffType == BuffObject.buffType.Multiplication)
                    {
                        buffStr += "增加" + kvp2.Value*100 + "%\n";
                    }
                    else if (kvp.Value.m_buffType == BuffObject.buffType.Equal)
                    {
                        buffStr += + kvp2.Value + "\n";
                    }
                    else if (kvp.Value.m_buffType == BuffObject.buffType.ContinuousAddition)
                    {
                        buffStr += "每回合增加" + kvp2.Value + "\n";
                    }
                    else if (kvp.Value.m_buffType == BuffObject.buffType.ContinuousMultiplication)
                    {
                        buffStr += "每回合增加" + kvp2.Value*100 + "%\n";
                    }
                }
            }
            return buffStr;
        }

        // 获取指定键的数据值
        public float GetData(string key)
        {
            if (_data.ContainsKey(key))
            {
                return _data[key];
            }
            else
            {
                return 0;
            }
        }

        // 设置指定键的数据值
        public void SetData(string key, float value)
        {
            if (_data.ContainsKey(key))
            {
                _data[key] = value;
            }
            else
            {
                _data.Add(key, value);
            }
        }
        public void Reset()
        {
            this._data["热度"] = 1000;
            this._data["心理压力"] = 10;
            this._data["取证进度"] = 0;
            this._data["热度增速"] = -0.05f;
            this._data["热度等级"] = 0;
            this._data["压力增速"] = -0.05f;
            this._data["精力"] = 100;
            this._data["上热搜了"] = 0;
            this._data["死亡方式"] = -1;
            this._buffs.Clear();
            this._buffsToRemove.Clear();
        }
    }
}