using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using System.Threading;
using TMPro;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using UnityEngine.Networking;
using System.Text.RegularExpressions;

public class FirestoreDataManager : MonoBehaviour
{
    private static FirestoreDataManager instance;

    private string authCode;
    private string userID;
    private bool isConnected;

    public static FirestoreDataManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(gameObject);
        }

    }

    public void StartPlayLogin()
    {
        isConnected = false;

        PlayGamesPlatform.Activate();
        GPGSLogin();
    }

    public void SavaData()
    {
        if (isConnected)
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"AccountSettings", GameSystem.instance.AccountSettings.NoAds },
            };

            DocumentReference docRef = db.Collection("PlayerData").Document(userID);
            docRef.SetAsync(data).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Save completed");
                }

                else
                {
                    Debug.Log("Error saving data");
                }
            });
        }

        else
        {
            Debug.Log("Save error : Firebase not connected");
        }
    }



    public void LoadData()
    {
        if (isConnected)
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            
            DocumentReference docRef = db.Collection("PlayerData").Document(userID);
            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                DocumentSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    GameSystem.instance.AccountSettings.NoAds = snapshot.GetValue<bool>("AccountSettings");
                    //if (snapshot.GetValue<AccountSettings>("AccountSettings").LatestLevelUnlocked >= GameSystem.instance.AccountSettings.LatestLevelUnlocked)
                    //{
                    //    GameSystem.instance.AccountSettings = snapshot.GetValue<AccountSettings>("AccountSettings");
                    //    GameSystem.instance.LevelInfos = snapshot.GetValue<List<LevelInfo>>("LevelInfos");
                    //}
                }

                else
                {
                    Debug.Log("Load failed : No previous Data");
                }
            });
     

        }

        else
        {
            Debug.Log("Load failed : Firebase not connected");
        }
    }

    public void GPGSLogin()
    {
        PlayGamesPlatform.Instance.Authenticate((success) =>
        {
            if (success == SignInStatus.Success)
            {
                Debug.Log("Logged in");

                Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.Result == Firebase.DependencyStatus.Available)
                    {
                        //No Dependancy issue with firebase continue with login
                        ConnectToFireBase();
                    }

                    else
                    {
                        //error fixing firebase dependency check plugin
                        Debug.Log("Dependency error");
                    }
                });
            }
        });
    }

    private void ConnectToFireBase()
    {
        PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
        {
            authCode = code;
            Firebase.Auth.FirebaseAuth FBAuth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            Firebase.Auth.Credential FBCred = Firebase.Auth.PlayGamesAuthProvider.GetCredential(authCode);
            
            FBAuth.SignInWithCredentialAsync(FBCred).ContinueWithOnMainThread(task =>
            {
                if(task.IsCanceled)
                {
                    Debug.Log("Sign in Cancelled");
                }
                if(task.IsFaulted)
                {
                    Debug.Log("Error " + task.Result);
                }

                Firebase.Auth.FirebaseUser user = FBAuth.CurrentUser;
                
                if(user != null)
                {
                    userID = user.UserId;
                    Debug.Log("Signed in as " + user.DisplayName);
                    isConnected = true;
                }

                else
                {
                    //error getting user
                }
            });
        });
    }
}
