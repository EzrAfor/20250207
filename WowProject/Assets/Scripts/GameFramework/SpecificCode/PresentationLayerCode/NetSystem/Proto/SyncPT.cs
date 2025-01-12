using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTSyncCharacter : PTBase//同步协议
{
    public PTSyncCharacter() { protoName = "PTSyncCharacter"; }
    public string id;
    public float x;
    public float y;
    public float z;
    public float ex;
    public float ey;
    public float ez;
    public CHARACTERSTATE characterState;
}