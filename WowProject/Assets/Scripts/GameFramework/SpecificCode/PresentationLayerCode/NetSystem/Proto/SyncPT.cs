using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTSyncCharacter : PTBase//ͬ��Э��
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