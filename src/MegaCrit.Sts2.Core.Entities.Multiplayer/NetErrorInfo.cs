using System;
using System.Collections.Generic;
using System.Text;
using Godot;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Platform.Steam;
using Steamworks;

namespace MegaCrit.Sts2.Core.Entities.Multiplayer;

/// <summary>
/// Contains information about why a network operation failed, or why we disconnected from a multiplayer session.
/// Prefer passing this instead of NetError, as this contains more information about the underlying error that might
/// have been mapped to our NetError.
/// </summary>
public struct NetErrorInfo
{
	/// <summary>
	/// Used for errors that occur above the platform level, in our transport layers.
	/// </summary>
	private readonly NetError? _reason;

	/// <summary>
	/// Used when the disconnection was raised from the JoinFlow.
	/// </summary>
	private readonly ConnectionFailureReason? _connectionReason;

	/// <summary>
	/// Used when the disconnection was raised from the Steam transport.
	/// </summary>
	private readonly SteamDisconnectionReason? _steamReason;

	/// <summary>
	/// Used for Steam hosting when the lobby creation fails.
	/// </summary>
	private readonly EResult? _lobbyCreationResult;

	/// <summary>
	/// Use for Steam joining when entering the lobby fails. Even though it refers to "chat room", it really means "lobby".
	/// </summary>
	private readonly EChatRoomEnterResponse? _lobbyEnterResponse;

	/// <summary>
	/// String that accompanies the disconnection. Currently only used on the Steam platform.
	/// </summary>
	private readonly string? _debugReason;

	/// <summary>
	/// Used specifically for ENet hosting when lobby creation fails.
	/// If Godot errors occur elsewhere, expand the mapping for this error.
	/// </summary>
	private readonly Error? _godotError;

	/// <summary>
	/// Set to true if the disconnection was initiated by the local peer. For example, if the reason is Quit and this is
	/// true, then the disconnection was caused by a quit initiated locally. If the reason is Quit and this is false, then
	/// the disconnection was caused by a quit initiated remotely.
	/// </summary>
	public bool SelfInitiated { get; }

	/// <summary>
	/// Extra info about the disconnection.
	/// You usually do not need to access this directly; use GetErrorString and show that to the user.
	/// </summary>
	public ConnectionFailureExtraInfo? ConnectionExtraInfo { get; }

	/// <summary>
	/// Set to true if ANYONE in the session has mods.
	/// Don't trust this value fully, it probably doesn't get set for every error.
	/// </summary>
	public bool IsModded { get; private set; }

	public NetErrorInfo(NetError reason, bool selfInitiated)
	{
		_connectionReason = null;
		_steamReason = null;
		_lobbyCreationResult = null;
		_lobbyEnterResponse = null;
		_debugReason = null;
		_godotError = null;
		ConnectionExtraInfo = null;
		IsModded = false;
		_reason = reason;
		SelfInitiated = selfInitiated;
	}

	public NetErrorInfo(ConnectionFailureReason reason, ConnectionFailureExtraInfo? extraInfo = null)
	{
		_reason = null;
		_steamReason = null;
		_lobbyCreationResult = null;
		_lobbyEnterResponse = null;
		_debugReason = null;
		_godotError = null;
		IsModded = false;
		_connectionReason = reason;
		ConnectionExtraInfo = extraInfo;
		SelfInitiated = false;
	}

	public NetErrorInfo(SteamDisconnectionReason steamReason, string? debugReason, bool selfInitiated)
	{
		_reason = null;
		_connectionReason = null;
		_lobbyCreationResult = null;
		_lobbyEnterResponse = null;
		_godotError = null;
		ConnectionExtraInfo = null;
		IsModded = false;
		_steamReason = steamReason;
		_debugReason = debugReason;
		SelfInitiated = selfInitiated;
	}

	public NetErrorInfo(EChatRoomEnterResponse lobbyEnterResponse)
	{
		_reason = null;
		_connectionReason = null;
		_steamReason = null;
		_lobbyCreationResult = null;
		_debugReason = null;
		_godotError = null;
		ConnectionExtraInfo = null;
		IsModded = false;
		_lobbyEnterResponse = lobbyEnterResponse;
		SelfInitiated = true;
	}

	public NetErrorInfo(EResult lobbyCreationResult)
	{
		_reason = null;
		_connectionReason = null;
		_steamReason = null;
		_lobbyEnterResponse = null;
		_debugReason = null;
		_godotError = null;
		ConnectionExtraInfo = null;
		IsModded = false;
		_lobbyCreationResult = lobbyCreationResult;
		SelfInitiated = true;
	}

