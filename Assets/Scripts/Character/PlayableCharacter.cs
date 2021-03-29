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

    public virtual void Freeze() {
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
        GameObject newPlayer = PhotonNetwork.Instantiate(ghostPrefab.name, new Vector3(1,2,-10), Quaternion.identity);

        // Kill the player for everyone else
        GetComponent<PhotonView>().RPC("KillPlayer", RpcTarget.All, newPlayer.GetComponent<PhotonView>().ViewID);

        PlayableCharacter newCharacter = newPlayer.GetComponent<PlayableCharacter>(); 
        NetworkManager.myCharacter = newCharacter; 
    }

    [PunRPC]
    public void KillPlayer(int newPlayerViewId) {
        PlayableCharacter newCharacter = PhotonView.Find(newPlayerViewId).GetComponent<PlayableCharacter>();
        newCharacter.playerTile = playerTile;
        newCharacter.playersUI = playersUI;
        playersUI.SetToDead(newCharacter);
        
        Destroy(gameObject);
    }
}
