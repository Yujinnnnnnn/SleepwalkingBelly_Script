using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class Dice_Manager : MonoBehaviour
{
    private static Dice_Manager _instance;

    public List<Dice> diceList; // Dice 스크립트를 가지고 있는 오브젝트들의 리스트

    public int All_dice_Count => diceList.Count; // 주사위의 갯수.
    public int All_dice_NumTotal { get; private set; } // 주사위 숫자 합계

    public static Dice_Manager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("주사위 매니저 없음 ");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Set_DiceList()
    {
        diceList = FindObjectsOfType<Dice>().ToList();
        All_dice_NumTotal = diceList.Sum(dice => dice.current_Num);
        diceList.ForEach(dice => Debug.Log(dice.current_Num));
    }
}