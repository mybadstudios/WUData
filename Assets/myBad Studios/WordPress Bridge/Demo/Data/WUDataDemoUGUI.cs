using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using MBS;

public class WUDataDemoUGUI : MonoBehaviour {
	
	enum EWUDStates {AllData, GameData, GenerateData, NoWULogin, Count}

    #region public variables / UGUI references
    public GameObject
        background_obj,
        no_login_text,
        demo_start_obj,
        data_generation_obj,
        all_data_obj,
        tab0_obj,
        tab1_obj;

    public Text
        global_data_text,
        cml_text,
        col0_text,
        col1_text;
    #endregion

    #region private variables
    CML 
        raw_data,
        per_game_data;

	string
        raw_data_string = "",
        per_game_string = "No game selected...",
        per_game_name = "No data...",
        all_left_col,
        all_right_col;

    string[]
	game_names;

    List<CMLData>
        one_games_categories;

    Dictionary<string, List<CMLData>>
		categories;

	MBSStateMachine<EWUDStates>
		state;	
    #endregion

    void Start()
	{
		//setup the various display states to perform relevant actions...
		state = new MBSStateMachine<EWUDStates>();
		state.AddState(EWUDStates.AllData			, DoStateFetchedEverything );
		state.AddState(EWUDStates.GameData			, DoStateFetchPerGame);
		state.AddState(EWUDStates.GenerateData		, DoStateDataGenerationPrompt);
		state.AddState(EWUDStates.NoWULogin			, DoStateNoLogin);
		state.AddState(EWUDStates.Count);
		state.SetState( EWUDStates.Count ); 	//anything except AllData		

        WULogin.OnLoggedIn += ShowFetchEverything;
	}

	/*
    this is setup in Start.
    After login completes successfully this gets called automatically
    Shows a button to click to either generate or fetch demo data
    */
	void ShowFetchEverything(CML data)
	{
		DisableAll();
		demo_start_obj.SetActive( true );
	}

    void SetState( EWUDStates new_state)
    {
        if ( state.CompareState( new_state ) )
            return;

        DisableAll();
        state.SetState( new_state );
        state.PerformAction();
    }

	//selects what happens when you press one of the two top buttons after fetching the game data
    public void SelectTab( int tab_index )
    {
        EWUDStates wud_state = (EWUDStates)tab_index;
		if (wud_state == EWUDStates.GameData)
	        col1_text.text = string.Empty;

        per_game_string = per_game_name = string.Empty;
        one_games_categories = null;

        if ( !state.CompareState( wud_state ) )
            SetState( wud_state );

        //click the first button
        if (wud_state == EWUDStates.GameData)
            PerGameActions(0);
    }

    //make sure other screens are disabled when showing a specific one
    void DisableAll(bool show_background = true)
    {
        background_obj.SetActive( show_background );
        no_login_text.SetActive( false );
        demo_start_obj.SetActive( false );
        data_generation_obj.SetActive( false );
        all_data_obj.SetActive( false );
    }

    //Shows an error: There's no Login prefab in the scene
	void DoStateNoLogin()
    {
        DisableAll();
		background_obj.SetActive (true);
        no_login_text.SetActive ( true );
    }
	
	void DoStateDataGenerationPrompt()
    {
//		DisableAll ();
        data_generation_obj.SetActive( true );
    }

	void DoStateFetchedEverything()
	{
		if ( null == raw_data)
		{
			SetState( EWUDStates.GenerateData );
			return;
		}
		
		if (null == game_names)
		{
			col1_text.text = "No games...";
			col0_text.text = string.Empty;
		} else
		{
			col1_text.text = all_right_col;
			col0_text.text = all_left_col;
		}
		
		cml_text.text = raw_data_string;
		background_obj.SetActive( true );
		all_data_obj.SetActive( true );
		tab0_obj.gameObject.SetActive( true );
		tab1_obj.gameObject.SetActive( false );
	}
	
