using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public static class CreateDungeon
{
    /// <summary>
    /// 방 생성
    /// </summary>
    /// <param name="_par">부모 오브젝트</param>
    /// <param name="_pos">위치</param>
    /// <param name="_count">만들어진 방 갯수</param>
    /// <param name="x">위치</param>
    /// <param name="y">위치</param>
    /// <param name="_roomlist">만들어진 방 리스트</param>
    /// <param name="_onelist">만들어진 1개만 생성되는 방 리스트</param>
    /// <param name="_createlist">생성되는 방 정보</param>
    /// <returns>만들어진 방 정보 클래스</returns>
    static RoomInfo CreateRoom(GameObject _par, Vector3 _pos, ref int _count, int x, int y, ref List<RoomInfo> _roomlist, 
        ref List<RoomCreateInfo> _onelist, List<RoomCreateInfo> _createlist)
    {
        int randinx = 0;        //랜덤 인덱스

        int tempc = 0;  //무한루프 방지용
        while (tempc < 10000)
        {
            ++tempc;

            bool isflag = true;     
            randinx = Random.Range(0, _createlist.Count);

            //1개만 생성되는 방 확인
            foreach (RoomCreateInfo item in _onelist)
            {
                if(randinx == item.index)
                {
                    isflag = false;
                    break;
                }    
            }

            if (isflag)
                break;
        }

        RoomInfo temp_room = new RoomInfo();        //방 정보 클래스 생성
        GameObject temp = (GameObject)PrefabUtility.InstantiatePrefab(_createlist[randinx].Room, _par.transform); //방 오브젝트 생성
        temp.transform.localPosition = _pos;        //위치 설정

        temp_room.roomObj = temp.GetComponent<Room>();      //방 오브젝트 할당
        temp_room.index = randinx;      //구분 할 인덱스 할당
        //위치  
        temp_room.x = x;        
        temp_room.y = y;
        //배열 생성
        temp_room.DoorObj = new Door[(int)E_DIR.LEFT + 1];      
        temp_room.isDoor = new bool[(int)E_DIR.LEFT + 1];

        _roomlist.Add(temp_room); //생성되는 방 리스트에 추가

        //1개만 만들어진 방일 경우
        if (_createlist[randinx].onlyOne)
            _onelist.Add(_createlist[randinx]);     //1개만 만드는 방 리스트에 추가

        _count++;       //생성된 방 갯수 카운트 올림

        return temp_room;
    }

    /// <summary>
    /// 2차원 배열 넘는지 확인
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="maxx"></param>
    /// <param name="maxy"></param>
    /// <returns></returns>
    static bool IsOverArr(int x, int y, int maxx,int maxy)
    {
        if (x >= maxx
            || x < 0
            || y >= maxy
            || y < 0)
            return false;

        return true;
    }

    //4방향
    public enum E_DIR
    {
        UP,
        DOWN,
        RIGHT,
        LEFT
    }

    /// <summary>
    /// 방 생성 위치 얻기
    /// </summary>
    /// <param name="cursorObj">현재 방</param>
    /// <param name="rand">난수</param>
    /// <returns>다음 방 생성 위치</returns>
    static Vector3 GetCreateRoomVec(SpriteRenderer _spren, E_DIR rand)
    {
        Vector3 tempvec_Obj = Vector3.zero;     //다음 방 위치

        switch (rand)
        {
            case E_DIR.UP:
                tempvec_Obj.y = _spren.bounds.size.y + 20f;
                break;
            case E_DIR.DOWN:
                tempvec_Obj.y = -_spren.bounds.size.y - 20f;
                break;
            case E_DIR.RIGHT:
                tempvec_Obj.x = _spren.bounds.size.x + 20f;
                break;
            case E_DIR.LEFT:
                tempvec_Obj.x = -_spren.bounds.size.x - 20f;
                break;
        }
        return tempvec_Obj;
    }

    /// <summary>
    /// 맵 생성
    /// </summary>
    /// <param name="RoomList">설정한 방 정보 리스트</param>
    /// <param name="_count">만들 갯수</param>
    /// <param name="Door">문 프리팹</param>
    public static void CreateMap(List<RoomCreateInfo> RoomList, int _count, GameObject Door)
    {
        if (RoomList.Count <= 0)
        {
            Debug.LogError("선택된 방이 없습니다.");
            return;
        }

        MapInfo tempMapInfo = new MapInfo();            //임시 맵 정보

        tempMapInfo.RoomArr = new RoomInfo[_count, _count];     //맵 배열

        List<RoomInfo> room_List = new List<RoomInfo>();     //만들어진 룸리스트
        List<RoomCreateInfo> only_One = new List<RoomCreateInfo>();   //1개만 나오는 방

        RoomList = RoomList.FindAll((x) => x.isCreate);         //만들 수 있는 방 검색
        for (int i = 0; i < RoomList.Count; i++)
        {
            RoomList[i].index = i;  //구분 인덱스 세팅
        }

        GameObject par = new GameObject("Map");     //부모 오브젝트
        par.transform.position = new Vector3(0f, 0f, 30f);      //위치
        RoomManager roomM = par.AddComponent<RoomManager>();    //부모 오브젝트에 매니저 추가

        RoomInfo CurRoom = null;                //현재 방
        //방 위치  
        int x = Random.Range(0, _count);        
        int y = Random.Range(0, _count);
        int curcount = 0;   //현재 카운트

        SpriteRenderer spren = RoomList[0].Room.GetComponent<SpriteRenderer>();     //사이즈를 위한 스프라이트 렌더러

        CurRoom = CreateRoom(par, new Vector3(x * (spren.bounds.size.x * 2f), y * (spren.bounds.size.y * 2f)), ref curcount,x,y, ref room_List,ref  only_One, RoomList);       //첫번째 방 생성
        tempMapInfo.RoomArr[CurRoom.y, CurRoom.x] = CurRoom;    //맵 정보에 만들어진 방 추가

        E_DIR randDir;                  //랜덤 방향
        //반복 현재 만들어진 방 갯수가 만들어야하는 방 갯수까지
        for (; curcount < _count; )
        {
            int tempc = 0;      //무한루프 방지
            while (tempc < 10000)
            {
                ++tempc;

                randDir = (E_DIR)Random.Range(0, 4);     //랜덤 난수 생성
                //다음 방 위치
                switch (randDir)
                {
                    case E_DIR.UP:
                        y = CurRoom.y - 1;
                        break;
                    case E_DIR.DOWN:
                        y = CurRoom.y + 1;
                        break;
                    case E_DIR.RIGHT:
                        x = CurRoom.x + 1;
                        break;
                    case E_DIR.LEFT:
                        x = CurRoom.x - 1;
                        break;
                }

                //배열 벗어나는지 확인, 그 위치에 만들어진 방이 있는지 확인
                if (IsOverArr(x, y, _count, _count)
                    && tempMapInfo.RoomArr[y, x] == null)
                    break;
            }

            Vector3 tempvec_Obj = new Vector3(x * (spren.bounds.size.x * 2f), y * (spren.bounds.size.y*2f));        //위치 (배열위치 * (방사이즈 * 2f))
            CurRoom = CreateRoom(par, tempvec_Obj, ref curcount, x, y, ref room_List, ref only_One, RoomList);       //방 생성
            tempMapInfo.RoomArr[CurRoom.y, CurRoom.x] = CurRoom;        //맵 정보에 만들어진 방 등록
        }

        //반드시 생성해야하는 방 리스트
        List<RoomCreateInfo> never_room = RoomList.FindAll((x) => x.never);

        //반드시 생성해야하는 방이 있는지 확인
        foreach (RoomCreateInfo item in never_room)
        {
            if (room_List.Find((x) => x.index == item.index) == null)
            {
                while (true)
                {
                    //없으면 랜덤 자리에 생성
                    int i = Random.Range(0, room_List.Count);
                    //그 방이 반드시 생성해야하는 방인지 확인 맞으면 난수 다시 생성
                    if (never_room.Find((x) => room_List[i].index == x.index) != null)
                        continue;

                    GameObject temp = (GameObject)PrefabUtility.InstantiatePrefab(item.Room, par.transform); //오브젝트 생성
                    GameObject tempobj = room_List[i].roomObj.gameObject;       //만들어져있던 방
                    room_List[i].roomObj = temp.GetComponent<Room>();       //방 오브젝트
                    room_List[i].index = item.index;                                   //인덱스
                    GameObject.DestroyImmediate(tempobj);       //만들어져 있던 방 삭제
                    break;
                }

            }
        }
        roomM.StartRoom = room_List[0].roomObj;     //매니저에 시작 방 세팅
                
        SetDoor(tempMapInfo, room_List, Door);      //문 세팅
    }

    /// <summary>
    /// 문 세팅
    /// </summary>
    /// <param name="_mapinfo">맵 정보</param>
    /// <param name="_roomlist">만들어진 방 정보</param>
    /// <param name="_Door">문 프리팹</param>
    static void SetDoor(MapInfo _mapinfo, List<RoomInfo> _roomlist, GameObject _Door)
    {
        //문 생성
        foreach (RoomInfo room in _roomlist)
        {
            CreateDoor(_mapinfo, _Door, room, E_DIR.UP);
            CreateDoor(_mapinfo, _Door, room, E_DIR.DOWN);
            CreateDoor(_mapinfo, _Door, room, E_DIR.RIGHT);
            CreateDoor(_mapinfo, _Door, room, E_DIR.LEFT);
        }

        //다음 방 위치 벡터
        Vector2Int[] tempDirArr =
        {
            new Vector2Int(0, -1),
            new Vector2Int(0, +1),
            new Vector2Int(+1, 0),
            new Vector2Int(-1, 0),
        };

        //생성된 문 이동 위치 설정
        foreach (RoomInfo room  in _roomlist)
        {
            for (int i = 0; i < (int)E_DIR.LEFT + 1; i++)
            {
                //해당 방향에 문이 있는지 확인
                if(room.isDoor[i])
                {
                    Vector2Int temppos = tempDirArr[i];     //다음 방 위치 벡터
                    RoomInfo nextRoom = _mapinfo.RoomArr[room.y + temppos.y, room.x + temppos.x];       //다음 방
                    Transform temptrans = null;     //이동할 문 위치
                    //다음 방 문 위치
                    switch ((E_DIR)i)
                    {
                        case E_DIR.UP:
                            temptrans = nextRoom.DoorObj[(int)E_DIR.DOWN].transform;
                            break;
                        case E_DIR.DOWN:
                            temptrans = nextRoom.DoorObj[(int)E_DIR.UP].transform;
                            break;
                        case E_DIR.RIGHT:
                            temptrans = nextRoom.DoorObj[(int)E_DIR.LEFT].transform;
                            break;
                        case E_DIR.LEFT:
                            temptrans = nextRoom.DoorObj[(int)E_DIR.RIGHT].transform;
                            break;
                    }

                    room.DoorObj[i].NextRoom = temptrans;       //문 위치 설정
                }
            }
        }
    }

    /// <summary>
    /// 문 생성
    /// </summary>
    /// <param name="_mapinfo">맵 정보</param>
    /// <param name="_Door">문 프리팹</param>
    /// <param name="room">방 정보</param>
    /// <param name="_dir">방향</param>
    static void CreateDoor(MapInfo _mapinfo, GameObject _Door, RoomInfo room, E_DIR _dir)
    {
        //다음 방 위치 벡터
        Vector2Int[] tempDirArr =
        {
                new Vector2Int(0, -1),
                new Vector2Int(0, +1),
                new Vector2Int(+1, 0),
                new Vector2Int(-1, 0),
         };

        Vector2Int temppos = tempDirArr[(int)_dir];  //다음 방 위치 벡터
        
        //배열 넘는지 확인
        if (!IsOverArr(room.x + temppos.x, room.y + temppos.y, _mapinfo.RoomArr.GetLength(0), _mapinfo.RoomArr.GetLength(1)))
            return;

        RoomInfo nextRoom = _mapinfo.RoomArr[room.y + temppos.y, room.x + temppos.x];       //다음 방

        //해당 방의 방향에 문이 없는지 확인, 다음 방이 있는지 확인
        if (!room.isDoor[(int)_dir] && nextRoom != null)
        {
            SpriteRenderer tempSprite = room.roomObj.GetComponent<SpriteRenderer>();        //사이즈를 위한 스프라이트
            SpriteRenderer tempDoorSprite = _Door.GetComponent<SpriteRenderer>();        //사이즈를 위한 스프라이트

            //문 위치
            Vector3[] doorPosArr =
            {
                new Vector3(tempSprite.bounds.center.x, tempSprite.bounds.min.y + tempDoorSprite.bounds.extents.y),
                new Vector3(tempSprite.bounds.center.x, tempSprite.bounds.max.y - tempDoorSprite.bounds.extents.y),
                new Vector3(tempSprite.bounds.max.x - tempDoorSprite.bounds.extents.x, tempSprite.bounds.center.y),
                new Vector3(tempSprite.bounds.min.x + tempDoorSprite.bounds.extents.x, tempSprite.bounds.center.y),
            };

            GameObject tempobj = (GameObject)PrefabUtility.InstantiatePrefab(_Door, room.roomObj.transform); //문 오브젝트 생성
            Door tempdoor = tempobj.GetComponent<Door>();   //문 스크립트
            tempdoor.transform.position = new Vector3(doorPosArr[(int)_dir].x, doorPosArr[(int)_dir].y);        //위치
            
            room.isDoor[(int)_dir] = true;  //방 정보의 해당 방향 문 true
            room.DoorObj[(int)_dir] = tempdoor; //방 정보의 해당 방향 문 오브젝트 세팅
        }
    }

}
