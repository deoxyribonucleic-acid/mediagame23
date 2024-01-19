using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using DataSystem;
namespace BuffSystem
{
    class BuffManager : MonoBehaviour
    {
        public static Dictionary<int, BuffObject> allBuffs = new Dictionary<int, BuffObject>();
        public Dictionary<int, BuffObject> activeBuffs = new Dictionary<int, BuffObject>();
        public List<int> buffsToRemove = new List<int>();

        public void PrintBuffs()
        {
            foreach (KeyValuePair<int, BuffObject> kvp in allBuffs)
            {
                kvp.Value.Print();
            }
        }

        public string VisualizeActiveBuffs()
        {
            string str = "";
            foreach (KeyValuePair<int, BuffObject> kvp in activeBuffs)
            {
                str += kvp.Value.Visualize();
            }
            return str;
        }

        public void ActivateBuff(int buffID, int duration = -1)
        {
            BuffObject buff = allBuffs[buffID];
            // 判断：如果buff是一次性buff，那么不需要添加到buff列表中
            if (buff.buffType != BuffObject.BuffTypes.OneTimeAddition && buff.buffType != BuffObject.BuffTypes.OneTimeMultiplication)
            {
                activeBuffs.Add(buffID, buff); //将buff添加到buff列表中
                if (duration != -1)
                {
                    buff.duration = duration;
                }
            }
            //调用buff的OnAdd()函数
            buff.OnAdd();
            gameObject.GetComponent<GameLogic>().UpdateUI();
        }
        public void DeactivateBuff(int buffID)
        {
            allBuffs[buffID].OnRemove();
            buffsToRemove.Add(buffID);
            gameObject.GetComponent<GameLogic>().UpdateUI();
        }
        public void Refresh()
        {
            if (activeBuffs.Count > 0)
            {
                foreach (KeyValuePair<int, BuffObject> kvp in activeBuffs)
                {
                    if (kvp.Value.timer >= kvp.Value.duration)
                    {
                        kvp.Value.OnRemove();
                        buffsToRemove.Add(kvp.Key);
                    }
                    kvp.Value.OnUpdate();
                }
                if (buffsToRemove.Count > 0)
                {
                    foreach (int buffID in buffsToRemove)
                    {
                        activeBuffs.Remove(buffID);
                    }
                    buffsToRemove.Clear();
                }
            }
        }
        public void Reset()
        {
            allBuffs.Clear();
            activeBuffs.Clear();
            buffsToRemove.Clear();
        }

        public void ReadBuffsFromJson(string jsonpath)
        {
            string json_str = System.IO.File.ReadAllText(jsonpath);
            JsonTextReader reader = new JsonTextReader(new StringReader(json_str));
            BuffObject curBuff = null;
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.StartObject)
                {
                    curBuff = new BuffObject();
                }
                else if (reader.TokenType == JsonToken.EndObject && curBuff != null)
                {
                    allBuffs.Add(curBuff.buffID, curBuff);
                }
                else if (reader.TokenType == JsonToken.PropertyName)
                {
                    string property = reader.Value.ToString();
                    switch (property)
                    {
                        case "BuffID":
                            curBuff.buffID = reader.ReadAsInt32().Value;
                            break;
                        case "BuffName":
                            curBuff.buffName = reader.ReadAsString();
                            break;
                        case "BuffType":
                            string typeStr = reader.ReadAsString();
                            BuffObject.BuffTypes newType = (BuffObject.BuffTypes)System.Enum.Parse(typeof(BuffObject.BuffTypes), typeStr);
                            curBuff.buffType = newType;
                            break;
                        case "Duration":
                            curBuff.duration = (float)reader.ReadAsDouble().Value;
                            break;
                        case "TargetEffects":
                            Dictionary<string, float> effects = new Dictionary<string, float>();
                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonToken.PropertyName)
                                {
                                    string target_effect = reader.Value.ToString();
                                    float target_effect_value = (float)reader.ReadAsDouble().Value;
                                    effects.Add(target_effect, target_effect_value);
                                }
                                else if (reader.TokenType == JsonToken.EndObject)
                                {
                                    break;
                                }
                            }
                            curBuff.effects = effects;
                            curBuff.targetObject = gameObject.GetComponent<DataObject>();
                            break;
                        default:
                            throw new System.Exception("BuffManager ReadBuffsFromJson Error: Unexpected property " + property);
                    }
                }
            }
        }
    }
}