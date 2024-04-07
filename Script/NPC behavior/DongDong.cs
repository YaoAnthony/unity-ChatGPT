using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//这里将定义NPC的所有行为，包括对话，记忆等，如果GPT想要和玩家对话，就需要调用这个类
public class DongDong : MonoBehaviour
{
    public string NPCName;
    public string description;
    public string[] memory;
    public string[] dialogueLines;

    public Talkable talkable;
    public bool wantToTalk;
    private string tagWithSomeoneWantToTalk;

    public NPCFollow npcFollow;

    //用于标记重要的行为，如果达成了某项重要行为，则+1
    private int importantActioStep;
    

    void Start()
    {
        wantToTalk = false;
        importantActioStep = 0;
        tagWithSomeoneWantToTalk = "";
    }

    //遇到了想遇到的人，想说才能说
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(tagWithSomeoneWantToTalk)) wantToTalk = true;
    }
    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag(tagWithSomeoneWantToTalk)) wantToTalk = false;
    }

    void Update()
    {
        switch(importantActioStep)
        {
            case 0: StepOne(); break;
            case 1: StepTwo(); break;
            case 2: StepThree(); break;
            default: break;
        }
    }

    private void StepOne (){
        //如果玩家在NPC面前按下空格，将触发对话
        dialogueLines = new string[] { 
            "n- ???", 
            "怎么了？",
            "n- user"
        };
        tagWithSomeoneWantToTalk = "Player";
        
        if (Input.GetKeyDown(KeyCode.Space) && wantToTalk){
            talkable.TalkWithSomeone(tagWithSomeoneWantToTalk, dialogueLines, NPCName);
            //如果玩家邀请NPC跟随，将触发NPC跟随
            //importantActioStep = -1;
        }
        
    }

    

    private void StepTwo (){
        //如果玩家在NPC面前按下空格，将触发对话
        dialogueLines = new string[] { 
            "n- ???", 
            "怎么了？",
            "n- user"
        };

        if (Input.GetKeyDown(KeyCode.Space) && talkable != null && wantToTalk){
            talkable.TalkWithSomeone("Player", dialogueLines, NPCName);
        }else{
            Debug.Log("No talkable component found");
        }
        //如果玩家邀请NPC跟随，将触发NPC跟随
    }

    private void StepThree (){
        //如果玩家在NPC面前按下空格，将触发对话
        dialogueLines = new string[] { 
            "n- ???", 
            "怎么了？",
            "n- user"
        };

        if (Input.GetKeyDown(KeyCode.Space) && talkable != null && wantToTalk){
            talkable.TalkWithSomeone("Player", dialogueLines, NPCName);
        }else{
            Debug.Log("No talkable component found");
        }
        //如果玩家邀请NPC跟随，将触发NPC跟随
    }
}
