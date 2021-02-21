using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface Completable {
    bool isCompleted {get;}
    void Complete();
}
