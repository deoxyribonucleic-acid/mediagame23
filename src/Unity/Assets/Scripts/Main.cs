using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using BuffSystem;
public class Main : MonoBehaviour
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
    TextMeshProUGUI strength;
    [SerializeField]
    Text buffText;
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
        this.dataManager.Register("player", this.player);

        this.buffManager = gameObject.GetComponent<BuffManager>();
        //批量读取buff文件
        this.buffManager.ReadBuffsFromJson("Assets/configs/StageBuffs.json");
        this.buffManager.ReadBuffsFromJson("Assets/configs/骂街-Buffs.json");
        this.buffManager.ReadBuffsFromJson("Assets/configs/快捷buff.json");
        this.buffManager.ReadBuffsFromJson("Assets/configs/阶段3buff.json");
        this.buffManager.ReadBuffsFromJson("Assets/configs/死亡.json");

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


        this.eventManager.ShowEvent(1);


        UpdateUI();

        this.button1 = GameObject.Find("ChoiceA").GetComponent<Button>();
        this.button2 = GameObject.Find("ChoiceB").GetComponent<Button>();
        this.button3 = GameObject.Find("ChoiceC").GetComponent<Button>();
        this.button4 = GameObject.Find("ChoiceD").GetComponent<Button>();
        this.buttonNext = GameObject.Find("Next").GetComponent<Button>();
        this.buttonPause = GameObject.Find("Pause").GetComponent<Button>();
        this.buttonNext.onClick.AddListener(() =>
        {
            this.RefreshAndReval(); //下一回合
        });

    }

    public void UpdateUI()
    {
        strength.text = this.player.GetData("精力").ToString();
        popularityText.text = this.player.GetData("热度").ToString();
        pressureText.text = this.player.GetData("心理压力").ToString();
        evidenceText.text = this.player.GetData("证据").ToString();
        stageText.text = this.player.GetData("热度等级").ToString();
        buffText.text = this.player.RevealBuff();
    }

    void RefreshAndReval()
    {
        this.dataManager.Refresh();
        this.UpdateUI();
    }
}
