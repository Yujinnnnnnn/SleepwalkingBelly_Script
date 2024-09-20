using Newtonsoft.Json.Linq;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PathFindNode;
using static UnityEngine.GraphicsBuffer;

// ��� Ÿ�� ��ȸ
// �� ��ȸ �ϸ鼭  ���� ��ΰ� ������ ä�� 


public class RealPathFinder : MonoBehaviour
{
	private bool isDone;

    private PathFindNode pathFindNode;
    private List<PathFindNode> pathFindNodeList;
    private int maxPathNum;
    [SerializeField]
    private GameObject cover;

    public MapBuilder mapBuilder;
    public Dice_Manager dice_Manager;

    public void StartFind(SkeletonAnimation animation)
    {
        Tile _startTile = MapBuilder.Instance.GetStartTile_Func();
        PathFindNode _pathFindNode = new PathFindNode(_startTile, (_iTile) =>
            {
                Tile _tile = _iTile as Tile;
                return _tile.isOccupied == false;
            });
        _pathFindNode.OnPathFind_Func(MapBuilder.Instance, _startTile);

        this.pathFindNode = _pathFindNode;

        Tile _arriveTile = MapBuilder.Instance.GetArriveTile_Func();
        List<ITile> _iTileList = new List<ITile>();
        if (_pathFindNode.TryGetPath_Func(_arriveTile, out _iTileList, _iTileList) == true)
        {
            Debug.Log("��ã��!");

            //foreach (Tile _tile in _iTileList)
            //    Debug.Log(_tile.ToString());

			StartCoroutine(GotoGoal(_iTileList, animation));
		}
        else
        {
            Debug.Log("�� �� ã��...");

            int _longestPathNum = -1;
            PathFindNode _longestPathFindNode = null;

            _pathFindNode.GetPathFindNode_Func((_pathNum, _pathFindNode) =>
            {
                if(_longestPathNum < _pathNum)
                {
                    _longestPathNum = _pathNum;
                    _longestPathFindNode = _pathFindNode;
                }
            });

            if(_longestPathFindNode != null)
            {
                _longestPathFindNode.GetTile_Func(_iTileList);

                //foreach (Tile _tile in _iTileList)
                //    Debug.Log(_tile.ToString());

                StartCoroutine(GotoFail(_iTileList,animation));
            }
        }

    }

	IEnumerator GotoGoal(List<ITile> _iTileList, SkeletonAnimation animation)
    {
		animation.AnimationName = "F_move";
		var playerPosition = animation.transform;


        foreach (Tile _tile in _iTileList)
        {
            while (playerPosition.position.x != _tile.transform.position.x && playerPosition.position.y != _tile.transform.position.y)
            {


				playerPosition.position = Vector3.MoveTowards(playerPosition.position, _tile.transform.position, 2 * Time.deltaTime);

                _tile.ainm_move();
                yield return null;
            }
        }
        StartCoroutine(FadeOutPlayer(playerPosition.gameObject));
        Lobby.Staticlevel++;
        if(Lobby.Staticlevel < 20)
        {
			cover.SetActive(true);
			yield return new WaitForSeconds(0.5f);
			GameManager.Instance.RestartGame();
            LobbyManager.ClearStage_Num++;

        }
	}

    IEnumerator FadeOutPlayer(GameObject playerObject)
    {
        float fadeDuration = 1.0f; 
        float elapsedTime = 0.0f;
        SkeletonAnimation skeletonAnimation = playerObject.GetComponent<SkeletonAnimation>();

        float startAlpha = skeletonAnimation.skeleton.A;

        while (elapsedTime < fadeDuration)
        {
            
            float alpha = Mathf.Lerp(startAlpha, 0.0f, elapsedTime / fadeDuration);
            skeletonAnimation.skeleton.SetColor(new Color(1f, 1f, 1f, alpha)); 

            elapsedTime += Time.deltaTime;
            yield return null;
        }

       
    }


    IEnumerator GotoFail(List<ITile> _iTileList, SkeletonAnimation animation)
	{
		animation.AnimationName = "F_move";
		var playerPosition = animation.transform;


		foreach (Tile _tile in _iTileList)
		{
			while (playerPosition.position.x != _tile.transform.position.x && playerPosition.position.y != _tile.transform.position.y)
			{

                animation.transform.GetComponent<MeshRenderer>().sortingOrder = _tile.ArrayX - _tile.ArrayY;
                playerPosition.position = Vector3.MoveTowards(playerPosition.position, _tile.transform.position, 2 * Time.deltaTime);
				yield return null;
			}
		}

        if (animation.gameObject.name == "Enemy")
        {
			cover.SetActive(true);
			yield return new WaitForSeconds(0.5f);

			GameManager.Instance.GoToLobby();
		}

	}

	float timer;


