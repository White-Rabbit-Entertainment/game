using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TypeReferences;

public class CannonBall : Placeable {
  public List<TypeReference> destinationTypes;
  private List<PickupDestination> destinations;

  public void Start() {
    base.Start();
    destinations = new List<PickupDestination>(FindObjectsOfType<PickupDestination>());
    Debug.Log(destinations);
    if (destination == null) {
      SetDestination(destinations);
    }
  }

  [PunRPC]
  public void AddDestination(int viewId) {
      PhotonView itemView = PhotonView.Find(viewId);
      Debug.Log($"itemView {viewId}: {itemView}");
      destination = itemView.gameObject.GetComponent<PickupDestination>();
  }

  public void SetDestination(List<PickupDestination> destinations) {
    List<PickupDestination> possibleDestinations = GetPossibleDestinations(destinations);
    if (possibleDestinations.Count > 0) {
      System.Random random = new System.Random(System.Guid.NewGuid().GetHashCode());
      int randomIndex = random.Next(possibleDestinations.Count);
      Debug.Log(possibleDestinations[randomIndex]);
      Debug.Log($"id in set destination {possibleDestinations[randomIndex].GetComponent<PhotonView>().ViewID}");
      View.RPC("AddDestination", RpcTarget.All, possibleDestinations[randomIndex].GetComponent<PhotonView>().ViewID);
    }
  }

  public List<PickupDestination> GetPossibleDestinations(List<PickupDestination> destinations) {
    List<PickupDestination> possibleDestinations = new List<PickupDestination>();
    foreach (PickupDestination destination in destinations) {
      bool hasCorrectType = false;
      foreach(TypeReference type in destinationTypes) {
        if (destination.GetComponent(type.Type) != null) hasCorrectType = true;
      }
      if (destination != null && hasCorrectType) {
        possibleDestinations.Add(destination);
      }
    }
    return possibleDestinations;
  }

  public override void Reset() {
      taskDescription = "Fill cannon";
      base.Reset();
  }
}
