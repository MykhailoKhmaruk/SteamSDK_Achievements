using UnityEngine;
using System.Collections;
using Steamworks;

// Enum for possible game states on the client
enum EClientGameState {
	k_EClientGameActive,
	k_EClientGameWinner,
	k_EClientGameLoser,
};

class CSGO : MonoBehaviour {
	SteamStatsAndAchievements m_StatsAndAchievements;

	private void OnEnable() {
		m_StatsAndAchievements = GameObject.FindObjectOfType<SteamStatsAndAchievements>();

	}

	private void OnGUI() {
		m_StatsAndAchievements.Render();
		    }
}
