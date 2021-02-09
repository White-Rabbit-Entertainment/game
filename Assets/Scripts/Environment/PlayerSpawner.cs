using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{

    public GameObject robberPrefab;
    public GameObject seekerPrefab;
    // Start is called before the first frame update
    void Start()
    {
      GameObject playerPrefab;
      if (PhotonNetwork.LocalPlayer.CustomProperties["PlayerTeam"] == "seeker") {
          playerPrefab = seekerPrefab;
      } else {
          playerPrefab = robberPrefab;
      }
      PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(1,2,-10), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
