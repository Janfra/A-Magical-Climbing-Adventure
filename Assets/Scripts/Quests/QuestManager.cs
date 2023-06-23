using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public Quests currentQuest;

    public List<Quests> finishedQuests;
    // Start is called before the first frame update
    void Start()
    {
        
        finishedQuests = new List<Quests>();
    }

    public void ChangeCurrentQuest(ref Quests newQuest)
    {
        if(currentQuest != newQuest)
        {
            currentQuest = newQuest;
        }
        else {Debug.Log("you already have this quest");}
    }

    
}
