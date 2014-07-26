using UnityEngine;
using System.Collections;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities;

public class Login : MonoBehaviour {
	private SmartFox smartFox;
	public string serverName = "127.0.0.1";
	public int serverPort = 9339;
	public string zone = "BasicExamples";
	public bool debug = true;
	
	public string userName ="fafa";
	public float dis = 0.0F;
	void Awake() {
		if (SmartFoxConnection.IsInitialized)
		{
			smartFox = SmartFoxConnection.Connection;
		} else {
			smartFox = new SmartFox(debug);
		}
		// Register callback delegate
		smartFox.AddEventListener(SFSEvent.CONNECTION, OnConnection);
		smartFox.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
		smartFox.AddEventListener(SFSEvent.LOGIN, OnLogin);
		smartFox.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
		smartFox.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserLeaveRoom);
		smartFox.AddEventListener(SFSEvent.USER_COUNT_CHANGE, OnUserCountChange);
		smartFox.AddEventListener(SFSEvent.LOGOUT, OnLogOut);
		
		smartFox.Connect(serverName, serverPort);
		
	}
	
	void FixedUpdate() {
		smartFox.ProcessEvents();
	}	
	
	void OnGUI(){
		if(GUI.Button(new Rect(Screen.width/2,Screen.height/2+dis,200,20),userName+"Login Out")){
			smartFox.Send( new LogoutRequest());
		}
	}
	private void UnregisterSFSSceneCallbacks() {
		// This should be called when switching scenes, so callbacks from the backend do not trigger code in this scene
		smartFox.RemoveAllEventListeners();
	}
	
	/************
	 * Callbacks from the SFS API
	 ************/
	
	public void OnConnection(BaseEvent evt) {
		bool success = (bool)evt.Params["success"];
		string error = (string)evt.Params["error"];
		
		if (success) {
			SmartFoxConnection.Connection = smartFox;
			Debug.Log("Connection successfully");
			smartFox.Send(new LoginRequest(userName, "", zone));
		} else {
			Debug.Log("OnConnection error: "+error);
		}
	}
	
	public void OnConnectionLost(BaseEvent evt) {
		Debug.Log("Connection lost / no connection to server");
		UnregisterSFSSceneCallbacks();
	}
	
	public void OnLogin(BaseEvent evt) {
		bool success = true;
		if (evt.Params.ContainsKey("success") && !(bool)evt.Params["success"]) {
			string error = (string)evt.Params["errorMessage"];
			Debug.Log("Login error: "+error);
		} else {
			Debug.Log("Login successfully");
			smartFox.Send(new JoinRoomRequest("CardRoom"));
		}
	}
	
	public void OnUserEnterRoom(BaseEvent evt) {
		User user = (User)evt.Params["user"];
		Debug.Log(user.Name + " joined room");
	}
	
	private void OnUserLeaveRoom(BaseEvent evt) {
		User user = (User)evt.Params["user"];
		Debug.Log(user.Name + " left room");
	}
	
	private void OnUserCountChange(BaseEvent evt) {
		Room room = (Room)evt.Params["room"];
		if(room.UserCount == 1){
			Debug.Log("waiting for anther user to joint game");
		}else if ( room.UserCount == 2 ) {
			Debug.Log("now can begin card game");
		}
	}
	
	public void OnLogOut(BaseEvent evt) {
		bool success = true;
		if (evt.Params.ContainsKey("success") && !(bool)evt.Params["success"]) {
			string error = (string)evt.Params["errorMessage"];
			Debug.Log("Log Out error: "+error);
		} else {
			Debug.Log("Log Out  successfully");
		}
	}
}
