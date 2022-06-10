using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCreateInfo
{
    public GameObject Room;
    public bool isCreate;           //생성하는지
    public bool onlyOne;           //1개만?
    public bool never;    //무조건 나오는지
    public int index;
}


public class MapInfo
{
    public RoomInfo[,] RoomArr;         //맵 배열
}