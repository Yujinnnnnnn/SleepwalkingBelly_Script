using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

public class MapBuilder : MonoBehaviour, PathFindNode.ITileManager
{
	public const int MaxX = 7;
	public const int MaxY = 8;

	public GameObject tile;
    public GameObject StartTile;
    public GameObject FinalTile;
    public GameObject cube;
    public GameObject building;
	public List<Tile> tileList;

    public Transform Tile_parent_transform;
    public Transform Dice_parent_transform;
	public int StageLevel;

	private int parseNum;

    private static MapBuilder _instance;
	public object[,] array;
	public Tile[,] tileObjectArr;

	public SkeletonAnimation Player; 
	public SkeletonAnimation Enemy;

    public TextMeshProUGUI Stage_text;


	public static MapBuilder Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject singletonObject = new GameObject();
                _instance = singletonObject.AddComponent<MapBuilder>();
                singletonObject.name = typeof(GameManager).ToString() + " (Singleton)";
                DontDestroyOnLoad(singletonObject);
            }
            return _instance;
        }
    }


	private void Awake()
    {
		if (_instance == null)
        {
            _instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }


 
    // Start is called before the first frame update
    void Start()
    {
		//Debug.Log(Lobby.Staticlevel);
        array = new object[MaxX, MaxY];
        tileObjectArr = new Tile[MaxX,MaxY];
		MapInit(Lobby.Staticlevel);
	}

    private void MapInit(int level)
    {
        List<Dictionary<string, object>> data_Dialog = CSVReader.Read("level design");

        int stageNow = level + 1;

        Stage_text.text = "Stage " + stageNow;

        for (int i = 0; i < 7; i++)
        {
            foreach (KeyValuePair<string, object> item in data_Dialog[i + level * 7 + level])
            {
                //Debug.LogFormat("[{0}:{1}]", item.Key, item.Value);
                if (item.Key == "start")
                    array[i, 0] = item.Value;
                else if (item.Key == "block1")
                    array[i, 1] = item.Value;
                else if (item.Key == "block2")
                    array[i, 2] = item.Value;
                else if (item.Key == "block3")
                    array[i, 3] = item.Value;
                else if (item.Key == "block4")
                    array[i, 4] = item.Value;
                else if (item.Key == "block5")
                    array[i, 5] = item.Value;
                else if (item.Key == "block6")
                    array[i, 6] = item.Value;
                else if (item.Key == "destination")
                    array[i, 7] = item.Value;

            }
        }

        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {

                //Debug.Log(i + "  " + j + "  " + array[i, j]);
                if (array[i, j].ToString() != "n")
                {
                    Tile temTile;
                    if (array[i, j].ToString() == "c")
                    {

                        temTile = Instantiate(StartTile, new Vector3(i + j, ((0.7f * i) - (0.7f * j)) * -1, 0), Quaternion.identity, Tile_parent_transform).GetComponent<Tile>();
                        Player.transform.position = new Vector3(temTile.transform.position.x, temTile.transform.position.y, 0);
                        Enemy.transform.position = new Vector3(temTile.transform.position.x, temTile.transform.position.y, 0);
                        //Instantiate(Player, new Vector3(temTile.transform.position.x, temTile.transform.position.y, 0), Quaternion.identity, Tile_parent_transform);
                    }
                    else if (array[i, j].ToString() == "l")
                    {
                        temTile = Instantiate(FinalTile, new Vector3(i + j, ((0.7f * i) - (0.7f * j)) * -1, 0), Quaternion.identity, Tile_parent_transform).GetComponent<Tile>();
                        //Instantiate(building, new Vector3(temTile.transform.position.x, temTile.transform.position.y + 1.2f, 0), Quaternion.identity, Tile_parent_transform);
                    }
                    else
                    {
                        temTile = Instantiate(tile, new Vector3(i + j, ((0.7f * i) - (0.7f * j)) * -1, 0), Quaternion.identity, Tile_parent_transform).GetComponent<Tile>();
                        temTile.gameObject.name = i + " " + j + " " + "Tile";
                    }

                    temTile.SetArrayIndex(i, j);
                    //temTile.GetComponent<SpriteRenderer>().sortingOrder = -5-j;

                    if (int.TryParse(array[i, j].ToString(), out parseNum))
                    {
                        if (parseNum > 0)
                        {
                            Instantiate(cube, new Vector3(temTile.transform.position.x, temTile.transform.position.y + 0.5f, j), Quaternion.identity, Dice_parent_transform).GetComponent<Dice>().Set_Dice((int)array[i, j], i, j);
                            temTile.isOccupied = true;
                        }
                    }
                    tileObjectArr[i, j] = temTile;
                    tileList.Add(temTile);
                }
            }
        }

        Dice_Manager.Instance.Set_DiceList();

    }
       



    //----------------------------------------

	private void Update()
	{
		
	}

	public Tile GetStartTile_Func()
	{
		for (int i = 0; i < this.tileObjectArr.GetLength(0); i++)
		{
			for (int j = 0; j < this.tileObjectArr.GetLength(1); j++)
			{
				if (this.tileObjectArr[i, j] != null)
				{
					if (this.tileObjectArr[i, j].name == "StartTile(Clone)")
					{
						return this.tileObjectArr[i, j];
					}
				}
			}
		}

		Debug.LogError("?");
		return null;
	}
	public Tile GetArriveTile_Func()
	{
		for (int i = 0; i < this.tileObjectArr.GetLength(0); i++)
		{
			for (int j = 0; j < this.tileObjectArr.GetLength(1); j++)
			{
				if (this.tileObjectArr[i, j] != null)
				{
					if (this.tileObjectArr[i, j].name == "FinalTile(Clone)")
					{
						return this.tileObjectArr[i, j];
					}
				}
			}
		}

		Debug.LogError("?");
		return null;
	}

	public bool TryGetTile_Func(PathFindNode.ITile _checkTile, DirectionType _dirType, out PathFindNode.ITile _returnItile)
	{
		bool _isResult = this.TryGetTile_Func(_checkTile as Tile, _dirType, out Tile _returnTile);

		_returnItile = _returnTile;

		return _isResult;
	}
	public bool TryGetTile_Func(Tile _checkTile, DirectionType _dirType, out Tile _returnTile)
	{
		int _checkX = _checkTile.ArrayX;
		int _checkY = _checkTile.ArrayY;

		int _returnX = -1;
		int _returnY = -1;

        switch (_dirType)
        {
            case DirectionType.Right:
                {
					if(_checkX + 1 < MaxX)
                    {
						_returnX = _checkX + 1;
						_returnY = _checkY;
                    }
				}
                break;

            case DirectionType.Left:
				{
					if (0 <= _checkX - 1)
					{
						_returnX = _checkX - 1;
						_returnY = _checkY;
					}
				}
				break;

            case DirectionType.Up:
                {
					if (_checkY + 1 < MaxY)
					{
						_returnX = _checkX;
						_returnY = _checkY + 1;
					}
				}
                break;

            case DirectionType.Down:
				if (0 <= _checkY - 1)
				{
					_returnX = _checkX;
					_returnY = _checkY - 1;
				}
				break;

            default:
				Debug.LogError("_dirType : " + _dirType);
				break;
        }

		if(0 <= _returnX && 0 <= _returnY)
        {
			Tile _tile = this.tileObjectArr[_returnX, _returnY];
			if (_tile != null)
			{
				_returnTile = _tile;
				return true;
			}
        }

		_returnTile = null;
		return false;
	}
}

public enum DirectionType
{
	None = 0,

	Right,
	Left,
	Up,
	Down,
}


//for (int i = level *6; i < (level+1) * 6; i++)