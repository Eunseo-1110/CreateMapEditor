using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoomManager : MonoBehaviour
{
    public Room StartRoom;      //시작하는 방
    public Transform StartPos;          //시작 위치
    List<Room> m_RoomList;      //방 리스트

    void Awake()
    {
        //방리스트 세팅
        m_RoomList = new List<Room>(GetComponentsInChildren<Room>());
        foreach (Room room in m_RoomList)
        {
            room.Init();
        }
    }
    void Start()
    {
        foreach (Room room in m_RoomList)
        {
            if (room != StartRoom)
                room.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        
    }
}
