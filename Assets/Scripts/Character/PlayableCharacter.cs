using Photon.Pun;
using UnityEngine;

public abstract class PlayableCharacter : Character {
    public Meal meal;
    public Color colour;

    [PunRPC]
    public void AssignColour(float r, float g, float b) {
        colour = new Color(r,g,b);
        //meal.colour = new Color(r,g,b);
    }

    [PunRPC]
    public void SwapMeal(int playerViewId) {
        PhotonView playerView = PhotonView.Find(playerViewId);
        PlayableCharacter player = playerView.GetComponent<PlayableCharacter>();
        Meal tempMeal = player.meal;
        player.meal = meal;
        meal.transform.parent = player.transform;
        meal = tempMeal;
        meal.transform.parent = transform;
    }
}