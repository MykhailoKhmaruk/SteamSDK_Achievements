using UnityEngine;
using System.Collections;
using System.ComponentModel;
using Steamworks;

// This is a port of StatsAndAchievements.cpp from SpaceWar, the official Steamworks Example.
class SteamStatsAndAchievements : MonoBehaviour {

    Vector2 scrollPosition;
    public enum Achievement : int {
        None = 0,
        avenge_friend,
        base_scamper,
        bloodless_victory,
        bomb_defuse_close_call,
        bomb_defuse_low,
        bomb_defuse_needed_kit,
        bomb_multikill,
        bomb_plant_in_25_seconds,
        bomb_plant_low,
        born_ready,
        break_props,
        break_windows,
        cause_friendly_fire_with_flashbang,
        concurrent_dominations,
        damage_no_kill,
        dead_grenade_kill,
        defuse_defense,
        domination_overkills_high,
        domination_overkills_low,
        domination_overkills_match,
        dominations_high,
        dominations_low,
        donate_weapons,
        earn_money_high,
        earn_money_low,
        earn_money_med,
        extended_domination,
        famas_expert02,
        fast_hostage_rescue,
        fast_round_win,
        flawless_victory,
        give_damage_high,
        give_damage_low,
        give_damage_med,
        goose_chase,
        grenade_multikill,
        gun_game_conservationist,
        gun_game_first_kill,
        gun_game_first_thing_first,
        gun_game_kill_knifer,
        gun_game_knife_kill_knifer,
        gun_game_rampage,
        gun_game_rounds_high,
        gun_game_rounds_low,
        gun_game_rounds_med,
        gun_game_select_suicide_with_knife,
        gun_game_smg_kill_knifer,
        gun_game_target_secured,
        hat_trick_gray,
        headshots,
        headshots_in_round,
        hip_shot,
        immovable_object,
        kill_bomb_defuser,
        kill_bomb_pickup,
        kill_enemies_while_blind,
        kill_enemies_while_blind_hard,
        kill_enemy_ak47,
        kill_enemy_aug,
        kill_enemy_awp,
        kill_enemy_bizon,
        kill_enemy_blinded,
        kill_enemy_deagle,
        kill_enemy_elite,
        kill_enemy_famas,
        kill_enemy_fiveseven,
        kill_enemy_g3sg1,
        kill_enemy_galilar,
        kill_enemy_glock,
        kill_enemy_hegrenade,
        kill_enemy_high,
        kill_enemy_hkp2000,
        kill_enemy_in_air,
        kill_enemy_knife,
        kill_enemy_last_bullet,
        kill_enemy_low,
        kill_enemy_m4a1,
        kill_enemy_m249,
        kill_enemy_mac10,
        kill_enemy_mag7,
        kill_enemy_med,
        kill_enemy_molotov,
        kill_enemy_mp7,
        kill_enemy_mp9,
        kill_enemy_negev,
        kill_enemy_nova,
        kill_enemy_p90,
        kill_enemy_p250,
        kill_enemy_reloading,
        kill_enemy_sawedoff,
        kill_enemy_scar20,
        kill_enemy_sg556,
        kill_enemy_ssg08,
        kill_enemy_taser,
        kill_enemy_team,
        kill_enemy_tec9,
        kill_enemy_ump45,
        kill_enemy_xm1014,
        kill_hostage_rescuer,
        kill_low_damage,
        kill_sniper_with_knife,
        kill_sniper_with_sniper,
        kill_snipers,
        kill_two_with_one_shot,
        kill_when_at_low_health,
        kill_while_in_air,
        kill_with_every_weapon,
        kill_with_own_gun,
        killed_defuser_with_grenade,
        killer_and_enemy_in_air,
        killing_spree,
        killing_spree_ender,
        kills_enemy_weapon,
        kills_with_multiple_guns,
        last_player_alive,
        lossless_extermination,
        marcsman,
        medalist,
        meta_pistol,
        meta_rifle,
        meta_shotgun,
        meta_smg,
        meta_weaponmaster,
        one_shot_one_kill,
        pistol_round_knife_kill,
        play_every_gungame_map,
        rescue_all_hostages,
        rescue_hostages_low,
        rescue_hostages_med,
        revenges_high,
        revenges_low,
        run_of_the_mill,
        silent_win,
        sm1014_expert,
        still_alive,
        survive_grenade,
        survive_many_attacks,
        tourist,
        tr_bomb_defuse_low,
        tr_bomb_plant_low,
        unstoppable_force,
        win_bomb_defuse,
        win_bomb_plant,
        win_bomb_plant_after_recovery,
        win_dual_duel,
        win_every_gungame_map,
        win_gun_game_rounds_extreme,
        win_gun_game_rounds_high,
        win_gun_game_rounds_low,
        win_gun_game_rounds_med,
        win_gun_game_rounds_ultimate,
        win_knife_fights_high,
        win_knife_fights_low,
        win_map_ar_baggage,
        win_map_ar_shoots,
        win_map_cs_italy,
        win_map_cs_office,
        win_map_de_aztec,
        win_map_de_bank,
        win_map_de_boathouse,
        win_map_de_dust,
        win_map_de_dust2,
        win_map_de_house,
        win_map_de_inferno,
        win_map_de_lake,
        win_map_de_nuke,
        win_map_de_safehouse,
        win_map_de_shacks,
        win_map_de_shorttrain,
        win_map_de_stmarc,
        win_map_de_sugarcane,
        win_map_de_train,
        win_map_gg_baggage,
        win_map_gg_vietnam,
        win_pistolrounds_high,
        win_pistolrounds_low,
        win_pistolrounds_med,
        win_rounds_high,
        win_rounds_low,
        win_rounds_med,
        win_rounds_without_buying

    };


  
    // Our GameID
    private CGameID m_GameID;

