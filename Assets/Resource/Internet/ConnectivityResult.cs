using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectivityResult : IConnectivityResult {
    private bool error;
    private string detail;

    #region Getters & Setters
    public bool Error {
        get { return error; }
    }

    public string Detail {
        get { return detail; }
    }
    #endregion

    #region Constructor
    public ConnectivityResult(bool aError, string aDetail = null) {
        error = aError;
        detail = aDetail;
    }
    #endregion

}
