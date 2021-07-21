using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Favorite 
{
    public string id;
    public string armodels_id;
    public string accounts_id;

    public Favorite(string armodels_id, string accounts_id)
    {
     
        this.armodels_id = armodels_id;
        this.accounts_id = accounts_id;
    }
    
    
}
