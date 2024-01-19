using System;
using System.Collections.Generic;
using UnityEngine;
using DataSystem;
namespace BuffSystem
{
    public class BuffObject
    {
        public enum buffType //区分加法和乘法
        {
            Addition, //加法，移除buff时候减去buff值
            Multiplication, //乘法，移除buff时候除以buff值
            Equal, //等于，移除buff时不恢复原值
            OneTimeAddition, //一次性加法，移除buff时候不减去buff值
            OneTimeMultiplication, //一次性乘法，移除buff时候不除以buff值
            ContinuousAddition, //持续加法
            ContinuousMultiplication //持续乘法
        }
        public buffType m_buffType;
        public int buffID;
        public string buffName;
        public float timer;
        public float duration;
        public DataObject target_object;
        public Dictionary<string, float> target_effects; //目标数值的名称和对应的buff值

        public BuffObject()
        {

        }

        public virtual void OnAdd() //由 target object 在添加此buff时候调用
        {
            target_object = GameObject.Find("逻辑控件").GetComponent<DataManager>().GetInstance();
            if (m_buffType == buffType.Addition || m_buffType == buffType.ContinuousAddition || m_buffType == buffType.OneTimeAddition)
            {
                foreach (KeyValuePair<string, float> kvp in target_effects)
                {
                    target_object._data[kvp.Key] += kvp.Value;
                }
            }
            else if (m_buffType == buffType.Multiplication || m_buffType == buffType.ContinuousMultiplication || m_buffType == buffType.OneTimeMultiplication)
            {
                foreach (KeyValuePair<string, float> kvp in target_effects)
                {
                    target_object._data[kvp.Key] *= kvp.Value;
                }
            }
            else if (m_buffType == buffType.Equal)
            {
                foreach (KeyValuePair<string, float> kvp in target_effects)
                {
                    target_object._data[kvp.Key] = kvp.Value;
                }
            }
            else if (m_buffType == buffType.OneTimeAddition)
            {
                foreach (KeyValuePair<string, float> kvp in target_effects)
                {
                    target_object._data[kvp.Key] += kvp.Value;
                }
            }
            else if (m_buffType == buffType.OneTimeMultiplication)
            {
                foreach (KeyValuePair<string, float> kvp in target_effects)
                {
                    target_object._data[kvp.Key] *= kvp.Value;
                }
            }
            timer = 1;
        }
        public virtual void OnUpdate() //在 target object 的 Refresh() 中调用
        {
            timer += 1;
            if (timer > duration)
            {
                OnRemove(); //超时，移除buff
            }
            else //刷新目标的数据
            {
                if (m_buffType == buffType.ContinuousAddition)
                {
                    foreach (KeyValuePair<string, float> kvp in target_effects)
                    {
                        target_object._data[kvp.Key] += kvp.Value;
                    }
                }
                else if (m_buffType == buffType.ContinuousMultiplication)
                {
                    foreach (KeyValuePair<string, float> kvp in target_effects)
                    {
                        target_object._data[kvp.Key] *= kvp.Value;
                    }
                }
            }
        }
        public virtual void OnRemove() //由 target object 移除此buff时使用
        {
            //告知目标移除buff
            if (m_buffType == buffType.Addition)
            {
                foreach (KeyValuePair<string, float> kvp in target_effects)
                {
                    target_object._data[kvp.Key] -= kvp.Value;
                }
            }
            else if (m_buffType == buffType.Multiplication)
            {
                foreach (KeyValuePair<string, float> kvp in target_effects)
                {
                    target_object._data[kvp.Key] /= kvp.Value;
                }
            }
            target_object._buffsToRemove.Add(buffID);
        }
        public virtual void OnPrint() //在 target object 的 Reveal() 中调用
        {
            string dataStr = "";
            dataStr += "BuffID: " + buffID + "\n";
            dataStr += "BuffName: " + buffName + "\n";
            dataStr += "BuffType: " + m_buffType + "\n";
            dataStr += "Duration: " + duration + "\n";
            dataStr += "TargetObject: " + target_object + "\n";
            dataStr += "TargetEffects: " + "\n";
            foreach (KeyValuePair<string, float> kvp in target_effects)
            {
                dataStr += kvp.Key + ":" + kvp.Value + ", ";
            }
            Debug.Log(dataStr);
        }
    }
}