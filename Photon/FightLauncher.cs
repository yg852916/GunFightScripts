using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class FightLauncher : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    [SerializeField] GameObject loadingPanel;
    PhotonView pv;
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    void Start()
    {
        //Initialization();
        PhotonNetwork.Instantiate("PlayerManager", Vector3.zero, Quaternion.identity);
        if (PhotonNetwork.IsMasterClient)
        {
            //CreateAllItem();
            StartCoroutine(Enum_CreateAllItem());
        }
    }
    [PunRPC]
    void RPC_LoadingComplete()
    {
        StartCoroutine(Enum_LoadingComplete());
    }
    IEnumerator Enum_LoadingComplete()
    {
        float duration = 2;
        float timer = duration;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            Color color = loadingPanel.GetComponent<Image>().color;
            color.a = (timer / duration);
            loadingPanel.GetComponent<Image>().color = color;
            yield return new WaitForEndOfFrame();
        }
        loadingPanel.SetActive(false);
    }
    void Initialization()
    {


        PhotonNetwork.Instantiate("PlayerManager", Vector3.zero, Quaternion.identity);

        /*if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.InstantiateRoomObject("AK47", new Vector3(1.5f, 0.9f, 2.3f), Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject("AK47", new Vector3(1f, 0.9f, 2.3f), Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject("M1911", new Vector3(0.5f, 0.9f, 2.3f), Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject("M1911", new Vector3(0f, 0.9f, 2.3f), Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject("M4_8", new Vector3(-0.5f, 0.9f, 2.3f), Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject("M4_8", new Vector3(-1f, 0.9f, 2.3f), Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject("M107", new Vector3(-1.5f, 0.9f, 2.3f), Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject("M107", new Vector3(-2f, 0.9f, 2.3f), Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject("M249", new Vector3(-2.5f, 0.9f, 2.3f), Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject("M249", new Vector3(-3f, 0.9f, 2.3f), Quaternion.identity);

            PhotonNetwork.InstantiateRoomObject("RGD-5", new Vector3(1f, 1f, 3f), Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject("RGD-5", new Vector3(0f, 1f, 3f), Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject("RGD-5", new Vector3(-1f, 1f, 3f), Quaternion.identity);

            PhotonNetwork.InstantiateRoomObject("hookSkillItem", new Vector3(1f, 0.5f, 4f), Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject("hookSkillItem", new Vector3(-1f, 0.5f, 4f), Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject("hookSkillItem", new Vector3(-3f, 0.5f, 4f), Quaternion.identity);

            PhotonNetwork.InstantiateRoomObject("PotionHealth", new Vector3(Random.Range(-10, 10), 10, Random.Range(-10, 10)), Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject("PotionHealth", new Vector3(Random.Range(-10, 10), 10, Random.Range(-10, 10)), Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject("PotionHealth", new Vector3(Random.Range(-10, 10), 10, Random.Range(-10, 10)), Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject("PotionHealth", new Vector3(Random.Range(-10, 10), 10, Random.Range(-10, 10)), Quaternion.identity);

            CreateAllItem();
        }*/

    }
    IEnumerator Enum_CreateAllItem()
    {
        Transform[] allPoint = ItemRebirthPoint.instance.allPoint;
        for (int i = 0; i < allPoint.Length; i++)
        {
            PhotonNetwork.Instantiate("RGD-5", allPoint[i].position, Quaternion.identity);
            int randomNumGun = Random.Range(1, 6);
            int randomNumItem = Random.Range(1, 11);
            //Debug.Log(randomNumItem + "  " + randomNumGun);
            if (randomNumItem > 3)
            {
                PhotonNetwork.Instantiate("PotionHealth", allPoint[i].position, Quaternion.identity);
            }
            if (randomNumItem > 6)
            {
                PhotonNetwork.Instantiate("hookSkillItem", allPoint[i].position, Quaternion.identity);
            }
            if (randomNumGun == 1)
            {
                PhotonNetwork.Instantiate("AK47", allPoint[i].position, Quaternion.identity);
            }
            else if (randomNumGun == 2)
            {
                PhotonNetwork.Instantiate("M1911", allPoint[i].position, Quaternion.identity);
            }
            else if (randomNumGun == 3)
            {
                PhotonNetwork.Instantiate("M4_8", allPoint[i].position, Quaternion.identity);
            }
            else if (randomNumGun == 4)
            {
                PhotonNetwork.Instantiate("M107", allPoint[i].position, Quaternion.identity);
            }
            else if (randomNumGun == 5)
            {
                PhotonNetwork.Instantiate("M249", allPoint[i].position, Quaternion.identity);
            }
        }

        pv.RPC("RPC_LoadingComplete", RpcTarget.AllBuffered);
        yield return 0;
    }
    void CreateAllItem()
    {

        Transform[] allPoint = ItemRebirthPoint.instance.allPoint;
        for (int i = 0; i < allPoint.Length; i++)
        {
            PhotonNetwork.InstantiateRoomObject("RGD-5", allPoint[i].position, Quaternion.identity);
            int randomNumGun = Random.Range(1, 6);
            int randomNumItem = Random.Range(1, 11);
            //Debug.Log(randomNumItem + "  " + randomNumGun);
            if (randomNumItem > 3)
            {
                PhotonNetwork.InstantiateRoomObject("PotionHealth", allPoint[i].position, Quaternion.identity);
            }
            if (randomNumItem > 6)
            {
                PhotonNetwork.InstantiateRoomObject("hookSkillItem", allPoint[i].position, Quaternion.identity);
            }
            if (randomNumGun == 1)
            {
                PhotonNetwork.InstantiateRoomObject("AK47", allPoint[i].position, Quaternion.identity);
            }
            else if (randomNumGun == 2)
            {
                PhotonNetwork.InstantiateRoomObject("M1911", allPoint[i].position, Quaternion.identity);
            }
            else if (randomNumGun == 3)
            {
                PhotonNetwork.InstantiateRoomObject("M4_8", allPoint[i].position, Quaternion.identity);
            }
            else if (randomNumGun == 4)
            {
                PhotonNetwork.InstantiateRoomObject("M107", allPoint[i].position, Quaternion.identity);
            }
            else if (randomNumGun == 5)
            {
                PhotonNetwork.InstantiateRoomObject("M249", allPoint[i].position, Quaternion.identity);
            }
        }
    }
    public override void OnLeftRoom()
    {
        //base.OnLeftRoom();
        Debug.Log("OnLeftRoom...");

        //reset
        PhotonNetwork.LocalPlayer.CustomProperties.Clear();


        SceneManager.LoadScene(0);
    }
}
