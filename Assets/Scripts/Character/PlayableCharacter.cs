using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public abstract class PlayableCharacter : Character {
    public Color colour;

    public GameObject ghostPrefab;
    public GameObject mealPrefab;

    public ContextTaskUI contextTaskUI;

    public Player owner;

    public override void Start() { 
      owner = GetComponent<PhotonView>().Owner;
      // If the player has not yet been assinged a meal
      if (!NetworkManager.instance.PlayerHasProperty("MealId", owner)) {
        int mealId =  NetworkManager.instance.GetPlayerProperty<int>("MealId", owner);
        // Create a meal for them 
        Meal meal = PhotonNetwork.Instantiate(mealPrefab.name, new Vector3(0,0,0), Quaternion.identity).GetComponent<Meal>();
        // Set the colour of the meal to the player's colour
        meal.GetComponent<PhotonView>().RPC("SetColour", RpcTarget.All, colour.r, colour.g, colour.b);
        // Set their meal to this meal id
        SetMeal(meal);
      }

      base.Start();
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


    // Retrun the meal which this player owns
    public Meal GetMeal() {
      int mealId = NetworkManager.instance.GetPlayerProperty<int>("MealId", owner);
      return PhotonView.Find(mealId).GetComponent<Meal>();
    }

    // Sets the players meal to a given meal
    public void SetMeal(Meal meal) {
      NetworkManager.instance.SetPlayerProperty("MealId", meal.GetComponent<PhotonView>().ViewID, owner);
    }

    [PunRPC]
    public void Kill() {
        NetworkManager.instance.SetPlayerProperty("Team", Team.Ghost, owner);
        GetComponent<PhotonView>().RPC("DestroyPlayer", RpcTarget.All);
        PhotonNetwork.Instantiate(ghostPrefab.name, new Vector3(1,2,-10), Quaternion.identity);
    }

    [PunRPC]
    public void DestroyPlayer() {
        Destroy(gameObject);
    }

    [PunRPC]
    public void AssignColour(float r, float g, float b) {
        colour = new Color(r,g,b);
    }

    // Given a player to swap with, this player swaps meals with that player
    public void SwapMeal(PlayableCharacter player) {
        Meal tempMeal = player.GetMeal();
        player.SetMeal(this.GetMeal());
        this.SetMeal(tempMeal);
    }
}
