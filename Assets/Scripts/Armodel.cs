using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Armodel
{
    public string id;
    public string name;
    public string description;
    public string model;
    public float x_location;
    public float y_location;
    public float z_location;
    public float floor;
    public string museums_id;
    public float x_scale;
    public float y_scale;
    public float z_scale;

    public Armodel(string id, string name,string description, string model, float x_location, float y_location, float z_location, float x_scale, float y_scale, float z_scale, float floor, string museums_id)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.model = model;
        this.x_location = x_location;
        this.y_location = y_location;
        this.z_location = z_location;
        this.floor = floor;
        this.museums_id = museums_id;
        this.x_scale = x_scale;
        this.y_scale = y_scale;
        this.z_scale = z_scale;
    }

    public Armodel()
    {

    }
}
