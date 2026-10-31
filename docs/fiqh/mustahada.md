# Mustahada: when a case is complex, and its kinds

## Definition: blood beyond 15 days

- [al-Khatib, Mughni 1/285](https://shamela.ws/book/11444/201):
  «ثُمَّ أَخَذَ فِي بَيَانِ مَا إذَا جَاوَزَ دَمُ الْمَرْأَةِ خَمْسَةَ عَشَرَ يَوْمًا، وَتُسَمَّى بِالْمُسْتَحَاضَةِ، وَلَهَا سَبْعَةُ أَحْوَالٍ؛ لِأَنَّهَا إمَّا مُمَيِّزَةٌ أَوْ لَا، وَكُلٌّ مِنْهُمَا إمَّا مُبْتَدَأَةٌ أَوْ مُعْتَادَةٌ، وَغَيْرُ الْمُمَيِّزَةِ النَّاسِيَةُ لِلْعَادَةِ وَهِيَ الْمُتَحَيِّرَةُ إمَّا نَاسِيَةٌ لِلْقَدْرِ وَالْوَقْتِ أَوْ لِلْأَوَّلِ دُونَ الثَّانِي، أَوْ لِلثَّانِي دُونَ الْأَوَّلِ»
- [ar-Ramli, Nihaya 1/340f.](https://shamela.ws/book/3565/338): «ثُمَّ شَرَعَ فِي بَيَانِ مَا لَوْ جَاوَزَ دَمُ الْمَرْأَةِ خَمْسَةَ عَشَرَ يَوْمًا يُسَمَّى بِالْمُسْتَحَاضَةِ، وَلَهَا سَبْعَةُ أَحْوَالٍ ...» ([3565/339](https://shamela.ws/book/3565/339))
- [Ibn Hajar, Tuhfa 1/401](https://shamela.ws/book/9059/402):
  «(فَإِنْ عَبَرَهُ) أَيْ الدَّمُ أَكْثَرَهُ فَإِمَّا أَنْ تَكُونَ مُبْتَدَأَةً أَوْ مُعْتَادَةً، وَكُلٌّ مِنْهُمَا مَا مُمَيِّزَةٌ أَوْ غَيْرُ مُمَيِّزَةٍ وَالْمُعْتَادَةُ إمَّا ذَاكِرَةً لِلْقَدْرِ وَالْوَقْتِ أَوْ نَاسِيَةً لَهُمَا أَوْ لِأَحَدِهِمَا فَالْأَقْسَامُ سَبْعَةٌ»
- [Ibn Hajar, al-Fatawa al-Kubra 1/102](https://shamela.ws/book/21628/101): exceeding 15 days makes
  her a mustahada also when the blood stops later (before 30 days):
  «وَنَقْصُهُ عَنْ ثَلَاثِينَ لَا يُبَيِّنُ أَنَّهَا غَيْرُ مُسْتَحَاضَةٍ كَمَا يُصَرِّحُ بِهِ قَوْلُهُمْ حَيْثُ جَاوَزَ الدَّمُ خَمْسَةَ عَشَرَ، فَهِيَ مُسْتَحَاضَةٌ (الشَّامِلُ) لِمَا إذَا اسْتَمَرَّ الدَّمُ أَوْ انْقَطَعَ قَبْلَ الْخَمْسَةَ عَشَرَ الثَّانِيَةِ»
- Intermittent blood: «وَلَوْ عَبَرَ التَّقَطُّعُ خَمْسَةَ عَشَرَ جَاءَ مَا مَرَّ فِي الْمُسْتَحَاضَاتِ» (Nihaya 1/356, [3565/354](https://shamela.ws/book/3565/354));
  «وَإِنْ زَادَتْ مَعَ النَّقَاءِ بَيْنَهَا عَلَى خَمْسَةَ عَشَرَ يَوْمًا فَهِيَ دَمُ اسْتِحَاضَةٍ» (Mughni 1/294, [11444/210](https://shamela.ws/book/11444/210)).
  Restricted by the 3/12/3 case (rest-of-tuhr.md); partly open (open-questions.md).

## The seven kinds

| # | Kind | Rule (default view) | File |
|---|---|---|---|
| 1 | mubtada'a mumayyiza | strong blood hayd, weak blood purity, if the four conditions hold | tamyiz.md |
| 2 | mubtada'a ghayr mumayyiza (one quality, or a condition of tamyiz missing) | a day and a night hayd, 29 days purity | below |
| 3 | mu'tada ghayr mumayyiza | back to her 'ada in amount and time | ada.md |
| 4 | mu'tada mumayyiza | tamyiz decides (asahh) | ada.md |
| 5-7 | mutahayyira (forgot amount and time / only amount / only time) | ihtiyat; not implemented | (TODO) |

A woman who has a tamyiz with a missing condition is called "mumayyiza whose tamyiz does not
count" or, in ar-Rawda, ghayr mumayyiza; only the name differs, the ruling is that of kind 2
(Mughni 1/287 tanbih; Tuhfa 1/404; Nihaya 1/342).

## Classification: mustahada case or basic rules, per hayd cycle

The calculator has to decide, for each hayd cycle, whether the basic rules settle it or whether
the blood exceeds 15 days so that the mustahada rules (tamyiz, 'ada) apply. One course of bleeding
can contain both: e.g. 3 red, 12 pure, 20 red: cycle 1 (the 3 days) is basic, the returning blood
completes the purity and then exceeds 15 days, so cycle 2 is a mustahada case (Tuhfa 1/399,
Bughya; rest-of-tuhr.md).

State in the code: today a mustahada case shows up as `InfoException(TextUtil.ComplexIstihadaNotImplemented)`
from `HaydCalculation.DoComplexIstihada`. TODO (in the code and the tests): a bool per hayd cycle
(e.g. `IsComplexIstihada`). Tests with more than one cycle keep the per-cycle assertions commented
out and throw `NotImplementedException`.

Expected values (tests in `HaydCalculatorServiceTests_IstihadaClassification`):

| Course (days) | Expected | Main source |
|---|---|---|
| 16 red | mustahada case | Mughni 1/285, Nihaya 1/340 [IMPLICIT] |
| 15 red, then 5 red as a separate entry | mustahada case | same |
| 5 black, 15 red | mustahada case | Tuhfa 1/399 (5 black then red: beyond 15 only the black is hayd) |
| 10 red, 3 pure, 20 red | mustahada case | Nihaya 1/356, Mughni 1/294 (the returning blood starts inside the 15 days and runs far beyond) |
| 3 red, 12 pure, 20 red | cycle 1 basic, cycle 2 mustahada (disputed for the mubtada'a: open question 7) | Tuhfa 1/399, Shabramallisi 1/340, Bughya |
| 1 red, 14 pure, 20 red | cycle 1 basic, cycle 2 mustahada (disputed for the mubtada'a: open question 7) | Tuhfa 1/399 [IMPLICIT], Bughya [IMPLICIT] |
| 3 red, 14 pure, 17 red | cycle 1 basic, cycle 2 mustahada (rest 16 days) | Bughya, Tuhfa 1/399 |
| 5 red, 25 pure, 5 red, 14 pure, 17 red | cycles 1-2 basic, cycle 3 mustahada | Bughya, Tuhfa 1/399 |
| 15 red, stop | basic | Mughni 1/284, Nihaya 1/339 |
| 5 black, 5 red, stop | basic | Tuhfa 1/399, Nihaya 1/339 |
| 3 red, 12 pure, 3 red, stop | basic (last 3 dam fasad) | Mughni 1/284, Nihaya 1/340, Tuhfa 1/399, FK 1/86 |
| 3 red, 5 pure, 3 red, stop | basic | Nihaya 1/356, Mughni 1/294 |
| 10 red, 4 pure, 1 red (ends at day 15) | basic | Shabramallisi 1/340, Nihaya 1/356 |
| 3 red, 14 pure, 4 red | basic | Bughya, Ibn Qasim 1/399 |
| 10 red, 8 pure, 12 red | basic | Bughya, Ibn Qasim 1/399 |
| 3 red, 14 pure, 16 red | basic (rest exactly 15 days) | Bughya, Ibn Qasim 1/399 |
| 5 red, 25 pure, 5 red, 14 pure, 16 red | basic | Bughya, Ibn Qasim 1/399 |

Existing test `IdentifyComplexIstihadahCaseWithNotImplementedException` (11/1/1/5 and 13/1/1/5)
expects a mustahada case; this is the 13/1/5 pattern and therefore open (open-questions.md).

## Mubtada'a ghayr mumayyiza: a day and a night hayd, 29 days purity

- [Ibn Hajar, Tuhfa 1/404](https://shamela.ws/book/9059/405):
  «(فَالْأَظْهَرُ أَنَّ حَيْضَهَا يَوْمٌ وَلَيْلَةٌ وَ) أَنَّ (طُهْرَهَا تِسْعٌ وَعِشْرُونَ) لِتَيَقُّنِ سُقُوطِ الصَّلَاةِ عَنْهَا فِي الْأَقَلِّ وَمَا بَعْدَهُ مَشْكُوكٌ فِيهِ ... لَكِنَّهَا فِي الدَّوْرِ الْأَوَّلِ تَصْبِرُ إلَى خَمْسَةَ عَشَرَ لَعَلَّهُ يَنْقَطِعُ ... وَعَبَّرَ بِتِسْعٍ وَعِشْرِينَ لَا بِبَقِيَّةِ الشَّهْرِ؛ لِأَنَّ شَهْرَ الْمُسْتَحَاضَةِ الَّذِي هُوَ دَوْرُهَا لَا يَكُونُ إلَّا ثَلَاثِينَ هَذَا كُلُّهُ إنْ عَرَفَتْ وَقْتَ ابْتِدَاءِ الدَّمِ وَإِلَّا فَمُتَحَيِّرَةٌ»
- [al-Khatib, Mughni 1/286](https://shamela.ws/book/11444/202):
  «(فَالْأَظْهَرُ أَنَّ حَيْضَهَا يَوْمٌ وَلَيْلَةٌ) مِنْ أَوَّلِ الدَّمِ وَإِنْ كَانَ ضَعِيفًا؛ لِأَنَّ ذَلِكَ هُوَ الْمُتَيَقَّنُ، وَمَا زَادَ مَشْكُوكٌ فِيهِ فَلَا يُحْكَمُ بِأَنَّهُ حَيْضٌ (وَطُهْرُهَا تِسْعٌ وَعِشْرُونَ) تَتِمَّةُ الشَّهْرِ لِيَتِمَّ الدَّوْرُ ثَلَاثِينَ ... وَالثَّانِي: تَحِيضُ غَالِبَ الْحَيْضِ سِتَّةً أَوْ سَبْعَةً، وَبَقِيَّةُ الشَّهْرِ طُهْرٌ»
- [ar-Ramli, Nihaya 1/342](https://shamela.ws/book/3565/340): same matn and division.
- [Ibn Hajar, al-Fatawa al-Kubra 1/104](https://shamela.ws/book/21628/103): «فَفِي الشَّهْرِ الْأَوَّلِ هِيَ مُبْتَدِئَةٌ لَا تَمْيِيزَ لَهَا وَفِي مَرَدِّهَا الْقَوْلَانِ، أَيْ وَأَصَحُّهُمَا يَوْمٌ وَلَيْلَةٌ وَطُهْرُهَا تِسْعٌ وَعِشْرُونَ»

KHILAF: a day and a night is the azhar; muqabil al-azhar (strong): the usual 6 or 7 days, the rest
of the month purity (Mughni 1/286). Setting `MubtadaaGhayrMumayyizaHayd`. The cycle of a
mustahada is always 30 days (Tuhfa 1/404).

### 15 red, then 15 black, then the black continues: 31 days without prayer

- [Ibn Hajar, Tuhfa 1/402f.](https://shamela.ws/book/9059/403):
  «وَلَوْ رَأَتْ مُبْتَدَأَةٌ خَمْسَةَ عَشَرَ حُمْرَةً ثُمَّ مِثْلَهَا أَسْوَدَ تَرَكَتْ الصَّلَاةَ وَالصَّوْمَ جَمِيعَ الشَّهْرِ؛ لِأَنَّهُ لَمَّا اسْوَدَّ فِي الثَّانِيَةِ تَبَيَّنَ أَنَّ مَا قَبْلَهُ اسْتِحَاضَةً، ثُمَّ إنْ اسْتَمَرَّ الْأَسْوَدُ كَانَتْ غَيْرَ مُمَيِّزَةٍ فَحَيْضُهَا يَوْمٌ وَلَيْلَةٌ مِنْ أَوَّلِ كُلِّ شَهْرٍ وَقَضَتْ الصَّلَاةَ» ([9059/404](https://shamela.ws/book/9059/404))
- [al-Khatib, Mughni 1/287](https://shamela.ws/book/11444/203): «فَرْعٌ: لَوْ رَأَتْ الْمُبْتَدَأَةُ خَمْسَةَ عَشْرَ حُمْرَةً ثُمَّ مِثْلَهَا سَوَادًا تَرَكَتْ الصَّوْمَ وَالصَّلَاةَ ... شَهْرًا ثُمَّ إنْ اسْتَمَرَّ الْأَسْوَدُ فَلَا تَمْيِيزَ لَهَا، وَحَيْضُهَا يَوْمٌ وَلَيْلَةٌ مِنْ أَوَّلِ كُلِّ شَهْرٍ وَتَقْضِي الصَّوْمَ وَالصَّلَاةَ»
- KHILAF (not in the settings, concerns what she does while waiting): the case with five qualities
  of 15 days each (brownish, yellow, ashqar, red, black): Ibn Hajar says it is not analogous
  («خِلَافًا لِجَمْعٍ»); ar-Ramli and al-Khatib side with the group (ash-Sharwani on Tuhfa 1/403).

Tests: `MubtadaaGhayrMumayyiza_ContinuousRedBlood_OneDayHaydAndTwentyNineDaysTuhur`,
`MubtadaaGhayrMumayyiza_FifteenDaysRedThenBlackContinues_OneDayHaydPerMonth`,
`MubtadaaGhayrMumayyiza_SixteenDaysBlack_TamyizFailsSoOneDayHayd`,
`MubtadaaGhayrMumayyiza_AlternatingOneDayBlackTwoDaysRed_TamyizFailsSoOneDayHayd`.

## Mutahayyira

Not implemented (see the project memory "TODO Mutahayyira": needs a new API, because she forgot
her 'ada). Mughni 1/289 ([11444/205](https://shamela.ws/book/11444/205)): the mashhur is ihtiyat;
"fi qawl" she is like a mubtada'a (weak, muqabil al-mashhur).
