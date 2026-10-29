# Open questions

Points the sources checked so far do not settle. Tests for them live in
`HaydCalculatorServiceTests_OpenQuestions` and throw `OpenFiqhQuestionException`, or are
marked in their comments. Do not decide them yourself; collect the passages and ask the user.

## 1. Returning blood that starts inside the 15 days, runs past day 15 and then stops (13/1/5)

Case: 13 red, 1 pure, 5 red, then it stops (19 days). Also the existing test
`IdentifyComplexIstihadahCaseWithNotImplementedException` (11/1/1/5 and 13/1/1/5), which expects
a mustahada case. The Bughya rule does not apply directly: the first blood and the purity
together are under 15 days, so the returning blood starts inside the 15 days (rest-of-tuhr.md).

Passages:

- Mughni 1/294 ([11444/210](https://shamela.ws/book/11444/210)): «وَإِنْ زَادَتْ مَعَ النَّقَاءِ بَيْنَهَا عَلَى خَمْسَةَ عَشَرَ يَوْمًا فَهِيَ دَمُ اسْتِحَاضَةٍ»;
  Nihaya 1/356 ([3565/354](https://shamela.ws/book/3565/354)): «وَلَوْ عَبَرَ التَّقَطُّعُ خَمْسَةَ عَشَرَ جَاءَ مَا مَرَّ فِي الْمُسْتَحَاضَاتِ».
  Read alone: a mustahada case (for a mubtada'a ghayr mumayyiza only a day and a night of hayd).
- 3/12/3 (Mughni 1/284, Nihaya 1/340, Tuhfa 1/399): blood that stops inside the remaining purity
  is dam fasad. Applied here: 13 days hayd, the 5 days dam fasad.
- Ash-Shabramallisi, Nihaya 1/340 ([3565/338](https://shamela.ws/book/3565/338)): the general rule
  is restricted by the 3/12/3 case («يُخَصُّ ذَاكَ بِهَذَا»): if it stops at the end of the 15
  days or within them, blood and purity are hayd. He does not say where the line runs otherwise.
- **New (2026-10-29): Ibn Hajar, al-Fatawa al-Kubra 1/86 ([21628/85](https://shamela.ws/book/21628/85))**, two statements:
  1. A general rule with "no khilaf":
     «وَكَذَا لَوْ رَأَتْ أَوَّلًا يَوْمًا وَلَيْلَةً أَوْ أَكْثَرَ ثُمَّ طَهُرَتْ، وَرَأَتْ دَمًا قَبْلَ خَمْسَةَ عَشَرَ مِنْ أَوَّلِ الطُّهْرِ، وَانْقَطَعَ بِحَيْثُ لَا يُمْكِنُ كَوْنُ الدَّمَيْنِ فِي خَمْسَةَ عَشَرَ وَلَا خِلَافَ فِي شَيْءٍ مِنْ هَذَا»
     (first blood of a day and a night or more, then purity, then blood before 15 days from the
     start of the purity, stopping so that the two bloods cannot lie within 15 days: like 3/12/3,
     i.e. the second blood is dam fasad). 13/1/5 fits this wording: the blood comes 1 day after the
     start of the purity, and the two bloods (19 days) cannot lie within 15 days. Read alone, this
     would mean "13 days hayd, 5 days dam fasad, no mustahada case"; but see the research below:
     the worked examples of this rule all have the second blood start after day 15.
  2. Right after it:
     «وَكَذَا لَوْ رَأَتْ يَوْمًا وَلَيْلَةً دَمًا ثُمَّ ثَلَاثَةَ عَشَرَ نَقَاءً ثُمَّ ثَلَاثَةً دَمًا، فَحَيْضُهَا الثَّلَاثَةُ الْأَخِيرَةُ عَلَى الْحَاصِلِ مِنْ رَاجِحِ الْمَذْهَبِ ذَكَرَ هَذَا النَّوْعَ فِي فَصْلِ التَّلْفِيقِ آخِرَ الْحَيْضِ، وَفِي أَوَّلِهِ إشَارَةٌ إلَيْهِ»
     (a day and a night blood, 13 pure, 3 blood: her hayd is the last three, according to the
     preponderant view of the school, from the chapter on talfiq). Here too the two bloods cannot
     lie within 15 days (17 days), yet the result is the opposite: the last blood is the hayd, not
     the first. The difference to statement 1 is not explained on this page (first blood exactly a
     day and a night? the returning blood starts on day 15, inside the 15 days of the first?).
     Not understood yet.
  The page is a list of masa'il that Ibn Hajar reports ("ذكره", "ذكر هذا النوع").

Also affected: the classification test `TenDaysRedThreeDaysPurityThenTwentyDaysRed_IsMustahadaCase`
(10 red, 3 pure, 20 red). It expects a mustahada case because the returning blood starts inside
the 15 days and runs far beyond them. If the reasoning of statement 1 and the Bughya rule were
carried over to returning blood that starts inside the 15 days, days 14-25 would be dam fasad and
days 26-33 (8 days) hayd, i.e. no mustahada case. No source says so for this constellation.

### Research in the talfiq passages (2026-10-29)

Searched: the naqa' / talfiq passage at the end of the hayd chapter in the Tuhfa (1/412, both
parts, with ash-Sharwani and Ibn Qasim), the Nihaya (1/356, both parts, with ash-Shabramallisi),
the Mughni (1/293-294); phrase searches over all downloaded Shafi'i books for «عبر التقطع» and
«ثلاثة عشر نقاء».

1. **The three shuruh have no detailed talfiq chapter.** At the end of the chapter they only give
   the matn on naqa' (sahb azhar, laqt the other view), its three conditions and one sentence on
   crossing 15 days: Mughni 1/294 «وَإِنْ زَادَتْ مَعَ النَّقَاءِ بَيْنَهَا عَلَى خَمْسَةَ عَشَرَ يَوْمًا فَهِيَ دَمُ اسْتِحَاضَةٍ»,
   Nihaya 1/356 «وَلَوْ عَبَرَ التَّقَطُّعُ خَمْسَةَ عَشَرَ جَاءَ مَا مَرَّ فِي الْمُسْتَحَاضَاتِ»; the Tuhfa (1/412) not even that.
   The hawashi there (ash-Sharwani, Ibn Qasim, ash-Shabramallisi) discuss the wording of the matn,
   fatra and nifas, not crossing 15 days. None of them gives 13/1/5, 1/13/3 or 10/3/20. The
   "fasl at-talfiq" that FK 1/86 refers to is a chapter of ar-Rawda / al-Majmu' (not usable directly).

2. **Ibn Hajar, al-Fatawa al-Kubra 1/108-109 ([21628/107](https://shamela.ws/book/21628/107), part 2, and [21628/108](https://shamela.ws/book/21628/108))**
   summarises that chapter of al-Majmu' while answering an author he criticises («الْمُصَنِّف»):
   > «بَسَطَ ذَلِكَ فِي الْمَجْمُوعِ بِمَا حَاصِلُهُ رَأَتْ ثَلَاثَةً دَمًا ثُمَّ اثْنَيْ عَشَرَ نَقَاءً ثُمَّ ثَلَاثَةً دَمًا ثُمَّ انْقَطَعَ فَالثَّلَاثَةُ الْأُولَى حَيْضٌ لِأَنَّهُ فِي زَمَنِ الْإِمْكَانِ وَالثَّلَاثَةُ الْأَخِيرَةُ دَمُ فَسَادٍ لَا حَيْضٍ مَعَ الْأُولَى لِمُجَاوَزَتِهِ خَمْسَةَ عَشَرَ وَلَا مُنْفَرِدًا لِأَنَّهُ لَمْ يَتَقَدَّمْهُ أَقَلُّ طُهْرٍ وَهَكَذَا لَوْ رَأَتْ يَوْمًا وَلَيْلَةً دَمًا أَوْ يَوْمَيْنِ أَوْ ثَلَاثَةً أَوْ أَرْبَعَةً أَوْ خَمْسَةً أَوْ سِتَّةً أَوْ غَيْرَ ذَلِكَ ثُمَّ رَأَتْ النَّقَاءَ تَمَامَ خَمْسَةَ عَشَرَ ثُمَّ رَأَتْ يَوْمًا وَلَيْلَةً فَأَكْثَرَ دَمًا فَالْأَوَّلُ حَيْضٌ وَالْأَخِيرُ دَمُ فَسَادٍ وَلَا خِلَافَ فِي شَيْءٍ مِنْ هَذَا وَلَوْ رَأَتْ دَمًا دُونَ يَوْمٍ وَلَيْلَةٍ نَقَاءً تَمَامَ خَمْسَةَ عَشَرَ ثُمَّ دَمًا يَوْمًا وَلَيْلَةً أَوْ ثَلَاثَةَ أَيَّامٍ أَوْ خَمْسَةً وَنَحْوَ ذَلِكَ فَالْأَوَّلُ دَمُ فَسَادٍ وَالثَّانِي حَيْضٌ لِوُقُوعِهِ فِي زَمَنِ الْإِمْكَانِ وَلَوْ رَأَتْ نِصْفَ يَوْمٍ دَمًا ثُمَّ تَمَامَ خَمْسَةَ عَشَرَ نَقَاءً ثُمَّ نِصْفَ يَوْمٍ دَمًا فَالدَّمَانِ جَمِيعًا دَمُ فَسَادٍ وَلَا حَيْضَ لَهَا اتِّفَاقًا لِعَدَمِ إمْكَانِ ضَمِّ أَحَدِهِمَا إلَى الْآخَرِ لِمُجَاوَزَةِ خَمْسَةَ عَشَرَ. وَلَوْ رَأَتْ يَوْمًا بِلَا لَيْلَةٍ دَمًا ثُمَّ ثَلَاثَةَ عَشَرَ نَقَاءً ثُمَّ ثَلَاثَةَ أَيَّامٍ دَمًا فَقَدْ رَأَتْ فِي الْخَمْسَةَ عَشَرَ يَوْمَيْنِ دَمًا فِي أَوَّلِهَا يَوْمًا وَفِي آخِرِهَا يَوْمًا فَإِنْ قُلْنَا لَا تَلْفِيقَ فَحَيْضُهَا الدَّمُ الثَّانِي كُلُّهُ وَالْأَوَّلُ دَمُ فَسَادٍ وَإِنْ لَفَّقْنَا مِنْ الْعَادَةِ فَكَذَلِكَ لِأَنَّ الْمُبْتَدِئَةَ تُرَدُّ إلَى يَوْمٍ وَلَيْلَةٍ وَلَيْسَ فِي هَذَا الزَّمَانِ مَا يُمْكِنُ جَعْلُهُ حَيْضًا وَإِنْ لَفَّقْنَا مِنْ مُدَّةِ الْإِمْكَانِ وَهِيَ خَمْسَةَ عَشَرَ فَإِنْ قُلْنَا الْمُبْتَدِئَةُ تُرَدُّ إلَى يَوْمٍ وَلَيْلَةٍ حَيَّضْنَاهَا الْيَوْمَ الْأَوَّلَ وَمِنْ الْخَامِسَ عَشَرَ مِقْدَارَ لَيْلَةٍ لِيَتِمَّ لَهَا يَوْمٌ وَلَيْلَةٌ اهـ»

   And Ibn Hajar's own comment on the last option (1/109):
   > «قَدْ عُلِمَ مِمَّا ذَكَرْته آنِفًا عَنْ الْمَجْمُوعِ أَنَّ صَلَاحِيَّةَ الْأَوَّلِ لِاجْتِمَاعِهِ بِبَعْضِ الثَّلَاثَةِ الْأَخِيرَةِ إنَّمَا يَتَأَتَّى عَلَى الضَّعِيفِ الَّذِي مَرَّ آخِرَ عِبَارَةِ الْمَجْمُوعِ السَّابِقَةِ عَنْهُ»

   Findings from this passage:
   - **1/13/3 is explained.** In al-Majmu' (as quoted here) the first blood is «يَوْمًا بِلَا لَيْلَةٍ»,
     a day without its night, i.e. under 24 hours. Such a blood cannot be a hayd on its own, so
     the whole second blood (3 days) is the hayd and the first is dam fasad; joining the first day
     to part of the last blood is only possible on a weak view (talfiq from the period of
     possibility). FK 1/86 has «يَوْمًا وَلَيْلَةً» at the same place; given FK 1/108, that is most
     likely a slip in the text of 1/86 or of its source. With a first blood of a full day and a
     night, the general rule of 3/12/3 applies (first blood hayd, second dam fasad). So there is
     no contradiction to statement 1 left, as long as the first blood is under 24 h in 1/13/3.
     (This reading of the slip is mine; the user should confirm it.)
   - **The 3/12/3 reasoning:** the second blood is not hayd together with the first «لِمُجَاوَزَتِهِ خَمْسَةَ عَشَرَ»
     and not on its own «لِأَنَّهُ لَمْ يَتَقَدَّمْهُ أَقَلُّ طُهْرٍ», and «لَا خِلَافَ فِي شَيْءٍ مِنْ هَذَا».
   - **Stopping versus continuing:** on the same pages (1/108 and 1/109) Ibn Hajar repeats against
     that author that the cases where the blood stops and the cases where the intermittent blood
     continues beyond 15 days are different: «لِأَنَّهَا فِيمَا إذَا اسْتَمَرَّ الْأَسْوَدُ ... وَهُنَّ فِيمَا إذَا انْقَطَعَ»,
     «أَنَّ تِلْكَ الثَّلَاثَ فِيمَا إذَا انْقَطَعَ وَمَسْأَلَةُ يَوْمٍ وَلَيْلَةٍ أَسْوَدُ وَمِثْلُهُمَا أَحْمَرُ وَهَكَذَا حَتَّى جَاوَزَ خَمْسَةَ عَشَرَ فِيمَا إذَا اسْتَمَرَّ التَّقَطُّعُ كَذَلِكَ»,
     and «هَذَا مَبْنِيٌّ عَلَى إشْكَالَاتِهِ السَّابِقَةِ الْمَبْنِيَّةِ كُلِّهَا عَلَى تَوَهُّمِهِ أَنَّهُ لَا فَرْقَ بَيْنَ الْمُتَقَطِّعِ وَالْمُسْتَمِرِّ».
     **Correction (re-checked):** this distinction is about tamyiz (al-Mutawalli's point that
     condition 3 is only needed if the blood continues; alternating black / red), not about
     intermittent blood with purity that crosses 15 days. It does not support 13/1/5 as basic.
   - Ash-Shabramallisi's restriction (Nihaya 1/340) explains itself as «إنْ قَطَعَ عَلَى رَأْسِ الْخَمْسَةَ عَشَرَ أَوْ فِيهَا كَانَ الدَّمُ مَعَ النَّقَاءِ حَيْضًا»:
     it says nothing about blood that stops after day 15. It does not support 13/1/5 as basic either.
   - Every case the sources work through with numbers has the second blood start **after** day 15
     of the first (3/12/3; in the al-Majmu' summary of FK 1/108 «ثُمَّ رَأَتْ النَّقَاءَ تَمَامَ خَمْسَةَ عَشَرَ»,
     purity up to the completion of 15 days; the Bughya condition). In 13/1/5 the second blood
     starts inside the 15 days and runs past them: that is the case "intermittent blood crosses
     15 days", for which Mughni 1/294 and Nihaya 1/356 say istihada / the mustahada rules.

3. **What this means for the cases** (assessment after re-checking; decision with the user):

   | Case | For a mustahada case | For "basic" | Reading |
   |---|---|---|---|
   | 13/1/5 (stops) | Mughni 1/294 and Nihaya 1/356 (intermittent blood crossing 15 days, two authors, directly on point); FK 1/102 (exceeding 15 days makes her a mustahada even if the blood stops later: «الشَّامِلُ لِمَا إذَا اسْتَمَرَّ الدَّمُ أَوْ انْقَطَعَ») | only the broad wording of FK 1/86 rule 1 («دَمًا قَبْلَ خَمْسَةَ عَشَرَ مِنْ أَوَّلِ الطُّهْرِ»), whose worked examples are all of the "second blood after day 15" type | leans to a mustahada case (what the tests expect); not certain, no source works through this constellation |
   | 11/1/1/5 and 13/1/1/5 (existing test, stop) | same | same | same; keep the test as it is |
   | 1/13/3 | none | FK 1/108 (al-Majmu': first blood «يَوْمًا بِلَا لَيْلَةٍ») | first blood under 24 h: the 3 days are hayd, the first blood dam fasad. FK 1/86's «يَوْمًا وَلَيْلَةً» is most likely a slip (my reading). No test covers 1/13/3 yet (`HaydCalculation_OneDayOfBloodWithinRemainingMinimalTuhur_IsDamFasad` is 10/10/1) |
   | 10/3/20 (continues long) | Mughni 1/294, Nihaya 1/356 | none | mustahada case, as the test expects |

   Still to look for: a later Shafi'i text that works through a returning blood that starts inside
   the 15 days and stops after day 15 (I'anat at-Talibin and Hashiyat al-Bajuri not searched yet).
### Photos of the Bughya commentary, pp. 278-279 (sent by the user, 2026-10-29)

Neither the commentary nor what it quotes is approved as a source (sources.md). Recorded here as
leads only.

- **Main text of the Bughya, top of p. 278** (a fatwa it quotes; fn. 1: al-Fatawa al-Hijraniyya,
  al-Ifada al-Hadramiyya, i.e. Abu Makhrama), for a mu'tada with an 'ada of 5:
  «وكالأربعةَ عشرَ ما دونَها إلى العشرةِ ؛ فما يكمِّلُ الخمسةَ عشرَ استحاضةٌ ، ثمَّ خمسةٌ حيضٌ ، وخمسةَ عشرَ طهرٌ ، بخلافِ ما لو نقصَ النقاءُ عنِ العشرةِ ؛ فليسَ بطهرٍ ؛ لأنَّ الدمَ الذي بعدَهُ يجتمعُ معَ الذي قبلَهُ حيضاً) انتهى ، وفيهِ مخالفةٌ لِمَا ذُكِرَ ، فتأمَّلْهُ»
  Purity of 14 down to 10 days after her 5: the returning blood completes the 15, then 5 hayd, 15
  tuhr. If the purity is under 10 days, it is not tuhr, because the returning blood (which then
  starts inside the 15 days) joins the first as hayd. This is exactly the 13/1/5 line: returning
  blood that starts inside the 15 days is joined to the first hayd, not treated as rest of tuhr.
- **Commentary [616] on «فليسَ بطهرٍ»:** «أي : إن لم يجاوزِ المجموعُ خمسةَ عشرَ ، وإلَّا .. فالجميعُ استحاضةٌ ، قالَ في «المجموعِ» : (لو رأتْ ستةَ أيامٍ دماً ، ثمَّ ستةَ نقاءً ، ثمَّ ستةَ دماً .. فهيَ في الجميعِ مستحاضةٌ باتفاقِ الأصحابِ) انتهى»
  (only if the whole does not exceed 15; otherwise it is all istihada), citing al-Majmu' 2/469
  (another edition). Checked on Shamela: al-Majmu' (Muniriyya) 2/507, [2186/1035](https://shamela.ws/book/2186/1035):
  «فَلَوْ اتَّصَلَ الدَّمُ بِالدَّمِ بِأَنْ رَأَتْ سِتَّةَ أَيَّامٍ دَمًا ثُمَّ سِتَّةً نَقَاءً ثُمَّ سِتَّةً دَمًا فَالسَّادِسَ عَشَرَ فِيهِ دَمٌ مُتَّصِلٌ بِدَمِ الْخَامِسَ عَشَرَ فَقَدْ وَافَقَ ابْنُ بِنْتِ الشَّافِعِيِّ وَغَيْرُهُ الْأَصْحَابَ وَقَالَ هِيَ فِي الْجَمِيعِ مُسْتَحَاضَةٌ»
  Context check: the passage is about intermittent blood that exceeds 15 days (Ibn bint
  ash-Shafi'i held that day 16 onwards is tuhr if day 16 is pure; the Ashab reject that). 6/6/6 is
  his own example where day 15 and 16 are both blood. Whether the blood stops after day 18 is not
  said there; the chapter is about blood that crosses 15 days, not explicitly about blood that
  stops. Still, 6/6/6 is structurally the 13/1/5 case (returning blood starts inside the 15 days,
  ends after day 15), and the answer is "mustahada, by agreement".
- Net effect for 13/1/5: two further leads (Abu Makhrama's fatwa in the Bughya, al-Majmu' via the
  commentary) point to **a mustahada case**, i.e. to what the tests expect. Neither is an allowed
  source yet; the user decides whether to approve them.

**Decision of the user (2026-10-29): 13/1/5 stays open.** The tests stay as they are:
`ThirteenRedOnePurityFiveRed_DamFasadOrMustahada` keeps throwing `OpenFiqhQuestionException`,
`IdentifyComplexIstihadahCaseWithNotImplementedException` (11/1/1/5, 13/1/1/5) keeps expecting a
mustahada case, `TenDaysRedThreeDaysPurityThenTwentyDaysRed_IsMustahadaCase` keeps expecting a
mustahada case. Changing any of them needs a new decision of the user.

## 2. Who holds the muqabil al-asahh for strong blood that comes later

Nihaya 1/341f.: strong blood in the middle or later is hayd «عَلَى الْأَصَحِّ». The other view
(strong) and its result for cases like 10 red, 10 black, stop are not named in the allowed works
checked so far. Needed for a KHILAF line with content and possibly a setting.

## 3. 8 black, 8 red, 8 black: the last day

FK 1/86 gives only the first black as hayd (bi-l-ittifaq). The last day of the second black (day
24) lies after a full minimal purity of 15 days following the first black. The sources do not
discuss that day. Test `MubtadaaMumayyiza_EightBlackEightRedEightBlack_OnlyFirstBlackIsHayd` notes it.

## 4. Condition 4 with a long weak blood at the end

Test `Condition4_ContinuouslyBlackAndRedAndAtTheEndLongRed` (1 black / 1 red six times, then 10
red) is skipped and has no source yet.

## 5. Mutahayyira

Not implemented; needs a new API (the woman forgot her 'ada). Rules: Mughni 1/289ff., Tuhfa and
Nihaya in the same chapter (not collected yet).

## 6. Exactly a day and a night as first blood in the Bughya case

Bughya describes the first blood as «زادَ علىٰ يومٍ وليلةٍ» (more than a day and a night). For a
first blood of exactly 24 h (1 red, 14 pure, 20 red) the result is taken over as [IMPLICIT]; see
also statement 2 under question 1, which is about exactly such a first blood.

## 7. Mubtada'a whose returning blood continues: does she become a mustahada from the first blood on?

Found on 2026-10-29 while checking the findings of another session (Bughya commentary, see
review-notes.md, section E). Affects the classification tests
`ThreeDaysRedTwelveDaysPurityThenTwentyDaysRed_FirstCycleNoMustahadaCaseSecondCycleMustahadaCase`
and `OneDayRedFourteenDaysPurityThenTwentyDaysRed_FirstCycleNoMustahadaCaseSecondCycleMustahadaCase`
(both expect: cycle 1 basic, cycle 2 mustahada), and possibly
`ThreeDaysRedFourteenDaysPurityThenSeventeenDaysRed_...`.

Passages:

- Ibn Hajar, Tuhfa 1/399 ([9059/400](https://shamela.ws/book/9059/400)), on 3/12/3 when the blood
  continues: «فَإِنْ كَانَتْ مُبْتَدَأَةً فَغَيْرُ مُمَيِّزَةٍ أَوْ مُعْتَادَةٍ عَمِلَتْ بِعَادَتِهَا»
- Hawashi there (part 2 of the page):
  - al-Basri: the wording misleads; "a day and a night" would fit better than "ghayr mumayyiza":
    «لَا يَخْفَى مَا فِي هَذَا الصَّنِيعِ مِنْ إيهَامِ أَنَّ الْمُعْتَادَةَ فِي هَذَا الْحَالِ مُمَيِّزَةٌ فَالْأَنْسَبُ فَيَوْمٌ وَلَيْلَةٌ بَدَلَ فَغَيْرُ مُمَيِّزَةٍ»
  - al-Bujayrimi (quoted by ash-Sharwani): "ghayr mumayyiza" means one whose tamyiz lacks a
    condition, **because the time of the purity counts as weak blood and is under the minimal purity**:
    «وَقَوْلُ ابْنِ حَجَرٍ فَغَيْرُ مُمَيِّزَةٍ أَيْ مُسْتَكْمِلَةٌ لِلشُّرُوطِ فَلَا يُنَافِي أَنَّهَا تُسَمَّى مُمَيِّزَةً فَاقِدَةَ شَرْطٍ ... وَإِنَّمَا كَانَتْ فَاقِدَةَ شَرْطِ تَمْيِيزٍ لِأَنَّ زَمَنَ النَّقَاءِ حُكْمُهُ حُكْمُ الضَّعِيفِ، وَقَدْ نَقَصَ عَنْ أَقَلِّ الطُّهْرِ»
  - ash-Shabramallisi (same page and Nihaya 1/340): Ibn Hajar's wording implies «أَنَّ الْمُبْتَدَأَةَ تَحِيضُ يَوْمًا وَلَيْلَةً مِنْ أَوَّلِ الشَّهْرِ».
- Bughya (Dar al-Minhaj) 1/277 (approved exception): when the rest exceeds 15 days, «فتأخذُ المُبتدَأةُ غيرُ المميِّزةِ مِنْ أولِ الزائدِ يوماً وليلةً» (a day and a night **from the start of the rest**).

The conflict: on the Tuhfa reading of al-Bujayrimi / ash-Shabramallisi, the mubtada'a in 3/12/
continuing is a mumayyiza lacking a condition from the first blood on, with a day and a night of
hayd from the start of the month (= day 1). Then the first 3 days are not a basic hayd; the whole
course is one mustahada case from day 1. On the Bughya, the first blood is hayd and the
mustahada rules start with the rest. The tests follow the Bughya reading.

Also: the Bughya condition is that first blood and purity together **exceed** 15 days
(«زادَ عليها»). In 3/12/20 and 1/14/20 they are exactly 15; only 3/14/17 meets it strictly. Tuhfa
1/399 itself is 3/12 (exactly 15). So for exactly these two tests the Tuhfa passage is the closer
source, and its hawashi read it against "cycle 1 basic".

Ibn Hamid (Bughya commentary p. 279, fn. 6; photo sent by the user 2026-10-29; not an approved source):
«استشكلَهُ ابنُ حامدٍ في «فتاويهِ» : بأنَّهُ كيفَ تكونُ مبتدأةً وقد سبقَ لها عادةُ حيضٍ وطهرٍ ؟! وأجابَ عنهُ : بأنَّ عبارةَ «التحفةِ» محرَّفةٌ ، وصوابُها : (فإنْ كانتْ مميزةً .. عملتْ بتمييزِها ، أو معتادةً .. عملتْ بعادتِها) ، ودورُها «18» يوماً»
(Fatawa Ibn Hamid pp. 185-186). His argument: after 3 days hayd and a completed purity she
already has an 'ada of hayd and tuhr, so she cannot be a mubtada'a; her cycle is 18 days (3 hayd,
15 tuhr). That is the reading the tests follow (cycle 1 basic, then her 'ada). Against him:
al-Bujayrimi (via ash-Sharwani, allowed) defends the text; ash-Shabramallisi (allowed) reads it
as a day and a night from the start of the month. So the allowed hawashi lean against the
tests here, the unapproved Ibn Hamid and Bughya for them.

For the mu'tada (Tuhfa's own example 'ada 5, 14 pure, continuing, and FK 1/111) there is no such
conflict: the first day of the returning blood completes the purity, then her 'ada.

To resolve with the user: which reading the tests follow for the mubtada'a (Bughya: cycle 1 basic;
Tuhfa with al-Bujayrimi / ash-Shabramallisi: mustahada from day 1). The tests are unchanged.

## Resolved

- 3/14/4 and 10/8/12 (blood passes the completion of the purity and then stops): resolved on
  2026-10-29 with Bughyat al-Mustarshidin, Dar al-Minhaj ed., vol. 1, p. 277 (approved exception) and Ibn Qasim's "la yab'ud":
  the part that completes the purity is dam fasad, the rest is hayd (rest-of-tuhr.md). The open
  question test was removed.
