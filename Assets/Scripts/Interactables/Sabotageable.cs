using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// Extends interactable to allow an item to be sabotaged. When a traitor
// interacts with a sabotageable a sabotage event is started. Currently the
// only sabotageable in the game is flammable.
// When a sabotage events starts:
//  1. the traitor has 5 seconds to get away from the sabotageable
//  2. Then all the players are notified that a sabotage has started and loayls
//     are all assigned the sabotageable as their task.
//  3. Players can then interact with (hold down on) the sabotageable to
//     fix it.
//  4. If fixed in time then the event ends and all the loayls are reassigned a
//     tasks. Otherwise the game ends and the traitor wins
public class Sabotageable : Interactable {

    // Is the sabotage current sabotaged. Once fixed this becomes false.
    public bool isSabotaged;

    // Is this player (local player) currently fixing this item
    public bool fixing = false;

    // The number of players currently fixing this item
    public int numberOfPlayersFixing = 0;

    // UI
    public GameObject sabotagedIndicator;
    public Color color;
    public string warningText;
    public string infoText;
    [SerializeField] private string sabotageTargetText; 
    private Target sabotageMarker;

    // The number of seconds required to fix the item (if multiple people are
    // fiixng then this time can be divided between them).
    public float startingAmountToFix = 100f;
    // Amount remaining to fix (starts equal to startingAmountToFix)
    public float amountToFix;
   
    // Factor to speed up the fix time
    public int fixTimeFactor = 7;
    // The amount of time before the players are notified of the sabotage (ie
    // when it actaully starts) after the traitor interacts with the
    // sabotageable
    public int sabotageDelaySeconds = 5;

    // Store which player have contributed to fixing the item 
    public List<PlayableCharacter> playersThatFixed = new List<PlayableCharacter>();
    
    // The object which shows the sabotage (eg fire animation for flammable)
    [SerializeField] private GameObject animationObject;

    private GameSceneManager gameSceneManager;
    private TimerManager timerManager;
    private SabotageManager sabotageManager;
    private VotingManager votingManager;

    
    
    // Start is called before the first frame update
    void Start() {
        isSabotaged = false;
        amountToFix = startingAmountToFix;
        base.Start();
        
        // Set the task and sabotage marker to the sabotage marker style 
        sabotageMarker = gameObject.AddComponent<Target>() as Target;
        sabotageMarker.boxText = "SABOTAGE";
        sabotageMarker.TargetColor = Color.red;
        sabotageMarker.enabled = false;
        sabotageMarker.NeedArrowIndicator = false;
        taskMarker.TargetColor = Color.red;
        taskMarker.boxText = sabotageTargetText;  
   
        // Get all the required managers
        gameSceneManager = GameObject.Find("/GameSceneManager").GetComponent<GameSceneManager>();
        sabotageManager = GameObject.Find("/SabotageManager").GetComponent<SabotageManager>();
        timerManager = GameObject.Find("/TimerManager").GetComponent<TimerManager>();
        votingManager = GameObject.Find("/VotingManager").GetComponent<VotingManager>();
    }

    void Update() {
        // Master client checks if sabotage is sabotaged and then broadcasts
        // how much is left for each player to fix, the number of players
        // currently fixing and updating the amount of fixing needing to be done
        amountToFix -= numberOfPlayersFixing * fixTimeFactor * Time.deltaTime;
        if (PhotonNetwork.LocalPlayer.IsMasterClient && isSabotaged) {
            // If there is no more to fix then call fix RPC for all players
            // i.e.  tell each player that sabotage is fixed and finished and
            // inform the sabotagen manager of that
            if (amountToFix <= 0) {
                View.RPC("Fix", RpcTarget.All); 
            }
        }
    }

    private void EnableSabotageMarker() {
        if (NetworkManager.instance.GetMe() is Traitor) {
            sabotageMarker.enabled = true;
        }
    }

    private void DisableSabotageMarker() {
        sabotageMarker.enabled = false;
    }

    private void Reset() {
        team = Team.Traitor;
        taskTeam = Team.Real;
    }

