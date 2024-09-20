using System.Collections;
using UnityEngine;
using Spine.Unity;

public class Tile : MonoBehaviour, PathFindNode.ITile
{



	public int ArrayX;
	public int ArrayY;

	public bool isOccupied = false;

    public SkeletonAnimation skeletonAnimation;

    void Start()
    {
        if (skeletonAnimation == null)
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>(); 
        }
    }

    public void SetArrayIndex(int x, int y)
	{
		ArrayX = x;	
		ArrayY = y;

        SetSortingOrder(y);
    }

    public void SetSortingOrder(int sortingOrder)
    {
        if (skeletonAnimation != null)
        {
            Renderer renderer = skeletonAnimation.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sortingOrder = -100-sortingOrder;
            }
            
        }
        
    }

    public override string ToString()
    {
        return $"{this.ArrayX}, {this.ArrayY}";
    }

	public void ainm_move()
	{
        skeletonAnimation.AnimationState.SetAnimation(0, "press", false);
	}
}
