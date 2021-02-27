using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Text _roomName;

    private RoomsCanvases _roomsCanvases;

    public void FirstInitialise(RoomsCanvases canvases)
    {
        _roomsCanvases = canvases;
    }


    public void OnClick_CreateRoom()
    {
        if (!PhotonNetwork.IsConnected)
            return;

        //CreateRoom
        //JoinOrCreateRoom
        RoomOptions options = new RoomOptions();
        options.BroadcastPropsChangeToAll = true;

        options.MaxPlayers = 4;

        PhotonNetwork.JoinOrCreateRoom(_roomName.text, options, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room Successfully");
        _roomsCanvases.CurrentRoomCanvas.Show();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room creation failed: " + message);
    }
}