    public override void PrimaryInteraction(Character character) {
        //If a sabotage hasn't started and character is a traitor, they can trigger a sabotage on this sabotageable
        if (!isSabotaged && character.team == Team.Traitor && !Timer.sabotageTimer.IsStarted()) {
            DisableSabotageMarker();
            // Start the sabotageTimer (the 5 second countdown)
            timerManager.StartTimer(Timer.sabotageTimer);
            // Sabotage the item for all players
            View.RPC("Sabotage", RpcTarget.All);
        
        // If a sabotage has started then any player can attempt to fix
        } else if (isSabotaged && (Team.Real | Team.Ghost).HasFlag(character.team)) {
            if (!fixing) {
                // Start fixing the sabotagealbe
                // The manager handles the sabotage UI
                sabotageManager.LocalPlayerStartedFixing();    
                character.Fix(this);
                fixing = true;
                View.RPC("IncrementNumberOfFixers", RpcTarget.All);
            }
        }
    }
    
    public override void PrimaryInteractionOff(Character character) {
        //If a player is fixing sabotage but lets go of mouseclick then they stop fixing
        // the master client is notified that one less player is fixing
        if (fixing) {
            character.StopFix(this);
            sabotageManager.LocalPlayerStoppedFixing();    
            fixing = false;
            View.RPC("DecrementNumberOfFixers", RpcTarget.All);
        } 
    }

    [PunRPC]
    public void IncrementNumberOfFixers() {
        this.numberOfPlayersFixing++;
    }

    [PunRPC]
    public void DecrementNumberOfFixers() {
        this.numberOfPlayersFixing--;
    }

    
    public override bool CanInteract(Character character) {
        //If a sabotage hasn't started and character is a traitor, they can trigger a sabotage on this sabotageable
        if (!isSabotaged && !Timer.voteTimer.IsStarted() && character.team == Team.Traitor && !Timer.sabotageTimer.IsStarted()) return true;
        // If a sabotage has started then any player can attempt to fix        
        if (isSabotaged && (Team.Real | Team.Ghost).HasFlag(character.team)) return true;
        return false;
    }

    // When a sabotage is trigger launch the coroutines to react to the
    // sabotage, these are delayed by 5 seconds
    [PunRPC]
    public virtual void Sabotage() {
        StartCoroutine(sabotageManager.SabotageStarted(this));
        StartCoroutine(SabotageEnumerator());
        StartCoroutine(StartAnimation());
    }

    //Start sabotage after a given number of seconds
    //Set task, set as true and give marker
    public IEnumerator SabotageEnumerator() {
        DisableSabotageMarker();
        // Wait till actual start
        yield return new WaitForSeconds(5);
        // Add sabotage overlay
        sabotageManager.SetBackgroundImageColor(this);
        // Add a task to the sabotage so everyone knows to go and fix it
        AddTaskWithTimerRPC(Timer.sabotageTimer);
        task.isUndoable = false;
        task.description = "Fix the " + this.name + "";
        EnableTaskMarker(); 

        isSabotaged = true;
    }

    // The fixing of the sabotage is completed 
    [PunRPC]
    public virtual void Fix() {
        // If the sabotagable is fully fixed
        isSabotaged = false;

        // Tell everyone that the task is now completed
        task.CompleteRPC(false);
        task = null;
        Destroy(GetComponent<Task>());
        DisableTaskMarker();
       
        // Update UI and animation
        sabotageManager.SabotageFixed();
        animationObject.SetActive(false);

        // Reset sabotage/task idicators
        PlayableCharacter me = NetworkManager.instance.GetMe();
        if (inRange && me is Traitor) {
            EnableSabotageMarker();
        } else if (me is Loyal) {
            me.EnableTaskMarker();
        }

        // After an item is fixed its no longer interactable for anyone, as
        // they are single use
        Destroy(this);
    }

    public IEnumerator StartAnimation(){
        yield return new WaitForSeconds(5);
        animationObject.SetActive(true);
    }

    // When a traitor comes near a sabotageable they should be shown an indicator
    public override void OnEnterPlayerRadius() {
        Debug.Log("On enter player radius");
        base.OnEnterPlayerRadius();
        if (NetworkManager.instance.GetMe() is Traitor && !isSabotaged) {
            EnableSabotageMarker();
        }
    }
    
    public override void OnExitPlayerRadius() {
        Debug.Log("On exit player radius");
        base.OnExitPlayerRadius();
        DisableSabotageMarker();
    }

    protected virtual void OnDestroy() {
        base.OnDestroy();
        DisableSabotageMarker();
    }
}
