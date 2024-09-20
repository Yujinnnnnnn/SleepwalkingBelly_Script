using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Spine.Unity;

public class Lobby : MonoBehaviour
{
	public static int Staticlevel;

	public bool isClear = false;

    public SkeletonGraphic skeletonAnimation;

    private void Start()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, "star", false);
    }



    public void GotoGame(int level)
    {
		Staticlevel = level-1;

        GameManager.Instance.lv_now = Staticlevel;

        SceneManager.LoadScene("mapBuilder");
	}



}
