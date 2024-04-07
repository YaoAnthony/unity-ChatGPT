using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talkable : MonoBehaviour
{
    public void TalkWithSomeone(string tag, string[] lines, string NPCName)
    {
        Debug.Log("Talkable");
        if(DialogueMangement.instance.dialogueBox.activeInHierarchy == false)
        {
            Debug.Log(lines[0]);
            //TODO: setup NPC name
            DialogueMangement.instance.ShowDialogue(lines, true);
        }
    }
    
}
