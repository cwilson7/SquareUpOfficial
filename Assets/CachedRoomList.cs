using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CachedRoomList : MonoBehaviour
{
    public static CachedRoomList cachedRoomList;

    public List<RoomInfo> rooms;
    
    void Awake()
    {
        if (CachedRoomList.cachedRoomList == null)
        {
            CachedRoomList.cachedRoomList = this;
            rooms = new List<RoomInfo>();
        }
        else
        {
            if (CachedRoomList.cachedRoomList != this)
            {
                this.rooms = CachedRoomList.cachedRoomList.rooms;
                Destroy(CachedRoomList.cachedRoomList.gameObject);
                CachedRoomList.cachedRoomList = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
