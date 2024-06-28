using UnityEngine;
using MBS;

/// <summary>
/// This demo will systematically run through every single function in this kit
/// creating sample data, fetching back that data in the form of a field, category or everything
/// then removing it again and leaving us with no trace in the database that this demo even ran
/// 
/// This demo manipulates your own data, shared data / game settings as well as other people's data
/// </summary>
public class WUDataDemo3 : MonoBehaviour {

    [SerializeField] bool WUData_pro = false;

    CMLData demo_data;

    const int ID_of_someone_else = 999;
    const int FictionalGameID = 999;

    void Start() => WULogin.OnLoggedIn += RunDemo;

    void RunDemo( CML ignore )
    {
        WUData.WUDataPro = WUData_pro;

        //first let's create some data
        demo_data = new CMLData();
        demo_data.Set( "Foo", "bar" );
        demo_data.Set( "Marco", "Polo" );
        demo_data.Set( "Hallo", "world" );
        demo_data.Set( "superhero", "Wonder Woman" );

        //lets start by saving some sample data to two categories under our own account for fictional game id 999
        WUData.UpdateCategory( "", demo_data, FictionalGameID );
        WUData.UpdateCategory( "demo", demo_data, FictionalGameID, FetchAField, PrintError );

        //also, let's save some global data
        demo_data.Clear();
        demo_data.Seti( "Shoe_Size", 8 );
        WUData.UpdateCategory( "personal", demo_data, 0 );
    }

    void FetchAField( CML response ) => WUData.FetchField( "superhero", "demo", FetchACategory, FictionalGameID, PrintError );
    void FetchACategory( CML response )
    {
        PrintResponse( response );
        WUData.FetchCategory( "demo", FetchCategoryLike, FictionalGameID, PrintError );
    }

    void FetchCategoryLike( CML response )
    {
        PrintResponse( response );
        WUData.FetchCategoryLike( "em", FetchAllCategories, FictionalGameID, PrintError );
    }

    void FetchAllCategories( CML response )
    {
        PrintResponse( response );
        WUData.FetchGameInfo( FetchUncatgorised, FictionalGameID, PrintError );
    }

    void FetchUncatgorised( CML response )
    {
        PrintResponse( response );
        WUData.FetchGlobalInfo( RemoveAField, PrintError);
    }

    void RemoveAField( CML response )
    {
        PrintResponse( response );
        WUData.RemoveField( "Marco", "demo", RemoveACategory, FictionalGameID, PrintError );
    }

    void RemoveACategory( CML response )
    {
        print( "Removed Marco" );
        WUData.RemoveCategory( "", FetchEVERYTHING, FictionalGameID, PrintError );
    }

    void FetchEVERYTHING( CML response )
    {
        print( "Removed Category" );
        WUData.FetchEverything( DeleteEverything, PrintError );
    }

    void DeleteEverything( CML response )
    {
        PrintResponse( response );
        //let's remove that global data first...
        WUData.RemoveCategory( "personal", gid: 0 );
        //now remove everything stored about the game
        WUData.RemoveGameData( FictionalGameID, RunSharedDemo, PrintError ); //TODO validate this line!!!
    }

    //------------------------------------------------------------

    void RunSharedDemo( CML ignore )
    {
        //first let's create some data
        demo_data = new CMLData();
        demo_data.Set( "FooShared", "bar" );
        demo_data.Set( "MarcoShared", "Polo" );
        demo_data.Set( "HalloShared", "world" );
        demo_data.Set( "superheroShared", "Wonder Woman" );

        //lets start by saving some sample data to two categories under our own account for fictional game id 999
        //also, let's create some global settings for this user
        WUData.UpdateSharedCategory( "", demo_data, FictionalGameID );
        WUData.UpdateSharedCategory( "demo", demo_data, FictionalGameID, FetchASharedField, PrintError );

        print("SHARED DATA");
    }

