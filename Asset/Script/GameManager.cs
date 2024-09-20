using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;


    [Header("UI")]

	public Dice_Manager dice_Manager;
    public MapBuilder mapBuilder;

    public float elapsedTime;

    public float totalTime;

    public bool isRunning;

    [SerializeField]
    private Transform cameraTransform;
	[SerializeField]
	private float ShakeAmount = 2f;
	float ShakeTime;

	[SerializeField]
	private float m_roughness;      
	[SerializeField]
	private float m_magnitude;


    public int lv_now;
  



	public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject singletonObject = new GameObject();
                _instance = singletonObject.AddComponent<GameManager>();
                singletonObject.name = typeof(GameManager).ToString() + " (Singleton)";
                
            }
            return _instance;
        }
    }

    private void Awake()
    {
       
        elapsedTime = 0f;
        isRunning = true;

        GameObject mapBuilderObject = GameObject.Find("MapBuilder");

        if (mapBuilderObject != null)
        {
            // "MapBuilder" 오브젝트에서 SomeComponent 컴포넌트를 가져옴
            mapBuilder = mapBuilderObject.GetComponent<MapBuilder>();


        }
    }

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			StartCoroutine(isShake());
		}

        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
        }
    }

 
    public void StopTimer()
    {
        isRunning = false;
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void GoToLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }



    IEnumerator isShake()
	{
		Vector3 InitVec = Camera.main.transform.position;	
		while(ShakeTime<0.05f)
		{
			ShakeTime += Time.deltaTime;
			cameraTransform.transform.Translate(Vector3.left * 5 * Time.deltaTime);
			yield return null;
		}

		ShakeTime = 0;

		while (ShakeTime < 0.1f)
		{
			ShakeTime += Time.deltaTime;
			cameraTransform.transform.Translate(Vector3.right * 5 * Time.deltaTime);
			yield return null;
		}

		ShakeTime = 0;

		while (ShakeTime < 0.05f)
		{
			ShakeTime += Time.deltaTime;
			cameraTransform.transform.Translate(Vector3.left * 5 * Time.deltaTime);
			yield return null;
		}

		ShakeTime = 0;

		cameraTransform.transform.position = InitVec;
	}
}
