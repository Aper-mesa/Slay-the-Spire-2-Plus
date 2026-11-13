using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.TreasureRelicPicking;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.TreasureRoomRelic;

/// <summary>
/// Selection screen for relics at the treasure room screen.
/// In singleplayer, this just shows one relic. In multiplayer, this shows multiple relics and handles animation that
/// occurs after all players select a relic.
/// </summary>
[ScriptPath("res://src/Core/Nodes/Screens/TreasureRoomRelic/NTreasureRoomRelicCollection.cs")]
public class NTreasureRoomRelicCollection : Control, IScreenContext
{
	/// <summary>
	/// Cached StringNames for the methods contained in this class, for fast lookup.
	/// </summary>
	public new class MethodName : Control.MethodName
	{
		/// <summary>
		/// Cached name for the '_Ready' method.
		/// </summary>
		public new static readonly StringName _Ready = "_Ready";

		/// <summary>
		/// Cached name for the '_EnterTree' method.
		/// </summary>
		public new static readonly StringName _EnterTree = "_EnterTree";

		/// <summary>
		/// Cached name for the '_ExitTree' method.
		/// </summary>
		public new static readonly StringName _ExitTree = "_ExitTree";

		/// <summary>
		/// Cached name for the 'InitializeRelics' method.
		/// </summary>
		public static readonly StringName InitializeRelics = "InitializeRelics";

		/// <summary>
		/// Cached name for the 'RefreshNavigation' method.
		/// </summary>
		public static readonly StringName RefreshNavigation = "RefreshNavigation";

		/// <summary>
		/// Cached name for the 'SpawnEmptyChestVfx' method.
		/// </summary>
		public static readonly StringName SpawnEmptyChestVfx = "SpawnEmptyChestVfx";

		/// <summary>
		/// Cached name for the 'SetSelectionEnabled' method.
		/// </summary>
		public static readonly StringName SetSelectionEnabled = "SetSelectionEnabled";

		/// <summary>
		/// Cached name for the 'AnimIn' method.
		/// </summary>
		public static readonly StringName AnimIn = "AnimIn";

		/// <summary>
		/// Cached name for the 'AnimOut' method.
		/// </summary>
		public static readonly StringName AnimOut = "AnimOut";

		/// <summary>
		/// Cached name for the 'PickRelic' method.
		/// </summary>
		public static readonly StringName PickRelic = "PickRelic";

		/// <summary>
		/// Cached name for the 'RefreshVotes' method.
		/// </summary>
		public static readonly StringName RefreshVotes = "RefreshVotes";
	}

