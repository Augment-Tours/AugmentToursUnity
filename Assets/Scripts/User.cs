using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class User
{
    public string userName;
    public int userScore; 

    public User(string userName, int userScore)
    {
        this.userName = userName;
        this.userScore = userScore;
    }

    public User()
    {

    }
}
