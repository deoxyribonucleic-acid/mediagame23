using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BuffSystem;
using System.IO;
using Newtonsoft.Json;
using DataSystem;
namespace CustomEventSystem
{
    public class EventManager : MonoBehaviour
    {
        private Dictionary<int, EventObject> pendingEvents = new Dictionary<int, EventObject>(); // 待选事件列表
        public Dictionary<int, EventObject> allEvents = new Dictionary<int, EventObject>(); // 所有事件列表
        DataManager dataManager;
        BuffManager buffManager;
        [SerializeField]
        GameObject eventWindow;
        public int userChoiceIndex = -1;
        void Start()
        {
            this.dataManager = gameObject.GetComponent<DataManager>();
            this.buffManager = gameObject.GetComponent<BuffManager>();
            Button[] buttons = eventWindow.transform.Find("Buttons").GetComponentsInChildren<Button>();
            this.ActivateEvent(49,75);
            this.ActivateEvent(48,0);
            this.ActivateEvent(47,0);
            this.ActivateEvent(29,0);
        }
        public bool JudgeCondition(List<Tuple<string, int, EventObject.compareOperator>> conditions)
        {
            if (conditions.Count > 0)
            {
                foreach (Tuple<string, int, EventObject.compareOperator> condition in conditions)
                {
                    switch (condition.Item3)
                    {
                        case EventObject.compareOperator.GreaterThan:
                            if (dataManager.GetData(condition.Item1) <= condition.Item2)
                            {
                                return false;
                            }
                            break;
                        case EventObject.compareOperator.LessThan:
                            if (dataManager.GetData(condition.Item1) >= condition.Item2)
                            {
                                return false;
                            }
                            break;
                        case EventObject.compareOperator.EqualTo:
                            if (dataManager.GetData(condition.Item1) != condition.Item2)
                            {
                                return false;
                            }
                            break;
                        case EventObject.compareOperator.GreaterThanOrEqualTo:
                            if (dataManager.GetData(condition.Item1) < condition.Item2)
                            {
                                return false;
                            }
                            break;
                        case EventObject.compareOperator.LessThanOrEqualTo:
                            if (dataManager.GetData(condition.Item1) > condition.Item2)
                            {
                                return false;
                            }
                            break;
                        default:
                            throw new Exception("Invalid compare operator!");
                    }
                }
            }
            return true;
        }

        void Update()
        {
            if (pendingEvents.Count > 0)
            {
                foreach (KeyValuePair<int, EventObject> pendingEvent in pendingEvents)
                {
                    if ((this.eventWindow.activeSelf != true) && pendingEvent.Value.scheduledTurn <= dataManager.turn && this.JudgeCondition(pendingEvent.Value.conditions))
                    {
                        showWindow(pendingEvent.Value);
                    }
                }
            }
        }

        //强制触发事件
        public void ShowEvent(int eventID)
        {
            EventObject currentEvent = allEvents[eventID];
            showWindow(currentEvent);
        }
        public void ClickChoice(int choiceIndex, int eventID = -1)
        {
            userChoiceIndex = choiceIndex;
            if (eventID >= 0)
            {
                EventObject currentEvent = allEvents[eventID];
                if (userChoiceIndex >= 0 && userChoiceIndex < currentEvent.optionNumbers)
                {
                    Option selectedOption = currentEvent.eventOptions[userChoiceIndex];
                    if (selectedOption.buffs.Count > 0)
                    {
                        foreach (KeyValuePair<int, int> buff in selectedOption.buffs)
                        {
                            buffManager.ActivateBuff(buff.Key, buff.Value);
                        }
                    }
                    if (selectedOption.upcoming_events.Count > 0)
                    {
                        foreach (KeyValuePair<int, int> upcoming_event in selectedOption.upcoming_events)
                        {
                            ActivateEvent(upcoming_event.Key, upcoming_event.Value);
                        }
                    }
                    eventWindow.SetActive(false);
                    if (this.pendingEvents.ContainsKey(eventID))
                    {
                        this.pendingEvents.Remove(eventID);
                    }
                    currentEvent.isTriggered = true;
                    userChoiceIndex = -1;
                    gameObject.GetComponent<GameLogic>().UpdateUI();
                }
            }
        }

