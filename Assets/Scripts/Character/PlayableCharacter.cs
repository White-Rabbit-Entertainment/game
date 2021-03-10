using Photon.Pun;

public abstract class PlayableCharacter : Character {
    public Meal meal;

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