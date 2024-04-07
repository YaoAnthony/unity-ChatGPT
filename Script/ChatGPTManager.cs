using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using UnityEditor.PackageManager.Requests;
using UnityEngine.Events;
using System.Threading.Tasks;

public class ChatGPTManager : MonoBehaviour
{
    public OnResponeEvent OnResponse;

    [System.Serializable] 
    public class OnResponeEvent : UnityEvent<string> { }
    private OpenAIApi openAI = new();
    private List<ChatMessage> chatMessages = new();

    public async void AskChatGPT(string newText){
        DialogueMangement.instance.SetLoading(true);
        //检查用户是否想要让dongdong跟着他
        bool isFollow = await CheckIntent(newText, "想要你跟着他");

        if(isFollow){
            string[] responseText = { "n- 东东", "好的，我跟着你"};
            DialogueMangement.instance.ShowDialogue(responseText, true);
            DialogueMangement.instance.SetLoading(false);
            return;
        }


        
        ChatMessage newMessage = new()
        {
            Content = newText,
            Role = "user"
        };

        chatMessages.Add(new ChatMessage(){
            Content = "ChatGPT,首先请记住你的设定，你是一个冷漠，但内心藏着一丝温柔的职业御鬼师，名字叫东东，平时会将遇到的鬼魂直接消灭。", Role = "system"});
        chatMessages.Add(new ChatMessage(){
            Content = "这是你的记忆：今天是你的度假，你身穿着一身粉色的衣服和棕色的鞋子，带了少许点食物，来到一片遗迹的阴凉地方休息，这时有个陌生的人靠了过来，你问他：？ 他回答道：", Role = "system"});
        chatMessages.Add(newMessage);
        chatMessages.Add(new ChatMessage(){Content = "你面对这样的回答，你会怎么说？", Role = "system"});
        chatMessages.Add(new ChatMessage(){Content = "回答前，请记住以下规则：1.只回答问题，不要描述自己的状态。2.使用第一人称。3.引导对方邀请你加入他的队伍", Role = "system"});
        CreateChatCompletionRequest req = new()
        {
            Messages = chatMessages,
            Model = "gpt-4-0125-preview",
        };

        //get response from GPT-3.5
        var res = await openAI.CreateChatCompletion(req);

        if(res.Choices != null && res.Choices.Count > 0){
            var chatResponse = res.Choices[0].Message;
            chatMessages.Add(chatResponse);

            //Debug.Log(chatMessages[^1].Content);
            string[] responseText = { "n- 东东", chatMessages[^1].Content,"n- user",};
            DialogueMangement.instance.ShowDialogue(responseText, true);
            DialogueMangement.instance.SetLoading(false);
            //OnResponse.Invoke(chatResponse.Content);
        }
    }

    //通过chatgpt检查用户是否有某个意向，如果有某个意向，则需要GPT返回true
    public async Task<bool> CheckIntent(string newText, string intent){
        string question = "这是用户发的文字:\""+ newText + "\"。请帮我分析这个用户有没有" + intent + "的意向,如果有请返回英文的true,否则返回英文的false。\n\n 是否拥有: <fill in>";
        List<ChatMessage> tempChat = new()
        {
            new ChatMessage() { Content = question, Role = "system" }
        };
        CreateChatCompletionRequest req = new()
        {
            Messages = tempChat,
            Model = "gpt-4-0125-preview",
        };

        //get response from GPT-4
        var res = await openAI.CreateChatCompletion(req);
        if(res.Choices != null && res.Choices.Count > 0){
            var chatResponse = res.Choices[0].Message;
            tempChat.Add(chatResponse);
            string str = tempChat[^1].Content;
            if(str.Contains("true")){
                return true;
            }
        }
        return false;
    }

}
