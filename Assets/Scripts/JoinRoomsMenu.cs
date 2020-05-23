using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class JoinRoomsMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject content;
    private List<RoomListing> roomListings;
    [SerializeField] private RoomListing rlPrefab;

    private void Start()
    {
        InitializeRoomListings();
    }

    private void FixedUpdate()
    {
        
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList.ToList())
        {
            if (info.RemovedFromList)
            {
                int index = roomListings.FindIndex(x => x.RoomInfo.Name == info.Name);
                if(index != -1)
                {
                    Destroy(roomListings[index].gameObject);
                    roomListings.RemoveAt(index);
                    CachedRoomList.cachedRoomList.rooms.Remove(info);
                }
            }
            else
            {
                RoomListing listing = Instantiate(rlPrefab, content.transform);
                if (listing != null) listing.SetRoomListing(info);
                roomListings.Add(listing);
                if(!CachedRoomList.cachedRoomList.rooms.Contains(info)) CachedRoomList.cachedRoomList.rooms.Add(info);
            }
        }
    }

    private void InitializeRoomListings()
    {
        roomListings = new List<RoomListing>();
        OnRoomListUpdate(CachedRoomList.cachedRoomList.rooms);
    }
}
