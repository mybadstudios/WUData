using UnityEngine;
using MBS;

/// <summary>
/// This quick demo shows how to upload entire categories of data, complete with data in it as well as how
/// to fetch back either a single field or an entire category of fields all at once. Finally, it shows how
/// to delete individual fields from a category and the entire category as a whole.
/// 
/// This demo adds data to your database, fetches it back again, displays it and then removes it from the 
/// database leaving no trace of the data after thedemo has run. It will be as if nothing happened at all
/// </summary>
public class WUDataDemo2 : MonoBehaviour {

	public Rect area;
	public GUISkin the_skin;

	void Start () {
		area.x = (Screen.width - area.width) / 2;
		area.y = (Screen.height - area.height) / 2;
	}
	
	void OnGUI()
	{
		if (!(WULogin.IsLoggedIn)) return;

		GUI.skin = the_skin;
		GUI.Window(0, area, DrawWindow, "");
	}

	void PrintResponse(CML response) => print(response.ToString());

	void DrawWindow(int id)
	{
		if (GUILayout.Button("Add some shared data"))
		{
			CMLData data = new CMLData();
			data.Set("Field_1", "Value 1");
			data.Set("Field_2", "Value 2");
			WUData.UpdateSharedCategory("Category1", data, response: PrintResponse);
			data.Set("Field_3", "Value 3");
			data.Set("Field_4", "Value 4");
			WUData.UpdateSharedCategory("Category2", data, response: PrintResponse);
		}
		
		if (GUILayout.Button("Fetch a single shared field"))
			WUData.FetchSharedField("Field_1", "Category1", PrintResponse, WPServer.GameID);
		
		if (GUILayout.Button("Fetch a shared category"))
			WUData.FetchSharedCategory("Category1", PrintResponse, WPServer.GameID);
		
		if (GUILayout.Button("Fetch all shared data"))
			WUData.FetchAllSharedInfo(PrintResponse, WPServer.GameID);
		
		if (GUILayout.Button("Remove single shared field"))
			WUData.RemoveSharedField("Field_1", "Category1", PrintResponse, WPServer.GameID);
		
		if (GUILayout.Button("Remove shared category"))
			WUData.RemoveSharedCategory("Category1", PrintResponse, WPServer.GameID);
	}
	
}
