using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQTEButton", menuName = "QTE/ButtonData")]
public class QTEUpgrade : ScriptableObject
{
    public string buttonID;
    public Sprite imageToDisplay;
}