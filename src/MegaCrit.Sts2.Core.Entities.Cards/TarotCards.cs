using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MegaCrit.Sts2.Core.Entities.Cards;

public static class TarotCards
{
	public static List<CardModel> All
	{
		get
		{
			int num = 22;
			List<CardModel> list = new List<CardModel>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<CardModel> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = ModelDb.Card<Judgement>();
			num2++;
			span[num2] = ModelDb.Card<TheFool>();
			num2++;
			span[num2] = ModelDb.Card<TheHighPriestess>();
			num2++;
			span[num2] = ModelDb.Card<TheEmperor>();
			num2++;
			span[num2] = ModelDb.Card<TheEmpress>();
			num2++;
			span[num2] = ModelDb.Card<TheHangedMan>();
			num2++;
			span[num2] = ModelDb.Card<TheHierophant>();
			num2++;
			span[num2] = ModelDb.Card<TheChariot>();
			num2++;
			span[num2] = ModelDb.Card<Justice>();
			num2++;
			span[num2] = ModelDb.Card<TheMagician>();
			num2++;
			span[num2] = ModelDb.Card<TheMoon>();
			num2++;
			span[num2] = ModelDb.Card<TheTower>();
			num2++;
			span[num2] = ModelDb.Card<Strength>();
			num2++;
			span[num2] = ModelDb.Card<Death>();
			num2++;
			span[num2] = ModelDb.Card<Temperance>();
			num2++;
			span[num2] = ModelDb.Card<TheDevil>();
			num2++;
			span[num2] = ModelDb.Card<TheHermit>();
			num2++;
			span[num2] = ModelDb.Card<TheLovers>();
			num2++;
			span[num2] = ModelDb.Card<TheStar>();
			num2++;
			span[num2] = ModelDb.Card<TheSun>();
			num2++;
			span[num2] = ModelDb.Card<TheWorld>();
			num2++;
			span[num2] = ModelDb.Card<WheelOfFortune>();
			return list;
		}
	}
}