	public NetErrorInfo(Error error)
	{
		_reason = null;
		_connectionReason = null;
		_steamReason = null;
		_lobbyCreationResult = null;
		_lobbyEnterResponse = null;
		_debugReason = null;
		ConnectionExtraInfo = null;
		IsModded = false;
		_godotError = error;
		SelfInitiated = true;
	}

	public void SetModdedFlagIfModded(RunLobby? runLobby)
	{
		IsModded = runLobby?.AnyPlayerHadMods ?? false;
	}

	/// <summary>
	/// Returns a disconnection reason mapped from the underlying transport disconnection reason.
	/// </summary>
	public NetError GetReason()
	{
		if (_reason.HasValue)
		{
			return _reason.Value;
		}
		if (_connectionReason.HasValue)
		{
			ConnectionFailureReason value = _connectionReason.Value;
			return value switch
			{
				ConnectionFailureReason.None => NetError.None, 
				ConnectionFailureReason.LobbyFull => NetError.LobbyFull, 
				ConnectionFailureReason.RunInProgress => NetError.RunInProgress, 
				ConnectionFailureReason.NotInSaveGame => NetError.NotInSaveGame, 
				ConnectionFailureReason.VersionMismatch => NetError.VersionMismatch, 
				ConnectionFailureReason.ModMismatch => NetError.ModMismatch, 
				ConnectionFailureReason.HandshakeTimeout => NetError.HandshakeTimeout, 
			};
		}
		if (_steamReason.HasValue)
		{
			return _steamReason.Value.ToApp();
		}
		if (_lobbyCreationResult.HasValue)
		{
			return NetError.FailedToHost;
		}
		if (_lobbyEnterResponse.HasValue)
		{
			EChatRoomEnterResponse value2 = _lobbyEnterResponse.Value;
			return value2 switch
			{
				EChatRoomEnterResponse.k_EChatRoomEnterResponseDoesntExist => NetError.InvalidJoin, 
				EChatRoomEnterResponse.k_EChatRoomEnterResponseNotAllowed => NetError.InternalError, 
				EChatRoomEnterResponse.k_EChatRoomEnterResponseFull => NetError.LobbyFull, 
				EChatRoomEnterResponse.k_EChatRoomEnterResponseError => NetError.UnknownNetworkError, 
				EChatRoomEnterResponse.k_EChatRoomEnterResponseBanned => NetError.JoinBlockedByUser, 
				EChatRoomEnterResponse.k_EChatRoomEnterResponseLimited => NetError.UnknownNetworkError, 
				EChatRoomEnterResponse.k_EChatRoomEnterResponseClanDisabled => NetError.JoinBlockedByUser, 
				EChatRoomEnterResponse.k_EChatRoomEnterResponseCommunityBan => NetError.JoinBlockedByUser, 
				EChatRoomEnterResponse.k_EChatRoomEnterResponseMemberBlockedYou => NetError.JoinBlockedByUser, 
				EChatRoomEnterResponse.k_EChatRoomEnterResponseYouBlockedMember => NetError.JoinBlockedByUser, 
				EChatRoomEnterResponse.k_EChatRoomEnterResponseRatelimitExceeded => NetError.RateLimited, 
				EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess => NetError.None, 
			};
		}
		if (_godotError.HasValue)
		{
			return NetError.FailedToHost;
		}
		throw new InvalidOperationException("Tried to get DisconnectionReason from DisconnectionInfo without any assigned errors");
	}