	// Did we get the stats from Steam?
	private bool m_bRequestedStats;
	private bool m_bStatsValid;

	// Should we store stats this frame?
	private bool m_bStoreStats;
    private int[] statsArray = new int[19];
    

	protected Callback<UserStatsReceived_t> m_UserStatsReceived;
	protected Callback<UserStatsStored_t> m_UserStatsStored;
	protected Callback<UserAchievementStored_t> m_UserAchievementStored;

	void OnEnable() {
		if (!SteamManager.Initialized)
			return;

		// Cache the GameID for use in the Callbacks
		m_GameID = new CGameID(SteamUtils.GetAppID());

		m_UserStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
		m_UserStatsStored = Callback<UserStatsStored_t>.Create(OnUserStatsStored);
		m_UserAchievementStored = Callback<UserAchievementStored_t>.Create(OnAchievementStored);

		// These need to be reset to get the stats upon an Assembly reload in the Editor.
		m_bRequestedStats = false;
		m_bStatsValid = false;
	}

	private void Update() {
		if (!SteamManager.Initialized)
			return;

		if (!m_bRequestedStats) {
			// Is Steam Loaded? if no, can't get stats, done
			if (!SteamManager.Initialized) {
				m_bRequestedStats = true;
				return;
			}
			
			// If yes, request our stats
			bool bSuccess = SteamUserStats.RequestCurrentStats();

			// This function should only return false if we weren't logged in, and we already checked that.
			// But handle it being false again anyway, just ask again later.
			m_bRequestedStats = bSuccess;
		}

		if (!m_bStatsValid)
			return;

		// Get info from sources
        //Store stats in the Steam database if necessary
		if (m_bStoreStats) {

            bool bSuccess = SteamUserStats.StoreStats();
            // If this failed, we never sent anything to the server, try
            // again later.
            m_bStoreStats = !bSuccess;
        }
	}

