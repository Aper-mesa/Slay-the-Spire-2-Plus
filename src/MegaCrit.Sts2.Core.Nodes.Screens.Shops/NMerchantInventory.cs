using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Shops;

/// <summary>
/// Manages the shop items for a shop.
/// </summary>
[ScriptPath("res://src/Core/Nodes/Screens/Shops/NMerchantInventory.cs")]
public class NMerchantInventory : Control, IScreenContext
{
	[Signal]
	public delegate void InventoryClosedEventHandler();

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
		/// Cached name for the 'Open' method.
		/// </summary>
		public static readonly StringName Open = "Open";

		/// <summary>
		/// Cached name for the 'SubscribeToEntries' method.
		/// </summary>
		public static readonly StringName SubscribeToEntries = "SubscribeToEntries";

		/// <summary>
		/// Cached name for the 'Close' method.
		/// </summary>
		public static readonly StringName Close = "Close";

		/// <summary>
		/// Cached name for the 'UpdateBlitzSelectionText' method.
		/// </summary>
		public static readonly StringName UpdateBlitzSelectionText = "UpdateBlitzSelectionText";

		/// <summary>
		/// Cached name for the 'OnCardRemovalUsed' method.
		/// </summary>
		public static readonly StringName OnCardRemovalUsed = "OnCardRemovalUsed";

		/// <summary>
		/// Cached name for the 'UpdateNavigation' method.
		/// </summary>
		public static readonly StringName UpdateNavigation = "UpdateNavigation";

		/// <summary>
		/// Cached name for the 'UpdateBlitzNavigation' method.
		/// </summary>
		public static readonly StringName UpdateBlitzNavigation = "UpdateBlitzNavigation";

		/// <summary>
		/// Cached name for the 'UpdateHorizontalNavigation' method.
		/// </summary>
		public static readonly StringName UpdateHorizontalNavigation = "UpdateHorizontalNavigation";

		/// <summary>
		/// Cached name for the 'UpdateVerticalNavigation' method.
		/// </summary>
		public static readonly StringName UpdateVerticalNavigation = "UpdateVerticalNavigation";

		/// <summary>
		/// Cached name for the 'BlockInput' method.
		/// </summary>
		public static readonly StringName BlockInput = "BlockInput";

		/// <summary>
		/// Cached name for the 'UnblockInput' method.
		/// </summary>
		public static readonly StringName UnblockInput = "UnblockInput";

