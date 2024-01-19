using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BuffSystem;
using CustomEventSystem;
namespace DataSystem
{
    public class DataManager : MonoBehaviour
    {
        public int turn = 0;
        DataObject dt_obj;
        EventManager eventManager;
        BuffManager buffManager;
        int cur_stage = 0;

        void Start()
        {
            eventManager = gameObject.GetComponent<EventManager>();
            buffManager = gameObject.GetComponent<BuffManager>();
            dt_obj = gameObject.GetComponent<DataObject>();
        }
        void Update()
        {
        }
        public void Refresh()
        {
            turn++;
            cur_stage = (int)dt_obj.GetData("热度等级");
            dt_obj.Refresh();
            float randomChoice = 0;
            switch (cur_stage)
            {
                case 0:
                    break; // 等级0说明游戏还没开始，不触发
                case 1:
                    break; // 等级-酝酿阶段，不触发
                case 2:
                    // 50%概率触发等级1的骂街
                    randomChoice = Random.Range(0f, 1f);
                    if (randomChoice < 0.5)
                    {
                        eventManager.ShowEvent(6);
                    }
                    break;
                case 3:
                    // 25%概率触发等级2的骂街 25%概率触发门口泼粪 25概率出现门口喷漆
                    randomChoice = Random.Range(0f, 1f);
                    Debug.Log("randomChoice: " + randomChoice);
                    if (randomChoice < 0.25)
                    {
                        eventManager.ShowEvent(7);
                    }
                    else if (randomChoice < 0.5 && randomChoice >= 0.25)
                    {
                        eventManager.ShowEvent(16);
                    }
                    else if (randomChoice < 0.75 && randomChoice >= 0.5)
                    {
                        eventManager.ShowEvent(19);
                    }
                    break;
                case 4:
                    // 前置条件：触发热搜
                    // 20%概率触发等级3的骂街 20%概率触发父亲生病 20%概率触发母亲生病 20%概率触发被辞退
                    //除了骂街 都只能触发一次
                    randomChoice = Random.Range(0f, 1f);
                    bool hot_state = this.dt_obj.GetData("上热搜了") == 1;  //判断是否上热搜
                    if (randomChoice < 0.2)
                    {
                        if (hot_state && eventManager.allEvents.ContainsKey(31) && eventManager.allEvents[31].isTriggered == false)
                        {
                            eventManager.ShowEvent(31);
                        }
                        else
                        {
                            eventManager.ShowEvent(8);
                        }
                    }
                    else if (randomChoice < 0.4 && randomChoice >= 0.2)
                    {
                        if (hot_state && eventManager.allEvents.ContainsKey(33) && eventManager.allEvents[33].isTriggered == false)
                        {
                            eventManager.ShowEvent(33);
                        }
                        else
                        {
                            eventManager.ShowEvent(8);
                        }
                    }
                    else if (randomChoice < 0.6 && randomChoice >= 0.4)
                    {
                        if (hot_state && eventManager.allEvents.ContainsKey(34) && eventManager.allEvents[34].isTriggered == false)
                        {
                            eventManager.ShowEvent(34);
                        }
                        else
                        {
                            eventManager.ShowEvent(8);
                        }
                    }
                    else if (randomChoice < 0.8 && randomChoice >= 0.6)
                    {
                        eventManager.ShowEvent(8);
                    }

                    if (turn >= 35 || (this.eventManager.JudgeCondition(this.eventManager.allEvents[39].conditions) && turn > 30))
                    {
                        this.buffManager.DeactivateBuff(4);
                        this.eventManager.ShowEvent(39);
                    }
                    break;
                case 5:
                    if (turn >= 75)
                    {
                        this.eventManager.ShowEvent(49);
                    }
                    break;
                default:
                    throw new System.Exception("热度等级错误");
            }
        }
        public float GetData(string key)
        {
            return dt_obj.GetData(key);
        }
        public void SetData(string key, float value)
        {
            dt_obj.SetData(key, value);
        }
        public DataObject GetInstance()
        {
            return dt_obj;
        }
        public void Reset()
        {
            dt_obj.Reset();
            turn = 0;
            cur_stage = 0;
        }
    }
}