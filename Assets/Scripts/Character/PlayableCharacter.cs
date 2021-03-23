using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public abstract class PlayableCharacter : Character {
    public GameObject ghostPrefab;

    public ContextTaskUI contextTaskUI;

    public Player Owner {
      get { return GetComponent<PhotonView>().Owner; }
    }

    public override void Start() { 
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

    [PunRPC]
    public void Kill() {
        NetworkManager.instance.SetPlayerProperty("Team", Team.Ghost, Owner);
        GetComponent<PhotonView>().RPC("DestroyPlayer", RpcTarget.All);
        GameObject newPlayer = PhotonNetwork.Instantiate(ghostPrefab.name, new Vector3(1,2,-10), Quaternion.identity);
        NetworkManager.myCharacter = newPlayer.GetComponent<PlayableCharacter>();
    }

    [PunRPC]
    public void DestroyPlayer() {
        Destroy(gameObject);
    }
}