        // 设置时间窗口并显示时间窗口
        public void showWindow(EventObject currentEvent)
        {
            eventWindow.transform.Find("Title").GetComponent<Text>().text = currentEvent.title;
            eventWindow.transform.Find("Content").GetComponent<Text>().text = currentEvent.content;
            eventWindow.transform.Find("Image").GetComponent<Image>().sprite = ImageUtil.LoadFromFile(currentEvent.image);
            Transform ButtonPanel = eventWindow.transform.Find("Buttons");
            // 设置选项
            for (int i = 0; i < currentEvent.optionNumbers; i++)
            {
                int choiceIndex = i;
                int eventID = currentEvent.eventID;
                Button choiceButton = ButtonPanel.Find("Choice" + (i + 1)).GetComponent<Button>();
                choiceButton.onClick.RemoveAllListeners();
                choiceButton.onClick.AddListener(() => { ClickChoice(choiceIndex, eventID); });
                choiceButton.GetComponentInChildren<Text>().text = currentEvent.eventOptions[i].name + "\n" + currentEvent.eventOptions[i].description;
            }
            // 调整选项大小和位置，并且将多余的选项隐藏
            if (currentEvent.optionNumbers == 1)
            {
                ButtonPanel.Find("Choice1").gameObject.SetActive(true);
                ButtonPanel.Find("Choice2").gameObject.SetActive(false);
                ButtonPanel.Find("Choice3").gameObject.SetActive(false);
            }
            else if (currentEvent.optionNumbers == 2)
            {
                ButtonPanel.Find("Choice1").gameObject.SetActive(true);
                ButtonPanel.Find("Choice2").gameObject.SetActive(true);
                ButtonPanel.Find("Choice3").gameObject.SetActive(false);
            }
            else if (currentEvent.optionNumbers == 3)
            {
                ButtonPanel.Find("Choice1").gameObject.SetActive(true);
                ButtonPanel.Find("Choice2").gameObject.SetActive(true);
                ButtonPanel.Find("Choice3").gameObject.SetActive(true);
            }
            // 显示事件窗口
            eventWindow.SetActive(true);
        }

        public void ActivateEvent(int eventID, int delayTurn)
        {
            if (delayTurn < 0)
            {
                throw new Exception("Invalid delay turn!");
            }
            if (allEvents.ContainsKey(eventID))
            {
                if (pendingEvents.ContainsKey(eventID))
                {
                    allEvents[eventID].scheduledTurn = dataManager.turn + delayTurn;  // 如果已经在pendingEvents里了，就直接改scheduledTurn
                }
                else
                {
                    if (allEvents[eventID].isTriggered && !allEvents[eventID].isRepeatable)
                    {
                        throw new Exception("Attempt to schedule a triggered unrepeatable event!");
                    }
                    EventObject EventObject = allEvents[eventID];
                    EventObject.scheduledTurn = this.dataManager.turn + delayTurn;
                    pendingEvents.Add(eventID, EventObject);
                    Debug.Log("Add event " + eventID + " to pendingEvents " + "at turn " + EventObject.scheduledTurn);
                }
            }
            else
            {
                throw new Exception("Invalid event ID!");
            }
        }

