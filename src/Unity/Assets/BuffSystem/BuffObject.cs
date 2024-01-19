using System;
using System.Collections.Generic;
using UnityEngine;
using DataSystem;
namespace BuffSystem
{
    public class BuffObject
    {
        public enum BuffTypes
        {
            Addition, //加法，移除buff时候减去buff值
            Multiplication, //乘法，移除buff时候除以buff值
            Equal, //等于，移除buff时不恢复原值
            OneTimeAddition, //一次性加法，移除buff时候不减去buff值
            OneTimeMultiplication, //一次性乘法，移除buff时候不除以buff值
            ContinuousAddition, //持续加法
            ContinuousMultiplication //持续乘法
        }
        public BuffTypes buffType;
        public int buffID;
        public string buffName;
        public float timer;
        public float duration;
        public DataObject targetObject;
        public Dictionary<string, float> effects; //数值的名称和buffID

        public virtual void OnAdd()
        {
            targetObject = GameObject.Find("逻辑控件").GetComponent<DataManager>().GetInstance();
            if (buffType == BuffTypes.Addition || buffType == BuffTypes.ContinuousAddition || buffType == BuffTypes.OneTimeAddition)
            {
                foreach (KeyValuePair<string, float> kvp in effects)
                {
                    targetObject.dataDict[kvp.Key] += kvp.Value;
                }
            }
            else if (buffType == BuffTypes.Multiplication || buffType == BuffTypes.ContinuousMultiplication || buffType == BuffTypes.OneTimeMultiplication)
            {
                foreach (KeyValuePair<string, float> kvp in effects)
                {
                    targetObject.dataDict[kvp.Key] *= kvp.Value;
                }
            }
            else if (buffType == BuffTypes.Equal)
            {
                foreach (KeyValuePair<string, float> kvp in effects)
                {
                    targetObject.dataDict[kvp.Key] = kvp.Value;
                }
            }
            else if (buffType == BuffTypes.OneTimeAddition)
            {
                foreach (KeyValuePair<string, float> kvp in effects)
                {
                    targetObject.dataDict[kvp.Key] += kvp.Value;
                }
            }
            else if (buffType == BuffTypes.OneTimeMultiplication)
            {
                foreach (KeyValuePair<string, float> kvp in effects)
                {
                    targetObject.dataDict[kvp.Key] *= kvp.Value;
                }
            }
            timer = 1;
        }
        public virtual void OnUpdate() //在 target object 的 Refresh() 中调用
        {
            timer += 1;
                if (buffType == BuffTypes.ContinuousAddition)
                {
                    foreach (KeyValuePair<string, float> kvp in effects)
                    {
                        targetObject.dataDict[kvp.Key] += kvp.Value;
                    }
                }
                else if (buffType == BuffTypes.ContinuousMultiplication)
                {
                    foreach (KeyValuePair<string, float> kvp in effects)
                    {
                        targetObject.dataDict[kvp.Key] *= kvp.Value;
                    }
                }
        }
        public virtual void OnRemove() //由 target object 移除此buff时使用
        {
            //告知目标移除buff
            if (buffType == BuffTypes.Addition)
            {
                foreach (KeyValuePair<string, float> kvp in effects)
                {
                    targetObject.dataDict[kvp.Key] -= kvp.Value;
                }
            }
            else if (buffType == BuffTypes.Multiplication)
            {
                foreach (KeyValuePair<string, float> kvp in effects)
                {
                    targetObject.dataDict[kvp.Key] /= kvp.Value;
                }
            }
        }
        public string Visualize()
        {
            string buffStr = "";
            buffStr += buffName + ":" +timer + "/" + duration + "\n";
            foreach (KeyValuePair<string, float> kvp2 in effects)
            {
                buffStr += kvp2.Key + ":";
                if (buffType == BuffObject.BuffTypes.Addition)
                {
                    buffStr += "增加" + kvp2.Value + "\n";
                }
                else if (buffType == BuffObject.BuffTypes.Multiplication)
                {
                    buffStr += "增加" + kvp2.Value * 100 + "%\n";
                }
                else if (buffType == BuffObject.BuffTypes.Equal)
                {
                    buffStr += +kvp2.Value + "\n";
                }
                else if (buffType == BuffObject.BuffTypes.ContinuousAddition)
                {
                    buffStr += "每回合增加" + kvp2.Value + "\n";
                }
                else if (buffType == BuffObject.BuffTypes.ContinuousMultiplication)
                {
                    buffStr += "每回合增加" + kvp2.Value * 100 + "%\n";
                }
            }
            return buffStr;
        }
        public void Print()
        {
            string dataStr = "";
            dataStr += "BuffID: " + buffID + "\n";
            dataStr += "BuffName: " + buffName + "\n";
            dataStr += "BuffType: " + buffType + "\n";
            dataStr += "Duration: " + duration + "\n";
            dataStr += "TargetObject: " + targetObject + "\n";
            dataStr += "TargetEffects: " + "\n";
            foreach (KeyValuePair<string, float> kvp in effects)
            {
                dataStr += kvp.Key + ":" + kvp.Value + ", ";
            }
            Debug.Log(dataStr);
        }
    }
}