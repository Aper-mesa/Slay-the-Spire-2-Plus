using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Blitz;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Nodes.Screens.CustomRun;
using MegaCrit.Sts2.Core.Nodes.Screens.DailyRun;
using MegaCrit.Sts2.Core.Nodes.Screens.Dread;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Platform.Steam;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.Timeline.Epochs;

namespace MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

[ScriptPath("res://src/Core/Nodes/Screens/MainMenu/NMultiplayerHostSubmenu.cs")]
public class NMultiplayerHostSubmenu : NSubmenu
{
	/// <summary>
	/// Cached StringNames for the methods contained in this class, for fast lookup.
	/// </summary>
	public new class MethodName : NSubmenu.MethodName
	{
		/// <summary>
		/// Cached name for the 'Create' method.
		/// </summary>
		public static readonly StringName Create = "Create";

		/// <summary>
		/// Cached name for the '_Ready' method.
		/// </summary>
		public new static readonly StringName _Ready = "_Ready";

		/// <summary>
		/// Cached name for the 'RefreshButtons' method.
		/// </summary>
		public static readonly StringName RefreshButtons = "RefreshButtons";

		/// <summary>
		/// Cached name for the 'OnSubmenuOpened' method.
		/// </summary>
		public new static readonly StringName OnSubmenuOpened = "OnSubmenuOpened";

		/// <summary>
		/// Cached name for the 'OnStandardPressed' method.
		/// </summary>
		public static readonly StringName OnStandardPressed = "OnStandardPressed";

		/// <summary>
		/// Cached name for the 'OnDailyPressed' method.
		/// </summary>
		public static readonly StringName OnDailyPressed = "OnDailyPressed";

		/// <summary>
		/// Cached name for the 'OnBlitzPressed' method.
		/// </summary>
		public static readonly StringName OnBlitzPressed = "OnBlitzPressed";

		/// <summary>
		/// Cached name for the 'OnDreadPressed' method.
		/// </summary>
		public static readonly StringName OnDreadPressed = "OnDreadPressed";

		/// <summary>
		/// Cached name for the 'OnCustomPressed' method.
		/// </summary>
		public static readonly StringName OnCustomPressed = "OnCustomPressed";

		/// <summary>
		/// Cached name for the 'StartHost' method.
		/// </summary>
		public static readonly StringName StartHost = "StartHost";
	}

	/// <summary>
	/// Cached StringNames for the properties and fields contained in this class, for fast lookup.
	/// </summary>
	public new class PropertyName : NSubmenu.PropertyName
	{
		/// <summary>
		/// Cached name for the 'InitialFocusedControl' property.
		/// </summary>
		public new static readonly StringName InitialFocusedControl = "InitialFocusedControl";

		/// <summary>
		/// Cached name for the '_standardButton' field.
		/// </summary>
		public static readonly StringName _standardButton = "_standardButton";

		/// <summary>
		/// Cached name for the '_dailyButton' field.
		/// </summary>
		public static readonly StringName _dailyButton = "_dailyButton";

		/// <summary>
		/// Cached name for the '_dreadButton' field.
		/// </summary>
		public static readonly StringName _dreadButton = "_dreadButton";

		/// <summary>
		/// Cached name for the '_blitzButton' field.
		/// </summary>
		public static readonly StringName _blitzButton = "_blitzButton";

		/// <summary>
		/// Cached name for the '_customButton' field.
		/// </summary>
		public static readonly StringName _customButton = "_customButton";

		/// <summary>
		/// Cached name for the '_loadingOverlay' field.
		/// </summary>
		public static readonly StringName _loadingOverlay = "_loadingOverlay";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : NSubmenu.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/multiplayer_host_submenu");

	private NSubmenuButton _standardButton;

	private NSubmenuButton _dailyButton;

	private NSubmenuButton _dreadButton;

	private NSubmenuButton _blitzButton;

	private NSubmenuButton _customButton;

	private const string _keyStandard = "STANDARD_MP";

	private const string _keyDaily = "DAILY_MP";

	private const string _keyDread = "DREAD_MP";

	private const string _keyBlitz = "BLITZ_MP";

	private const string _keyCustom = "CUSTOM_MP";

	private const string _iconStandard = "standard";

	private const string _iconDaily = "daily";

	private const string _iconBlitz = "blitz";

	private const string _iconDread = "dread";

	private const string _iconCustom = "custom";

	private Control _loadingOverlay;

	public static string[] AssetPaths => new string[6]
	{
		_scenePath,
		NSubmenuPortrait.GetIconPath("standard"),
		NSubmenuPortrait.GetIconPath("daily"),
		NSubmenuPortrait.GetIconPath("blitz"),
		NSubmenuPortrait.GetIconPath("dread"),
		NSubmenuPortrait.GetIconPath("custom")
	};

	protected override Control InitialFocusedControl => _standardButton;

	public static NMultiplayerHostSubmenu? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NMultiplayerHostSubmenu>(PackedScene.GenEditState.Disabled);
	}