        public void ReadEventsFromJson(string jsonpath)
        {
            string json_str = System.IO.File.ReadAllText(jsonpath);
            JsonTextReader reader = new JsonTextReader(new StringReader(json_str));
            EventObject curEvent = null;
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.StartObject)
                {
                    curEvent = new EventObject();
                }
                else if (reader.TokenType == JsonToken.EndObject && curEvent != null)
                {
                    allEvents.Add(curEvent.eventID, curEvent);
                }
                else if (reader.TokenType == JsonToken.PropertyName)
                {
                    string property = reader.Value.ToString();
                    switch (property)
                    {
                        case "eventID":
                            curEvent.eventID = reader.ReadAsInt32().Value;
                            break;
                        case "eventName":
                            curEvent.title = reader.ReadAsString();
                            break;
                        case "content":
                            curEvent.content = reader.ReadAsString();
                            break;
                        case "image":
                            curEvent.image = reader.ReadAsString();
                            break;
                        case "optionNumbers":
                            curEvent.optionNumbers = reader.ReadAsInt32().Value;
                            break;
                        case "isTriggered":
                            curEvent.isTriggered = reader.ReadAsBoolean().Value;
                            break;
                        case "isRepeatable":
                            curEvent.isRepeatable = reader.ReadAsBoolean().Value;
                            break;
                        case "scheduledTurn":
                            curEvent.scheduledTurn = reader.ReadAsInt32().Value;
                            break;
                        case "eventOptions":
                            List<Option> options = new List<Option>();
                            string option_name = null;
                            string option_description = null;
                            string buffs_str = null;
                            string buffs_delay_str = null;
                            string upcoming_events_str = null;
                            string upcoming_events_delay_str = null;
                            Dictionary<int, int> new_buffs = null;
                            Dictionary<int, int> new_upcoming_events = null;
                            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            {
                                if (reader.TokenType == JsonToken.StartObject)
                                {
                                    new_buffs = new Dictionary<int, int>();
                                    new_upcoming_events = new Dictionary<int, int>();
                                }
                                else if (reader.TokenType == JsonToken.EndObject)
                                {
                                    options.Add(new Option(option_name, option_description, new_buffs, new_upcoming_events));
                                }
                                else if (reader.TokenType == JsonToken.PropertyName)
                                {
                                    string optionProperty = reader.Value.ToString();
                                    switch (optionProperty)
                                    {
                                        case "name":
                                            option_name = reader.ReadAsString();
                                            break;
                                        case "description":
                                            option_description = reader.ReadAsString();
                                            break;
                                        case "buffs":
                                            buffs_str = reader.ReadAsString();
                                            break;
                                        case "duration":
                                            buffs_delay_str = reader.ReadAsString();
                                            if (buffs_str != "none")
                                            {
                                                string[] buffs = buffs_str.Split(',');
                                                string[] buffs_delay = buffs_delay_str.Split(',');
                                                for (int i = 0; i < buffs.Length; i++)
                                                {
                                                    new_buffs.Add(int.Parse(buffs[i]), int.Parse(buffs_delay[i]));
                                                }
                                            }
                                            break;
                                        case "upcoming_events":
                                            upcoming_events_str = reader.ReadAsString();
                                            break;
                                        case "event_delays":
                                            upcoming_events_delay_str = reader.ReadAsString();
                                            if (upcoming_events_str != "none")
                                            {
                                                string[] upcoming_events = upcoming_events_str.Split(',');
                                                string[] upcoming_events_delay = upcoming_events_delay_str.Split(',');
                                                for (int i = 0; i < upcoming_events.Length; i++)
                                                {
                                                    new_upcoming_events.Add(int.Parse(upcoming_events[i]), int.Parse(upcoming_events_delay[i]));
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                            curEvent.eventOptions = options;
                            break;
                        case "conditions":
                            List<Tuple<string, int, EventObject.compareOperator>> conditions = new List<Tuple<string, int, EventObject.compareOperator>>();
                            Tuple<string, int, EventObject.compareOperator> condition = null;
                            string target_data_name = null;
                            int threshold = 0;
                            EventObject.compareOperator compareOperator = EventObject.compareOperator.EqualTo;
                            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            {
                                if (reader.TokenType == JsonToken.StartObject)
                                {
                                }
                                else if (reader.TokenType == JsonToken.EndObject)
                                {
                                    conditions.Add(condition);
                                }
                                else if (reader.TokenType == JsonToken.PropertyName)
                                {
                                    string conditionProperty = reader.Value.ToString();
                                    switch (conditionProperty)
                                    {
                                        case "target_data":
                                            target_data_name = reader.ReadAsString();
                                            break;
                                        case "threshold":
                                            threshold = reader.ReadAsInt32().Value;
                                            break;
                                        case "operator":
                                            string operator_str = reader.ReadAsString();
                                            compareOperator = (EventObject.compareOperator)System.Enum.Parse(typeof(EventObject.compareOperator), operator_str);
                                            condition = new Tuple<string, int, EventObject.compareOperator>(target_data_name, threshold, compareOperator);
                                            break;
                                    }
                                }
                            }
                            curEvent.conditions = conditions;
                            break;
                        default:
                            throw new System.Exception("Read Event Json Error: Unexpected property " + property);
                    }
                }
            }
        }
        public void PrintEvents()
        {
            Debug.Log("EventManager: Print all, count: " + allEvents.Count);
            foreach (KeyValuePair<int, EventObject> EventObject in allEvents)
            {
                EventObject.Value.Print();
            }
        }
        public void Reset()
        {
            this.pendingEvents.Clear();
            this.allEvents.Clear();
            this.userChoiceIndex = -1;
        }
    }
}