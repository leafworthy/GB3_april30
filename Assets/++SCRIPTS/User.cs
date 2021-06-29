using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class User
{
	public string userName = "unnamed";
	private List<Player> userPlayers;

	public User(string userName, List<Player> userPlayers)
	{
		this.userName = userName;
		this.userPlayers = userPlayers;
	}

	public List<Player> getPlayers()
	{
		return userPlayers;
	}
}
