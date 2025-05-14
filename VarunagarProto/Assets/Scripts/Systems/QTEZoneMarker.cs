using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEZoneMarker : MonoBehaviour
{
    public string zoneName = "Zone";
    public float angleSpan = 85f; // largeur en degrés
    public Color debugColor = Color.cyan;
    public bool successZone = true;
    public int Affinity = 0;
    public int Level = 1;
    public bool Hit = false;

    public QTEZone DataZone;
}
