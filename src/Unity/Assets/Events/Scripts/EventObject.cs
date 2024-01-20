using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BuffSystem;
using System.IO;
using Newtonsoft.Json;
namespace CustomEventSystem
{
    // 选项结构体
    public struct Option
    {
        public string name; // 名称
        public string description; // 描述
        public Dictionary<int, int> buffs; // 各个选项附带的buffID和持续时间
        public Dictionary<int, int> upcomingEvents; // 各个选项的接续eventID和延后回合数
        public Option(string name, string description, Dictionary<int, int> buffs, Dictionary<int, int> upcomingEvents)
        {
            this.name = name;
            this.description = description;
            this.buffs = buffs;
            this.upcomingEvents = upcomingEvents;
        }
    }
    // 基础事件类
    public class EventObject
    {
        public enum compareOperator
        {
            GreaterThan,
            LessThan,
            EqualTo,
            GreaterThanOrEqualTo,
            LessThanOrEqualTo
        }
        public int eventID; // 事件ID
        public string title; // 事件标题
        public string content; // 事件文本
        public string image; // 事件插图
        public bool isTriggered; // 事件是否被触发
        public bool isRepeatable; // 事件是否可重复触发
        public int scheduledRound = -1; // 事件被触发的回合数
        public int optionNumbers; // 包含选择数
        public List<Option> eventOptions; // 包含若干选择的数组
        public List<Tuple<string, int, compareOperator>> conditions; // 事件触发条件,包含属性名称，阈值, 比较符
        public void Print()
        {
            string str = "";
            str += "Title: " + this.title + "\n";
            str += "Content: " + this.content + "\n";
            str += "Image: " + this.image + "\n";
            str += "OptionNumbers: " + this.optionNumbers + "\n";
            str += "isTriggered: " + this.isTriggered + "\n";
            str += "isRepeatable: " + this.isRepeatable + "\n";
            str += "scheduledRound: " + this.scheduledRound + "\n";
            str += "EventOptions: " + "\n";
            foreach (Option option in this.eventOptions)
            {
                str += "OptionName: " + option.name + "\n";
                str += "OptionDescription: " + option.description + "\n";
                str += "OptionBuffs: " + "\n";
                if (option.buffs.Count == 0)
                {
                    str += "None" + "\n";
                }
                else
                {
                    foreach (KeyValuePair<int, int> buff in option.buffs)
                    {
                        str += "BuffID: " + buff.Key + ", ";
                        str += "BuffDuration: " + buff.Value + "\n";
                    }
                }
                str += "OptionUpcomingEvents: " + "\n";
                foreach (KeyValuePair<int, int> upcoming_event in option.upcomingEvents)
                {
                    str += "UpcomingEventID: " + upcoming_event.Key + ", ";
                    str += "UpcomingEventDelay: " + upcoming_event.Value + "\n";
                }
            }
            str += "Conditions: " + "\n";
            foreach (Tuple<string, int, compareOperator> condition in this.conditions)
            {
                str += "ConditionTargetDataName: " + condition.Item1 + "\n";
                str += "ConditionThreshold: " + condition.Item2 + "\n";
                str += "ConditionCompareOperator: " + condition.Item3 + "\n";
            }
            Debug.Log(str);
        }
    }
}