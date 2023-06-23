using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateQuest : MonoBehaviour
{
    public Quests questToGive;
   
    public GameObject questBoard;

    private void Start() {
        
    }
    public void GiveQuestToPlayer(ScriptableObject questToGive)
    {
        //questBoard.GetComponent<MainCanvas>().UpdateQuest(questToGive)
    }
}
