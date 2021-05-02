using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Sabotageable : Interactable {

    public bool isSabotaged;
    public int numberOfPlayersToFix = 1;

    public bool fixing = false;

    public int numberOfPlayersFixing = 0;
    public GameObject sabotagedIndicator;
    public Color color;
    public string warningText;
    public string infoText;

    public float amountToFix = 100f;

    public List<PlayableCharacter> playersThatFixed = new List<PlayableCharacter>();
    [SerializeField] private GameObject animationObject;
    private Target sabotageMarker;
    private GameSceneManager gameSceneManager;
    private TimerManager timerManager;
    private SabotageManager sabotageManager;

    public int fixTimeFactor = 7;

    public int sabotageDelaySeconds = 5;
    
    
    // Start is called before the first frame update
    void Start() {
        // TODO Make all sabotagables glow red for traitors when not sabotaged
        isSabotaged = false;
        base.Start();

        sabotageMarker = gameObject.AddComponent<Target>() as Target;
        sabotageMarker.boxText = "SABOTAGE";
        sabotageMarker.TargetColor = Color.red;
        sabotageMarker.enabled = false;
        sabotageMarker.NeedArrowIndicator = false;
    
        gameSceneManager = GameObject.Find("/GameSceneManager").GetComponent<GameSceneManager>();
        sabotageManager = GameObject.Find("/SabotageManager").GetComponent<SabotageManager>();
        timerManager = GameObject.Find("/TimerManager").GetComponent<TimerManager>();
    }

    void Update() {
        /* Master client checks if sabotage is sabotaged and then broadcasts how much is left for each player to fix, the number of players
        currently fixing and updating the amount of fixing needing to be done
        */
        if (PhotonNetwork.LocalPlayer.IsMasterClient && isSabotaged) {
            sabotageManager.SetAmountToFix(amountToFix);
            sabotageManager.SetNumPlayersFixing(numberOfPlayersFixing);
            amountToFix -= numberOfPlayersFixing * fixTimeFactor * Time.deltaTime;
            //If there is no more to fix then call fix RPC for all players i.e. tell each player that sabotage is fixed and finished and inform the sabotagen manager of that
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
            timerManager.StartTimer(Timer.sabotageTimer);
            View.RPC("Sabotage", RpcTarget.All);
            sabotageManager.SabotageStarted();
        } else if (isSabotaged && (Team.Real | Team.Ghost).HasFlag(character.team)) {
            // If a sabotage has started then any player can attempt to fix
            if (!fixing) {
                sabotageManager.LocalPlayerFixing();    
                character.Fix(this);
                fixing = true;
                View.RPC("IncrementNumberOfFixers", PhotonNetwork.MasterClient);
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
            View.RPC("DecrementNumberOfFixers", PhotonNetwork.MasterClient);
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
        if (!isSabotaged && character.team == Team.Traitor && !Timer.sabotageTimer.IsStarted()) return true;
        // If a sabotage has started then any player can attempt to fix        
        if (isSabotaged && (Team.Real | Team.Ghost).HasFlag(character.team)) return true;
        return false;
    }

    [PunRPC]
    public virtual void Sabotage() {
        StartCoroutine(SabotageEnumerator());
        StartCoroutine(StartAnimation());
    }

    //Start sabotage after a given number of seconds
    //Set task, set as true and give marker
    public IEnumerator SabotageEnumerator() {
        DisableSabotageMarker();
        yield return new WaitForSeconds(5);
        sabotageManager.SetBackgroundImageColor(this);
        AddTaskWithTimerRPC(Timer.sabotageTimer);
        task.isUndoable = false;
        task.description = "Fix the " + this.name + "";
        isSabotaged = true;
        EnableTaskMarker(); 
    }

    [PunRPC]
    public virtual void Fix() {
        // If the sabotagable is fully fixed
        isSabotaged = false;

        // Tell everyone that the task is now completed
        task.CompleteRPC(false);
        task = null;
        Destroy(GetComponent<Task>());
        DisableTaskMarker();
        
        sabotageManager.SabotageFixed();
        animationObject.SetActive(false);

        PlayableCharacter me = NetworkManager.instance.GetMe();
        if (inRange && me is Traitor) {
            EnableSabotageMarker();
        } else if (me is Loyal) {
            me.EnableTaskMarker();
        }

        // After an item is fixed its no longer interactable for anyone
        Destroy(this);
    }

    public IEnumerator StartAnimation(){
        yield return new WaitForSeconds(5);
        animationObject.SetActive(true);
    }

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