    void FetchASharedField( CML response ) => WUData.FetchSharedField( "superheroShared", "demo", FetchASharedCategory, FictionalGameID, PrintError );
    void FetchASharedCategory( CML response )
    {
        PrintResponse( response );
        WUData.FetchSharedCategory( "demo", FetchSharedCategoryLike, FictionalGameID, PrintError );
    }

    void FetchSharedCategoryLike( CML response )
    {
        PrintResponse( response );
        WUData.FetchSharedCategoryLike( "em", FetchAllSharedCategories, FictionalGameID, PrintError );
    }

    void FetchAllSharedCategories( CML response )
    {
        PrintResponse( response );
        WUData.FetchAllSharedInfo( RemoveASharedField, FictionalGameID, PrintError );
    }

    void RemoveASharedField( CML response )
    {
        PrintResponse( response );
        WUData.RemoveSharedField( "MarcoShared", "demo", RemoveASharedCategory, FictionalGameID, PrintError );
    }

    void RemoveASharedCategory( CML response )
    {
        print( "Removed a field" );
        WUData.RemoveSharedCategory( "demo", gid:FictionalGameID );
        WUData.RemoveSharedCategory( "", RunUserDemo, FictionalGameID, PrintError );
    }

    //-----------------------------------------------------------

    void RunUserDemo( CML ignore )
    {
        //first let's create some data
        demo_data = new CMLData();
        demo_data.Set( "Foo", "bar" );
        demo_data.Set( "Marco", "Polo" );
        demo_data.Set( "Hallo", "world" );
        demo_data.Set( "superhero", "Wonder Woman" );

        //lets start by saving some sample data to two categories under our own account for fictional game id 999
        WUData.UpdateUserCategory( ID_of_someone_else, "", demo_data, FictionalGameID );
        WUData.UpdateUserCategory( ID_of_someone_else, "demo", demo_data, FictionalGameID, FetchAUserField, PrintError );

        demo_data.Clear();
        demo_data.Set( "My name", "John Doe" );
        demo_data.Set( "My valentine", "Rosie McDonall" );
        WUData.UpdateUserCategory(ID_of_someone_else, "Personal", demo_data, 0 );
        print( "USER DATA" );
    }

    void FetchAUserField( CML response ) => WUData.FetchUserField( ID_of_someone_else, "superhero", "demo", FetchAUserCategory, FictionalGameID, PrintError );
    void FetchAUserCategory( CML response )
    {
        PrintResponse( response );
        WUData.FetchUserCategory( ID_of_someone_else, "demo", FetchUserCategoryLike, FictionalGameID, PrintError );
    }

    void FetchUserCategoryLike( CML response )
    {
        PrintResponse( response );
        WUData.FetchUserCategoryLike( ID_of_someone_else, "em", FetchAllUserCategories, FictionalGameID, PrintError );
    }

    void FetchAllUserCategories( CML response )
    {
        PrintResponse( response );
        WUData.FetchUserGameInfo( ID_of_someone_else, FetchUserUncatgorised, FictionalGameID, PrintError );
    }

    void FetchUserUncatgorised( CML response )
    {
        PrintResponse( response );
        WUData.FetchUserGlobalInfo( ID_of_someone_else, RemoveAUserField, PrintError );
    }

    void RemoveAUserField( CML response )
    {
        PrintResponse( response );
        WUData.RemoveUserField( ID_of_someone_else, "Marco", "demo", RemoveUserCategories, FictionalGameID, PrintError );
    }

    void RemoveUserCategories( CML response )
    {
        print( "Removed a field" );
        WUData.RemoveUserCategory( ID_of_someone_else, "Personal", gid: 0 );
        WUData.RemoveUserCategory( ID_of_someone_else, "demo", gid: FictionalGameID );
        WUData.RemoveUserCategory( ID_of_someone_else, "", WeAreDone, FictionalGameID, PrintError );
    }

    void WeAreDone(CML response) => print( "All Done! If we got this far then everything worked as expected! :)" );
    void PrintResponse( CML response ) => print( response.ToString() );
    void PrintError( CMLData response ) => Debug.LogWarning( "Error: " + response.ToString() );	
}