	void DoStateFetchPerGame()
	{
		string result = per_game_name == string.Empty ? string.Empty : ( "Game: " + per_game_name + "\n" );
		string eol = per_game_name == string.Empty ? "\n" : "\t";
		
		if ( null != one_games_categories )
		{
			foreach ( CMLData category in one_games_categories )
			{
				if ( category.String( "category" ) != string.Empty )
					result += "<color=red>Category: " + category.String( "category" ) + "</color>\n";
				string [] field_names = category.Keys;
				
				//in case there are only 3 fields, skip it (id, category and name) 
				if ( field_names.Length == 3 )
					continue;
				
				result += "<color=red>Fields (" + field_names.Length + "):</color>\n";
				foreach ( string field in field_names )
				{
					//ignore these two fields for this demo...
					if ( field == "id" || field == "category" )
						continue;
					
					result += "<color=navy>" + field + "</color>" + eol;
					result += category.String( field ) + "\n";
				}
			}
		}
		col0_text.text = result;
		cml_text.text = per_game_string;
		
		background_obj.SetActive( true );
		all_data_obj.SetActive( true );
		tab0_obj.SetActive( false );
		tab1_obj.gameObject.SetActive( true );
	}

    void ParsePerGameData(CML per_game_data)
	{		
		if (null != per_game_data && per_game_data.Count > 1)
		{
			per_game_string = per_game_data.ToString();
			one_games_categories = WUData.ExtractAllCategories(per_game_data);
			
			//This function's default params point to the game's global settings...
			CMLData global_settings = WUData.ExtractCategoryFromGame(per_game_data);
			per_game_name = global_settings.String("name");
            DoStateFetchPerGame();		
		} else
		{
			StatusMessage.Message = "No data was returned from the server";
		}
	}
	
	void ParsePerFieldData(CML data)
	{
		per_game_string = data.ToString();
        string result = string.Empty;
        CMLData category = data [1];
        foreach ( string s in category.Keys )
        {
            if ( s == "id" || s == "category" )
                continue;
            result += "<color=navy>"+s + "</color>\n\t" + category.String( s ) + "\n\n";
        }
        col0_text.text = result;
        cml_text.text = per_game_string;
	}
	
    /// <summary>
    /// This demo requires some demo data in order to work so let's create some.
    /// This function will generate daathat canbe shared across all games on your website
    /// as well as data for 3 separate categories under 2 separate games but all for the
    /// current player. 
    /// 
    /// Notice how all of this is achieved using the exact same function.
    /// The difference between globally shared data and gamedata is specified by setting
    /// the user to 0 or leaving the field blank so the current game's Id can be used.
    /// 
    /// Separating data into different games just requires the use of the game's ID
    /// Categorizing the data in "global data for the game" or categorized data simply
    /// requires that you either omit or specify the name of a category you want to use, respectively
    /// </summary>
    public void GenerateDemoData()
	{
		DisableAll (false);
		string[] cats = new string[]{"Stats", "Settings", "Keys"};
		string[] game_names = new string[]{"Pacman", "Asteroids"};

        /*
        gid 0 is considered GLOBAL while games have id's greater than 0
		so I will start the counter at 1 and store data for two games with IDs 1 and 2
        This data will "belong" to a specific game because we specify a game id when we save it
        I am going to store GLOBAL data and under 3 categories. See the string[] above
        */
        for (int gid = 1; gid <= game_names.Length; gid++)
		{
            //To save settings outside any category just don't specify a category name
            //This can be considered "GLOBAL DATA FOR THE GAME". 
            //In this instance I am storing the game's name "GLOBALLY FOR THE GAME"
            CMLData data = new CMLData();
            data.Set( "name", game_names [gid - 1] );
            WUData.UpdateCategory( "", data, gid );

            ///Store some random data for game "gid" into the 3 categories defined in "cats" above
            for ( int cat_index = 0; cat_index < cats.Length; cat_index++)
			{
				data = new CMLData();
				for(int i = 0; i < 3; i++)
					data.Seti( cats[cat_index] + i, Random.Range(0, 100));

                //Store the 3 values generated above under one of the three categories defined 
                //at the start of this function and set it's owner to be 1 or 2 based on gid
                WUData.UpdateCategory(cats[cat_index], data, gid);
			}
		}
		
		//Generate some GLOBAL settings. These settings are shared between games
		//Avoid using , ' " \ / ! and spaces in field names.
        //Don't start a field name with a number!
		CMLData global = new CMLData();
		global.Set ("Last_purchase", "Asteroids");
		global.Set ("Girlfriends_name", "Sally");
		global.Set ("Occupation", "Unity guru");
		global.Seti("Global_currency", 500);

        /*
        In order to make sure this data is available to all games
        be sure to set the game_id to 0.
        If not 0 then it belongs to a specific game only, like the examples above
        When we have saved this, let's fetch back everything for the demo to use
        Once this function returns from the server it will trigger AfterDataWasCreated
        */
        WUData.UpdateCategory("", global, 0, AfterDataWasCreated );
	}

    void AfterDataWasCreated( CML response ) => FetchEverything();

