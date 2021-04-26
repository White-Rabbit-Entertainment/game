using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public abstract class PlayableCharacter : Character {
    public GameObject ghostPrefab;

    public ContextTaskUI contextTaskUI;

    public TaskNotificationUI taskNotificationUI;

    public GameObject playerTile;
    public PlayersUI playersUI;

    public Task assignedMasterTask = null;
    public Task assignedSubTask = null;

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

    public override void PutDown(Pickupable item) {
      base.PutDown(item);
    }
     public bool IsMe() {
      return Owner == PhotonNetwork.LocalPlayer;
    }

    public virtual void Freeze() {
      GetComponent<PlayerMovement>().frozen = true;
      GetComponentInChildren<CameraMouseLook>().enabled = false;
    }

    public void Unfreeze() {
      GetComponent<PlayerMovement>().frozen = false;
      GetComponentInChildren<CameraMouseLook>().enabled = true;
    }

    public void UnassignTask() {
        if (assignedSubTask != null) {
            assignedSubTask.DisableTaskMarker();
            assignedSubTask.Unassign();
        }

        // This locally sets your tasks to nothing
        assignedSubTask = null;
        assignedMasterTask = null;
    }

    [PunRPC]
    public void Kill() {
        UnassignTask();

        NetworkManager.instance.SetPlayerProperty("Team", Team.Ghost, Owner);
        GameObject newPlayer = PhotonNetwork.Instantiate(ghostPrefab.name, new Vector3(1,10,-10), Quaternion.identity);

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
