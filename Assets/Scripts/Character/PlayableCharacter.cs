using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public abstract class PlayableCharacter : Character {
    public GameObject ghostPrefab;

    public ContextTaskUI contextTaskUI;

    public GameObject playerTile;
    public PlayersUI playersUI;

    protected override void Start() { 
      base.Start();
    }

    public override void Pickup(Pickupable item) {
      ItemInteract itemInteract = GetComponent<ItemInteract>();
      if (itemInteract.possibleInteractables.Contains(item)) {
         itemInteract.possibleInteractables.Remove(item);
      }
      base.Pickup(item);
    }

    public bool IsMe() {
      return Owner == PhotonNetwork.LocalPlayer;
    }

    public void Freeze() {
      GetComponent<PlayerMovement>().enabled = false;
      GetComponent<PlayerAnimation>().enabled = false;
      GetComponentInChildren<CameraMouseLook>().enabled = false;
    }

    public void Unfreeze() {
      GetComponent<PlayerMovement>().enabled = true;
      GetComponent<PlayerAnimation>().enabled = true;
      GetComponentInChildren<CameraMouseLook>().enabled = true;
    }

    [PunRPC]
    public void Kill() {
        NetworkManager.instance.SetPlayerProperty("Team", Team.Ghost, Owner);
        GetComponent<PhotonView>().RPC("DestroyPlayer", RpcTarget.All);
        GameObject newPlayer = PhotonNetwork.Instantiate(ghostPrefab.name, new Vector3(1,2,-10), Quaternion.identity);

        PlayableCharacter newCharacter = newPlayer.GetComponent<PlayableCharacter>(); 
        NetworkManager.myCharacter = newCharacter; 
        newCharacter.playerTile = playerTile;
        newCharacter.playersUI = playersUI;
        playersUI.SetToDead(newCharacter);
    }

    [PunRPC]
    public void DestroyPlayer() {
        Destroy(gameObject);
    }
}
