using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public GameObject[] StageList;

    public static int ClearStage_Num = 0;



	private void Awake()
	{
	}
	void Start()
    {
        Set_Stage();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Set_Stage()
    {

        for (int i = 0; i < ClearStage_Num + 1; i++)
        {
            StageList[i].SetActive(true);
        }
    }


}