	/// <summary>
	/// Cached StringNames for the properties and fields contained in this class, for fast lookup.
	/// </summary>
	public new class PropertyName : Control.PropertyName
	{
		/// <summary>
		/// Cached name for the 'DefaultFocusedControl' property.
		/// </summary>
		public static readonly StringName DefaultFocusedControl = "DefaultFocusedControl";

		/// <summary>
		/// Cached name for the '_relicContainer' field.
		/// </summary>
		public static readonly StringName _relicContainer = "_relicContainer";

		/// <summary>
		/// Cached name for the '_fightBackstop' field.
		/// </summary>
		public static readonly StringName _fightBackstop = "_fightBackstop";

		/// <summary>
		/// Cached name for the '_fightLabel' field.
		/// </summary>
		public static readonly StringName _fightLabel = "_fightLabel";

		/// <summary>
		/// Cached name for the '_emptyVfxTween' field.
		/// </summary>
		public static readonly StringName _emptyVfxTween = "_emptyVfxTween";

		/// <summary>
		/// Cached name for the '_hands' field.
		/// </summary>
		public static readonly StringName _hands = "_hands";

		/// <summary>
		/// Cached name for the '_openedTicks' field.
		/// </summary>
		public static readonly StringName _openedTicks = "_openedTicks";

		/// <summary>
		/// Cached name for the '_isEmptyChest' field.
		/// </summary>
		public static readonly StringName _isEmptyChest = "_isEmptyChest";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : Control.SignalName
	{
	}

	private const int _maxRelics = 6;

	private const ulong _noSelectionTimeMsec = 200uL;

	private Control _relicContainer;

	private Control _fightBackstop;

	private MegaLabel _fightLabel;

	private Tween? _emptyVfxTween;

	private NHandImageCollection _hands;

	private CancellationTokenSource _cts = new CancellationTokenSource();

	private readonly List<Control> _holderLayouts = new List<Control>();

	private List<NTreasureRoomRelicHolder> _holdersInUse = new List<NTreasureRoomRelicHolder>();

	private readonly TaskCompletionSource _relicPickingCompleteTaskCompletionSource = new TaskCompletionSource();

	private ulong _openedTicks;

	private IRunState _runState;

	private bool _isEmptyChest;

	public IReadOnlyList<NTreasureRoomRelicHolder> HoldersInUse => _holdersInUse;

	public Control? DefaultFocusedControl
	{
		get
		{
			if (_holdersInUse.Count <= 0)
			{
				return null;
			}
			return _holdersInUse[_runState.GetPlayerSlotIndex(LocalContext.GetMe(_runState.Players))];
		}
	}

	public override void _Ready()
	{
		_fightBackstop = GetNode<Control>("%FightBackstop");
		_hands = GetNode<NHandImageCollection>("%HandsContainer");
		_relicContainer = GetNode<Control>("Container");
		_holderLayouts.AddRange(_relicContainer.GetChildren().OfType<Control>().ToList());
		Control fightBackstop = _fightBackstop;
		Color modulate = _fightBackstop.Modulate;
		modulate.A = 0f;
		fightBackstop.Modulate = modulate;
		_fightBackstop.Visible = false;
		_fightLabel = GetNode<MegaLabel>("%FightLabel");
		_fightLabel.Text = new LocString("gameplay_ui", "TREASURE_FIGHT_TEXT").GetFormattedText();
		RunManager.Instance.TreasureRoomRelicSynchronizer.VotesChanged += RefreshVotes;
		RunManager.Instance.TreasureRoomRelicSynchronizer.RelicsAwarded += OnRelicsAwarded;
	}

	public override void _EnterTree()
	{
		_cts = new CancellationTokenSource();
	}

	public override void _ExitTree()
	{
		_cts.Cancel();
		RunManager.Instance.TreasureRoomRelicSynchronizer.VotesChanged -= RefreshVotes;
		RunManager.Instance.TreasureRoomRelicSynchronizer.RelicsAwarded -= OnRelicsAwarded;
	}

	public void Initialize(IRunState runState)
	{
		_runState = runState;
		_hands.Initialize(runState);
	}

	/// <summary>
	/// Initialize the relic display.
	/// This can't get called in _Ready because RelicPickingSynchronizer.BeginRelicPicking has not been called by then.
	/// </summary>
	public void InitializeRelics()
	{
		IReadOnlyList<RelicModel> currentRelics = RunManager.Instance.TreasureRoomRelicSynchronizer.CurrentRelics;
		if (currentRelics == null || currentRelics.Count == 0)
		{
			_isEmptyChest = true;
			foreach (Control holderLayout in _holderLayouts)
			{
				holderLayout.Visible = false;
			}
			SpawnEmptyChestVfx();
			return;
		}
		for (int i = 0; i < _holderLayouts.Count; i++)
		{
			if (i == currentRelics.Count - 1)
			{
				_holderLayouts[i].Visible = true;
				_holdersInUse = _holderLayouts[i].GetChildren().OfType<NTreasureRoomRelicHolder>().ToList();
			}
			else
			{
				_holderLayouts[i].Visible = false;
			}
		}
		for (int j = 0; j < _holdersInUse.Count; j++)
		{
			NTreasureRoomRelicHolder nTreasureRoomRelicHolder = _holdersInUse[j];
			nTreasureRoomRelicHolder.Initialize(currentRelics[j], _runState);
			nTreasureRoomRelicHolder.Index = j;
			nTreasureRoomRelicHolder.Connect(NClickableControl.SignalName.Released, Callable.From<NTreasureRoomRelicHolder>(PickRelic));
			nTreasureRoomRelicHolder.VoteContainer.RefreshPlayerVotes();
		}
		RefreshNavigation();
	}

	private void RefreshNavigation()
	{
		for (int i = 0; i < _holdersInUse.Count; i++)
		{
			_holdersInUse[i].SetFocusMode(FocusModeEnum.All);
			_holdersInUse[i].FocusNeighborTop = _holdersInUse[i].GetPath();
			_holdersInUse[i].FocusNeighborBottom = _holdersInUse[i].GetPath();
			NTreasureRoomRelicHolder nTreasureRoomRelicHolder = _holdersInUse[i];
			NodePath path;
			if (i <= 0)
			{
				List<NTreasureRoomRelicHolder> holdersInUse = _holdersInUse;
				path = holdersInUse[holdersInUse.Count - 1].GetPath();
			}
			else
			{
				path = _holdersInUse[i - 1].GetPath();
			}
			nTreasureRoomRelicHolder.FocusNeighborLeft = path;
			_holdersInUse[i].FocusNeighborRight = ((i < _holdersInUse.Count - 1) ? _holdersInUse[i + 1].GetPath() : _holdersInUse[0].GetPath());
		}
	}

	private void SpawnEmptyChestVfx()
	{
		MegaLabel node = GetNode<MegaLabel>("%EmptyLabel");
		node.Text = new LocString("gameplay_ui", "TREASURE_EMPTY").GetFormattedText();
		node.Visible = true;
		_emptyVfxTween = CreateTween().SetParallel();
		_emptyVfxTween.TweenProperty(node, "self_modulate:a", 1f, 0.25);
		_emptyVfxTween.TweenProperty(node, "position:y", node.Position.Y, 1.0).From(node.Position.Y + 64f).SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Spring);
		_emptyVfxTween.Chain().TweenInterval(1.0);
		_emptyVfxTween.Chain();
		_emptyVfxTween.TweenProperty(node, "self_modulate:a", 0f, 1.0);
		GpuParticles2D node2 = GetNode<GpuParticles2D>("%SmokePuffVfx");
		node2.Emitting = true;
	}

