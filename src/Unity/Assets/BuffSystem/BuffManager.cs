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
        void Start()
        {
        }
        public void PrintBuffs()
        {
            foreach (KeyValuePair<int, BuffObject> kvp in allBuffs)
            {
                kvp.Value.OnPrint();
            }
        }
        public void ActivateBuff(int buffID, int duration = -1)
        {
            //从AllBuffs中实例化一个buff对象
            BuffObject buff = allBuffs[buffID];
            // 判断：如果buff是一次性buff，那么不需要添加到target_object的buff列表中
            if (buff.m_buffType != BuffObject.buffType.OneTimeAddition && buff.m_buffType != BuffObject.buffType.OneTimeMultiplication)
            {
                buff.target_object._buffs.Add(buffID, buff); //将buff添加到目标的buff列表中
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
            BuffObject buff = allBuffs[buffID];
            buff.OnRemove();
            gameObject.GetComponent<GameLogic>().UpdateUI();
        }

        public void Reset()
        {
            allBuffs.Clear();
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
                            string buffType = reader.ReadAsString();
                            BuffObject.buffType m_buffType = (BuffObject.buffType)System.Enum.Parse(typeof(BuffObject.buffType), buffType);
                            curBuff.m_buffType = m_buffType;
                            break;
                        case "Duration":
                            curBuff.duration = (float)reader.ReadAsDouble().Value;
                            break;
                        case "TargetEffects":
                            Dictionary<string, float> target_effects = new Dictionary<string, float>();
                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonToken.PropertyName)
                                {
                                    string target_effect = reader.Value.ToString();
                                    float target_effect_value = (float)reader.ReadAsDouble().Value;
                                    target_effects.Add(target_effect, target_effect_value);
                                }
                                else if (reader.TokenType == JsonToken.EndObject)
                                {
                                    break;
                                }
                            }
                            curBuff.target_effects = target_effects;
                            curBuff.target_object = gameObject.GetComponent<DataObject>();
                            break;
                        default:
                            throw new System.Exception("BuffManager ReadBuffFromJson Error: Unexpected property " + property);
                    }
                }
            }
        }
    }
}