using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class ChatManager : MonoBehaviour
{
    PhotonView pv;

    public int maxMessage;
    public bool isEndEdit;

    //display contol


    public Transform chatContentParent;
    public InputField messageInputField;

    public GameObject chatCellPrototype;
    float chatCellHegiht;
    public RectTransform content;

    public GameObject scrollBarVertical;
    public GameObject viewport;

    Color chatManagerColor;
    //date
    //[SerializeField] List<GameObject> messages;
    [SerializeField] List<Message> messages = new List<Message>();
    //chat control end

    bool isChatting = false;
    bool isUpdating = false;
    class Message {
        public float duration = 5;
        public GameObject prefab;
        public Message(GameObject _prefab)
        {
            prefab = _prefab;
        }
    }
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    private void Start()
    {
        chatManagerColor = GetComponent<Image>().color;
        chatCellHegiht = chatCellPrototype.GetComponent<RectTransform>().rect.height;

        SetOpening(false);
    }
    private void Update()
    {
        //ChatControl();
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (isChatting)
            {
                isChatting = false;
                if (messageInputField.text != "")
                {
                    pv.RPC("RPC_SendMessage", RpcTarget.All, messageInputField.text);
                    messageInputField.text = "";
                }
                SetOpening(false);
            }
            else
            {
                isChatting = true;
                SetOpening(true);
                messageInputField.ActivateInputField();
            }
        }

        if (!isChatting && isUpdating)
        {
            DisplayUpdate();
        }
    }
    void DisplayUpdate()
    {
        bool isAllZero = true;
        for (int i = messages.Count - 1; i >= 0; i--)
        {
            if (messages[i].duration <= 0)
            {
                messages[i].prefab.GetComponent<CanvasGroup>().alpha = 0;
            }
            else
            {
                messages[i].duration -= Time.deltaTime;
                isAllZero = false;
            }
        }
        if (isAllZero)
        {
            isUpdating = false;
        }
    }
    void ChatControl()
    {
        if (!messageInputField.isFocused)
        {
            if (isEndEdit)
            {
                isEndEdit = false;
                if(Input.GetKeyDown(KeyCode.Return))
                {
                    if (messageInputField.text != "")
                    {
                        pv.RPC("RPC_SendMessage", RpcTarget.All, messageInputField.text);
                        messageInputField.text = "";
                    }
                }
                SetOpening(false);
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                SetOpening(true);
                messageInputField.ActivateInputField();
            }
        }
    }
    [PunRPC]
    void RPC_SendMessage(string str)
    {
        if (messages.Count >= maxMessage)
        {
            Destroy(messages[0].prefab);
            messages.RemoveAt(0);
        }

        GameObject chatCell = Instantiate(chatCellPrototype, chatContentParent);
        chatCell.SetActive(true);
        chatCell.GetComponentInChildren<TMPro.TMP_Text>().text = str;
        Message message = new Message(chatCell);
        messages.Add(message);

        float currentHeight = messages.Count * chatCellHegiht;
        if (currentHeight > content.rect.height)
        {
            content.sizeDelta += new Vector2(0, chatCellHegiht);
            scrollBarVertical.GetComponent<Scrollbar>().value = 0;
        }
        SetOpening(false);
    }
    public void SetOpening(bool isOpen)
    {
        if (isOpen)
        {
            for (int i = 0; i < messages.Count; i++)
            {
                messages[i].prefab.GetComponent<CanvasGroup>().alpha = 1;

                messages[i].prefab.SetActive(true);

                messages[i].duration = 5;
            }
        }

        scrollBarVertical.transform.parent.transform.gameObject.SetActive(isOpen);
        messageInputField.transform.parent.transform.gameObject.SetActive(isOpen);

        Color changeColor = chatManagerColor;
        if (!isOpen)
        {
            changeColor.a = 0;
            isUpdating = true;
        }
        GetComponent<Image>().color = changeColor;

        scrollBarVertical.GetComponent<Scrollbar>().value = 0;

    }
}