	public void SetSelectionEnabled(bool isEnabled)
	{
		if (isEnabled)
		{
			foreach (NTreasureRoomRelicHolder item in _holdersInUse)
			{
				item.Enable();
			}
			return;
		}
		foreach (NTreasureRoomRelicHolder item2 in _holdersInUse)
		{
			item2.Disable();
		}
	}

	/// <summary>
	/// Await this to know when relic picking finishes.
	/// </summary>
	public Task RelicPickingFinished()
	{
		return _relicPickingCompleteTaskCompletionSource.Task;
	}

	/// <summary>
	/// Animates in the relic collection.
	/// </summary>
	public void AnimIn()
	{
		Visible = true;
		Modulate = Colors.Transparent;
		Tween tween = CreateTween().SetParallel();
		tween.TweenProperty(this, "modulate", Colors.White, 0.4);
		if (_isEmptyChest)
		{
			LocalContext.GetMe(_runState)?.Relics.OfType<SilverCrucible>().FirstOrDefault()?.Flash();
			tween.TweenCallback(Callable.From(() =>
			{
				RunManager.Instance.TreasureRoomRelicSynchronizer.CompleteWithNoRelics();
			})).SetDelay(1.0);
			return;
		}
		foreach (NTreasureRoomRelicHolder holder in _holdersInUse)
		{
			holder.MouseFilter = MouseFilterEnum.Ignore;
			float num = ((_holdersInUse.Count == 1) ? 150f : 50f);
			float num2 = 0.2f + 0.2f * Rng.Chaotic.NextFloat();
			holder.Modulate = Colors.Black;
			NTreasureRoomRelicHolder nTreasureRoomRelicHolder = holder;
			Vector2 position = holder.Position;
			position.Y = holder.Position.Y + num;
			nTreasureRoomRelicHolder.Position = position;
			Tween tween2 = CreateTween().SetParallel();
			tween2.TweenProperty(holder, "modulate", Colors.White, 0.2).SetDelay(num2);
			tween2.TweenProperty(holder, "position:y", holder.Position.Y - num, 0.6).SetDelay(num2).SetEase(Tween.EaseType.Out)
				.SetTrans(Tween.TransitionType.Back);
			tween2.TweenCallback(Callable.From(() => holder.MouseFilter = MouseFilterEnum.Stop)).SetDelay((double)num2 + 0.6);
		}
		NRun.Instance.ScreenStateTracker.SetIsInSharedRelicPickingScreen(isInSharedRelicPicking: true);
		_hands.AnimateHandsIn();
	}