		/// <summary>
		/// Cached name for the 'OnActiveScreenUpdated' method.
		/// </summary>
		public static readonly StringName OnActiveScreenUpdated = "OnActiveScreenUpdated";
	}

	/// <summary>
	/// Cached StringNames for the properties and fields contained in this class, for fast lookup.
	/// </summary>
	public new class PropertyName : Control.PropertyName
	{
		/// <summary>
		/// Cached name for the 'IsOpen' property.
		/// </summary>
		public static readonly StringName IsOpen = "IsOpen";

		/// <summary>
		/// Cached name for the 'MerchantHand' property.
		/// </summary>
		public static readonly StringName MerchantHand = "MerchantHand";

		/// <summary>
		/// Cached name for the 'DefaultFocusedControl' property.
		/// </summary>
		public static readonly StringName DefaultFocusedControl = "DefaultFocusedControl";

		/// <summary>
		/// Cached name for the '_characterCardContainer' field.
		/// </summary>
		public static readonly StringName _characterCardContainer = "_characterCardContainer";

		/// <summary>
		/// Cached name for the '_colorlessCardContainer' field.
		/// </summary>
		public static readonly StringName _colorlessCardContainer = "_colorlessCardContainer";

		/// <summary>
		/// Cached name for the '_relicContainer' field.
		/// </summary>
		public static readonly StringName _relicContainer = "_relicContainer";

		/// <summary>
		/// Cached name for the '_potionContainer' field.
		/// </summary>
		public static readonly StringName _potionContainer = "_potionContainer";

		/// <summary>
		/// Cached name for the '_blitzContainer' field.
		/// </summary>
		public static readonly StringName _blitzContainer = "_blitzContainer";

		/// <summary>
		/// Cached name for the '_blitzSelectionText' field.
		/// </summary>
		public static readonly StringName _blitzSelectionText = "_blitzSelectionText";

		/// <summary>
		/// Cached name for the '_cardRemovalNode' field.
		/// </summary>
		public static readonly StringName _cardRemovalNode = "_cardRemovalNode";

		/// <summary>
		/// Cached name for the '_backButton' field.
		/// </summary>
		public static readonly StringName _backButton = "_backButton";

		/// <summary>
		/// Cached name for the '_merchantDialogue' field.
		/// </summary>
		public static readonly StringName _merchantDialogue = "_merchantDialogue";

		/// <summary>
		/// Cached name for the '_inventoryTween' field.
		/// </summary>
		public static readonly StringName _inventoryTween = "_inventoryTween";

		/// <summary>
		/// Cached name for the '_slotsContainer' field.
		/// </summary>
		public static readonly StringName _slotsContainer = "_slotsContainer";

		/// <summary>
		/// Cached name for the '_backstop' field.
		/// </summary>
		public static readonly StringName _backstop = "_backstop";

		/// <summary>
		/// Cached name for the '_inputBlocker' field.
		/// </summary>
		public static readonly StringName _inputBlocker = "_inputBlocker";

		/// <summary>
		/// Cached name for the '_isInputBlocked' field.
		/// </summary>
		public static readonly StringName _isInputBlocked = "_isInputBlocked";

		/// <summary>
		/// Cached name for the '_lastSlot' field.
		/// </summary>
		public static readonly StringName _lastSlot = "_lastSlot";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : Control.SignalName
	{
		/// <summary>
		/// Cached name for the 'InventoryClosed' signal.
		/// </summary>
		public static readonly StringName InventoryClosed = "InventoryClosed";
	}

	private const float _openPosition = 80f;

	private const float _closedPosition = -1000f;

	public const int blitzColumns = 3;

	private Control? _characterCardContainer;

	private Control? _colorlessCardContainer;

	protected Control? _relicContainer;

	private Control? _potionContainer;

	private Control? _blitzContainer;

	private MegaRichTextLabel? _blitzSelectionText;

	private NMerchantCardRemoval? _cardRemovalNode;

	private NBackButton _backButton;

	private NMerchantDialogue _merchantDialogue;

	private Tween? _inventoryTween;

	private Control _slotsContainer;

	private ColorRect _backstop;

	private Control _inputBlocker;

	private bool _isInputBlocked;

	private NMerchantSlot? _lastSlot;

	private InventoryClosedEventHandler backing_InventoryClosed;

	public MerchantInventory? Inventory { get; private set; }

	public bool IsOpen { get; private set; }

	public NMerchantHand MerchantHand { get; private set; }

	public Control? DefaultFocusedControl
	{
		get
		{
			if (_isInputBlocked)
			{
				return _inputBlocker;
			}
			NMerchantSlot lastSlot = _lastSlot;
			if (lastSlot != null)
			{
				MerchantEntry entry = lastSlot.Entry;
				if (entry != null && entry.IsStocked)
				{
					return _lastSlot;
				}
			}
			return GetAllSlots().FirstOrDefault((NMerchantSlot s) => s.Entry.IsStocked);
		}
	}

	/// <inheritdoc cref="T:MegaCrit.Sts2.Core.Nodes.Screens.Shops.NMerchantInventory.InventoryClosedEventHandler" />
	public event InventoryClosedEventHandler InventoryClosed
	{
		add
		{
			backing_InventoryClosed = (InventoryClosedEventHandler)Delegate.Combine(backing_InventoryClosed, value);
		}
		remove
		{
			backing_InventoryClosed = (InventoryClosedEventHandler)Delegate.Remove(backing_InventoryClosed, value);
		}
	}

	public override void _Ready()
	{
		_merchantDialogue = GetNode<NMerchantDialogue>("%Dialogue");
		_merchantDialogue.Modulate = Colors.Transparent;
		_slotsContainer = GetNode<Control>("%SlotsContainer");
		_slotsContainer.Position = new Vector2(_slotsContainer.Position.X, -1000f);
		_backstop = GetNode<ColorRect>("Backstop");
		MerchantHand = GetNode<NMerchantHand>("%MerchantHand");
		_characterCardContainer = GetNodeOrNull<Control>("%CharacterCards");
		_colorlessCardContainer = GetNodeOrNull<Control>("%ColorlessCards");
		_relicContainer = GetNodeOrNull<Control>("%Relics");
		_potionContainer = GetNodeOrNull<Control>("%Potions");
		_blitzContainer = GetNodeOrNull<Control>("%BlitzSlots");
		_blitzSelectionText = GetNodeOrNull<MegaRichTextLabel>("%BlitzSelectionText");
		_cardRemovalNode = GetNodeOrNull<NMerchantCardRemoval>("%MerchantCardRemoval");
		_backButton = GetNode<NBackButton>("%BackButton");
		_backButton.Connect(NClickableControl.SignalName.Released, Callable.From((NButton _) =>
		{
			Close();
		}));
		_backButton.Disable();
		_inputBlocker = GetNode<Control>("%InputBlocker");
		NGame.Instance.SetScreenShakeTarget(this);
	}

	public override void _EnterTree()
	{
		ActiveScreenContext.Instance.Updated += OnActiveScreenUpdated;
		SubscribeToEntries();
	}

	public override void _ExitTree()
	{
		ActiveScreenContext.Instance.Updated -= OnActiveScreenUpdated;
		if (Inventory == null)
		{
			return;
		}
		foreach (MerchantEntry allEntry in Inventory.AllEntries)
		{
			allEntry.PurchaseCompleted -= OnPurchaseCompleted;
			allEntry.PurchaseFailed -= _merchantDialogue.ShowForPurchaseAttempt;
		}
	}

	public void Initialize(MerchantInventory inventory, MerchantDialogueSet dialogue)
	{
		if (Inventory != null)
		{
			throw new InvalidOperationException("Merchant inventory already populated.");
		}
		Inventory = inventory;
		_characterCardContainer?.SetVisible(Inventory.CharacterCardEntries.Any());
		_colorlessCardContainer?.SetVisible(Inventory.ColorlessCardEntries.Any());
		_potionContainer?.SetVisible(Inventory.PotionEntries.Any());
		_relicContainer?.SetVisible(Inventory.RelicEntries.Any());
		_cardRemovalNode?.SetVisible(Inventory.CardRemovalEntry != null);
		_blitzContainer?.SetVisible(Inventory.BlitzEntries.Any());
		_blitzSelectionText?.SetVisible(Inventory.BlitzEntries.Any());
		for (int i = 0; i < Inventory.CharacterCardEntries.Count; i++)
		{
			NMerchantCard child = _characterCardContainer.GetChild<NMerchantCard>(i);
			child.Initialize(this);
			child.FillSlot(Inventory.CharacterCardEntries[i]);
		}
		for (int j = 0; j < Inventory.ColorlessCardEntries.Count; j++)
		{
			NMerchantCard child2 = _colorlessCardContainer.GetChild<NMerchantCard>(j);
			child2.Initialize(this);
			child2.FillSlot(Inventory.ColorlessCardEntries[j]);
		}
		for (int k = 0; k < Inventory.RelicEntries.Count; k++)
		{
			NMerchantRelic child3 = _relicContainer.GetChild<NMerchantRelic>(k);
			child3.Initialize(this);
			child3.FillSlot(Inventory.RelicEntries[k]);
		}
		for (int l = 0; l < Inventory.PotionEntries.Count; l++)
		{
			NMerchantPotion child4 = _potionContainer.GetChild<NMerchantPotion>(l);
			child4.Initialize(this);
			child4.FillSlot(Inventory.PotionEntries[l]);
		}
		if (Inventory.CardRemovalEntry != null)
		{
			_cardRemovalNode.Initialize(this);
			_cardRemovalNode.FillSlot(Inventory.CardRemovalEntry);
		}
		for (int m = 0; m < Inventory.BlitzEntries.Count; m++)
		{
			NBlitzMerchantSlot child5 = _blitzContainer.GetChild<NBlitzMerchantSlot>(m);
			child5.Initialize(this);
			child5.FillSlot(Inventory.BlitzEntries[m]);
		}
		UpdateBlitzSelectionText();
		SubscribeToEntries();
		UpdateNavigation();
		foreach (NMerchantSlot slot in GetAllSlots())
		{
			slot.Connect(Control.SignalName.FocusEntered, Callable.From(() =>
			{
				_lastSlot = slot;
			}));
		}
		_merchantDialogue.Initialize(dialogue);
	}

	public void Open()
	{
		if (!SaveManager.Instance.SeenFtue("merchant_ftue"))
		{
			SaveManager.Instance.MarkFtueAsComplete("merchant_ftue");
		}
		TaskHelper.RunSafely(DoOpenAnimation());
		MouseFilter = MouseFilterEnum.Stop;
		if (!_isInputBlocked)
		{
			_backButton.Enable();
		}
		foreach (NMerchantCard cardSlot in GetCardSlots())
		{
			cardSlot.OnInventoryOpened();
		}
		IsOpen = true;
		ActiveScreenContext.Instance.Update();
		TaskHelper.RunSafely(ShowOpenInventory());
	}

	public async Task ShowOpenInventory()
	{
		await Cmd.Wait(0.75f);
		SfxCmd.Play("event:/sfx/npcs/merchant/merchant_welcome");
		MerchantInventory inventory = Inventory;
		if (inventory != null)
		{
			IReadOnlyList<BlitzMerchantEntry> blitzEntries = inventory.BlitzEntries;
			if (blitzEntries != null && blitzEntries.Count > 0)
			{
				if (Inventory.BlitzChoicesRemaining <= 0)
				{
					_merchantDialogue.ShowForBlitzFinished();
				}
				return;
			}
		}
		_merchantDialogue.ShowOnInventoryOpen();
	}

	private void SubscribeToEntries()
	{
		if (!IsNodeReady() || Inventory == null)
		{
			return;
		}
		foreach (MerchantEntry allEntry in Inventory.AllEntries)
		{
			allEntry.PurchaseCompleted += OnPurchaseCompleted;
			allEntry.PurchaseFailed += _merchantDialogue.ShowForPurchaseAttempt;
		}
	}

	private async Task DoOpenAnimation()
	{
		_inventoryTween?.Kill();
		_inventoryTween = CreateTween().SetParallel();
		_inventoryTween.TweenProperty(_backstop, "modulate:a", 0.8f, 1.0).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine)
			.FromCurrent();
		_inventoryTween.TweenProperty(_slotsContainer, "position:y", 80f, 0.699999988079071).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quint)
			.FromCurrent();
		await _inventoryTween.AwaitFinished(this);
	}

	public void Close()
	{
		MerchantHand.StopPointing(0f);
		_inventoryTween?.Kill();
		_inventoryTween = CreateTween().SetParallel();
		_inventoryTween.TweenProperty(_backstop, "modulate:a", 0f, 0.800000011920929).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine)
			.FromCurrent();
		_inventoryTween.TweenProperty(_slotsContainer, "position:y", -1000f, 0.5).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic)
			.FromCurrent();
		MouseFilter = MouseFilterEnum.Ignore;
		_backButton.Disable();
		_lastSlot = null;
		IsOpen = false;
		ActiveScreenContext.Instance.Update();
		EmitSignalInventoryClosed();
	}

	private void OnPurchaseCompleted(PurchaseStatus status, MerchantEntry entry)
	{
		UpdateNavigation();
		NMerchantSlot lastSlot = GetAllSlots().FirstOrDefault((NMerchantSlot s) => s.Entry == entry);
		if (lastSlot != null && !_isInputBlocked)
		{
			NMerchantSlot nMerchantSlot = (from s in GetAllSlots()
				where s.Visible && s.Entry.IsStocked && s != lastSlot
				select s).MinBy((NMerchantSlot s) => (s.GlobalPosition - lastSlot.GlobalPosition).Length());
			if (!ActiveScreenContext.Instance.IsCurrent(this))
			{
				_lastSlot = nMerchantSlot;
			}
			else if (nMerchantSlot != null)
			{
				nMerchantSlot.TryGrabFocus();
			}
			else
			{
				ActiveScreenContext.Instance.FocusOnDefaultControl();
			}
		}
		SfxCmd.Play("event:/sfx/npcs/merchant/merchant_thank_yous");
		MerchantInventory inventory = Inventory;
		if (inventory != null)
		{
			IReadOnlyList<BlitzMerchantEntry> blitzEntries = inventory.BlitzEntries;
			if (blitzEntries != null && blitzEntries.Count > 0)
			{
				if (Inventory.BlitzChoicesRemaining <= 0)
				{
					_merchantDialogue.ShowForBlitzFinished();
				}
				UpdateBlitzSelectionText();
				return;
			}
		}
		_merchantDialogue.ShowForPurchaseAttempt(status);
	}

	private void UpdateBlitzSelectionText()
	{
		if (_blitzSelectionText == null)
		{
			return;
		}
		MerchantInventory inventory = Inventory;
		if (inventory == null)
		{
			return;
		}
		IReadOnlyList<BlitzMerchantEntry> blitzEntries = inventory.BlitzEntries;
		if (blitzEntries != null && blitzEntries.Count > 0)
		{
			LocString locString;
			if (Inventory.BlitzChoicesRemaining > 0)
			{
				locString = new LocString("merchant_room", "MERCHANT.blitzSelection.remaining");
				locString.Add("Amount", Inventory.BlitzChoicesRemaining);
			}
			else
			{
				locString = new LocString("merchant_room", "MERCHANT.blitzSelection.none");
			}
			_blitzSelectionText.SetTextAutoSize(locString.GetFormattedText());
		}
	}

	public void OnCardRemovalUsed()
	{
		_cardRemovalNode.OnCardRemovalUsed();
	}

	/// <summary>
	/// Returns the slots that hold an entry. Containers with no entries for this inventory are hidden in
	/// <see cref="M:MegaCrit.Sts2.Core.Nodes.Screens.Shops.NMerchantInventory.Initialize(MegaCrit.Sts2.Core.Entities.Merchant.MerchantInventory,MegaCrit.Sts2.Core.Entities.Merchant.MerchantDialogueSet)" /> and their slots are never filled, so they are skipped here.
	/// </summary>
	public IEnumerable<NMerchantSlot> GetAllSlots()
	{
		List<NMerchantSlot> list = new List<NMerchantSlot>();
		list.AddRange(GetCardSlots());
		Control relicContainer = _relicContainer;
		if (relicContainer != null && relicContainer.Visible)
		{
			list.AddRange(_relicContainer.GetChildren().OfType<NMerchantRelic>());
		}
		relicContainer = _potionContainer;
		if (relicContainer != null && relicContainer.Visible)
		{
			list.AddRange(_potionContainer.GetChildren().OfType<NMerchantPotion>());
		}
		relicContainer = _blitzContainer;
		if (relicContainer != null && relicContainer.Visible)
		{
			list.AddRange(_blitzContainer.GetChildren().OfType<NBlitzMerchantSlot>());
		}
		NMerchantCardRemoval cardRemovalNode = _cardRemovalNode;
		if (cardRemovalNode != null && cardRemovalNode.Visible)
		{
			list.Add(_cardRemovalNode);
		}
		return list;
	}

	private IEnumerable<NMerchantCard> GetCardSlots()
	{
		IEnumerable<Node>[] array = new IEnumerable<Node>[2];
		Control characterCardContainer = _characterCardContainer;
		array[0] = ((characterCardContainer != null && characterCardContainer.Visible) ? _characterCardContainer.GetChildren() : new Array<Node>());
		characterCardContainer = _colorlessCardContainer;
		array[1] = ((characterCardContainer != null && characterCardContainer.Visible) ? _colorlessCardContainer.GetChildren() : new Array<Node>());
		return array.SelectMany((IEnumerable<Node> n) => n).OfType<NMerchantCard>();
	}

	protected virtual void UpdateNavigation()
	{
		Control blitzContainer = _blitzContainer;
		if (blitzContainer != null && blitzContainer.Visible)
		{
			UpdateBlitzNavigation();
			return;
		}
		UpdateHorizontalNavigation();
		UpdateVerticalNavigation();
	}

	private void UpdateBlitzNavigation()
	{
		List<NMerchantSlot> list = _blitzContainer?.GetChildren().OfType<NMerchantSlot>().Where((NMerchantSlot c) => c.Visible)
			.ToList() ?? new List<NMerchantSlot>();
		if (list.Count == 0)
		{
			return;
		}
		List<List<NMerchantSlot>> list2 = (from r in list.Chunk(3)
			select r.ToList()).ToList();
		for (int num = 0; num < list2.Count; num++)
		{
			List<NMerchantSlot> list3 = list2[num];
			SetWrappedHorizontalNeighbors(list3);
			for (int num2 = 0; num2 < list3.Count; num2++)
			{
				NMerchantSlot nMerchantSlot = list3[num2];
				nMerchantSlot.FocusNeighborTop = GetStockedSlotInColumn(list2, num, num2, -1).GetPath();
				nMerchantSlot.FocusNeighborBottom = GetStockedSlotInColumn(list2, num, num2, 1).GetPath();
			}
		}
	}

	private static void SetWrappedHorizontalNeighbors(List<NMerchantSlot> row)
	{
		for (int i = 0; i < row.Count; i++)
		{
			row[i].FocusNeighborLeft = GetWrappedStockedSlotInRow(row, i, -1).GetPath();
			row[i].FocusNeighborRight = GetWrappedStockedSlotInRow(row, i, 1).GetPath();
		}
	}

	private static NMerchantSlot GetWrappedStockedSlotInRow(List<NMerchantSlot> row, int idx, int dir)
	{
		for (int i = 1; i < row.Count; i++)
		{
			NMerchantSlot nMerchantSlot = row[Mathf.PosMod(idx + dir * i, row.Count)];
			if (nMerchantSlot.Entry.IsStocked)
			{
				return nMerchantSlot;
			}
		}
		return row[idx];
	}

	private NMerchantSlot GetStockedSlotInColumn(List<List<NMerchantSlot>> rows, int rowIdx, int col, int dir)
	{
		for (int i = rowIdx + dir; i >= 0 && i < rows.Count; i += dir)
		{
			List<NMerchantSlot> row = rows[i];
			if (GetClosestStockedSlot(col, row) is NMerchantSlot result)
			{
				return result;
			}
		}
		return rows[rowIdx][col];
	}

	private void UpdateHorizontalNavigation()
	{
		List<NMerchantSlot> source = _characterCardContainer?.GetChildren().OfType<NMerchantSlot>().Where((NMerchantSlot c) => c.Visible)
			.ToList() ?? new List<NMerchantSlot>();
		List<NMerchantSlot> list = _colorlessCardContainer?.GetChildren().OfType<NMerchantSlot>().Where((NMerchantSlot c) => c.Visible)
			.ToList() ?? new List<NMerchantSlot>();
		List<NMerchantSlot> list2 = _relicContainer?.GetChildren().OfType<NMerchantSlot>().Where((NMerchantSlot c) => c.Visible)
			.ToList() ?? new List<NMerchantSlot>();
		List<NMerchantSlot> list3 = _potionContainer?.GetChildren().OfType<NMerchantSlot>().Where((NMerchantSlot c) => c.Visible)
			.ToList() ?? new List<NMerchantSlot>();
		List<NMerchantSlot> wrappedHorizontalNeighbors = source.ToList();
		IEnumerable<NMerchantSlot> first = list.Concat(list2);
		IEnumerable<NMerchantSlot> second;
		if (_cardRemovalNode == null)
		{
			IEnumerable<NMerchantSlot> enumerable = System.Array.Empty<NMerchantSlot>();
			second = enumerable;
		}
		else
		{
			IEnumerable<NMerchantSlot> enumerable = new _003C_003Ez__ReadOnlySingleElementList<NMerchantSlot>(_cardRemovalNode);
			second = enumerable;
		}
		List<NMerchantSlot> wrappedHorizontalNeighbors2 = first.Concat(second).ToList();
		IEnumerable<NMerchantSlot> first2 = list.Concat(list3);
		IEnumerable<NMerchantSlot> second2;
		if (_cardRemovalNode == null)
		{
			IEnumerable<NMerchantSlot> enumerable = System.Array.Empty<NMerchantSlot>();
			second2 = enumerable;
		}
		else
		{
			IEnumerable<NMerchantSlot> enumerable = new _003C_003Ez__ReadOnlySingleElementList<NMerchantSlot>(_cardRemovalNode);
			second2 = enumerable;
		}
		List<NMerchantSlot> wrappedHorizontalNeighbors3 = first2.Concat(second2).ToList();
		SetWrappedHorizontalNeighbors(wrappedHorizontalNeighbors);
		SetWrappedHorizontalNeighbors(wrappedHorizontalNeighbors3);
		SetWrappedHorizontalNeighbors(wrappedHorizontalNeighbors2);
		NMerchantSlot firstStockedSlot = GetFirstStockedSlot(GetRelicPotionColumnOrder(list2, list3, fromRight: false));
		NMerchantSlot firstStockedSlot2 = GetFirstStockedSlot(GetRelicPotionColumnOrder(list2, list3, fromRight: true));
		if (firstStockedSlot == null || firstStockedSlot2 == null)
		{
			return;
		}
		_cardRemovalNode?.SetFocusNeighbor(Side.Left, firstStockedSlot2.GetPath());
		list.LastOrDefault()?.SetFocusNeighbor(Side.Right, firstStockedSlot.GetPath());
		if (_cardRemovalNode == null)
		{
			list.FirstOrDefault((NMerchantSlot c) => c.Entry.IsStocked)?.SetFocusNeighbor(Side.Left, firstStockedSlot2.GetPath());
		}
	}

	/// <summary>
	/// Orders relics and potions column by column (relic above potion) starting from the left or right edge.
	/// </summary>
	private static IEnumerable<NMerchantSlot?> GetRelicPotionColumnOrder(List<NMerchantSlot> relics, List<NMerchantSlot> potions, bool fromRight)
	{
		int columns = Math.Max(relics.Count, potions.Count);
		for (int i = 0; i < columns; i++)
		{
			int col = (fromRight ? (columns - 1 - i) : i);
			yield return relics.ElementAtOrDefault(col);
			yield return potions.ElementAtOrDefault(col);
		}
	}

	/// <summary>
	/// Returns the first candidate that can take focus, skipping missing, hidden, unfilled, and sold out slots.
	/// </summary>
	private static NMerchantSlot? GetFirstStockedSlot(IEnumerable<NMerchantSlot?> candidates)
	{
		return candidates.FirstOrDefault((NMerchantSlot s) => s != null && s.Visible && (s.Entry?.IsStocked ?? false));
	}

	private void UpdateVerticalNavigation()
	{
		List<NMerchantSlot> source = _characterCardContainer?.GetChildren().OfType<NMerchantSlot>().ToList() ?? new List<NMerchantSlot>();
		List<NMerchantSlot> colorlessCards = _colorlessCardContainer?.GetChildren().OfType<NMerchantSlot>().ToList() ?? new List<NMerchantSlot>();
		List<NMerchantSlot> relics = _relicContainer?.GetChildren().OfType<NMerchantSlot>().ToList() ?? new List<NMerchantSlot>();
		List<NMerchantSlot> potions = _potionContainer?.GetChildren().OfType<NMerchantSlot>().ToList() ?? new List<NMerchantSlot>();
		List<NMerchantSlot> list = source.ToList();
		IEnumerable<NMerchantSlot> first = colorlessCards.Concat(relics);
		IEnumerable<NMerchantSlot> second;
		if (_cardRemovalNode == null)
		{
			IEnumerable<NMerchantSlot> enumerable = System.Array.Empty<NMerchantSlot>();
			second = enumerable;
		}
		else
		{
			IEnumerable<NMerchantSlot> enumerable = new _003C_003Ez__ReadOnlySingleElementList<NMerchantSlot>(_cardRemovalNode);
			second = enumerable;
		}
		List<NMerchantSlot> list2 = first.Concat(second).ToList();
		IEnumerable<NMerchantSlot> first2 = colorlessCards.Concat(potions);
		IEnumerable<NMerchantSlot> second2;
		if (_cardRemovalNode == null)
		{
			IEnumerable<NMerchantSlot> enumerable = System.Array.Empty<NMerchantSlot>();
			second2 = enumerable;
		}
		else
		{
			IEnumerable<NMerchantSlot> enumerable = new _003C_003Ez__ReadOnlySingleElementList<NMerchantSlot>(_cardRemovalNode);
			second2 = enumerable;
		}
		List<NMerchantSlot> list3 = first2.Concat(second2).ToList();
		for (int i = 0; i < list.Count; i++)
		{
			list[i].FocusNeighborTop = list[i].GetPath();
			if (list2.Count > 0)
			{
				Control closestStockedSlot = GetClosestStockedSlot(i, list2);
				if (closestStockedSlot != null)
				{
					list[i].FocusNeighborBottom = closestStockedSlot.GetPath();
					continue;
				}
			}
			Control closestStockedSlot2 = GetClosestStockedSlot(i, list3);
			if (closestStockedSlot2 != null)
			{
				list[i].FocusNeighborBottom = closestStockedSlot2.GetPath();
			}
			else
			{
				list[i].FocusNeighborBottom = list[i].GetPath();
			}
		}
		NMerchantSlot nMerchantSlot = list.ElementAtOrDefault(2);
		if (nMerchantSlot != null)
		{
			NMerchantSlot firstStockedSlot = GetFirstStockedSlot(new _003C_003Ez__ReadOnlyArray<NMerchantSlot>(new NMerchantSlot[5]
			{
				Relic(0),
				Relic(1),
				Potion(0),
				Potion(1),
				Colorless(1)
			}));
			if (firstStockedSlot != null)
			{
				nMerchantSlot.FocusNeighborBottom = firstStockedSlot.GetPath();
			}
		}
		NMerchantSlot nMerchantSlot2 = list.ElementAtOrDefault(3);
		if (nMerchantSlot2 != null)
		{
			NMerchantSlot firstStockedSlot2 = GetFirstStockedSlot(new _003C_003Ez__ReadOnlyArray<NMerchantSlot>(new NMerchantSlot[9]
			{
				Relic(2),
				Relic(1),
				Potion(2),
				Potion(1),
				_cardRemovalNode,
				Relic(0),
				Potion(0),
				Colorless(1),
				Colorless(0)
			}));
			if (firstStockedSlot2 != null)
			{
				nMerchantSlot2.FocusNeighborBottom = firstStockedSlot2.GetPath();
			}
		}
		NMerchantSlot nMerchantSlot3 = list.ElementAtOrDefault(4);
		if (nMerchantSlot3 != null)
		{
			NMerchantSlot firstStockedSlot3 = GetFirstStockedSlot(new _003C_003Ez__ReadOnlySingleElementList<NMerchantSlot>(_cardRemovalNode));
			if (firstStockedSlot3 != null)
			{
				nMerchantSlot3.FocusNeighborBottom = firstStockedSlot3.GetPath();
			}
		}
		for (int j = 0; j < list3.Count; list3[j].FocusNeighborBottom = list3[j].GetPath(), j++)
		{
			if (list2.Count > 0)
			{
				Control closestStockedSlot3 = GetClosestStockedSlot(j, list2);
				if (closestStockedSlot3 != null)
				{
					list3[j].FocusNeighborTop = closestStockedSlot3.GetPath();
					continue;
				}
			}
			Control closestStockedSlot4 = GetClosestStockedSlot(j, list);
			if (closestStockedSlot4 != null)
			{
				list3[j].FocusNeighborTop = closestStockedSlot4.GetPath();
			}
			else
			{
				list3[j].FocusNeighborTop = list3[j].GetPath();
			}
		}
		for (int k = 0; k < list2.Count; k++)
		{
			if (list.Count > 0)
			{
				Control closestStockedSlot5 = GetClosestStockedSlot(k, list);
				if (closestStockedSlot5 != null)
				{
					list2[k].FocusNeighborTop = closestStockedSlot5.GetPath();
					goto IL_03fd;
				}
			}
			list2[k].FocusNeighborTop = list2[k].GetPath();
			goto IL_03fd;
			IL_03fd:
			if (list3.Count > 0)
			{
				Control closestStockedSlot6 = GetClosestStockedSlot(k, list3);
				if (closestStockedSlot6 != null)
				{
					list2[k].FocusNeighborBottom = closestStockedSlot6.GetPath();
					continue;
				}
			}
			list2[k].FocusNeighborBottom = list2[k].GetPath();
		}
		NMerchantSlot? Colorless(int index)
		{
			return colorlessCards.ElementAtOrDefault(index);
		}
		NMerchantSlot? Potion(int index)
		{
			return potions.ElementAtOrDefault(index);
		}
		NMerchantSlot? Relic(int index)
		{
			return relics.ElementAtOrDefault(index);
		}
	}

	public void BlockInput()
	{
		_isInputBlocked = true;
		_inputBlocker.MouseFilter = MouseFilterEnum.Stop;
		NHotkeyManager.Instance.AddBlockingScreen(_inputBlocker);
		if (_backButton.IsEnabled)
		{
			_backButton.Disable();
		}
	}

	public void UnblockInput()
	{
		_isInputBlocked = false;
		_inputBlocker.MouseFilter = MouseFilterEnum.Ignore;
		NHotkeyManager.Instance.RemoveBlockingScreen(_inputBlocker);
		if (!_backButton.IsEnabled)
		{
			_backButton.Enable();
		}
	}

	private Control? GetClosestStockedSlot(int idx, List<NMerchantSlot> row)
	{
		NMerchantSlot nMerchantSlot = row[Math.Min(idx, row.Count - 1)];
		if (nMerchantSlot.Entry.IsStocked)
		{
			return nMerchantSlot;
		}
		int num = row.IndexOf(nMerchantSlot);
		int num2 = num - 1;
		int num3 = num + 1;
		while (num2 >= 0 || num3 < row.Count)
		{
			if (num2 >= 0)
			{
				if (row[num2].Entry.IsStocked)
				{
					return row[num2];
				}
				num2--;
			}
			if (num3 < row.Count)
			{
				if (row[num3].Entry.IsStocked)
				{
					return row[num3];
				}
				num3++;
			}
		}
		return null;
	}

	private void OnActiveScreenUpdated()
	{
		this.UpdateControllerNavEnabled();
		if (ActiveScreenContext.Instance.IsCurrent(this))
		{
			if (_characterCardContainer != null && NControllerManager.Instance.IsUsingDirectionalNavigation && _inventoryTween != null && _inventoryTween.IsRunning())
			{
				float num = 80f - _slotsContainer.Position.Y;
				NMerchantCard child = _characterCardContainer.GetChild<NMerchantCard>(0);
				MerchantHand.PointAtTarget(child, child.MerchantHandOffset + Vector2.Down * num);
			}
			if (!_isInputBlocked)
			{
				_backButton.Enable();
			}
		}
		else
		{
			_backButton.Disable();
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
		List<MethodInfo> list = new List<MethodInfo>(15);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.Open, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.SubscribeToEntries, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.Close, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.UpdateBlitzSelectionText, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnCardRemovalUsed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.UpdateNavigation, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.UpdateBlitzNavigation, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.UpdateHorizontalNavigation, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.UpdateVerticalNavigation, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.BlockInput, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.UnblockInput, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnActiveScreenUpdated, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
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
		if (method == MethodName.Open && args.Count == 0)
		{
			Open();
			ret = default;
			return true;
		}
		if (method == MethodName.SubscribeToEntries && args.Count == 0)
		{
			SubscribeToEntries();
			ret = default;
			return true;
		}
		if (method == MethodName.Close && args.Count == 0)
		{
			Close();
			ret = default;
			return true;
		}
		if (method == MethodName.UpdateBlitzSelectionText && args.Count == 0)
		{
			UpdateBlitzSelectionText();
			ret = default;
			return true;
		}
		if (method == MethodName.OnCardRemovalUsed && args.Count == 0)
		{
			OnCardRemovalUsed();
			ret = default;
			return true;
		}
		if (method == MethodName.UpdateNavigation && args.Count == 0)
		{
			UpdateNavigation();
			ret = default;
			return true;
		}
		if (method == MethodName.UpdateBlitzNavigation && args.Count == 0)
		{
			UpdateBlitzNavigation();
			ret = default;
			return true;
		}
		if (method == MethodName.UpdateHorizontalNavigation && args.Count == 0)
		{
			UpdateHorizontalNavigation();
			ret = default;
			return true;
		}
		if (method == MethodName.UpdateVerticalNavigation && args.Count == 0)
		{
			UpdateVerticalNavigation();
			ret = default;
			return true;
		}
		if (method == MethodName.BlockInput && args.Count == 0)
		{
			BlockInput();
			ret = default;
			return true;
		}
		if (method == MethodName.UnblockInput && args.Count == 0)
		{
			UnblockInput();
			ret = default;
			return true;
		}
		if (method == MethodName.OnActiveScreenUpdated && args.Count == 0)
		{
			OnActiveScreenUpdated();
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
		if (method == MethodName.Open)
		{
			return true;
		}
		if (method == MethodName.SubscribeToEntries)
		{
			return true;
		}
		if (method == MethodName.Close)
		{
			return true;
		}
		if (method == MethodName.UpdateBlitzSelectionText)
		{
			return true;
		}
		if (method == MethodName.OnCardRemovalUsed)
		{
			return true;
		}
		if (method == MethodName.UpdateNavigation)
		{
			return true;
		}
		if (method == MethodName.UpdateBlitzNavigation)
		{
			return true;
		}
		if (method == MethodName.UpdateHorizontalNavigation)
		{
			return true;
		}
		if (method == MethodName.UpdateVerticalNavigation)
		{
			return true;
		}
		if (method == MethodName.BlockInput)
		{
			return true;
		}
		if (method == MethodName.UnblockInput)
		{
			return true;
		}
		if (method == MethodName.OnActiveScreenUpdated)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.IsOpen)
		{
			IsOpen = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.MerchantHand)
		{
			MerchantHand = VariantUtils.ConvertTo<NMerchantHand>(in value);
			return true;
		}
		if (name == PropertyName._characterCardContainer)
		{
			_characterCardContainer = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._colorlessCardContainer)
		{
			_colorlessCardContainer = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._relicContainer)
		{
			_relicContainer = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._potionContainer)
		{
			_potionContainer = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._blitzContainer)
		{
			_blitzContainer = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._blitzSelectionText)
		{
			_blitzSelectionText = VariantUtils.ConvertTo<MegaRichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName._cardRemovalNode)
		{
			_cardRemovalNode = VariantUtils.ConvertTo<NMerchantCardRemoval>(in value);
			return true;
		}
		if (name == PropertyName._backButton)
		{
			_backButton = VariantUtils.ConvertTo<NBackButton>(in value);
			return true;
		}
		if (name == PropertyName._merchantDialogue)
		{
			_merchantDialogue = VariantUtils.ConvertTo<NMerchantDialogue>(in value);
			return true;
		}
		if (name == PropertyName._inventoryTween)
		{
			_inventoryTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		if (name == PropertyName._slotsContainer)
		{
			_slotsContainer = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._backstop)
		{
			_backstop = VariantUtils.ConvertTo<ColorRect>(in value);
			return true;
		}
		if (name == PropertyName._inputBlocker)
		{
			_inputBlocker = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._isInputBlocked)
		{
			_isInputBlocked = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._lastSlot)
		{
			_lastSlot = VariantUtils.ConvertTo<NMerchantSlot>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.IsOpen)
		{
			value = VariantUtils.CreateFrom<bool>(IsOpen);
			return true;
		}
		if (name == PropertyName.MerchantHand)
		{
			value = VariantUtils.CreateFrom<NMerchantHand>(MerchantHand);
			return true;
		}
		if (name == PropertyName.DefaultFocusedControl)
		{
			value = VariantUtils.CreateFrom<Control>(DefaultFocusedControl);
			return true;
		}
		if (name == PropertyName._characterCardContainer)
		{
			value = VariantUtils.CreateFrom(in _characterCardContainer);
			return true;
		}
		if (name == PropertyName._colorlessCardContainer)
		{
			value = VariantUtils.CreateFrom(in _colorlessCardContainer);
			return true;
		}
		if (name == PropertyName._relicContainer)
		{
			value = VariantUtils.CreateFrom(in _relicContainer);
			return true;
		}
		if (name == PropertyName._potionContainer)
		{
			value = VariantUtils.CreateFrom(in _potionContainer);
			return true;
		}
		if (name == PropertyName._blitzContainer)
		{
			value = VariantUtils.CreateFrom(in _blitzContainer);
			return true;
		}
		if (name == PropertyName._blitzSelectionText)
		{
			value = VariantUtils.CreateFrom(in _blitzSelectionText);
			return true;
		}
		if (name == PropertyName._cardRemovalNode)
		{
			value = VariantUtils.CreateFrom(in _cardRemovalNode);
			return true;
		}
		if (name == PropertyName._backButton)
		{
			value = VariantUtils.CreateFrom(in _backButton);
			return true;
		}
		if (name == PropertyName._merchantDialogue)
		{
			value = VariantUtils.CreateFrom(in _merchantDialogue);
			return true;
		}
		if (name == PropertyName._inventoryTween)
		{
			value = VariantUtils.CreateFrom(in _inventoryTween);
			return true;
		}
		if (name == PropertyName._slotsContainer)
		{
			value = VariantUtils.CreateFrom(in _slotsContainer);
			return true;
		}
		if (name == PropertyName._backstop)
		{
			value = VariantUtils.CreateFrom(in _backstop);
			return true;
		}
		if (name == PropertyName._inputBlocker)
		{
			value = VariantUtils.CreateFrom(in _inputBlocker);
			return true;
		}
		if (name == PropertyName._isInputBlocked)
		{
			value = VariantUtils.CreateFrom(in _isInputBlocked);
			return true;
		}
		if (name == PropertyName._lastSlot)
		{
			value = VariantUtils.CreateFrom(in _lastSlot);
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
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._characterCardContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._colorlessCardContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._relicContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._potionContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._blitzContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._blitzSelectionText, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._cardRemovalNode, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._backButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._merchantDialogue, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._inventoryTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._slotsContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._backstop, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._inputBlocker, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName._isInputBlocked, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._lastSlot, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName.IsOpen, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.MerchantHand, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.DefaultFocusedControl, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.IsOpen, Variant.From<bool>(IsOpen));
		info.AddProperty(PropertyName.MerchantHand, Variant.From<NMerchantHand>(MerchantHand));
		info.AddProperty(PropertyName._characterCardContainer, Variant.From(in _characterCardContainer));
		info.AddProperty(PropertyName._colorlessCardContainer, Variant.From(in _colorlessCardContainer));
		info.AddProperty(PropertyName._relicContainer, Variant.From(in _relicContainer));
		info.AddProperty(PropertyName._potionContainer, Variant.From(in _potionContainer));
		info.AddProperty(PropertyName._blitzContainer, Variant.From(in _blitzContainer));
		info.AddProperty(PropertyName._blitzSelectionText, Variant.From(in _blitzSelectionText));
		info.AddProperty(PropertyName._cardRemovalNode, Variant.From(in _cardRemovalNode));
		info.AddProperty(PropertyName._backButton, Variant.From(in _backButton));
		info.AddProperty(PropertyName._merchantDialogue, Variant.From(in _merchantDialogue));
		info.AddProperty(PropertyName._inventoryTween, Variant.From(in _inventoryTween));
		info.AddProperty(PropertyName._slotsContainer, Variant.From(in _slotsContainer));
		info.AddProperty(PropertyName._backstop, Variant.From(in _backstop));
		info.AddProperty(PropertyName._inputBlocker, Variant.From(in _inputBlocker));
		info.AddProperty(PropertyName._isInputBlocked, Variant.From(in _isInputBlocked));
		info.AddProperty(PropertyName._lastSlot, Variant.From(in _lastSlot));
		info.AddSignalEventDelegate(SignalName.InventoryClosed, backing_InventoryClosed);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.IsOpen, out var value))
		{
			IsOpen = value.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.MerchantHand, out var value2))
		{
			MerchantHand = value2.As<NMerchantHand>();
		}
		if (info.TryGetProperty(PropertyName._characterCardContainer, out var value3))
		{
			_characterCardContainer = value3.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._colorlessCardContainer, out var value4))
		{
			_colorlessCardContainer = value4.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._relicContainer, out var value5))
		{
			_relicContainer = value5.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._potionContainer, out var value6))
		{
			_potionContainer = value6.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._blitzContainer, out var value7))
		{
			_blitzContainer = value7.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._blitzSelectionText, out var value8))
		{
			_blitzSelectionText = value8.As<MegaRichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName._cardRemovalNode, out var value9))
		{
			_cardRemovalNode = value9.As<NMerchantCardRemoval>();
		}
		if (info.TryGetProperty(PropertyName._backButton, out var value10))
		{
			_backButton = value10.As<NBackButton>();
		}
		if (info.TryGetProperty(PropertyName._merchantDialogue, out var value11))
		{
			_merchantDialogue = value11.As<NMerchantDialogue>();
		}
		if (info.TryGetProperty(PropertyName._inventoryTween, out var value12))
		{
			_inventoryTween = value12.As<Tween>();
		}
		if (info.TryGetProperty(PropertyName._slotsContainer, out var value13))
		{
			_slotsContainer = value13.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._backstop, out var value14))
		{
			_backstop = value14.As<ColorRect>();
		}
		if (info.TryGetProperty(PropertyName._inputBlocker, out var value15))
		{
			_inputBlocker = value15.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._isInputBlocked, out var value16))
		{
			_isInputBlocked = value16.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._lastSlot, out var value17))
		{
			_lastSlot = value17.As<NMerchantSlot>();
		}
		if (info.TryGetSignalEventDelegate<InventoryClosedEventHandler>(SignalName.InventoryClosed, out var value18))
		{
			backing_InventoryClosed = value18;
		}
	}

	/// <summary>
	/// Get the signal information for all the signals declared in this class.
	/// This method is used by Godot to register the available signals in the editor.
	/// Do not call this method.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(SignalName.InventoryClosed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		return list;
	}

	protected void EmitSignalInventoryClosed()
	{
		EmitSignal(SignalName.InventoryClosed);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		if (signal == SignalName.InventoryClosed && args.Count == 0)
		{
			backing_InventoryClosed?.Invoke();
		}
		else
		{
			base.RaiseGodotClassSignalCallbacks(in signal, args);
		}
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if (signal == SignalName.InventoryClosed)
		{
			return true;
		}
		return base.HasGodotClassSignal(in signal);
	}
}
