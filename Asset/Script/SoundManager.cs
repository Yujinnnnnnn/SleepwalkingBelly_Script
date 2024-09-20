using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
	public AudioClip[] sound;
	public AudioSource audio;
	private void Awake()
	{
		DontDestroyOnLoad(this);
		//audio.clip = sound[1];
		//audio.Play();

		var obj = FindObjectsOfType<SoundManager>();
		if (obj.Length == 1)
		{
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void Update()
	{
		Scene currentScene = SceneManager.GetActiveScene();

		if(currentScene.name == "LobbyScene")
		{
			audio.clip = sound[1];

			if(!audio.isPlaying)
			audio.Play();
		}
		else if (currentScene.name == "mapBuilder")
		{
			audio.clip = sound[0];

			if (!audio.isPlaying)
				audio.Play();
		}
	}



}
