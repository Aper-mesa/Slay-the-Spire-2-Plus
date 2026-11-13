using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Container = Godot.Container;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Potions;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Nodes.TopBar;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.sts2.Core.Nodes.TopBar;

namespace MegaCrit.Sts2.Core.Nodes.CommonUi;

[ScriptPath("res://src/Core/Nodes/CommonUi/NTopBar.cs")]
public class NTopBar : Control
{
	public new class MethodName : Control.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public new static readonly StringName _EnterTree = "_EnterTree";

		public new static readonly StringName _ExitTree = "_ExitTree";

		public new static readonly StringName _Notification = "_Notification";

		public static readonly StringName ToggleAnimState = "ToggleAnimState";

		public new static readonly StringName _Input = "_Input";

		public static readonly StringName DebugHideTopBar = "DebugHideTopBar";

		public static readonly StringName AnimHide = "AnimHide";

		public static readonly StringName AnimShow = "AnimShow";

		public static readonly StringName MaxPotionsChanged = "MaxPotionsChanged";

		public static readonly StringName UpdateNavigation = "UpdateNavigation";
	}

	public new class PropertyName : Control.PropertyName
	{
		public static readonly StringName Portrait = "Portrait";

		public static readonly StringName PortraitTip = "PortraitTip";

		public static readonly StringName Hp = "Hp";

		public static readonly StringName Gold = "Gold";

		public static readonly StringName PotionContainer = "PotionContainer";

		public static readonly StringName RoomIcon = "RoomIcon";

		public static readonly StringName RoomIconsContainer = "RoomIconsContainer";

		public static readonly StringName BlitzMap = "BlitzMap";

		public static readonly StringName FloorIcon = "FloorIcon";

		public static readonly StringName BossIcon = "BossIcon";

		public static readonly StringName Timer = "Timer";

		public static readonly StringName Map = "Map";

		public static readonly StringName Deck = "Deck";

		public static readonly StringName Pause = "Pause";

		public static readonly StringName LeftContainer = "LeftContainer";

		public static readonly StringName RightContainer = "RightContainer";

		public static readonly StringName TrailContainer = "TrailContainer";

		public static readonly StringName ActiveScreenProxy = "ActiveScreenProxy";

		public static readonly StringName _modifiersContainer = "_modifiersContainer";

		public static readonly StringName _topBarMat = "_topBarMat";

		public static readonly StringName _capstoneContainer = "_capstoneContainer";

		public static readonly StringName _achievementLock = "_achievementLock";

		public static readonly StringName _ascensionIcon = "_ascensionIcon";

		public static readonly StringName _ascensionLabel = "_ascensionLabel";

		public static readonly StringName _ascensionIconHsv = "_ascensionIconHsv";

		public static readonly StringName _hideTween = "_hideTween";

		public static readonly StringName _isDebugHidden = "_isDebugHidden";
	}

	public new class SignalName : Control.SignalName
	{
	}

	private Control _modifiersContainer;

	private ShaderMaterial _topBarMat;

	private NCapstoneContainer _capstoneContainer;

	private static readonly StringName _fontColorTheme = "font_color";

	private static readonly StringName _fontOutlineTheme = "font_outline_color";

	private static readonly StringName _h = new StringName("h");

	private static readonly StringName _s = new StringName("s");

	private static readonly StringName _v = new StringName("v");

	private Control _achievementLock;

	private TextureRect _ascensionIcon;

	private MegaLabel _ascensionLabel;

	private ShaderMaterial _ascensionIconHsv;

	private Tween? _hideTween;

	private bool _isDebugHidden;

	private Player? _player;

	public NTopBarPortrait Portrait { get; private set; }

	public NTopBarPortraitTip PortraitTip { get; private set; }

	public NTopBarHp Hp { get; private set; }

	public NTopBarGold Gold { get; private set; }

	public NPotionContainer PotionContainer { get; private set; }

	public NTopBarRoomIcon RoomIcon { get; private set; }

	public Control RoomIconsContainer { get; private set; }

	public NBlitzTopBarMap BlitzMap { get; private set; }

	public NTopBarFloorIcon FloorIcon { get; private set; }

	public NTopBarBossIcon BossIcon { get; private set; }

	public NRunTimer Timer { get; private set; }

	public NTopBarMapButton Map { get; private set; }

	public NTopBarDeckButton Deck { get; private set; }

	public NTopBarPauseButton Pause { get; private set; }

	public Container LeftContainer { get; private set; }

	public Container RightContainer { get; private set; }

	public Node TrailContainer { get; private set; }

	public Control ActiveScreenProxy { get; private set; }

	public override void _Ready()
	{
		TrailContainer = GetNode<Node>("%TrailContainer");
		Map = GetNode<NTopBarMapButton>("%Map");
		Deck = GetNode<NTopBarDeckButton>("%Deck");
		Pause = GetNode<NTopBarPauseButton>("%PauseButton");
		PotionContainer = GetNode<NPotionContainer>("%PotionContainer");
		RoomIcon = GetNode<NTopBarRoomIcon>("%RoomIcon");
		RoomIconsContainer = GetNode<Control>("%RoomIcons");
		BlitzMap = GetNode<NBlitzTopBarMap>("%BlitzMap");
		FloorIcon = GetNode<NTopBarFloorIcon>("%FloorIcon");
		BossIcon = GetNode<NTopBarBossIcon>("%BossIcon");
		Gold = GetNode<NTopBarGold>("%TopBarGold");
		Hp = GetNode<NTopBarHp>("%TopBarHp");
		Portrait = GetNode<NTopBarPortrait>("%TopBarPortrait");
		PortraitTip = GetNode<NTopBarPortraitTip>("%TopBarPortraitTip");
		Timer = GetNode<NRunTimer>("%TimerContainer");
		ActiveScreenProxy = GetNode<Control>("%ActiveScreenProxy");
		LeftContainer = GetNode<Container>("LeftAlignedStuff");
		RightContainer = GetNode<Container>("RightAlignedStuff");
		_achievementLock = GetNode<Control>("%AchievementLock");
		_ascensionIcon = GetNode<TextureRect>("%AscensionIcon");
		_ascensionLabel = GetNode<MegaLabel>("%AscensionLabel");
		_ascensionIconHsv = (ShaderMaterial)_ascensionIcon.Material;
		_modifiersContainer = GetNode<Control>("%Modifiers");
		_topBarMat = (ShaderMaterial)GetNode<Control>("%BgImage").GetMaterial();
		ActiveScreenProxy.Connect(Control.SignalName.FocusEntered, Callable.From(() =>
		{
			ActiveScreenContext.Instance.GetCurrentScreen()?.FocusedControlFromTopBar?.TryGrabFocus();
		}));
		_capstoneContainer = GetParent().GetNode<NCapstoneContainer>("%CapstoneScreenContainer");
		_capstoneContainer.Connect(Node.SignalName.ChildEnteredTree, Callable.From<Node>(ToggleAnimState));
		_capstoneContainer.Connect(Node.SignalName.ChildExitingTree, Callable.From<Node>(ToggleAnimState));
	}

	public void Initialize(IRunState runState)
	{
		if (runState.GameMode == GameMode.Dread)
		{
			int dreadStreak = AscensionHelper.GetDreadStreak(runState);
			ApplyAscensionIconConfig(new DreadIconConfig(), (dreadStreak > 0) ? dreadStreak.ToString() : null);
		}
		else if (runState.GameMode == GameMode.Blitz)
		{
			ApplyAscensionIconConfig(new BlitzIconConfig(), (runState.AscensionLevel > 0) ? runState.AscensionLevel.ToString() : null);
		}
		else if (runState.AscensionLevel > 0)
		{
			ApplyAscensionIconConfig(new AscensionIconConfig(runState.Players.Count > 1), runState.AscensionLevel.ToString());
		}
		else
		{
			_ascensionIcon.Visible = false;
		}
		if (runState.GameMode == GameMode.Blitz)
		{
			Gold.Visible = false;
			RoomIconsContainer.Visible = false;
			Map.Visible = false;
			Map.Disable();
			BlitzMap.Initialize(runState, this);
		}
		_achievementLock.Visible = GameModeHelper.AreAchievementsAndEpochsLockedForRun(runState);
		_modifiersContainer.Visible = runState.Modifiers.Count > 0;
		foreach (ModifierModel modifier in runState.Modifiers)
		{
			NTopBarModifier child = NTopBarModifier.Create(modifier);
			_modifiersContainer.AddChildSafely(child);
		}
		_player = LocalContext.GetMe(runState);
		Deck.Initialize(_player);
		RoomIcon.Initialize(runState);
		FloorIcon.Initialize(runState);
		BossIcon.Initialize(runState);
		Gold.Initialize(_player);
		Hp.Initialize(_player);
		Pause.Initialize(runState);
		Portrait.Initialize(_player);
		PortraitTip.Initialize(runState);
		PotionContainer.Initialize(runState);
		_topBarMat.SetShaderParameter(_s, (runState.GameMode == GameMode.Dread) ? 0.25f : 1f);
		_player.RelicObtained += OnRelicsUpdated;
		_player.RelicRemoved += OnRelicsUpdated;
		_player.MaxPotionCountChanged += MaxPotionsChanged;
		Callable.From(UpdateNavigation).CallDeferred();
	}

	private void ApplyAscensionIconConfig(TopBarAscensionIconConfig config, string? labelText)
	{
		_ascensionIcon.Texture = config.Texture;
		_ascensionIconHsv.SetShaderParameter(_h, config.Hue);
		_ascensionIconHsv.SetShaderParameter(_s, config.Saturation);
		_ascensionIconHsv.SetShaderParameter(_v, config.Value);
		_ascensionLabel.AddThemeColorOverride(_fontColorTheme, config.FontColor);
		_ascensionLabel.AddThemeColorOverride(_fontOutlineTheme, config.FontOutlineColor);
		_ascensionLabel.Visible = labelText != null;
		if (labelText != null)
		{
			_ascensionLabel.SetTextAutoSize(labelText);
		}
		_ascensionIcon.Visible = true;
	}

	public override void _EnterTree()
	{
		ActiveScreenContext.Instance.Updated += UpdateNavigation;
	}

	public override void _ExitTree()
	{
		ActiveScreenContext.Instance.Updated -= UpdateNavigation;
	}

	public override void _Notification(int what)
	{
		if ((long)what == 1 && _player != null)
		{
			_player.RelicObtained -= OnRelicsUpdated;
			_player.RelicRemoved -= OnRelicsUpdated;
			_player.MaxPotionCountChanged -= MaxPotionsChanged;
		}
	}

	private void ToggleAnimState(Node _)
	{
		Pause.ToggleAnimState();
		Deck.ToggleAnimState();
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsActionReleased(DebugHotkey.hideTopBar))
		{
			DebugHideTopBar();
		}
	}

	private void DebugHideTopBar()
	{
		if (!_isDebugHidden)
		{
			NGame.Instance.AddChildSafely(NFullscreenTextVfx.Create("Hide Top Bar"));
			AnimHide();
		}
		else
		{
			NGame.Instance.AddChildSafely(NFullscreenTextVfx.Create("Show Top Bar"));
			AnimShow();
		}
		_isDebugHidden = !_isDebugHidden;
	}

	public void AnimHide()
	{
		FocusBehaviorRecursive = FocusBehaviorRecursiveEnum.Disabled;
		_hideTween?.Kill();
		_hideTween = CreateTween();
		_hideTween.TweenProperty(this, "position:y", -100f, 0.25).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
	}

	public void AnimShow()
	{
		FocusBehaviorRecursive = FocusBehaviorRecursiveEnum.Inherited;
		_hideTween?.Kill();
		_hideTween = CreateTween();
		_hideTween.TweenProperty(this, "position:y", 0f, 0.25).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
	}

	private void OnRelicsUpdated(RelicModel _)
	{
		Callable.From(UpdateNavigation).CallDeferred();
	}

	private void MaxPotionsChanged(int _)
	{
		Callable.From(UpdateNavigation).CallDeferred();
	}

	public List<Control> GetAllNavigableElements()
	{
		List<Control> list = new List<Control>();
		list.Add(PortraitTip);
		list.Add(Hp);
		list.Add(Gold);
		list.AddRange(PotionContainer.Holders);
		list.Add(RoomIcon);
		list.Add(FloorIcon);
		list.Add(BossIcon);
		list.AddRange(BlitzMap.MapPoints);
		list.AddRange(_modifiersContainer.GetChildren().OfType<Control>());
		return (from c in list.ToArray()
			where c.IsVisibleInTree()
			select c).ToList();
	}

	public void UpdateNavigation()
	{
		IReadOnlyList<NRelicInventoryHolder> readOnlyList = NRun.Instance?.GlobalUi.RelicInventory.RelicNodes ?? Array.Empty<NRelicInventoryHolder>();
		List<Control> allNavigableElements = GetAllNavigableElements();
		for (int i = 0; i < allNavigableElements.Count; i++)
		{
			Control control = allNavigableElements[i];
			Control control2 = null;
			for (int j = 1; j <= allNavigableElements.Count; j++)
			{
				control2 = allNavigableElements[(i - j + allNavigableElements.Count) % allNavigableElements.Count];
				if (control2.IsVisibleInTree())
				{
					break;
				}
			}
			Control control3 = null;
			for (int k = 1; k <= allNavigableElements.Count; k++)
			{
				control3 = allNavigableElements[(i + k) % allNavigableElements.Count];
				if (control3.IsVisibleInTree())
				{
					break;
				}
			}
			Control control4 = null;
			float num = 0f;
			Vector2 vector = control.GlobalPosition + control.Size * 0.5f;
			foreach (NRelicInventoryHolder item in readOnlyList)
			{
				Vector2 to = item.GlobalPosition + item.Size * 0.5f;
				float num2 = vector.DistanceSquaredTo(to);
				if (control4 == null || num2 < num)
				{
					control4 = item;
					num = num2;
				}
			}
			control.FocusNeighborLeft = control2?.GetPath() ?? control.GetPath();
			control.FocusNeighborRight = control3?.GetPath() ?? control.GetPath();
			control.FocusNeighborTop = control.GetPath();
			control.FocusNeighborBottom = control4?.GetPath() ?? control.GetPath();
		}
		NRun.Instance?.GlobalUi.RelicInventory.UpdateNavigation();
		Viewport viewport = GetViewport();
		if (viewport != null && viewport.GuiGetFocusOwner() == ActiveScreenProxy)
		{
			ActiveScreenContext.Instance.FocusOnDefaultControl();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(11);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._Notification, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Int, "what", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.ToggleAnimState, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Node"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "inputEvent", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("InputEvent"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.DebugHideTopBar, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.AnimHide, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.AnimShow, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.MaxPotionsChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Int, "_", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.UpdateNavigation, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName._Ready && args.Count == 0)
		{
			_Ready();
			ret = default;
			return true;
		}
		if (method == MethodName._EnterTree && args.Count == 0)
		{
			_EnterTree();
			ret = default;
			return true;
		}
		if (method == MethodName._ExitTree && args.Count == 0)
		{
			_ExitTree();
			ret = default;
			return true;
		}
		if (method == MethodName._Notification && args.Count == 1)
		{
			_Notification(VariantUtils.ConvertTo<int>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.ToggleAnimState && args.Count == 1)
		{
			ToggleAnimState(VariantUtils.ConvertTo<Node>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName._Input && args.Count == 1)
		{
			_Input(VariantUtils.ConvertTo<InputEvent>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.DebugHideTopBar && args.Count == 0)
		{
			DebugHideTopBar();
			ret = default;
			return true;
		}
		if (method == MethodName.AnimHide && args.Count == 0)
		{
			AnimHide();
			ret = default;
			return true;
		}
		if (method == MethodName.AnimShow && args.Count == 0)
		{
			AnimShow();
			ret = default;
			return true;
		}
		if (method == MethodName.MaxPotionsChanged && args.Count == 1)
		{
			MaxPotionsChanged(VariantUtils.ConvertTo<int>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.UpdateNavigation && args.Count == 0)
		{
			UpdateNavigation();
			ret = default;
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName._Ready)
		{
			return true;
		}
		if (method == MethodName._EnterTree)
		{
			return true;
		}
		if (method == MethodName._ExitTree)
		{
			return true;
		}
		if (method == MethodName._Notification)
		{
			return true;
		}
		if (method == MethodName.ToggleAnimState)
		{
			return true;
		}
		if (method == MethodName._Input)
		{
			return true;
		}
		if (method == MethodName.DebugHideTopBar)
		{
			return true;
		}
		if (method == MethodName.AnimHide)
		{
			return true;
		}
		if (method == MethodName.AnimShow)
		{
			return true;
		}
		if (method == MethodName.MaxPotionsChanged)
		{
			return true;
		}
		if (method == MethodName.UpdateNavigation)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.Portrait)
		{
			Portrait = VariantUtils.ConvertTo<NTopBarPortrait>(in value);
			return true;
		}
		if (name == PropertyName.PortraitTip)
		{
			PortraitTip = VariantUtils.ConvertTo<NTopBarPortraitTip>(in value);
			return true;
		}
		if (name == PropertyName.Hp)
		{
			Hp = VariantUtils.ConvertTo<NTopBarHp>(in value);
			return true;
		}
		if (name == PropertyName.Gold)
		{
			Gold = VariantUtils.ConvertTo<NTopBarGold>(in value);
			return true;
		}
		if (name == PropertyName.PotionContainer)
		{
			PotionContainer = VariantUtils.ConvertTo<NPotionContainer>(in value);
			return true;
		}
		if (name == PropertyName.RoomIcon)
		{
			RoomIcon = VariantUtils.ConvertTo<NTopBarRoomIcon>(in value);
			return true;
		}
		if (name == PropertyName.RoomIconsContainer)
		{
			RoomIconsContainer = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName.BlitzMap)
		{
			BlitzMap = VariantUtils.ConvertTo<NBlitzTopBarMap>(in value);
			return true;
		}
		if (name == PropertyName.FloorIcon)
		{
			FloorIcon = VariantUtils.ConvertTo<NTopBarFloorIcon>(in value);
			return true;
		}
		if (name == PropertyName.BossIcon)
		{
			BossIcon = VariantUtils.ConvertTo<NTopBarBossIcon>(in value);
			return true;
		}
		if (name == PropertyName.Timer)
		{
			Timer = VariantUtils.ConvertTo<NRunTimer>(in value);
			return true;
		}
		if (name == PropertyName.Map)
		{
			Map = VariantUtils.ConvertTo<NTopBarMapButton>(in value);
			return true;
		}
		if (name == PropertyName.Deck)
		{
			Deck = VariantUtils.ConvertTo<NTopBarDeckButton>(in value);
			return true;
		}
		if (name == PropertyName.Pause)
		{
			Pause = VariantUtils.ConvertTo<NTopBarPauseButton>(in value);
			return true;
		}
		if (name == PropertyName.LeftContainer)
		{
			LeftContainer = VariantUtils.ConvertTo<Container>(in value);
			return true;
		}
		if (name == PropertyName.RightContainer)
		{
			RightContainer = VariantUtils.ConvertTo<Container>(in value);
			return true;
		}
		if (name == PropertyName.TrailContainer)
		{
			TrailContainer = VariantUtils.ConvertTo<Node>(in value);
			return true;
		}
		if (name == PropertyName.ActiveScreenProxy)
		{
			ActiveScreenProxy = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._modifiersContainer)
		{
			_modifiersContainer = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._topBarMat)
		{
			_topBarMat = VariantUtils.ConvertTo<ShaderMaterial>(in value);
			return true;
		}
		if (name == PropertyName._capstoneContainer)
		{
			_capstoneContainer = VariantUtils.ConvertTo<NCapstoneContainer>(in value);
			return true;
		}
		if (name == PropertyName._achievementLock)
		{
			_achievementLock = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._ascensionIcon)
		{
			_ascensionIcon = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName._ascensionLabel)
		{
			_ascensionLabel = VariantUtils.ConvertTo<MegaLabel>(in value);
			return true;
		}
		if (name == PropertyName._ascensionIconHsv)
		{
			_ascensionIconHsv = VariantUtils.ConvertTo<ShaderMaterial>(in value);
			return true;
		}
		if (name == PropertyName._hideTween)
		{
			_hideTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		if (name == PropertyName._isDebugHidden)
		{
			_isDebugHidden = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.Portrait)
		{
			value = VariantUtils.CreateFrom<NTopBarPortrait>(Portrait);
			return true;
		}
		if (name == PropertyName.PortraitTip)
		{
			value = VariantUtils.CreateFrom<NTopBarPortraitTip>(PortraitTip);
			return true;
		}
		if (name == PropertyName.Hp)
		{
			value = VariantUtils.CreateFrom<NTopBarHp>(Hp);
			return true;
		}
		if (name == PropertyName.Gold)
		{
			value = VariantUtils.CreateFrom<NTopBarGold>(Gold);
			return true;
		}
		if (name == PropertyName.PotionContainer)
		{
			value = VariantUtils.CreateFrom<NPotionContainer>(PotionContainer);
			return true;
		}
		if (name == PropertyName.RoomIcon)
		{
			value = VariantUtils.CreateFrom<NTopBarRoomIcon>(RoomIcon);
			return true;
		}
		Control from;
		if (name == PropertyName.RoomIconsContainer)
		{
			from = RoomIconsContainer;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName.BlitzMap)
		{
			value = VariantUtils.CreateFrom<NBlitzTopBarMap>(BlitzMap);
			return true;
		}
		if (name == PropertyName.FloorIcon)
		{
			value = VariantUtils.CreateFrom<NTopBarFloorIcon>(FloorIcon);
			return true;
		}
		if (name == PropertyName.BossIcon)
		{
			value = VariantUtils.CreateFrom<NTopBarBossIcon>(BossIcon);
			return true;
		}
		if (name == PropertyName.Timer)
		{
			value = VariantUtils.CreateFrom<NRunTimer>(Timer);
			return true;
		}
		if (name == PropertyName.Map)
		{
			value = VariantUtils.CreateFrom<NTopBarMapButton>(Map);
			return true;
		}
		if (name == PropertyName.Deck)
		{
			value = VariantUtils.CreateFrom<NTopBarDeckButton>(Deck);
			return true;
		}
		if (name == PropertyName.Pause)
		{
			value = VariantUtils.CreateFrom<NTopBarPauseButton>(Pause);
			return true;
		}
		Container from2;
		if (name == PropertyName.LeftContainer)
		{
			from2 = LeftContainer;
			value = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (name == PropertyName.RightContainer)
		{
			from2 = RightContainer;
			value = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (name == PropertyName.TrailContainer)
		{
			value = VariantUtils.CreateFrom<Node>(TrailContainer);
			return true;
		}
		if (name == PropertyName.ActiveScreenProxy)
		{
			from = ActiveScreenProxy;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName._modifiersContainer)
		{
			value = VariantUtils.CreateFrom(in _modifiersContainer);
			return true;
		}
		if (name == PropertyName._topBarMat)
		{
			value = VariantUtils.CreateFrom(in _topBarMat);
			return true;
		}
		if (name == PropertyName._capstoneContainer)
		{
			value = VariantUtils.CreateFrom(in _capstoneContainer);
			return true;
		}
		if (name == PropertyName._achievementLock)
		{
			value = VariantUtils.CreateFrom(in _achievementLock);
			return true;
		}
		if (name == PropertyName._ascensionIcon)
		{
			value = VariantUtils.CreateFrom(in _ascensionIcon);
			return true;
		}
		if (name == PropertyName._ascensionLabel)
		{
			value = VariantUtils.CreateFrom(in _ascensionLabel);
			return true;
		}
		if (name == PropertyName._ascensionIconHsv)
		{
			value = VariantUtils.CreateFrom(in _ascensionIconHsv);
			return true;
		}
		if (name == PropertyName._hideTween)
		{
			value = VariantUtils.CreateFrom(in _hideTween);
			return true;
		}
		if (name == PropertyName._isDebugHidden)
		{
			value = VariantUtils.CreateFrom(in _isDebugHidden);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.Portrait, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.PortraitTip, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.Hp, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.Gold, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.PotionContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.RoomIcon, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.RoomIconsContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.BlitzMap, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.FloorIcon, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.BossIcon, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._modifiersContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.Timer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.Map, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.Deck, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.Pause, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._topBarMat, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._capstoneContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.LeftContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.RightContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.TrailContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.ActiveScreenProxy, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._achievementLock, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._ascensionIcon, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._ascensionLabel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._ascensionIconHsv, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._hideTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName._isDebugHidden, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.Portrait, Variant.From<NTopBarPortrait>(Portrait));
		info.AddProperty(PropertyName.PortraitTip, Variant.From<NTopBarPortraitTip>(PortraitTip));
		info.AddProperty(PropertyName.Hp, Variant.From<NTopBarHp>(Hp));
		info.AddProperty(PropertyName.Gold, Variant.From<NTopBarGold>(Gold));
		info.AddProperty(PropertyName.PotionContainer, Variant.From<NPotionContainer>(PotionContainer));
		info.AddProperty(PropertyName.RoomIcon, Variant.From<NTopBarRoomIcon>(RoomIcon));
		info.AddProperty(PropertyName.RoomIconsContainer, Variant.From<Control>(RoomIconsContainer));
		info.AddProperty(PropertyName.BlitzMap, Variant.From<NBlitzTopBarMap>(BlitzMap));
		info.AddProperty(PropertyName.FloorIcon, Variant.From<NTopBarFloorIcon>(FloorIcon));
		info.AddProperty(PropertyName.BossIcon, Variant.From<NTopBarBossIcon>(BossIcon));
		info.AddProperty(PropertyName.Timer, Variant.From<NRunTimer>(Timer));
		info.AddProperty(PropertyName.Map, Variant.From<NTopBarMapButton>(Map));
		info.AddProperty(PropertyName.Deck, Variant.From<NTopBarDeckButton>(Deck));
		info.AddProperty(PropertyName.Pause, Variant.From<NTopBarPauseButton>(Pause));
		info.AddProperty(PropertyName.LeftContainer, Variant.From<Container>(LeftContainer));
		info.AddProperty(PropertyName.RightContainer, Variant.From<Container>(RightContainer));
		info.AddProperty(PropertyName.TrailContainer, Variant.From<Node>(TrailContainer));
		info.AddProperty(PropertyName.ActiveScreenProxy, Variant.From<Control>(ActiveScreenProxy));
		info.AddProperty(PropertyName._modifiersContainer, Variant.From(in _modifiersContainer));
		info.AddProperty(PropertyName._topBarMat, Variant.From(in _topBarMat));
		info.AddProperty(PropertyName._capstoneContainer, Variant.From(in _capstoneContainer));
		info.AddProperty(PropertyName._achievementLock, Variant.From(in _achievementLock));
		info.AddProperty(PropertyName._ascensionIcon, Variant.From(in _ascensionIcon));
		info.AddProperty(PropertyName._ascensionLabel, Variant.From(in _ascensionLabel));
		info.AddProperty(PropertyName._ascensionIconHsv, Variant.From(in _ascensionIconHsv));
		info.AddProperty(PropertyName._hideTween, Variant.From(in _hideTween));
		info.AddProperty(PropertyName._isDebugHidden, Variant.From(in _isDebugHidden));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.Portrait, out var value))
		{
			Portrait = value.As<NTopBarPortrait>();
		}
		if (info.TryGetProperty(PropertyName.PortraitTip, out var value2))
		{
			PortraitTip = value2.As<NTopBarPortraitTip>();
		}
		if (info.TryGetProperty(PropertyName.Hp, out var value3))
		{
			Hp = value3.As<NTopBarHp>();
		}
		if (info.TryGetProperty(PropertyName.Gold, out var value4))
		{
			Gold = value4.As<NTopBarGold>();
		}
		if (info.TryGetProperty(PropertyName.PotionContainer, out var value5))
		{
			PotionContainer = value5.As<NPotionContainer>();
		}
		if (info.TryGetProperty(PropertyName.RoomIcon, out var value6))
		{
			RoomIcon = value6.As<NTopBarRoomIcon>();
		}
		if (info.TryGetProperty(PropertyName.RoomIconsContainer, out var value7))
		{
			RoomIconsContainer = value7.As<Control>();
		}
		if (info.TryGetProperty(PropertyName.BlitzMap, out var value8))
		{
			BlitzMap = value8.As<NBlitzTopBarMap>();
		}
		if (info.TryGetProperty(PropertyName.FloorIcon, out var value9))
		{
			FloorIcon = value9.As<NTopBarFloorIcon>();
		}
		if (info.TryGetProperty(PropertyName.BossIcon, out var value10))
		{
			BossIcon = value10.As<NTopBarBossIcon>();
		}
		if (info.TryGetProperty(PropertyName.Timer, out var value11))
		{
			Timer = value11.As<NRunTimer>();
		}
		if (info.TryGetProperty(PropertyName.Map, out var value12))
		{
			Map = value12.As<NTopBarMapButton>();
		}
		if (info.TryGetProperty(PropertyName.Deck, out var value13))
		{
			Deck = value13.As<NTopBarDeckButton>();
		}
		if (info.TryGetProperty(PropertyName.Pause, out var value14))
		{
			Pause = value14.As<NTopBarPauseButton>();
		}
		if (info.TryGetProperty(PropertyName.LeftContainer, out var value15))
		{
			LeftContainer = value15.As<Container>();
		}
		if (info.TryGetProperty(PropertyName.RightContainer, out var value16))
		{
			RightContainer = value16.As<Container>();
		}
		if (info.TryGetProperty(PropertyName.TrailContainer, out var value17))
		{
			TrailContainer = value17.As<Node>();
		}
		if (info.TryGetProperty(PropertyName.ActiveScreenProxy, out var value18))
		{
			ActiveScreenProxy = value18.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._modifiersContainer, out var value19))
		{
			_modifiersContainer = value19.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._topBarMat, out var value20))
		{
			_topBarMat = value20.As<ShaderMaterial>();
		}
		if (info.TryGetProperty(PropertyName._capstoneContainer, out var value21))
		{
			_capstoneContainer = value21.As<NCapstoneContainer>();
		}
		if (info.TryGetProperty(PropertyName._achievementLock, out var value22))
		{
			_achievementLock = value22.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._ascensionIcon, out var value23))
		{
			_ascensionIcon = value23.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName._ascensionLabel, out var value24))
		{
			_ascensionLabel = value24.As<MegaLabel>();
		}
		if (info.TryGetProperty(PropertyName._ascensionIconHsv, out var value25))
		{
			_ascensionIconHsv = value25.As<ShaderMaterial>();
		}
		if (info.TryGetProperty(PropertyName._hideTween, out var value26))
		{
			_hideTween = value26.As<Tween>();
		}
		if (info.TryGetProperty(PropertyName._isDebugHidden, out var value27))
		{
			_isDebugHidden = value27.As<bool>();
		}
	}
}