	private void Update()
	{
        if(Dice_Manager.Instance.All_dice_NumTotal == 0)
        {
            GameObject finalTile = GameObject.Find("FinalTile(Clone)");

            var finalTile_cs = finalTile.GetComponent<Tile>();
            finalTile_cs.isOccupied = false;


            StartFind(mapBuilder.Player);
			dice_Manager.All_dice_NumTotal = -100;
		}

        if(Dice_Manager.Instance.All_dice_NumTotal == -100)
        {
            timer += Time.deltaTime;

            if (timer > 3)
            {
                timer = 0;
                dice_Manager.All_dice_NumTotal = 10000;
                mapBuilder.Enemy.gameObject.SetActive(true);
				StartFind(mapBuilder.Enemy);

            }

		}
	}
}

public class PathFindNode
{
    public List<PathFindNode> pathFindNodeList;
    public ITile iTile;
    private Func<ITile, bool> checkConditionDel;
    public PathFindNode beforePathFindNode;

    public PathFindNode(ITile _iTile, Func<ITile, bool> _checkConditionDel = null, PathFindNode _beforePathFindNode = null)
    {
        this.pathFindNodeList = new List<PathFindNode>();
        this.iTile = _iTile;
        this.checkConditionDel = _checkConditionDel;
        this.beforePathFindNode = _beforePathFindNode;
    }

    public void OnPathFind_Func(ITileManager _iTileManager, ITile _iBeginTile)
    {
        this.iTile = _iBeginTile;

        ITile _returnTile = null;
        if (_iTileManager.TryGetTile_Func(_iBeginTile, DirectionType.Right, out _returnTile) == true)
            _Func(_returnTile);

        if (_iTileManager.TryGetTile_Func(_iBeginTile, DirectionType.Left, out _returnTile) == true)
            _Func(_returnTile);

        if (_iTileManager.TryGetTile_Func(_iBeginTile, DirectionType.Up, out _returnTile) == true)
            _Func(_returnTile);

        if (_iTileManager.TryGetTile_Func(_iBeginTile, DirectionType.Down, out _returnTile) == true)
            _Func(_returnTile);

        void _Func(ITile _addTile)
        {
            bool _isAddable = true;

            if (this.checkConditionDel?.Invoke(_addTile) == false)
                _isAddable = false;

            if (_isAddable == true)
            {
                if (this.beforePathFindNode != null)
                {
                    if (this.beforePathFindNode.IsHaveTile_Func(_addTile) == true)
                        _isAddable = false;
                }
            }

            if (_isAddable == true)
            {
                PathFindNode _pathFindNode = new PathFindNode(_addTile, this.checkConditionDel, this);
                _pathFindNode.OnPathFind_Func(_iTileManager, _addTile);

                this.pathFindNodeList.Add(_pathFindNode);
            }
        }
    }

    public bool IsHaveTile_Func(ITile _iTile)
    {
        if (this.iTile != _iTile)
        {
            if (this.beforePathFindNode != null)
            {
                return this.beforePathFindNode.IsHaveTile_Func(_iTile);
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }

    public bool TryGetPath_Func(ITile _goalTile, out List<ITile> _iTileList, List<ITile> _catchediTileList = null)
    {
        if (_catchediTileList == null)
            _catchediTileList = new List<ITile>();

        int _minPathNum = int.MaxValue;
        PathFindNode _minPathFindNode = null;

        this.GetPathFindNode_Func(0, (_pathNum, _pathFindNode) =>
        {
            if (_pathNum < _minPathNum)
            {
                _pathFindNode.GetTile_Func(_catchediTileList);

                foreach (ITile _iTile in _catchediTileList)
                {
                    if (_iTile == _goalTile)
                    {
                        if (_pathNum < _minPathNum)
                        {
                            _minPathNum = _pathNum;
                            _minPathFindNode = _pathFindNode;
                        }

                        break;
                    }
                }

                _catchediTileList.Clear();
            }
        });

        if (_minPathFindNode != null)
        {
            _minPathFindNode.GetTile_Func(_catchediTileList);

            _iTileList = _catchediTileList;

            return true;
        }
        else
        {
            _iTileList = _catchediTileList;

            return false;
        }
    }

    public void GetPathFindNode_Func(Action<int, PathFindNode> _resultDel)
    {
        this.GetPathFindNode_Func(0, _resultDel);
    }
    private void GetPathFindNode_Func(int _pathCnt, Action<int, PathFindNode> _resultDel)
    {
        if (0 < this.pathFindNodeList.Count)
        {
            foreach (PathFindNode _pathFindNode in this.pathFindNodeList)
            {
                _pathFindNode.GetPathFindNode_Func(_pathCnt + 1, _resultDel);
            }
        }
        else
        {
            _resultDel.Invoke(_pathCnt, this);
        }
    }

    public void GetTile_Func(List<ITile> _iTileList)
    {
        _iTileList.Insert(0, this.iTile);

        if (this.beforePathFindNode != null)
            this.beforePathFindNode.GetTile_Func(_iTileList);
    }

    public interface ITile { }
    public interface ITileManager
    {
        bool TryGetTile_Func(ITile _checkTile, DirectionType _dirType, out ITile _returnTile);
    }




   




}