	/// <summary>
	/// Animates out the relic collection, after relic picking is done.
	/// </summary>
	public void AnimOut()
	{
		Modulate = Colors.White;
		Tween tween = CreateTween().Parallel();
		tween.TweenProperty(this, "modulate", StsColors.transparentWhite, 0.3);
		tween.TweenCallback(Callable.From(() => Visible = false));
		NRun.Instance.ScreenStateTracker.SetIsInSharedRelicPickingScreen(isInSharedRelicPicking: false);
	}

	private void PickRelic(NTreasureRoomRelicHolder holder)
	{
		if (Time.GetTicksMsec() - _openedTicks > 200)
		{
			RunManager.Instance.TreasureRoomRelicSynchronizer.PickRelicLocally(holder.Index);
		}
	}

	private void OnRelicsAwarded(List<RelicPickingResult> results, bool isFinished)
	{
		TaskHelper.RunSafely(AnimateRelicAwards(results, isFinished));
	}

	private async Task AnimateRelicAwards(List<RelicPickingResult> results, bool isFinished)
	{
		CancellationToken ct = _cts.Token;
		SetSelectionEnabled(isEnabled: false);
		foreach (Player player in _runState.Players)
		{
			TreasureRoomRelicSynchronizer.PlayerVote playerVote = RunManager.Instance.TreasureRoomRelicSynchronizer.GetPlayerVote(player);
			if (playerVote.voteReceived && !playerVote.index.HasValue)
			{
				_hands.GetHand(player.NetId)?.SetSkipped();
			}
		}
		_hands.BeforeRelicsAwarded();
		List<Task> tasksToWait = new List<Task>();
		RelicPickingResultType? relicPickingResultType = null;
		results.Sort((RelicPickingResult r1, RelicPickingResult r2) => r1.type.CompareTo(r2.type));
		foreach (RelicPickingResult result in results)
		{
			NTreasureRoomRelicHolder holder = _holdersInUse[result.relicIndex];
			holder.AnimateAwayVotes();
			if (relicPickingResultType.HasValue && result.type != relicPickingResultType && result.type != RelicPickingResultType.Skipped)
			{
				await Cmd.Wait(0.5f, ct);
			}
			if (result.type == RelicPickingResultType.FoughtOver)
			{
				Node parent = _fightBackstop.GetParent();
				holder.Reparent(parent);
				parent.MoveChildSafely(holder, _fightBackstop.GetIndex() + 1);
				_fightBackstop.Visible = true;
				Tween tween = CreateTween();
				tween.TweenProperty(holder, "global_position", (_fightBackstop.Size - holder.Size) * 0.5f, 0.25).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.In);
				tween.TweenProperty(_fightBackstop, "modulate:a", 1f, 0.25);
				_hands.BeforeFightStarted(result.fight.playersInvolved);
				if (await tween.AwaitFinished(this, ct) != TweenOutcome.Finished)
				{
					return;
				}
				await Cmd.Wait(1f, ct);
				await _hands.DoFight(result, holder);
				if (HasBeenRemoved())
				{
					return;
				}
				tween = CreateTween();
				tween.TweenProperty(_fightBackstop, "modulate:a", 0f, 0.25);
				if (await tween.AwaitFinished(this, ct) != TweenOutcome.Finished)
				{
					return;
				}
				_fightBackstop.Visible = false;
			}
			else if (result.type == RelicPickingResultType.Skipped)
			{
				if (_runState.Players.Count > 1)
				{
					Tween tween2 = CreateTween().Parallel();
					tween2.TweenProperty(holder, "modulate", StsColors.transparentWhite, 0.3);
					tasksToWait.Add(tween2.AwaitFinished(this));
				}
			}
			else if (result.type != RelicPickingResultType.Skipped)
			{
				NHandImage hand = _hands.GetHand(result.player.NetId);
				if (hand != null)
				{
					tasksToWait.Add(TaskHelper.RunSafely(hand.GrabRelic(holder)));
					await Cmd.Wait(0.25f, ct);
				}
			}
			relicPickingResultType = result.type;
		}
		await Task.WhenAll(tasksToWait);
		if (tasksToWait.Count == 0 && _runState.Players.Count > 1)
		{
			await Cmd.Wait(0.7f, ct);
		}
		if (HasBeenRemoved())
		{
			return;
		}
		foreach (RelicPickingResult result2 in results)
		{
			NTreasureRoomRelicHolder nTreasureRoomRelicHolder = _holdersInUse[result2.relicIndex];
			RelicModel relic = result2.relic.ToMutable();
			if (result2.type != RelicPickingResultType.Skipped)
			{
				TaskHelper.RunSafely(RelicCmd.Obtain(relic, result2.player));
				if (LocalContext.IsMe(result2.player))
				{
					NRun.Instance.GlobalUi.RelicInventory.AnimateRelic(relic, nTreasureRoomRelicHolder.GlobalPosition, nTreasureRoomRelicHolder.Scale);
				}
				if (_runState.Players.Count == 1)
				{
					nTreasureRoomRelicHolder.Visible = false;
				}
			}
			foreach (Player player2 in _runState.Players)
			{
				if (player2 != result2.player)
				{
					player2.RelicGrabBag.MoveToFallback(result2.relic);
				}
			}
		}
		if (isFinished)
		{
			_relicPickingCompleteTaskCompletionSource.SetResult();
			return;
		}
		foreach (RelicPickingResult result3 in results)
		{
			_holdersInUse.RemoveAt(result3.relicIndex);
		}
		for (int num = 0; num < _holdersInUse.Count; num++)
		{
			_holdersInUse[num].Index = num;
		}
		RefreshNavigation();
		DefaultFocusedControl?.TryGrabFocus();
		SetSelectionEnabled(isEnabled: true);
		bool HasBeenRemoved()
		{
			if (this.IsValid())
			{
				return !IsInsideTree();
			}
			return true;
		}
	}

	private void RefreshVotes()
	{
		foreach (NTreasureRoomRelicHolder item in _holdersInUse)
		{
			item.VoteContainer.RefreshPlayerVotes();
		}
	}

	/// <summary>
	/// Get the method information for all the methods declared in this class.
	/// This method is used by Godot to register the available methods in the editor.
	/// Do not call this method.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(11);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.InitializeRelics, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.RefreshNavigation, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.SpawnEmptyChestVfx, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.SetSelectionEnabled, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Bool, "isEnabled", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.AnimIn, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.AnimOut, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.PickRelic, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "holder", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.RefreshVotes, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		return list;
	}

	/// <inheritdoc />
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
		if (method == MethodName.InitializeRelics && args.Count == 0)
		{
			InitializeRelics();
			ret = default;
			return true;
		}
		if (method == MethodName.RefreshNavigation && args.Count == 0)
		{
			RefreshNavigation();
			ret = default;
			return true;
		}
		if (method == MethodName.SpawnEmptyChestVfx && args.Count == 0)
		{
			SpawnEmptyChestVfx();
			ret = default;
			return true;
		}
		if (method == MethodName.SetSelectionEnabled && args.Count == 1)
		{
			SetSelectionEnabled(VariantUtils.ConvertTo<bool>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.AnimIn && args.Count == 0)
		{
			AnimIn();
			ret = default;
			return true;
		}
		if (method == MethodName.AnimOut && args.Count == 0)
		{
			AnimOut();
			ret = default;
			return true;
		}
		if (method == MethodName.PickRelic && args.Count == 1)
		{
			PickRelic(VariantUtils.ConvertTo<NTreasureRoomRelicHolder>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.RefreshVotes && args.Count == 0)
		{
			RefreshVotes();
			ret = default;
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	/// <inheritdoc />
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
		if (method == MethodName.InitializeRelics)
		{
			return true;
		}
		if (method == MethodName.RefreshNavigation)
		{
			return true;
		}
		if (method == MethodName.SpawnEmptyChestVfx)
		{
			return true;
		}
		if (method == MethodName.SetSelectionEnabled)
		{
			return true;
		}
		if (method == MethodName.AnimIn)
		{
			return true;
		}
		if (method == MethodName.AnimOut)
		{
			return true;
		}
		if (method == MethodName.PickRelic)
		{
			return true;
		}
		if (method == MethodName.RefreshVotes)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._relicContainer)
		{
			_relicContainer = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._fightBackstop)
		{
			_fightBackstop = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._fightLabel)
		{
			_fightLabel = VariantUtils.ConvertTo<MegaLabel>(in value);
			return true;
		}
		if (name == PropertyName._emptyVfxTween)
		{
			_emptyVfxTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		if (name == PropertyName._hands)
		{
			_hands = VariantUtils.ConvertTo<NHandImageCollection>(in value);
			return true;
		}
		if (name == PropertyName._openedTicks)
		{
			_openedTicks = VariantUtils.ConvertTo<ulong>(in value);
			return true;
		}
		if (name == PropertyName._isEmptyChest)
		{
			_isEmptyChest = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.DefaultFocusedControl)
		{
			value = VariantUtils.CreateFrom<Control>(DefaultFocusedControl);
			return true;
		}
		if (name == PropertyName._relicContainer)
		{
			value = VariantUtils.CreateFrom(in _relicContainer);
			return true;
		}
		if (name == PropertyName._fightBackstop)
		{
			value = VariantUtils.CreateFrom(in _fightBackstop);
			return true;
		}
		if (name == PropertyName._fightLabel)
		{
			value = VariantUtils.CreateFrom(in _fightLabel);
			return true;
		}
		if (name == PropertyName._emptyVfxTween)
		{
			value = VariantUtils.CreateFrom(in _emptyVfxTween);
			return true;
		}
		if (name == PropertyName._hands)
		{
			value = VariantUtils.CreateFrom(in _hands);
			return true;
		}
		if (name == PropertyName._openedTicks)
		{
			value = VariantUtils.CreateFrom(in _openedTicks);
			return true;
		}
		if (name == PropertyName._isEmptyChest)
		{
			value = VariantUtils.CreateFrom(in _isEmptyChest);
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
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._relicContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._fightBackstop, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._fightLabel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._emptyVfxTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._hands, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Int, PropertyName._openedTicks, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName._isEmptyChest, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.DefaultFocusedControl, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._relicContainer, Variant.From(in _relicContainer));
		info.AddProperty(PropertyName._fightBackstop, Variant.From(in _fightBackstop));
		info.AddProperty(PropertyName._fightLabel, Variant.From(in _fightLabel));
		info.AddProperty(PropertyName._emptyVfxTween, Variant.From(in _emptyVfxTween));
		info.AddProperty(PropertyName._hands, Variant.From(in _hands));
		info.AddProperty(PropertyName._openedTicks, Variant.From(in _openedTicks));
		info.AddProperty(PropertyName._isEmptyChest, Variant.From(in _isEmptyChest));
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._relicContainer, out var value))
		{
			_relicContainer = value.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._fightBackstop, out var value2))
		{
			_fightBackstop = value2.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._fightLabel, out var value3))
		{
			_fightLabel = value3.As<MegaLabel>();
		}
		if (info.TryGetProperty(PropertyName._emptyVfxTween, out var value4))
		{
			_emptyVfxTween = value4.As<Tween>();
		}
		if (info.TryGetProperty(PropertyName._hands, out var value5))
		{
			_hands = value5.As<NHandImageCollection>();
		}
		if (info.TryGetProperty(PropertyName._openedTicks, out var value6))
		{
			_openedTicks = value6.As<ulong>();
		}
		if (info.TryGetProperty(PropertyName._isEmptyChest, out var value7))
		{
			_isEmptyChest = value7.As<bool>();
		}
	}
}