	private void UnlockAchievement(Achievement_t achievement) {
		achievement.m_bAchieved = true;
		SteamUserStats.SetAchievement(achievement.m_eAchievementID.ToString());
		m_bStoreStats = true;
	}

 
    private void OnUserStatsReceived(UserStatsReceived_t pCallback)
    {
        if (!SteamManager.Initialized)
            return;

        // we may get callbacks for other games' stats arriving, ignore them
        if ((ulong)m_GameID == pCallback.m_nGameID)
        {
            if (EResult.k_EResultOK == pCallback.m_eResult)
            {
#if UNITY_EDITOR
                Debug.Log("Received stats and achievements from Steam\n");
#endif

                m_bStatsValid = true;


            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("RequestStats - failed, " + pCallback.m_eResult);
#endif
            }
        }
    }

    private void GetStatsFromSteam()
    {
        if (!SteamManager.Initialized)
            return;
        
    }

    private void SendStartsToSteam()
    {
        if (!SteamManager.Initialized)
            return;
       

        bool bSuccess = SteamUserStats.StoreStats();
    }
    //-----------------------------------------------------------------------------
    // Purpose: Our stats data was stored!
    //-----------------------------------------------------------------------------
    private void OnUserStatsStored(UserStatsStored_t pCallback)
    {
        // we may get callbacks for other games' stats arriving, ignore them
        if ((ulong)m_GameID == pCallback.m_nGameID)
        {
            if (EResult.k_EResultOK == pCallback.m_eResult)
            {
#if UNITY_EDITOR
                Debug.Log("StoreStats - success");
#endif
            }
            else if (EResult.k_EResultInvalidParam == pCallback.m_eResult)
            {
#if UNITY_EDITOR
                // One or more stats we set broke a constraint. They've been reverted,
                // and we should re-iterate the values now to keep in sync.
                Debug.Log("StoreStats - some failed to validate");
#endif
                // Fake up a callback here so that we re-load the values.
                UserStatsReceived_t callback = new UserStatsReceived_t();
                callback.m_eResult = EResult.k_EResultOK;
                callback.m_nGameID = (ulong)m_GameID;
                OnUserStatsReceived(callback);
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("StoreStats - failed, " + pCallback.m_eResult);
#endif
            }
        }
    }
    //-----------------------------------------------------------------------------
    // Purpose: An achievement was stored
    //-----------------------------------------------------------------------------
    private void OnAchievementStored(UserAchievementStored_t pCallback)
    {
        // We may get callbacks for other games' stats arriving, ignore them
        if ((ulong)m_GameID == pCallback.m_nGameID)
        {
            if (0 == pCallback.m_nMaxProgress)
            {
#if UNITY_EDITOR
                Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' unlocked!");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' progress callback, (" + pCallback.m_nCurProgress + "," + pCallback.m_nMaxProgress + ")");
#endif
            }
        }
    }

    public void ResetAllStats()
    {
        SteamUserStats.ResetAllStats(true);
        SteamUserStats.RequestCurrentStats();
        if (!m_bStatsValid)
            return;
   
    }
    public void Render() {
		if (!SteamManager.Initialized) {
			GUILayout.Label("Steamworks not Initialized");
			return;
		}
      

        GUILayout.BeginArea(new Rect(Screen.width-900, 20, 800, 800));
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(800), GUILayout.Height(500));
        foreach (Achievement ach in System.Enum.GetValues(typeof(Achievement)))
        {
            //GUILayout.Label(ach.m_eAchievementID.ToString());
            if (GUILayout.Button("Unlock " + ach.ToString()))
            {
                SteamUserStats.SetAchievement(ach.ToString());
            }
            //GUILayout.Label(ach.ToString());
            GUILayout.Space(10);
        }
        // End the scrollview we began above.
        GUILayout.EndScrollView();


        // FOR TESTING PURPOSES ONLY!
        if (GUILayout.Button("RESET STATS AND ACHIEVEMENTS")) {
            ResetAllStats();
        }
		GUILayout.EndArea();
	}

   

    private class Achievement_t {
		public Achievement m_eAchievementID;
		public string m_strName;
		public string m_strDescription;
		public bool m_bAchieved;
		public Achievement_t(Achievement achievementID, string name, string desc) {
			m_eAchievementID = achievementID;
			m_strName = name;
			m_strDescription = desc;
			m_bAchieved = false;
		}
	}
}
