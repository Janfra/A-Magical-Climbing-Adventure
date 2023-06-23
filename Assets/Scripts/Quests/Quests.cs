using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Objects/new Quest", order = 1)]
public class Quests : ScriptableObject
{
   
   public string questTitle;

   public string questDescription;

   public string questMaxAmount;
}
