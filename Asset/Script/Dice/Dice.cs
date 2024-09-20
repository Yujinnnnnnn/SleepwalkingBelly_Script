using System.Collections;
using UnityEngine;
using TMPro;
using Spine.Unity;
using UnityEngine.Rendering;

public class Dice : MonoBehaviour
{
    private int set_Num;
    public int set_Postion_X;
    public int set_Postion_Y;

    public int current_Num;
    public int current_Postion_X;
    public int current_Postion_Y;

    public int direction_X;
    public int direction_Y;

    public TextMeshProUGUI current_Num_text;

    private Vector3 targetPosition;
    public float moveSpeed = 2f;

    private Vector3 touchStartPos;
    public bool isDragging;
    public bool isDone = false;

    public SkeletonAnimation SkeletonAnimation;
    public AudioSource audio;
    public SortingGroup sortingGroup;

    private bool isCanMove = false;
    private string animName;

    public void Set_Dice(int set_Num, int set_Postion_X, int set_Postion_Y)
    {
        this.set_Num = set_Num;
        current_Num = set_Num;
        current_Num_text.text = current_Num.ToString();

        this.set_Postion_X = set_Postion_X;
        this.set_Postion_Y = set_Postion_Y;
        this.current_Postion_X = set_Postion_X;
        this.current_Postion_Y = set_Postion_Y;

        targetPosition = transform.position;
        current_Num_text.raycastTarget = false;
    }

    void Update()
    {
        UpdateSortingOrder();

        if (isDragging && current_Num > 0)
        {
            CheckDirection();
        }

        HandleMouseInput();

        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    private void UpdateSortingOrder()
    {
        if (sortingGroup != null)
        {
            int offset = (6 - current_Postion_X) * 10;
            sortingGroup.sortingOrder = -current_Postion_Y - offset;
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
            if (hit.collider != null)
            {
                GameObject clickedObject = hit.transform.gameObject;
                // 필요한 경우 클릭된 객체 처리
            }
        }
    }

    #region 이동 관련

    private void CheckDirection()
    {
        Vector3 delta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - touchStartPos;
        delta.z = 0f;

        if (delta.magnitude > 0.1f)
        {
            DetermineDirection(delta);
        }
    }

    private void DetermineDirection(Vector3 delta)
    {
        if (delta.x > 0 && delta.y > 0)
            SetMoveDirection(0, 1, "dice_3_move", "dice_3_fail");
        else if (delta.x > 0 && delta.y < 0)
            SetMoveDirection(1, 0, "dice_1_move", "dice_1_fail");
        else if (delta.x < 0 && delta.y > 0)
            SetMoveDirection(-1, 0, "dice_2_move", "dice_2_fail");
        else if (delta.x < 0 && delta.y < 0)
            SetMoveDirection(0, -1, "dice_4_move", "dice_4_fail");
    }

    private void SetMoveDirection(int dirX, int dirY, string moveAnim, string failAnim)
    {
        direction_X = dirX;
        direction_Y = dirY;

        if (Check_Moveable(dirX, dirY))
        {
            isCanMove = true;
            animName = moveAnim;
        }
        else
        {
            isCanMove = false;
            SkeletonAnimation.AnimationState.SetAnimation(0, "dice_wall", false);
            SkeletonAnimation.AnimationState.SetAnimation(0, failAnim, false);
        }
    }

    private void OnMouseDown()
    {
        if (!isDragging && !isDone)
        {
            isDragging = true;
            isCanMove = false;
            touchStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp()
    {
        if (isCanMove && !isDone)
        {
            HandleMove();
        }

        isDragging = false;
    }

    private void HandleMove()
    {
        if (current_Num - 1 == 0)
        {
            isDone = true;
        }

        StartCoroutine(FadeInAndOut());

        SkeletonAnimation.AnimationState.SetAnimation(0, animName, false);

        Tile currentTile = FindTargetTile(current_Postion_X, current_Postion_Y);
        currentTile.isOccupied = false;

        current_Postion_X += direction_X;
        current_Postion_Y += direction_Y;

        Tile targetTile = FindTargetTile(current_Postion_X, current_Postion_Y);
        targetTile.isOccupied = true;
        targetPosition = new Vector3(targetTile.transform.position.x, targetTile.transform.position.y + 0.5f, targetTile.transform.position.z);

        //targetTile.anim_move();
        audio.Play();
    }

    private Tile FindTargetTile(int x, int y)
    {
        return MapBuilder.Instance.tileList.Find(tile => tile.ArrayX == x && tile.ArrayY == y);
    }

    private bool Check_Moveable(int x, int y)
    {
        Tile targetTile = FindTargetTile(current_Postion_X + x, current_Postion_Y + y);
        return targetTile != null && !targetTile.isOccupied;
    }

    #endregion

    #region 주사위 숫자 관련

    private IEnumerator FadeInAndOut()
    {
        yield return FadeTo(0f, 0.3f);
        yield return new WaitForSeconds(0.2f);

        current_Num--;
        current_Num_text.text = current_Num.ToString();
        Dice_Manager.Instance.Set_DiceList();

        if (current_Num > 0)
        {
            yield return FadeTo(1f, 0.5f);
        }
        else
        {
            yield return new WaitForSeconds(0.2f);
            yield return FadeTo(0f, 0.5f);
        }
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        Color originalColor = current_Num_text.color;
        float startAlpha = originalColor.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            current_Num_text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        current_Num_text.color = new Color(originalColor.r, originalColor.g, originalColor.b, targetAlpha);
    }

    #endregion
}