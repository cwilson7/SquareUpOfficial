using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class JoinRoomsMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject content;
    private List<RoomListing> roomListings;
    [SerializeField] private RoomListing rlPrefab;

    private void Start()
    {
        InitializeRoomListings();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        Debug.Log("Room list update.");
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                int index = roomListings.FindIndex(x => x.RoomInfo.Name == info.Name);
                if(index != -1)
                {
                    Destroy(roomListings[index].gameObject);
                    roomListings.RemoveAt(index);
                }
            }
            else
            {
                RoomListing listing = Instantiate(rlPrefab, content.transform);
                if (listing != null) listing.SetRoomListing(info);
                roomListings.Add(listing);
            }
        }
    }

    private void InitializeRoomListings()
    {
        roomListings = new List<RoomListing>();
    }
}
