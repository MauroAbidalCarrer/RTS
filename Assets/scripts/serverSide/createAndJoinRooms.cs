using UnityEngine;
using Photon.Pun;
using TMPro;

public class createAndJoinRooms : MonoBehaviourPunCallbacks
{
    [SerializeField]
    TMP_InputField createInput;
    [SerializeField]
    TMP_InputField joinInput;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (joinInput.text != "")
                PhotonNetwork.JoinRoom(joinInput.text);
            else if (createInput.text != "")
                PhotonNetwork.CreateRoom(createInput.text);
        }
    }
    public override void OnJoinedRoom()
    { PhotonNetwork.LoadLevel("Game"); }
}
