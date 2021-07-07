using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Target
{
    public string id;
    public string information;
    public string model;
    public float x_location;
    public float y_location;
    public float floor;
    public string museums_id; 

    public Target(string id, string information, string model, float x_location, float y_location, float floor, string museums_id)
    {
        this.id = id;
        this.information = information;
        this.model = model;
        this.x_location = x_location;
        this.y_location = y_location;
        this.floor = floor;
        this.museums_id = museums_id;
    }

    public Target()
    {

    }
}
