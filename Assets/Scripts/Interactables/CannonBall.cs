using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TypeReferences;

public class CannonBall : Placeable {
    public List<TypeReference> destinationTypes;

    [PunRPC]
    public void AddDestination(int viewId) {
        PhotonView itemView = PhotonView.Find(viewId);
        destination = itemView.gameObject.GetComponent<PickupDestination>();
    }

    public void SetDestination(List<Transform> interactables) {
    List<PickupDestination> possibleDestinations = GetPossibleDestinations(interactables);
    if (possibleDestinations.Count > 0) {
      System.Random random = new System.Random(System.Guid.NewGuid().GetHashCode());
      int randomIndex = random.Next(possibleDestinations.Count);
      View.RPC("AddDestination", RpcTarget.All, possibleDestinations[randomIndex].GetComponent<PhotonView>().ViewID);
    }
  }

    public virtual List<PickupDestination> GetPossibleDestinations(List<Transform> interactables) {
    List<PickupDestination> possibleDestinations = new List<PickupDestination>();
    foreach (Transform interactable in interactables) {
      bool hasCorrectType = false;
      foreach(TypeReference type in destinationTypes) {
        if (interactable.GetComponent(type.Type) != null) hasCorrectType = true;
      }
      if (interactable.GetComponent<PickupDestination>() != null
      && hasCorrectType
      && !interactable.GetComponent<Interactable>().HasTask()) {
        possibleDestinations.Add(interactable.GetComponent<PickupDestination>());
      }
    }
    return possibleDestinations;
  }

    public override void Reset() {
        taskDescription = "Fill cannon";
        base.Reset();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Cannon") {
            Destroy(this.gameObject);
            task.Complete();
        }
    }

    
}
