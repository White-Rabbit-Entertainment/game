using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public abstract class PlayableCharacter : Character {

    public GameObject ghostPrefab;
    public CurrentTaskUI currentTaskUI;
    public DeathUI deathUI;
    public TaskNotificationUI taskNotificationUI;
    public PlayerTile playerTile;
    public PlayersUI playersUI;
    public Task assignedMasterTask = null;
    public Task assignedSubTask = null;
    private GameSceneManager gameSceneManager;

    public Camera Camera {
        get { return GetComponentInChildren<Camera>(); }
    }

    //call character start function and find gameSceneManager
    protected override void Start() { 
      base.Start();
      gameSceneManager = GameObject.Find("/GameSceneManager").GetComponent<GameSceneManager>();
    }

    //Picks up a pickupable item
    public override void Pickup(Pickupable item) {
      //Get itemInteract script
      ItemInteract itemInteract = GetComponent<ItemInteract>();

      //Remove the item from the list of possible interactables
      if (itemInteract.possibleInteractables.Contains(item)) {
         itemInteract.possibleInteractables.Remove(item);
      }

      //Call character pickup function
      base.Pickup(item);
    }

    //Puts down a pickupable item
    public override void PutDown(Pickupable item) {
      //Calls character pickup function
      base.PutDown(item);
    }
    
    //Returns true if the player owns this character
    public bool IsMe() {
      return Owner == PhotonNetwork.LocalPlayer;
    }

    //Freezes player input to stop movement when in pause menu
    public virtual void Freeze() {
      GetComponent<PlayerMovement>().frozen = true;
      GetComponentInChildren<CameraMouseLook>().enabled = false;
    }

    //Unfreezes player input to allow movement
    public void Unfreeze() {
      GetComponent<PlayerMovement>().frozen = false;
      GetComponentInChildren<CameraMouseLook>().enabled = true;
    }

    //Unassigns the current task from the character
    public void UnassignTask() {

        //Check if character has a current subtask
        //If so disable the task marker and unassign the task
        if (assignedSubTask != null) {
            assignedSubTask.DisableTaskMarker();
            assignedSubTask.Unassign();
        }

        //Locally set subtask and mastertask to null
        assignedSubTask = null;
        assignedMasterTask = null;
    }

    //Enables the marker for the current task if there is one
    public void EnableTaskMarker() {
        if (assignedSubTask != null) {
            assignedSubTask.EnableTaskMarker();
        }
    }

    //Disables the marker for the current task if there is one
    public void DisableTaskMarker() {
        if (assignedSubTask != null) {
            assignedSubTask.DisableTaskMarker();
        }
    }
    
    //Kills this character, this can be called by other players
    //as it is an RPC
    [PunRPC]
    public void Kill() {
        //Unassign character's task
        UnassignTask();

        //Change character's team to ghost and respawn them as a ghost
        NetworkManager.instance.SetPlayerProperty("Team", Team.Ghost, Owner);
        GameObject newPlayer = PhotonNetwork.Instantiate(ghostPrefab.name, gameSceneManager.RandomNavmeshLocation(), Quaternion.identity);

        //Kill the player for everyone else
        GetComponent<PhotonView>().RPC("KillPlayer", RpcTarget.All, newPlayer.GetComponent<PhotonView>().ViewID);

        //Tell the network manager that the new character belongs to this player
        PlayableCharacter newCharacter = newPlayer.GetComponent<PlayableCharacter>(); 
        NetworkManager.myCharacter = newCharacter;

        //Turn on UI to indicate that the player has died
        deathUI.gameObject.SetActive(true);

        //Remove items from inventory and held items
        PutDown(currentHeldItem);
        RemoveItemFromInventory();

        //Unassign the current master task if there is one
        if (assignedMasterTask != null) {
            assignedMasterTask.Unassign();
        }

        //Remove outline from current playable character
        if (GetComponent<ItemInteract>() != null) GetComponent<ItemInteract>().ClearInteractionOutline();

        FindObjectOfType<OffScreenIndicator>().SetCamera();
    }

    //Handles a character death for players that do not own the character
    [PunRPC]
    public void KillPlayer(int newPlayerViewId) {

        //Set attributes for the new playable character
        PlayableCharacter newCharacter = PhotonView.Find(newPlayerViewId).GetComponent<PlayableCharacter>();
        newCharacter.playerTile = playerTile;
        newCharacter.playersUI = playersUI;
        newCharacter.startingTeam = startingTeam;
        newCharacter.taskNotificationUI = taskNotificationUI;
        newCharacter.playerInfo = playerInfo;

        //Update UI to show player death
        playersUI.SetToDead(newCharacter);

        //Set new body game object to be parented by the new playable character transform
        GameObject body = Instantiate(playerInfo.ghostPrefab, new Vector3(0,0,0), Quaternion.identity);
        body.transform.parent = newCharacter.transform; // Sets the parent of the body to the player
        body.transform.position = newCharacter.transform.position;
        body.transform.Rotate(-90f, 0f, 0f);

        Destroy(gameObject);
    }
}
