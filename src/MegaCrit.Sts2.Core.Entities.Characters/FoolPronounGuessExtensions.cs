using System;

namespace MegaCrit.Sts2.Core.Entities.Characters;

public static class FoolPronounGuessExtensions
{
	public static CharacterGender ToGrammaticalGender(this FoolPronounGuess guess)
	{
		return guess switch
		{
			FoolPronounGuess.Neutral => CharacterGender.Neutral,
			FoolPronounGuess.Feminine => CharacterGender.Feminine,
			FoolPronounGuess.Masculine => CharacterGender.Masculine,
			FoolPronounGuess.It => CharacterGender.Neutral,
			_ => throw new ArgumentOutOfRangeException("guess", guess, null),
		};
	}

	public static string ToLocKey(this FoolPronounGuess guess)
	{
		return guess.ToString().ToLowerInvariant();
	}
}
