# Blood within the remainder of the minimal purity

After a hayd, the next 15 days are the minimal purity. Blood that falls into what is left of these
15 days ("baqiyyat tuhr") cannot be hayd. This file collects what happens with it.

## The 15-day window: which rule applies

Let A = the first blood (a valid hayd), P = the purity after it, B = the returning blood.

1. **A + P + B together within 15 days**: one hayd with the purity in between (qawl as-sahb), basics.md.
2. **B starts after day 15 of A (A + P is more than 15 days), but before the minimal purity is
   complete**: the "rest of tuhr" cases below (3/12/3, the Bughya rule).
3. **B starts inside the 15 days of A and runs past day 15** (A + P under 15 days): intermittent
   blood that exceeds 15 days. Mughni 1/294 and Nihaya 1/356 call it istihada / a mustahada case;
   whether that also holds when B stops soon after day 15 is an open question
   (13/1/5, open-questions.md).

The Bughya text names the condition for case 2 itself: «نقاءً دونَ خمسةَ عشرَ ، لٰكنْ لوِ اجتمعَ معَ الدمِ زادَ عليها»
(purity under 15 days which, together with the first blood, exceeds 15 days).

## Blood that stops inside the remaining purity: dam fasad (3/12/3)

- [al-Khatib, Mughni 1/284](https://shamela.ws/book/11444/200):
  «إلَّا أَنْ يَكُونَ عَلَيْهَا بَقِيَّةُ طُهْرٍ كَأَنْ رَأَتْ ثَلَاثَةَ أَيَّامٍ دَمًا، ثُمَّ اثْنَيْ عَشَرَ نَقَاءً، ثُمَّ ثَلَاثَةً دِمَاءً، ثُمَّ انْقَطَعَ، فَالثَّلَاثَةُ الْأَخِيرَةُ دَمُ فَسَادٍ لَا حَيْضٍ، ذَكَرَ ذَلِكَ فِي الْمَجْمُوعِ مُفَرَّقًا»
- [ar-Ramli, Nihaya 1/339f.](https://shamela.ws/book/3565/337):
  «وَيُشْتَرَطُ أَنْ لَا يَكُونَ عَلَيْهَا بَقِيَّةُ طُهْرٍ، فَإِنْ كَانَ بِأَنْ رَأَتْ ثَلَاثَةً دَمًا ثُمَّ اثْنَيْ عَشَرَ نَقَاءً ثُمَّ ثَلَاثَةً دَمًا ثُمَّ انْقَطَعَ فَالثَّلَاثَةُ الْأَخِيرَةُ دَمُ فَسَادٍ لَا حَيْضٍ كَمَا ذَكَرَهُ فِي الْمَجْمُوعِ مُفَرَّقًا»
- [Ibn Hajar, Tuhfa 1/399](https://shamela.ws/book/9059/400):
  «أَمَّا إذَا بَقِيَ عَلَيْهَا بَقِيَّةُ طُهْرٍ كَأَنْ رَأَتْ ثَلَاثَةً دَمًا، ثُمَّ اثْنَيْ عَشَرَ نَقَاءً، ثُمَّ ثَلَاثَةً دَمًا، ثُمَّ انْقَطَعَ فَالثَّلَاثَةُ الْأَخِيرَةُ دَمُ فَسَادٍ»
  Ash-Sharwani there: «(قَوْلُهُ فَالثَّلَاثَةُ الْأَخِيرَةُ إلَخْ) شَامِلٌ لِلْمُبْتَدَأَةِ أَيْضًا» (this includes the mubtada'a).
- [Ibn Hajar, al-Fatawa al-Kubra 1/86](https://shamela.ws/book/21628/85), with the general rule and "no khilaf":
  «رَأَتْ ثَلَاثَةَ أَيَّامٍ دَمًا ثُمَّ اثْنَيْ عَشَرَ نَقَاءً ثُمَّ ثَلَاثَةً دَمًا وَانْقَطَعَ، فَالثَّلَاثَةُ الْأُوَلُ حَيْضٌ وَالثَّانِيَةُ دَمُ فَسَادٍ لِكَوْنِهَا تَمَامَ قَدْرِ الطُّهْرِ وَكَذَا لَوْ رَأَتْ أَوَّلًا يَوْمًا وَلَيْلَةً أَوْ أَكْثَرَ ثُمَّ طَهُرَتْ، وَرَأَتْ دَمًا قَبْلَ خَمْسَةَ عَشَرَ مِنْ أَوَّلِ الطُّهْرِ، وَانْقَطَعَ بِحَيْثُ لَا يُمْكِنُ كَوْنُ الدَّمَيْنِ فِي خَمْسَةَ عَشَرَ وَلَا خِلَافَ فِي شَيْءٍ مِنْ هَذَا»
  The general rule here: first blood of a day and a night or more, then purity, then blood
  before 15 days from the start of the purity, which stops so that the two bloods cannot lie
  within 15 days: the second blood is dam fasad, no khilaf. (How this bears on 13/1/5: open-questions.md.)

No khilaf. Tests: `HaydCalculation_BloodWithinRemainingMinimalTuhur_IsDamFasad`,
`HaydCalculation_CalculationWithPurityBetweenMenstrualFlowAndIstihadaFlow` (7/8/2),
`HaydCalculation_OneDayOfBloodWithinRemainingMinimalTuhur_IsDamFasad`,
`ThreeDaysRedTwelveDaysPurityThreeDaysRedThenStop_IsNoMustahadaCase`.

## Blood that passes the completion of the purity and then stops: the rest is hayd (Bughya rule)

- [Ba 'Alawi, Bughyat al-Mustarshidin, Dar al-Minhaj ed., vol. 1, p. 277, after Fatawa Ibn Yahya p. 24] (approved exception, full text in sources.md):
  «فالأولُ : حيضٌ ، وما يكمِّلُ الطهرَ مِنَ العائدِ : دمُ فسادٍ ، والزائدُ : حيضٌ بشرطِهِ ، ما لم يجاوزْ أكثرَهُ ، وإلَّا .. فتأخذُ المُبتدَأةُ غيرُ المميِّزةِ مِنْ أولِ الزائدِ يوماً وليلةً وتطهرُ تسعةً وعشرينَ ، والمعتادةُ عادتَها حيضاً وطهراً»
  The first blood is hayd; the part of the returning blood that completes the purity is dam
  fasad; the rest is hayd if it meets its conditions (at least a day and a night) and does not
  exceed 15 days; otherwise (the rest exceeds 15 days) the mubtada'a ghayr mumayyiza takes a day
  and a night from the start of the rest and 29 days purity, the mu'tada her 'ada.
  Note: the text describes the first blood as "more than a day and a night and less than 15 days".
- [Ibn Qasim, Hashiya on the Tuhfa 1/399](https://shamela.ws/book/9059/400) (quoted by ash-Sharwani
  there and by ash-Shabramallisi in the Nihaya 1/340), tentatively:
  «وَانْظُرْ لَوْ كَانَ الدَّمُ الْمَرْئِيُّ بَعْدَ النَّقَاءِ سِتَّةً مَثَلًا فَهَلْ يُجْعَلُ الزَّائِدُ عَلَى تَكْمِلَةِ الطُّهْرِ حَيْضًا لَا يَبْعُدُ أَنْ يُجْعَلَ سم عَلَى حَجّ»
- [ash-Shabramallisi, Hashiya on the Nihaya 1/340](https://shamela.ws/book/3565/338): restricts the
  general rule (bloods that exceed 15 days with the purity are istihada) by this case:
  «وَكَتَبَ شَيْخُنَا بِرّ بِهَامِشِ شَرْحِ الْمَنْهَجِ مَا نَصُّهُ: اُنْظُرْ هَذَا مَعَ قَوْلِهِمْ آخِرَ الْبَابِ فِي مَسْأَلَةِ الدِّمَاءِ الْمُتَخَلَّلَةِ بِالنَّقَاءِ إذَا زَادَتْ عَلَى خَمْسَةَ عَشَرَ بِالنَّقَاءِ فَهِيَ اسْتِحَاضَةٌ اهـ. أَقُولُ: يُخَصُّ ذَاكَ بِهَذَا ... أَيْ فَيُقَالُ إنْ قَطَعَ عَلَى رَأْسِ الْخَمْسَةَ عَشَرَ أَوْ فِيهَا كَانَ الدَّمُ مَعَ النَّقَاءِ حَيْضًا، وَهَذَا التَّخْصِيصُ فِي الْحَقِيقَةِ هُوَ مَفْهُومُ قَوْلِهِمْ إذَا زَادَتْ عَلَى خَمْسَةَ عَشَرَ»
  and then, on Ibn Qasim's "la yab'ud":
  «وَظَاهِرُهُ أَنَّهُ لَا فَرْقَ بَيْنَ الْمُبْتَدَأَةِ وَالْمُعْتَادَةِ، لَكِنَّ فِيمَا تَقَدَّمَ عَنْ حَجّ مِنْ قَوْلِهِ كَمَا قَالُوهُ فَمَا لَوْ رَأَتْ خَمْسَتَهَا الْمَعْهُودَةَ أَوَّلَ الشَّهْرِ إلَخْ مَا يَقْتَضِي تَخْصِيصَ ذَلِكَ بِالْمُعْتَادَةِ، وَأَنَّ الْمُبْتَدَأَةَ تَحِيضُ يَوْمًا وَلَيْلَةً مِنْ أَوَّلِ الشَّهْرِ»
  He only notes what Ibn Hajar's wording "might imply" for the mubtada'a; he holds no view.
  Ibn Hajar's passage itself is about blood that continues (see next section), where Bughya says
  the same (mubtada'a: a day and a night). This was recorded as a KHILAF note, not as a
  contradiction; the user may decide otherwise.

Worked examples (day 1 = first day of the first blood):

| Course | Purity complete at end of day | Dam fasad | Hayd from the returning blood | Mustahada case? |
|---|---|---|---|---|
| 3 red, 14 pure, 4 red | 18 | day 18 | days 19-21 | no |
| 10 red, 8 pure, 12 red | 25 | days 19-25 | days 26-30 | no |
| 3 red, 14 pure, 16 red | 18 | day 18 | days 19-33 (15 days) | no |
| 3 red, 14 pure, 17 red | 18 | day 18 | rest 16 days | second cycle: yes |
| 5 red, 25 pure, 5 red, 14 pure, 16 red (mu'tada) | 50 | day 50 | days 51-65 | no |
| 5 red, 25 pure, 5 red, 14 pure, 17 red (mu'tada) | 50 | day 50 | rest 16 days | third cycle: yes |

Tests: `HaydCalculation_CalculationWithPurityBetweenTwoSeparateMenstrualCyclesIncludingIstihada` (3/14/4),
`HaydCalculation_CalculationWithPartiallyIstihadaAndPartiallyMenstrualFlow` (10/8/12), and the
classification tests for each row above.

History: 3/14/4 and 10/8/12 were open questions with only the lecture videos as source (part 8
?t=200, part 3 ?t=1910) until the user supplied the Bughya passage (2026-10-29).

## Blood that continues: a mustahada case

- [Ibn Hajar, Tuhfa 1/399](https://shamela.ws/book/9059/400), right after 3/12/3:
  «وَخَرَجَ بِانْقِطَاعِ مَا لَوْ اسْتَمَرَّ فَإِنْ كَانَتْ مُبْتَدَأَةً فَغَيْرُ مُمَيِّزَةٍ أَوْ مُعْتَادَةٍ عَمِلَتْ بِعَادَتِهَا كَمَا قَالُوهُ فِيمَا لَوْ رَأَتْ خَمْسَتَهَا الْمَعْهُودَةَ أَوَّلَ الشَّهْرِ، ثُمَّ نَقَاءَ أَرْبَعَةَ عَشَرَ، ثُمَّ عَادَ الدَّمُ وَاسْتَمَرَّ فَيَوْمٌ وَلَيْلَةٌ مِنْ أَوَّلِ الْعَائِدِ طُهْرٌ، ثُمَّ تَحِيضُ خَمْسَةَ أَيَّامٍ مِنْهُ وَيَسْتَمِرُّ دَوْرُهَا عِشْرِينَ»
- [ash-Shabramallisi, Nihaya 1/340](https://shamela.ws/book/3565/338): quotes the same from Ibn Hajar («اهـ حَجّ»).
- [Ibn Hajar, al-Fatawa al-Kubra 1/111](https://shamela.ws/book/21628/110), the same mu'tada case:
  «وَلَوْ لَمْ تَطْهُرْ إلَّا أَرْبَعَةَ عَشَرَ، ثُمَّ عَادَ الدَّمُ وَاسْتَمَرَّ كَانَ يَوْمٌ مِنْ أَوَّلِ الدَّمِ الْعَائِدِ اسْتِحَاضَةً تَكْمِيلًا لِلطُّهْرِ وَخَمْسَةٌ بَعْدَهُ حَيْضٌ وَخَمْسَةَ عَشَرَ طُهْرٌ وَصَارَ دَوْرُهَا عِشْرِينَ»
- Bughya (above): if the rest exceeds 15 days, mubtada'a ghayr mumayyiza a day and a night from
  the start of the rest, mu'tada her 'ada.

For the mubtada'a this is disputed: al-Bujayrimi and ash-Shabramallisi read Tuhfa 1/399 as "a day
and a night from the start of the month", i.e. a mustahada case from the first blood on
(open-questions.md, question 7). For the mu'tada there is no such dispute.

So the classification is per hayd cycle: the first blood is a basic hayd, the cycle that starts
with the rest of the returning blood is a mustahada case (mustahada.md).

Tests: `Mutada_FourteenDaysPurityThenContinuousBlood_FirstDayCompletesTuhur`,
`ThreeDaysRedTwelveDaysPurityThenTwentyDaysRed_FirstCycleNoMustahadaCaseSecondCycleMustahadaCase`,
`OneDayRedFourteenDaysPurityThenTwentyDaysRed_FirstCycleNoMustahadaCaseSecondCycleMustahadaCase`
(first blood exactly 24 h: Bughya says "more than a day and a night", so [IMPLICIT]).
