using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
* 对话管理器
* 用于处理对话框和对话文本的显示
* 以及处理用户输入和选择
*/
public class DialogueMangement : MonoBehaviour
{
    public static DialogueMangement instance; //让任意脚本都可以访问这个实例
    public GameObject dialogueBox; // 用于引用对话框
    public GameObject inputPanel; // 用于引用输入面板
    public GameObject choicePanel; // 用于引用选择对话框
    public TextMeshProUGUI dialogueText, nameText; // 用于引用对话文本和名称文本
    public TMP_InputField userInputField; // 用于引用输入字段

    public bool isScrolling; // 用于检查文本是否正在滚动
    public Queue<string> dialogueList; // 用于动态存储对话的列表, 用于输出的list
    private ChatGPTManager chatGPTManager; // 这将允许你在Unity编辑器中拖拽ChatGPTManager的实例。
    private bool isLoading; // 用于检查是否正在加载对话,如果在loading的刷，不允许打开输入面板

    private void Awake(){
        if(instance == null){
            instance = this;
            Debug.Log("DialogueMangement instance created");
        }else{
            Debug.Log("DialogueMangement instance bug detected! Destroying the new instance.");
            if(instance != this){
                Destroy(gameObject);
            }
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start(){
        try{
            // 获取同一个GameObject上的ChatGPTManager组件
            chatGPTManager = GetComponent<ChatGPTManager>();
        }catch{
            Debug.LogError("ChatGPTManager component not found in the scene.");
        }
        // 初始化对话列表
        dialogueList = new Queue<string>();
        // 隐藏输入面板
        inputPanel.SetActive(false);
        // 隐藏选择对话框
        choicePanel.SetActive(false);
        // 为input field添加一个监听器来响应用户按下回车
        userInputField.onEndEdit.AddListener(HandleInputSubmit);
        //TODO: 未来支持语音输入
    }

    void Update(){
        //检测输入面板
        HandleInputPanel();
        //检测对话列表
        DialogueListMonitor();
        //关闭对话框
        //CloseDialogue();
    }

    //当有NPC想要使用对话框时，调用这个方法，并且提供对话文本和是否有名字
    public void ShowDialogue(string[] lines, bool _hasName){
        dialogueBox.SetActive(true);
        nameText.gameObject.SetActive(_hasName);
        foreach(string line in lines){
            dialogueList.Enqueue(line);
        }
        PrintText();
    }
    

    //当对话文本为空，用户没有打开输入面板，同时也没有选项，则关闭对话框
    public void CloseDialogue(){
        if(dialogueList.Count == 0 && !inputPanel.activeSelf && !choicePanel.activeSelf && !isLoading){
            dialogueBox.SetActive(false);
        }
    }

    public void SetLoading(bool _isLoading){
        isLoading = _isLoading;
    }

    //唤醒对话框
    public void OpenDialogueBox(){
        dialogueBox.SetActive(true);
    }
    //关闭对话框
    public void CloseDialogueBox(){
        dialogueBox.SetActive(false);
    }


    //隐藏对话文本

    //显示输入面板,当用户按下回车键时
    private void HandleInputPanel(){
        // 检测input是否被打开，如果被打开，则回车键代表提交输入，如果没打开，则回车键代表打开输入面板
        if(inputPanel != null && inputPanel.activeSelf){
            if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Escape)){
                inputPanel.SetActive(false);
            }
        }else if (inputPanel != null && !inputPanel.activeSelf){
            // 检测用户是否按下回车键, 如果按下回车键, 则激活输入面板
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if(!isLoading){
                    Debug.Log("Someone is talking!");
                }else{
                    // 检查输入面板是否存在并且未激活
                    if (inputPanel != null && !inputPanel.activeSelf){
                        // 如果输入面板存在且未激活，则激活它
                        inputPanel.SetActive(true);
                        userInputField.Select();
                        userInputField.ActivateInputField();
                    }
                }
            }
        }
    }

    //检测如果dialogueList不为空，尝试输出整句对话并且滚动文本，滚动结束后，将输出的文本从队列中移除
    private void DialogueListMonitor(){
        if(!isScrolling && Input.GetKeyDown(KeyCode.Space)){ 
            PrintText();
        }
    }

    private void PrintText(){
        CheckName();       
        if(dialogueList.Count > 0){
            string dialogue = dialogueList.Dequeue();
            StartCoroutine(PrintTextScrolling(dialogue));
        }else{
            CloseDialogue();
        }
    }

    //检查输入的文本是否为名字（n-）
    private void CheckName(){
        if (dialogueList.Count > 0){
            //检查队列中的下一个对话是否以n-开头
            string dialogue = dialogueList.Peek();
            if (dialogue.StartsWith("n-")){
                Debug.Log("Name detected");
                string name = dialogue.Replace("n-", "");
                nameText.text = name;
                if (name.ToLower() == " user"){
                    // 如果名称是"user"，显示输入面板
                    inputPanel.SetActive(true);
                    userInputField.ActivateInputField(); // 激活输入字段以便用户可以开始输入
                    // 等待用户输入（这将在其他地方处理）
                }
                dialogueList.Dequeue();
            }
        }
    }

    //处理输入提交

    //处理对话文本滚动
    private IEnumerator PrintTextScrolling(string text){
        isScrolling = true;
        dialogueText.text = ""; //保证每个字符都是从最开始显示
        foreach( char letter in text.ToCharArray()){
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
        isScrolling = false;
    }

    //处理对话文本输入
    private void HandleInputSubmit(string input)
    {
        Debug.Log("用户输入: " + input);
        // 这里调用ChatGPTManager的AskChatGPT函数
        chatGPTManager.AskChatGPT(input);
        // 显示user输入的文本
        StartCoroutine(PrintTextScrolling(input));

        userInputField.text = ""; // 清空输入字段
        // 隐藏输入面板
        inputPanel.SetActive(false);
    }
    //生成选择对话框

    //处理选择对话框
        //当玩家想直接和随行的鬼魂对话时的时候，可以


}
