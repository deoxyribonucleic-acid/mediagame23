using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BuffSystem;
using DataSystem;
using CustomEventSystem;
public class GameLogic : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI popularityText;
    [SerializeField]
    TextMeshProUGUI pressureText;
    [SerializeField]
    TextMeshProUGUI evidenceText;
    [SerializeField]
    TextMeshProUGUI stageText;
    [SerializeField]
    TextMeshProUGUI powerText;
    [SerializeField]
    Text buffText;
    [SerializeField]
    GameObject EOG_BG;
    DataManager dataManager;
    EventManager eventManager;
    BuffManager buffManager;
    DataObject player;
    Button button1;
    Button button2;
    Button button3;
    Button button4;
    Button buttonNext;
    Button buttonPause;


    void Start()
    {
        gameObject.AddComponent<DataObject>();
        this.player = gameObject.GetComponent<DataObject>();
        this.dataManager = gameObject.GetComponent<DataManager>();
        this.buffManager = gameObject.GetComponent<BuffManager>();

        //批量读取buff文件
        this.buffManager.ReadBuffsFromJson("Assets/configs/StageBuffs.json");
        this.buffManager.ReadBuffsFromJson("Assets/configs/骂街-Buffs.json");
        this.buffManager.ReadBuffsFromJson("Assets/configs/快捷buff.json");
        this.buffManager.ReadBuffsFromJson("Assets/configs/阶段3buff.json");
        this.buffManager.ReadBuffsFromJson("Assets/configs/死亡.json");
        this.buffManager.ReadBuffsFromJson("Assets/configs/按键buff.json");

        this.buffManager.PrintBuffs();

        this.eventManager = gameObject.GetComponent<EventManager>();
        this.eventManager.ReadEventsFromJson("Assets/configs/骂街.json");
        this.eventManager.ReadEventsFromJson("Assets/configs/Stage1.json");
        this.eventManager.ReadEventsFromJson("Assets/configs/Stage2.json");
        this.eventManager.ReadEventsFromJson("Assets/configs/Stage3.json");
        this.eventManager.ReadEventsFromJson("Assets/configs/家门破坏.json");
        this.eventManager.ReadEventsFromJson("Assets/configs/阶段3随机.json");
        this.eventManager.ReadEventsFromJson("Assets/configs/Stage4.json");

        this.eventManager.PrintEvents();

        UpdateUI();

        this.button1 = GameObject.Find("ChoiceA").GetComponent<Button>();
        this.button2 = GameObject.Find("ChoiceB").GetComponent<Button>();
        this.button3 = GameObject.Find("ChoiceC").GetComponent<Button>();
        this.button4 = GameObject.Find("ChoiceD").GetComponent<Button>();
        this.buttonNext = GameObject.Find("Next").GetComponent<Button>();
        this.buttonPause = GameObject.Find("Pause").GetComponent<Button>();
        this.buttonNext.onClick.AddListener(() =>
        {
            this.Refresh(); //下一回合
        });
        this.button1.onClick.AddListener(() =>
        {
            this.buffManager.ActivateBuff(80);
        });
        this.button2.onClick.AddListener(() =>
        {
            this.buffManager.ActivateBuff(83);
        });
        this.button3.onClick.AddListener(() =>
        {
            this.buffManager.ActivateBuff(81);
            this.buffManager.ActivateBuff(82);
        });
        this.button4.onClick.AddListener(() =>
        {
            this.eventManager.ShowEvent(46);
        });
    }
    void Update()
    {
        if (this.dataManager.GetData("死亡方式") != -1)
        {
            switch (this.dataManager.GetData("死亡方式"))
            {
                case 0:
                    this.EOG_BG.GetComponentsInChildren<Text>()[1].text = "你触发了负面结局";
                    this.EOG_BG.SetActive(true);
                    break;
                case 1:
                    this.EOG_BG.GetComponentsInChildren<Text>()[1].text = "你触发了中立结局";
                    this.EOG_BG.SetActive(true);
                    break;
                case 2:
                    this.EOG_BG.GetComponentsInChildren<Text>()[1].text = "你触发了正面结局";
                    this.EOG_BG.SetActive(true);
                    break;
                default:
                    throw new System.Exception("死亡方式错误");
            }
        }
        if (this.dataManager.GetData("取证进度") >=100)
        {
            this.button4.interactable = true;
        }
        else
        {
            this.button4.interactable = false;
        }
        if (this.dataManager.GetData("精力") < 20)
        {
            this.button1.interactable = false;
        }
        else
        {
            this.button1.interactable = true;
        }
        if (this.dataManager.GetData("精力") < 50)
        {
            this.button2.interactable = false;
        }
        else
        {
            this.button2.interactable = true;
        }
        if (this.dataManager.GetData("精力" ) < 80)
        {
            this.button3.interactable = false;
        }
        else
        {
            this.button3.interactable = true;
        }
        this.button4.gameObject.GetComponentInChildren<Text>().text = "起诉网暴者\n取证进度" + this.dataManager.GetData("取证进度")+ "%";
    }
    public void UpdateUI()
    {
        powerText.text = this.player.GetData("精力").ToString();
        popularityText.text = ((int)this.player.GetData("热度")).ToString();
        pressureText.text = ((int)this.player.GetData("心理压力")).ToString();
        evidenceText.text = this.player.GetData("取证进度").ToString();
        stageText.text = this.player.GetData("热度等级").ToString();
        buffText.text = this.player.PrintBuff();
    }

    void Refresh()
    {
        this.dataManager.Refresh();
        this.UpdateUI();
    }
    public void Restart()
    {
        //重启游戏
        dataManager.Reset();
        eventManager.Reset();
        buffManager.Reset();
        SceneManager.LoadScene(0);
    }
}
