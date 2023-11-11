using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Firebase.Firestore;
using Firebase.Extensions;

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

    public bool IsConnected
    {
        get { return isConnected; }
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
               // {"AccountSettings: LatestLevelUnlocked", GameSystem.instance.AccountSettings.LatestLevelUnlocked },
                {"AccountSettings: NoAds", GameSystem.instance.AccountSettings.NoAds }
            };

            foreach(Skin skin in GameSystem.instance.AccountSettings.Skins)
            {
                data.Add("Skin: " + skin.projectileVfx.ToString() + " isLocked", skin.isLocked);
            }

            foreach(LevelInfo level in GameSystem.instance.LevelInfos)
            {
                // Format "Level: + levelID.toString() + Locked"
                data.Add("Level: " + level.levelID.ToString() + " Locked", level.locked);
                data.Add("Level: " + level.levelID.ToString() + " Stars", level.stars);
            }

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
                   // GameSystem.instance.AccountSettings.LatestLevelUnlocked = snapshot.GetValue<int>("AccountSettings: LatestLevelUnlocked");
                    GameSystem.instance.AccountSettings.NoAds = snapshot.GetValue<bool>("AccountSettings: NoAds");

                    foreach (Skin skin in GameSystem.instance.AccountSettings.Skins)
                    {
                        skin.isLocked = snapshot.GetValue<bool>("Skin: " + skin.projectileVfx.ToString() + " isLocked"); 
                    }

                    foreach (LevelInfo level in GameSystem.instance.LevelInfos)
                    {
                        level.locked = snapshot.GetValue<bool>("Level: " + level.levelID.ToString() + " Locked");
                        level.stars = snapshot.GetValue<int>("Level: " + level.levelID.ToString() + " Stars");
                    }

                    //if (GameSystem.instance.AccountSettings.LatestLevelUnlocked > 1)
                    //{
                    //    for (int i = 0; i < GameSystem.instance.AccountSettings.LatestLevelUnlocked; i++)
                    //    {
                    //        //Skin unlocks every 3 lvls
                    //        if (i % 3 == 0)
                    //        {

                    //        }
                    //    }
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