    /// <summary>
    /// Fetch all data from the database and call ParseRawData when done
    /// </summary>
    public void FetchEverything()
    {
        DisableAll( false );
        WUData.FetchEverything( ParseRawData );
    }

    void ParseRawData( CML data )
    {
        game_names = null;
        categories = null;

        raw_data = data;
        raw_data_string = raw_data.ToString();

        // if response contains ID only, quit since there was no data returned
        if ( raw_data.Count == 0 || raw_data [0].Keys.Length == 1 )
        {
            SetState( EWUDStates.GenerateData );
            return;
        }

        //get the "_GAME_" nodes...
        //if there are none then the data was either empty or returned as categories only. 
        //Either way, it was not meant for this parse function...
		List<CMLData>
			games = WUData.ExtractAllGames( raw_data );
        if ( null == games )
            return;

        //create a string[[] that will hold the game names only.
        //do not process the global settings as a game...
        game_names = new string [games.Count - 1];

        int index = 0;

        categories = new Dictionary<string, List<CMLData>>();
        foreach ( CMLData game in games )
        {
            //do not process the global settings as a game...
            if ( game.String( "gid" ) == "Global" )
            {
                //instead, store it to the global_settings variable
                CMLData global_settings = WUData.ExtractCategoryFromGame( raw_data );

                //I am goind to write the global data to screen directly since it is not going to
                //change and we already have it, so why not?
                global_data_text.text = string.Format( "I have ${0} left after buying {1} for {2} who works as a {3}",
                    global_settings.String( "Global_currency" ),
                    global_settings.String( "Last_purchase" ),
                    global_settings.String( "Girlfriends_name" ),
                    global_settings.String( "Occupation" ) );

                continue;
            }

            //now that we know we are working with a game, use it's
            //ID to search for it's name within it's categories
            string name = WUData.GetField( raw_data, "name", "", game.ID );

            //if no name is found, specify one manually so our
            //button doesn't look empty...
            if ( name == string.Empty )
                name = "Game " + ( index + 1 );

            game_names [index++] = name;

            //now, since we don't know if we changed the name or not, let's not
            //temp fate by using the name of the game. Instead, use the game's ID
            categories.Add( name, WUData.ExtractCategoriesForGame( raw_data, game.ID ) );
        }

        //Instead of formatting the strings every time you click the button in this demo
        //I am going to do it once and store it for later use.
        GenerateOuputOfFirstButton();

        //...like right now, in fact.
        SetState( EWUDStates.AllData );
    }

    void GenerateOuputOfFirstButton()
    {
		int col = 0;
        string result = string.Empty;
        foreach ( string game in game_names )
        {
            result += "Game: " + game + "\n";
			List<CMLData> inspecting = categories [game];
            foreach ( CMLData category in inspecting )
            {
                if ( category.String( "category" ) != string.Empty )
                    result += "<color=red>Category: " + category.String( "category" ) + "</color>\n";

                string [] field_names = category.Keys;

                //in case there are only 2 fields, skip it (id, category and ... )
                if ( field_names.Length == 3 )
                    continue;

                result += "<color=red>Fields (" + field_names.Length + "):</color>\n";
                foreach ( string field in field_names )
                {
                    //ignore these two fields for this demo...
                    if ( field == "id" || field == "category" )
                        continue;

                    result += "<color=navy>" + field + "</color>\t";
                    result += category.String( field ) + "\n";
                }
            }
            if ( col++ == 0 )
                all_right_col = result;
            else
                all_left_col = result;
            result = string.Empty;
        }
    }
	
    /// <summary>
    /// Run one of the various functions created for this demo
    /// </summary>
    /// <param name="index">Determines which function to run. 0,1 and 2 will fetch game data. 3 will fetch a field from GLOBAL data and 4 will fetch a category of data </param>
    public void PerGameActions( int index )
    {
        switch ( index )
        {
            case 0:
            case 1:
            case 2:
                WUData.FetchGameInfo( ParsePerGameData, index );
                break;

            case 3:
                WUData.FetchField( "Global_currency", "", ParsePerFieldData, 0 );
                break;

            case 4:
                WUData.FetchCategory( "Keys", ParsePerFieldData, 1 );
                break;
        }
        /*
        Some examples of other functions you could have called...
        WUData.RemoveField("Occupation", "", ParsePerFieldData,0)
        WUData.RemoveCategory("Stats", ParsePerFieldData,1)
        WUData.RemoveGameData(2, ParsePerFieldData)
        */
    }
}
