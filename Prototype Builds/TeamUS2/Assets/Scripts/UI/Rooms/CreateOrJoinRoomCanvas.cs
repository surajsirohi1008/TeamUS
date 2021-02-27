using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateOrJoinRoomCanvas : MonoBehaviour
{
    [SerializeField]
    private CreateRoomMenu _createRoomMenu;

    [SerializeField]
    private RoomListingMenu _roomListingMenu;

    private RoomsCanvases _roomCanvases;

    public void FirstInitialise (RoomsCanvases canvases)
    {
        _roomCanvases = canvases;
        _createRoomMenu.FirstInitialise(canvases);
        _roomListingMenu.FirstInitialise(canvases);
    }
}
