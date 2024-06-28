using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MBS;

/// <summary>
/// Drop the login prefab in the scene and hit play to run this automated demo
/// 
/// In this demo we have 2 Image components. One has a graphic on it and the other does not.
/// We take the graphic from the one object and store it online then use the normal WUData FetchCategory
/// method to fetch back the category we stored it under. From here we get the absolute URL of where the
/// image was saved to and we fetch it back from that URL, placing it on the second Image object once fetched.
/// 
/// Also demonstrate storing images globally and under other users usernames... for whatever reason you might need it
/// 
/// Things worth noting:
/// When using images that are in the project at design time their READABLE property must be set to true
/// When uploding graphics, duplicates get overwritten. 
/// The images are placed in the default WordPress Uploads folder under a folder named after the graphic's owner
/// </summary>
public class WUDataImagesDemo : MonoBehaviour {

    /// <summary>
    /// This is the object with the sourceimage on it
    /// </summary>
    public Image origin;
    /// <summary>
    /// This object will display the image we loaded from online
    /// </summary>
    public Image result;

    void Start () => WULogin.OnLoggedIn += RunDemo;
    void OnSuccess( CML response ) => WUData.FetchCategory("images", FetchItBack);
    void FetchItBack( CML response ) => StartCoroutine( FetchSplashScreen(response[1]) );

    void RunDemo( CML response )
    {
        int
            width = origin.sprite.texture.width,
            height = origin.sprite.texture.height;
        WUData.StoreSharedImage( origin.sprite.texture, "splash_screen", "images", WUData.EWUDImageTypes.jpg);
        WUData.StoreUserImage( 2, origin.sprite.texture, "splash_screen", "images", WUData.EWUDImageTypes.png );
        WUData.StoreImage( origin.sprite.texture, "splash_screen", "images", WUData.EWUDImageTypes.png, response: OnSuccess );
        origin.gameObject.SetActive( false );
    }

    IEnumerator FetchSplashScreen(CMLData data)
    {
        Debug.LogWarning( data.ToString() );
        var w = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(data.String("splash_screen"));
        yield return w.SendWebRequest();
        Texture2D texture;
        if (!string.IsNullOrEmpty(w.error))
        {
            texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();

        }
        else
        {
            texture = ((UnityEngine.Networking.DownloadHandlerTexture)w.downloadHandler).texture;
        }
        Sprite s = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        result.sprite = s;
        result.gameObject.SetActive(true);
    }

}
