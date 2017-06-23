using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConnectivityResult {
    bool Error { get; }
    string Detail { get; }    
}
