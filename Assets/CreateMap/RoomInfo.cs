using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomInfo
{
    public Room roomObj;     //방
    public int x, y;        //배열 위치
    public int index;       //구분 인덱스
    public bool[] isDoor;   //문이 있는지 확인 배열
    public Door[] DoorObj;  //문 오브젝트 배열
}