	/// <summary>
	/// Returns a string with more human-readable details about the underlying cause of the disconnection.
	/// This is suitable for displaying to users as long as platform requirements allow plain error codes in messages.
	/// </summary>
	public string GetErrorString()
	{
		bool flag = ConnectionExtraInfo?.localIsHost ?? false;
		if (_reason.HasValue)
		{
			return _reason.Value.ToString();
		}
		if (_connectionReason.HasValue)
		{
			if (_connectionReason == ConnectionFailureReason.ModMismatch)
			{
				StringBuilder stringBuilder = new StringBuilder();
				List<string> list = ConnectionExtraInfo?.GetMissingModsOnRemote(nonGameplay: false);
				List<string> list2 = ConnectionExtraInfo?.GetMissingModsOnLocal(nonGameplay: false);
				if (list != null && list.Count > 0)
				{
					string locEntryKey = "NETWORK_ERROR." + (flag ? "HOST." : "") + "MOD_MISMATCH.description.missingOnHost";
					LocString locString = new LocString("main_menu_ui", locEntryKey);
					locString.Add("mods", string.Join(", ", list));
					stringBuilder.AppendLine(locString.GetFormattedText());
				}
				if (list2 != null && list2.Count > 0)
				{
					string locEntryKey2 = "NETWORK_ERROR." + (flag ? "HOST." : "") + "MOD_MISMATCH.description.missingOnLocal";
					LocString locString2 = new LocString("main_menu_ui", locEntryKey2);
					locString2.Add("mods", string.Join(", ", list2));
					stringBuilder.AppendLine(locString2.GetFormattedText());
				}
				return stringBuilder.ToString();
			}
			if (_connectionReason == ConnectionFailureReason.VersionMismatch)
			{
				if (ConnectionExtraInfo?.remoteInfo.version != ConnectionExtraInfo?.localInfo.version)
				{
					if (ConnectionExtraInfo?.remoteInfo.branch != ConnectionExtraInfo?.localInfo.branch)
					{
						string locEntryKey3 = "NETWORK_ERROR." + (flag ? "HOST." : "") + "VERSION_MISMATCH.description.branchMismatch";
						LocString locString3 = new LocString("main_menu_ui", locEntryKey3);
						locString3.Add("remoteBranch", ConnectionExtraInfo?.remoteInfo.branch.ToName() ?? "<null>");
						locString3.Add("localBranch", ConnectionExtraInfo?.localInfo.branch.ToName() ?? "<null>");
						return locString3.GetFormattedText();
					}
					string locEntryKey4 = "NETWORK_ERROR." + (flag ? "HOST." : "") + "VERSION_MISMATCH.description.versionMismatch";
					LocString locString4 = new LocString("main_menu_ui", locEntryKey4);
					locString4.Add("remoteVersion", ConnectionExtraInfo?.remoteInfo.version ?? "<null>");
					locString4.Add("localVersion", ConnectionExtraInfo?.localInfo.version ?? "<null>");
					return locString4.GetFormattedText();
				}
				if (ConnectionExtraInfo?.remoteInfo.idDatabaseHash != ConnectionExtraInfo?.localInfo.idDatabaseHash)
				{
					string locEntryKey5 = "NETWORK_ERROR." + (flag ? "HOST." : "") + "VERSION_MISMATCH.description.modelDbMismatch";
					LocString locString5 = new LocString("main_menu_ui", locEntryKey5);
					locString5.Add("remoteHash", ConnectionExtraInfo?.remoteInfo.idDatabaseHash.ToString() ?? "<null>");
					locString5.Add("localHash", ConnectionExtraInfo?.localInfo.idDatabaseHash.ToString() ?? "<null>");
					return locString5.GetFormattedText();
				}
			}
			return _connectionReason.Value.ToString();
		}
		if (_steamReason.HasValue)
		{
			return $"{_steamReason} - {_debugReason}";
		}
		if (_lobbyCreationResult.HasValue)
		{
			return $"Lobby creation failed: {_lobbyCreationResult.Value}";
		}
		if (_lobbyEnterResponse.HasValue)
		{
			return $"Lobby join failed: {_lobbyEnterResponse.Value}";
		}
		if (_godotError.HasValue)
		{
			return _godotError.Value.ToString();
		}
		return "<null>";
	}

	/// <summary>
	/// Returns a string that should not be displayed to the user. Use this in debug logs.
	/// </summary>
	public override string ToString()
	{
		if (_reason.HasValue)
		{
			return $"DisconnectionReason {_reason.Value} {SelfInitiated}";
		}
		if (_connectionReason.HasValue)
		{
			return $"ConnectionFailureReason {_connectionReason.Value} {SelfInitiated}";
		}
		if (_steamReason.HasValue)
		{
			return $"SteamDisconnectionReason {_steamReason.Value} {_debugReason} {SelfInitiated}";
		}
		if (_lobbyCreationResult.HasValue)
		{
			return $"EResult {_lobbyCreationResult.Value} {SelfInitiated}";
		}
		if (_lobbyEnterResponse.HasValue)
		{
			return $"EChatRoomEnterResponse {_lobbyEnterResponse.Value} {SelfInitiated}";
		}
		if (_godotError.HasValue)
		{
			return $"Godot.Error {_godotError.Value} {SelfInitiated}";
		}
		return "<null>";
	}
}
