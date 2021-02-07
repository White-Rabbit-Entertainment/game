using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{

    public GameObject playerPrefab;
    // Start is called before the first frame update
    void Start()
    {
       PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0,10,0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
