using UnityEngine;
using System.Collections;
using System.IO;
using MTUnity.Utils;
using UnityEngine.UI;
using MTUnity.Actions;
using MTXxtea;
using System;
using System.Collections.Generic;
using MTUnity;
//using Facebook.Unity;



public enum SettingEnum
{
    
    SoundControl,


    TestLvNum
   
}


public class LimitedQueue<T> : Queue<T>
{
    public int Limit { get; set; }

    public LimitedQueue(int limit) : base(limit)
    {
        Limit = limit;
    }

    public new void Enqueue(T item)
    {
        while (Count >= Limit)
        {
            Dequeue();
        }
        base.Enqueue(item);
    }
}


public class SettingMgr : MonoBehaviour {

    public static SettingMgr current;
    void Awake()
    {

        current = this;
        LoadFile();
        InitState();
        Register();

    }




    void Register()
    {
       // LoadDone += AdMgr.RegisterAllAd;
      
    }


    public Action LoadDone;

 
    void Start()
    {


        if (LoadDone != null)
        {
            LoadDone();
            //FacebookMgr.current.InitFacebook();
            
        }
        SFXPool.Instance.Init(5);
    }
    const string settingFileName = "setting.dt";
    //public UserModel _user;
    public int SoundControl = 1; //bool





   




    public void LoadFile()
    {
        var filePath = GetPath();
        if (!File.Exists(filePath))
        {

            SaveToFile();
            
        }
        LoadSetting();
    }


    public Toggle sound;


    void LoadSetting()
    {
        var bt = File.ReadAllBytes(GetPath());
        string content =MTXXTea.DecryptToString (bt, SKEY); //File.ReadAllText(GetPath());


        MTJSONObject setJs = MTJSON.Deserialize(content);
        if(setJs == null)
        {
            SaveToFile();
        }else
        {
            SoundControl = setJs.GetInt(SettingEnum.SoundControl.ToString());


            









            
        }

        SetToggleState();

        LoadTrack();
        AddToggleListener();

    }

    void InitState()
    {

        //MTTracker.Instance.UpdateConfVersion(confversion);
    }




    const string TRACK_FILE = "track.tr";

    public LimitedQueue<TrackData> _trackQueue = new LimitedQueue<TrackData>(3);
    void LoadTrack()
    {
        if (!SoFileMgr.Exists(TRACK_FILE))
        {
            for(int i = 0; i < 3; i ++)
            {
                _trackQueue.Enqueue(new TrackData());
            }
            SaveTrack();
        }else
        {
            var content = SoFileMgr.Load(TRACK_FILE);
            var mtJson = MTJSON.Deserialize(content);
            for(int i = 0; i < mtJson.count; i++)
            {
                var trackData = new TrackData();
                trackData.InitBy(mtJson[i]);
                _trackQueue.Enqueue(trackData);
            }
        }
       

    }



   public void SaveTrack()
    {
        MTJSONObject trackList = MTJSONObject.CreateList();
        if(_trackQueue.Count == 0)
        {
            for(int i = 0; i < 3; i ++)
            {
                _trackQueue.Enqueue(new TrackData());
            }
        }

        var trackArr = _trackQueue.ToArray();
        for(int i  = 0; i <trackArr.Length;i++)
        {
            var curData = trackArr[i];
            trackList.Add(curData.ToJson());

        }
        SoFileMgr.Save(TRACK_FILE, trackList.ToString());


    }






    void AddToggleListener()
    {
        if (sound != null)
        {
            sound.onValueChanged.AddListener(OnsoundToggle);
        }
    }

    void SetToggleState()
    {
        if (sound != null)
        {
            sound.isOn = SoundControl == 1;
        }



    }

    void PlayToggleSound()
    {
        //SoundManager.Current.Play_switch(0);
    }
    void OnsoundToggle(bool b)
    {
        //Debug.Log("OnsoundToggle" + b.ToString());
        PlayToggleSound(); 
        if (b)
        {
            SoundControl = 1;
        }else
        {
            SoundControl = 0;
        }
    }










    string GetPath()
    {
        return Application.persistentDataPath + "/" + settingFileName;
    }

   public void SaveToFile()
    {
        MTJSONObject setJs = MTJSONObject.CreateDict();


        setJs.Set(SettingEnum.SoundControl.ToString(), SoundControl);



        var bt = MTXXTea.Encrypt(setJs.ToString(), SKEY);

        SaveTrack();
        File.WriteAllBytes(GetPath(), bt);
    }

    public static readonly string SKEY = "b8167365ee0a51e4dcc49";








    IEnumerator DelayHide(float t,GameObject g)
    {
        yield return new WaitForSeconds(t);
        g.SetActive(false);

    }








    internal UserModel _user;
}
