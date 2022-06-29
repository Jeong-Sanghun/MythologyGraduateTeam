using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class JsonManager    //SH
{
    //세이브데이터를 세이브해줌.
    //    public static void SaveJson(SaveDataClass saveData,int index)
    //    {
    //        string jsonText;


    //        //안드로이드에서의 저장 위치를 다르게 해주어야 한다
    //        //Application.dataPath를 이용하면 어디로 가는지는 구글링 해보길 바란다
    //        //안드로이드의 경우에는 데이터조작을 막기위해 2진데이터로 변환을 해야한다

    //        string savePath = Application.dataPath;
    //        string appender = "/userData/";
    //        string nameString = "SaveData";
    //        string dotJson = ".json";

    //#if UNITY_EDITOR_WIN

    //#endif
    //#if UNITY_ANDROID
    //        //이거나중에 살려야됨
    //        //savePath = Application.persistentDataPath;

    //#endif
    //        StringBuilder builder = new StringBuilder(savePath);
    //        builder.Append(appender);
    //        if (!Directory.Exists(builder.ToString()))
    //        {
    //            //디렉토리가 없는경우 만들어준다
    //            Directory.CreateDirectory(builder.ToString());

    //        }
    //        builder.Append(nameString);
    //        builder.Append(index.ToString());
    //        builder.Append(dotJson);

    //        jsonText = JsonUtility.ToJson(saveData, true);
    //        //ClassData to JsonText
    //        //이러면은 일단 데이터가 텍스트로 변환이 된다
    //        //jsonUtility를 이용하여 data인 WholeGameData를 json형식의 text로 바꾸어준다

    //        //파일스트림을 이렇게 지정해주고 저장해주면된당 끗
    //        FileStream fileStream = new FileStream(builder.ToString(), FileMode.Create);
    //        byte[] bytes = Encoding.UTF8.GetBytes(jsonText);
    //        fileStream.Write(bytes, 0, bytes.Length);
    //        fileStream.Close();
    //    }

    //    public T ResourceDataLoad<T>(string name)
    //    {
    //        //이제 우리가 이전에 저장했던 데이터를 꺼내야한다
    //        //만약 저장한 데이터가 없다면? 이걸 실행 안하고 튜토리얼을 실행하면 그만이다. 그 작업은 씬로더에서 해준다
    //        T gameData;

    //        string language = GameManager.singleton.saveDataTimeWrapper.nowLanguageDirectory;
    //        string directory = "JsonData/";

    //        string appender1 = name;
    ////        string appender2 = ".json";
    //        StringBuilder builder = new StringBuilder(directory);
    //        builder.Append(language);
    //        builder.Append(appender1);
    //        //      builder.Append(appender2);
    //        //위까지는 세이브랑 똑같다
    //        //파일스트림을 만들어준다. 파일모드를 open으로 해서 열어준다. 다 구글링이다
    //        TextAsset jsonString = Resources.Load<TextAsset>(builder.ToString());
    //        gameData = JsonUtility.FromJson<T>(jsonString.ToString());

    //        return gameData;
    //        //이 정보를 게임매니저나, 로딩으로 넘겨주는 것이당
    //    }

    //    //이거 세이브데이터타임래퍼
    //    public T ResourceDataLoadBeforeGame<T>(string name,string languageDirectory)
    //    {
    //        //이제 우리가 이전에 저장했던 데이터를 꺼내야한다
    //        //만약 저장한 데이터가 없다면? 이걸 실행 안하고 튜토리얼을 실행하면 그만이다. 그 작업은 씬로더에서 해준다
    //        T gameData;

    //        string language = languageDirectory;
    //        string directory = "JsonData/";

    //        string appender1 = name;
    //        //        string appender2 = ".json";
    //        StringBuilder builder = new StringBuilder(directory);
    //        builder.Append(language);
    //        builder.Append(appender1);
    //        //      builder.Append(appender2);
    //        //위까지는 세이브랑 똑같다
    //        //파일스트림을 만들어준다. 파일모드를 open으로 해서 열어준다. 다 구글링이다
    //        TextAsset jsonString = Resources.Load<TextAsset>(builder.ToString());
    //        gameData = JsonUtility.FromJson<T>(jsonString.ToString());

    //        return gameData;
    //        //이 정보를 게임매니저나, 로딩으로 넘겨주는 것이당
    //    }

    //    //세이브데이터 테스트를 위해 만든거
    public static void SaveJson<T>(T _data, string _name)
    {
        string _jsonText;


        //안드로이드에서의 저장 위치를 다르게 해주어야 한다
        //Application.dataPath를 이용하면 어디로 가는지는 구글링 해보길 바란다
        //안드로이드의 경우에는 데이터조작을 막기위해 2진데이터로 변환을 해야한다

        string _savePath = Application.dataPath;
        string _appender = "/userData/";
        string _nameString = _name + ".json";

#if UNITY_ANDROID
        //savePath = Application.persistentDataPath;
        
#endif
        StringBuilder _builder = new StringBuilder(_savePath);
        _builder.Append(_appender);
        if (!Directory.Exists(_builder.ToString()))
        {
            //디렉토리가 없는경우 만들어준다
            Debug.Log("뭐야");
            Directory.CreateDirectory(_builder.ToString());

        }
        _builder.Append(_nameString);
        //stringBuilder는 최적화에 좋대서 쓰고있다. string+string은 메모리낭비가 심하다
        // 사실 이정도 한두번 쓰는건 상관없긴한데 그냥 써주자. 우리의 컴은 좋으니까..

        _jsonText = JsonUtility.ToJson(_data, true);
        //이러면은 일단 데이터가 텍스트로 변환이 된다
        //jsonUtility를 이용하여 data인 WholeGameData를 json형식의 text로 바꾸어준다

        //파일스트림을 이렇게 지정해주고 저장해주면된당 끗
        FileStream _fileStream = new FileStream(_builder.ToString(), FileMode.Create);
        byte[] _bytes = Encoding.UTF8.GetBytes(_jsonText);
        _fileStream.Write(_bytes, 0, _bytes.Length);
        _fileStream.Close();
    }

    //    public SaveDataTimeWrapper LoadSaveDataTime()
    //    {
    //        //이제 우리가 이전에 저장했던 데이터를 꺼내야한다
    //        //만약 저장한 데이터가 없다면? 이걸 실행 안하고 튜토리얼을 실행하면 그만이다. 그 작업은 씬로더에서 해준다
    //        SaveDataTimeWrapper gameData;
    //        string loadPath = Application.dataPath;
    //        string directory = "/userData";
    //        string appender = "/SaveDataTimeWrapper.json";
    //#if UNITY_EDITOR_WIN

    //#endif

    //#if UNITY_ANDROID
    //        //loadPath = Application.persistentDataPath;


    //#endif
    //        StringBuilder builder = new StringBuilder(loadPath);
    //        builder.Append(directory);
    //        //위까지는 세이브랑 똑같다
    //        //파일스트림을 만들어준다. 파일모드를 open으로 해서 열어준다. 다 구글링이다
    //        string builderToString = builder.ToString();
    //        if (!Directory.Exists(builderToString))
    //        {
    //            //디렉토리가 없는경우 만들어준다
    //            Directory.CreateDirectory(builderToString);

    //        }
    //        builder.Append(appender);

    //        if (File.Exists(builder.ToString()))
    //        {
    //            //세이브 파일이 있는경우

    //            FileStream stream = new FileStream(builder.ToString(), FileMode.Open);

    //            byte[] bytes = new byte[stream.Length];
    //            stream.Read(bytes, 0, bytes.Length);
    //            stream.Close();
    //            string jsonData = Encoding.UTF8.GetString(bytes);

    //            //텍스트를 string으로 바꾼다음에 FromJson에 넣어주면은 우리가 쓸 수 있는 객체로 바꿀 수 있다
    //            gameData = JsonUtility.FromJson<SaveDataTimeWrapper>(jsonData);
    //        }
    //        else
    //        {
    //            //세이브파일이 없는경우
    //            gameData = new SaveDataTimeWrapper();
    //        }
    //        return gameData;
    //        //이 정보를 게임매니저나, 로딩으로 넘겨주는 것이당
    //    }


    //로딩, 게임매니저에서 호출
    public static T LoadSaveData<T>(string _name)
    {
        //이제 우리가 이전에 저장했던 데이터를 꺼내야한다
        //만약 저장한 데이터가 없다면? 이걸 실행 안하고 튜토리얼을 실행하면 그만이다. 그 작업은 씬로더에서 해준다
        T _gameData;
        string _loadPath = Application.dataPath;
        string _directory = "/userData";
        string _appender = "/";

        string _dotJson = ".json";

#if UNITY_ANDROID
            //loadPath = Application.persistentDataPath;


#endif
        StringBuilder _builder = new StringBuilder(_loadPath);
        _builder.Append(_directory);
        //위까지는 세이브랑 똑같다
        //파일스트림을 만들어준다. 파일모드를 open으로 해서 열어준다. 다 구글링이다
        string _builderToString = _builder.ToString();
        if (!Directory.Exists(_builderToString))
        {
            //디렉토리가 없는경우 만들어준다
            Directory.CreateDirectory(_builderToString);

        }
        _builder.Append(_appender);
        _builder.Append(_name);
        _builder.Append(_dotJson);

        if (File.Exists(_builder.ToString()))
        {
            //세이브 파일이 있는경우

            FileStream _stream = new FileStream(_builder.ToString(), FileMode.Open);

            byte[] _bytes = new byte[_stream.Length];
            _stream.Read(_bytes, 0, _bytes.Length);
            _stream.Close();
            string _jsonData = Encoding.UTF8.GetString(_bytes);

            //텍스트를 string으로 바꾼다음에 FromJson에 넣어주면은 우리가 쓸 수 있는 객체로 바꿀 수 있다
            _gameData = JsonUtility.FromJson<T>(_jsonData);
        }
        else
        {
            //세이브파일이 없는경우
            _gameData = default(T);
        }
        return _gameData;
        //이 정보를 게임매니저나, 로딩으로 넘겨주는 것이당
    }
}