	public override void _Ready()
	{
		ConnectSignals();
		_loadingOverlay = GetNode<Control>("%LoadingOverlay");
		_standardButton = GetNode<NSubmenuButton>("%StandardButton");
		_standardButton.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(OnStandardPressed));
		_standardButton.SetLocalization("STANDARD_MP");
		_standardButton.SetIcon("standard");
		_dailyButton = GetNode<NSubmenuButton>("%DailyButton");
		_dailyButton.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(OnDailyPressed));
		_dailyButton.SetLocalization("DAILY_MP");
		_dailyButton.SetIcon("daily");
		_blitzButton = GetNode<NSubmenuButton>("%BlitzButton");
		_blitzButton.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(OnBlitzPressed));
		_blitzButton.SetLocalization("BLITZ_MP");
		_blitzButton.SetIcon("blitz");
		_dreadButton = GetNode<NSubmenuButton>("%DreadButton");
		_dreadButton.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(OnDreadPressed));
		_dreadButton.SetLocalization("DREAD_MP");
		_dreadButton.SetIcon("dread");
		_customButton = GetNode<NSubmenuButton>("%CustomRunButton");
		_customButton.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(OnCustomPressed));
		_customButton.SetLocalization("CUSTOM_MP");
		_customButton.SetIcon("custom");
	}

	/// <summary>
	/// Checks the unlock state of Custom and Daily to determine whether they should be locked or not,
	/// and the unlock state of Dread to determine whether it should be hidden and unnavigable or not.
	/// Is not checked only at _Ready() because the MainMenu isn't recreated upon returning from the Timeline.
	/// </summary>
	private void RefreshButtons()
	{
		_dailyButton.SetEnabled(SaveManager.Instance.IsEpochRevealed<DailyRunEpoch>());
		_blitzButton.SetEnabled(SaveManager.Instance.IsEpochRevealed<BlitzEpoch>());
		_customButton.SetEnabled(SaveManager.Instance.IsEpochRevealed<CustomAndSeedsEpoch>());
		_dailyButton.RefreshLabels("DAILY");
		_blitzButton.RefreshLabels("BLITZ");
		_customButton.RefreshLabels("CUSTOM");
		bool flag = SaveManager.Instance.IsEpochRevealed<DreadEpoch>();
		_dreadButton.SetEnabled(flag);
		_dreadButton.Visible = flag;
		if (_dreadButton.Visible)
		{
			GetNode<Control>("%ButtonContainer").Scale = Vector2.One * 0.85f;
		}
	}

	public override void OnSubmenuOpened()
	{
		RefreshButtons();
	}

	private void OnStandardPressed(NButton _)
	{
		StartHost(GameMode.Standard, isCustom: false);
	}

	private void OnDailyPressed(NButton _)
	{
		StartHost(GameMode.Daily, isCustom: false);
	}

	private void OnBlitzPressed(NButton _)
	{
		StartHost(GameMode.Blitz, isCustom: false);
	}

	private void OnDreadPressed(NButton _)
	{
		StartHost(GameMode.Dread, isCustom: false);
	}

	private void OnCustomPressed(NButton _)
	{
		StartHost(GameMode.Standard, isCustom: true);
	}

	public void StartHost(GameMode gameMode, bool isCustom)
	{
		TaskHelper.RunSafely(StartHostAsync(gameMode, isCustom, _loadingOverlay, _stack));
	}

	public static async Task StartHostAsync(GameMode gameMode, bool isCustom, Control loadingOverlay, NSubmenuStack stack)
	{
		PlatformType platformType = ((SteamInitializer.Initialized && !CommandLineHelper.HasArg("fastmp")) ? PlatformType.Steam : PlatformType.None);
		loadingOverlay.Visible = true;
		try
		{
			NetHostGameService netService = new NetHostGameService(PeerVersionInfo.LocalDefault());
			NetErrorInfo? netErrorInfo = null;
			if (platformType == PlatformType.Steam)
			{
				netErrorInfo = await netService.StartSteamHost(4);
			}
			else
			{
				netService.StartENetHost(33771, 4);
			}
			if (!netErrorInfo.HasValue)
			{
				if (isCustom)
				{
					NCustomRunScreen submenuType = stack.GetSubmenuType<NCustomRunScreen>();
					submenuType.InitializeMultiplayerAsHost(netService, 4);
					stack.Push(submenuType);
					return;
				}
				switch (gameMode)
				{
				case GameMode.Standard:
				{
					NCharacterSelectScreen submenuType5 = stack.GetSubmenuType<NCharacterSelectScreen>();
					submenuType5.InitializeMultiplayerAsHost(netService, 4);
					stack.Push(submenuType5);
					break;
				}
				case GameMode.Daily:
				{
					NDailyRunScreen submenuType4 = stack.GetSubmenuType<NDailyRunScreen>();
					submenuType4.InitializeMultiplayerAsHost(netService);
					stack.Push(submenuType4);
					break;
				}
				case GameMode.Blitz:
				{
					NBlitzScreen submenuType3 = stack.GetSubmenuType<NBlitzScreen>();
					submenuType3.InitializeMultiplayerAsHost(netService, 4);
					stack.Push(submenuType3);
					break;
				}
				case GameMode.Dread:
				{
					NDreadScreen submenuType2 = stack.GetSubmenuType<NDreadScreen>();
					submenuType2.InitializeMultiplayerAsHost(netService, 4);
					stack.Push(submenuType2);
					break;
				}
				default:
					throw new InvalidOperationException();
				}
			}
			else
			{
				NErrorPopup nErrorPopup = NErrorPopup.Create(netErrorInfo.Value);
				if (nErrorPopup != null)
				{
					NModalContainer.Instance.Add(nErrorPopup);
				}
			}
		}
		catch
		{
			NErrorPopup nErrorPopup2 = NErrorPopup.Create(new NetErrorInfo(NetError.InternalError, selfInitiated: false));
			if (nErrorPopup2 != null)
			{
				NModalContainer.Instance.Add(nErrorPopup2);
			}
			throw;
		}
		finally
		{
			loadingOverlay.Visible = false;
		}
	}

	/// <summary>
	/// Get the method information for all the methods declared in this class.
	/// This method is used by Godot to register the available methods in the editor.
	/// Do not call this method.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(10);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false), MethodFlags.Normal | MethodFlags.Static, null, null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.RefreshButtons, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnSubmenuOpened, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnStandardPressed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.OnDailyPressed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.OnBlitzPressed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.OnDreadPressed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.OnCustomPressed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.StartHost, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Int, "gameMode", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
			new PropertyInfo(Variant.Type.Bool, "isCustom", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.Create && args.Count == 0)
		{
			ret = VariantUtils.CreateFrom<NMultiplayerHostSubmenu>(Create());
			return true;
		}
		if (method == MethodName._Ready && args.Count == 0)
		{
			_Ready();
			ret = default;
			return true;
		}
		if (method == MethodName.RefreshButtons && args.Count == 0)
		{
			RefreshButtons();
			ret = default;
			return true;
		}
		if (method == MethodName.OnSubmenuOpened && args.Count == 0)
		{
			OnSubmenuOpened();
			ret = default;
			return true;
		}
		if (method == MethodName.OnStandardPressed && args.Count == 1)
		{
			OnStandardPressed(VariantUtils.ConvertTo<NButton>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.OnDailyPressed && args.Count == 1)
		{
			OnDailyPressed(VariantUtils.ConvertTo<NButton>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.OnBlitzPressed && args.Count == 1)
		{
			OnBlitzPressed(VariantUtils.ConvertTo<NButton>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.OnDreadPressed && args.Count == 1)
		{
			OnDreadPressed(VariantUtils.ConvertTo<NButton>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.OnCustomPressed && args.Count == 1)
		{
			OnCustomPressed(VariantUtils.ConvertTo<NButton>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.StartHost && args.Count == 2)
		{
			StartHost(VariantUtils.ConvertTo<GameMode>(in args[0]), VariantUtils.ConvertTo<bool>(in args[1]));
			ret = default;
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.Create && args.Count == 0)
		{
			ret = VariantUtils.CreateFrom<NMultiplayerHostSubmenu>(Create());
			return true;
		}
		ret = default;
		return false;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.Create)
		{
			return true;
		}
		if (method == MethodName._Ready)
		{
			return true;
		}
		if (method == MethodName.RefreshButtons)
		{
			return true;
		}
		if (method == MethodName.OnSubmenuOpened)
		{
			return true;
		}
		if (method == MethodName.OnStandardPressed)
		{
			return true;
		}
		if (method == MethodName.OnDailyPressed)
		{
			return true;
		}
		if (method == MethodName.OnBlitzPressed)
		{
			return true;
		}
		if (method == MethodName.OnDreadPressed)
		{
			return true;
		}
		if (method == MethodName.OnCustomPressed)
		{
			return true;
		}
		if (method == MethodName.StartHost)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._standardButton)
		{
			_standardButton = VariantUtils.ConvertTo<NSubmenuButton>(in value);
			return true;
		}
		if (name == PropertyName._dailyButton)
		{
			_dailyButton = VariantUtils.ConvertTo<NSubmenuButton>(in value);
			return true;
		}
		if (name == PropertyName._dreadButton)
		{
			_dreadButton = VariantUtils.ConvertTo<NSubmenuButton>(in value);
			return true;
		}
		if (name == PropertyName._blitzButton)
		{
			_blitzButton = VariantUtils.ConvertTo<NSubmenuButton>(in value);
			return true;
		}
		if (name == PropertyName._customButton)
		{
			_customButton = VariantUtils.ConvertTo<NSubmenuButton>(in value);
			return true;
		}
		if (name == PropertyName._loadingOverlay)
		{
			_loadingOverlay = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.InitialFocusedControl)
		{
			value = VariantUtils.CreateFrom<Control>(InitialFocusedControl);
			return true;
		}
		if (name == PropertyName._standardButton)
		{
			value = VariantUtils.CreateFrom(in _standardButton);
			return true;
		}
		if (name == PropertyName._dailyButton)
		{
			value = VariantUtils.CreateFrom(in _dailyButton);
			return true;
		}
		if (name == PropertyName._dreadButton)
		{
			value = VariantUtils.CreateFrom(in _dreadButton);
			return true;
		}
		if (name == PropertyName._blitzButton)
		{
			value = VariantUtils.CreateFrom(in _blitzButton);
			return true;
		}
		if (name == PropertyName._customButton)
		{
			value = VariantUtils.CreateFrom(in _customButton);
			return true;
		}
		if (name == PropertyName._loadingOverlay)
		{
			value = VariantUtils.CreateFrom(in _loadingOverlay);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	/// <summary>
	/// Get the property information for all the properties declared in this class.
	/// This method is used by Godot to register the available properties in the editor.
	/// Do not call this method.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._standardButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._dailyButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._dreadButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._blitzButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._customButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._loadingOverlay, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.InitialFocusedControl, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._standardButton, Variant.From(in _standardButton));
		info.AddProperty(PropertyName._dailyButton, Variant.From(in _dailyButton));
		info.AddProperty(PropertyName._dreadButton, Variant.From(in _dreadButton));
		info.AddProperty(PropertyName._blitzButton, Variant.From(in _blitzButton));
		info.AddProperty(PropertyName._customButton, Variant.From(in _customButton));
		info.AddProperty(PropertyName._loadingOverlay, Variant.From(in _loadingOverlay));
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._standardButton, out var value))
		{
			_standardButton = value.As<NSubmenuButton>();
		}
		if (info.TryGetProperty(PropertyName._dailyButton, out var value2))
		{
			_dailyButton = value2.As<NSubmenuButton>();
		}
		if (info.TryGetProperty(PropertyName._dreadButton, out var value3))
		{
			_dreadButton = value3.As<NSubmenuButton>();
		}
		if (info.TryGetProperty(PropertyName._blitzButton, out var value4))
		{
			_blitzButton = value4.As<NSubmenuButton>();
		}
		if (info.TryGetProperty(PropertyName._customButton, out var value5))
		{
			_customButton = value5.As<NSubmenuButton>();
		}
		if (info.TryGetProperty(PropertyName._loadingOverlay, out var value6))
		{
			_loadingOverlay = value6.As<Control>();
		}
	}
}
