# Khilaf points

Every point where the school has more than one view relevant to the calculator. "Setting" is the
property in `HaydCalculatorSettings`; only views that are at least strong get one. Presets:
`Default` = `Ramli` = `Shirbini`; `IbnHajar` changes the two points where the Tuhfa differs.

## In the settings

| Point | Default view | Other view (strength) | Holders / sources | Setting |
|---|---|---|---|---|
| Purity between bloods within 15 days | hayd, qawl as-sahb (azhar) | tuhr, qawl al-laqt / at-talfiq (muqabil al-azhar, strong) | Tuhfa 1/412, Nihaya 1/356, Mughni 1/293f. | `PurityBetweenBloods` |
| Yellow and brownish blood | hayd (asahh) | not hayd (muqabil al-asahh, strong) | Mughni 1/284, Nihaya 1/340, Tuhfa 1/400 | `YellowAndTurbidAreHayd` |
| Hayd of the mubtada'a ghayr mumayyiza | a day and a night, purity 29 (azhar) | the usual 6 or 7 days, rest of the month purity (muqabil al-azhar, strong) | Tuhfa 1/404, Mughni 1/286, FK 1/104 | `MubtadaaGhayrMumayyizaHayd` |
| Occurrences that establish an 'ada | one (asahh; FK 1/110: asahh of four views by agreement) | two; three (muqabil al-asahh, strong) | Nihaya 1/345, Mughni 1/288, FK 1/110 | `OccurrencesToEstablishAda` |
| Mu'tada mumayyiza | tamyiz decides (asahh) | 'ada decides (muqabil al-asahh, strong) | Nihaya 1/345, Mughni 1/288 | `MutadaMumayyizaFollows` |
| Strong, weak, strong beyond 15 days (7/7/7) | first strong with the weak (ar-Ramli, al-Khatib, after al-Majmu' / ar-Rawda and Ibn Surayj) | only the first strong (Ibn Hajar, after at-Tahqiq and ar-Ruyani; "most of the later scholars") | Mughni 1/286, Nihaya 1/342, Tuhfa 1/401, FK 1/102 and 1/104; lecture part 8 ?t=2342 | `WeakBetweenStrongBeyondMaximum` |
| Strong, weaker, weak (5 black, 5 yellow, then red) | the stronger weak must follow the strong directly, so only the black (ar-Ramli, al-Khatib, after ar-Ruyani, the commentators of al-Hawi as-Saghir, at-Tahqiq) | black and yellow are hayd (Ibn Hajar) | Nihaya 1/342, Mughni 1/286, Tuhfa 1/403f.; lecture part 8 ?t=1961 | `StrongerWeakMustFollowStrongDirectly` |

Note on 7/7/7: Tuhfa 1/401 states it for 7 black, 7 red, 3 black (then it stops); ash-Sharwani
there: the Nihaya and the Mughni disagree with him in that case; Ibn Qasim notes that at-Tahqiq
has 7/7/7. The case 8/8/8 has no khilaf (FK 1/86, "bi-l-ittifaq").

Note on strong / weaker / weak: al-Majmu' and ar-Rawda treat 5 black, 5 yellow, red like red
between two blacks (reported in Mughni 1/286 and Nihaya 1/342); the later works do not follow that.

## Known, but not in the settings

| Point | Views | Why not in the settings | Source |
|---|---|---|---|
| Strong blood that comes later or in the middle | hayd "'ala al-asahh" | the muqabil al-asahh (strong) is not named in the allowed works checked so far; open | Nihaya 1/341f.; Tuhfa 1/402; FK 1/100, 1/102 |
| Full purity between 'ada and tamyiz | both hayd (asahh) / "wa qila" the khilaf applies | "wa qila" is weak | Mughni 1/288f. |
| Yellow inside the 'ada | hayd without khilaf / at-Tatimma: needs a strong blood with it / "wa qila" strong must come first | weak views | Mughni 1/285 |
| Five qualities of 15 days each (waiting period) | Ibn Hajar: not analogous to 15 red / 15 black; ar-Ramli, al-Khatib: analogous | concerns what she does while waiting, not the final result | Tuhfa 1/403 with ash-Sharwani |
| Mutahayyira | ihtiyat (mashhur) / like a mubtada'a ("fi qawl") | weak; mutahayyira not implemented | Mughni 1/289 |
| 'Ada in the second five, blood from the start of the month continuing | the usual five (sahih with al-Muhadhdhab and others) / the first five (Abu l-'Abbas) | strength not checked, no test yet | FK 1/111 |
| 5 red, 5 black, 5 red, stop | all hayd (madhhab, stated decisively) / a wajh on the first red | weak | FK 1/86 |
| Minimum of hayd | a day and a night / a day only (a text of ash-Shafi'i) / a moment ("gharib") | not mu'tamad | Mughni 1/278 |
| Mu'tada ('ada 5), 14 days purity, then continuous blood | mu'tamad: first day istihada, 5 hayd, 15 tuhr, cycle 20 (Tuhfa 1/399, FK 1/111; Abu Makhrama: «فالأصحّ»). Three awjuh (muqabil al-asahh, strong, per the Bughya commentary after al-I'ab): (1) first day istihada, rest of the month (10) + 5 days hayd, then 25 tuhr, old cycle kept; (2) first day istihada, 5 hayd, 25 tuhr, always; (3) all returning blood to the end of the month istihada, old cycle from the next month | sources not approved (Abu Makhrama, Bughya commentary, al-I'ab manuscript); the allowed works name no khilaf; the user earlier removed these ar-Rawda views. User's decision (2026-10-29): KHILAF line only (marked "source not approved") at `Mutada_FourteenDaysPurityThenContinuousBlood_FirstDayCompletesTuhur`, no setting, no test | Bughya p. 277-278 with commentary [615]; review-notes.md E 3a |
| Changing 'ada, last one shorter than an earlier one (10, 5, 4) | hayd = the last 'ada on all views; precaution days up to the longest 'ada (Sharh al-Manhaj, via ash-Shabramallisi; Nihaya 1/345 and Mughni 1/288 for the unordered case) / no precaution (Ibn Qasim after al-'Ubab; wording of Tuhfa 1/405 unless she forgot) | the calculator has no precaution category; strength of the views not stated | ada.md |
| 16 black, then 16 red | not mumayyiza (mu'tamad) / Ibn Surayj: mumayyiza, a day and a night hayd, 15 purity (via the Risala of al-Khatib p. 51-52, after Ibn Hajar's commentary on Ba Qushayr in FK 1/101; **not checked on Shamela yet**) | Ibn Surayj's view here is not called strong anywhere checked | tamyiz.md (test `MubtadaaGhayrMumayyiza_SixteenDaysBlack_TamyizFailsSoOneDayHayd`) |
| Mubtada'a in the Bughya case (rest after the purity, then it stops) | the rest is hayd for every woman (Bughya, Ibn Qasim "la yab'ud") | ash-Shabramallisi only remarks that Ibn Hajar's wording might imply something else for the mubtada'a; no view held | Nihaya 1/340; rest-of-tuhr.md |

## No khilaf (stated so in the sources)

- Blood within the remaining purity that then stops is dam fasad (FK 1/86: «وَلَا خِلَافَ فِي شَيْءٍ مِنْ هَذَا»).
- 8 black, 8 red, then black: the first black (FK 1/86, bi-l-ittifaq).
- Strong blood that comes first is hayd "qat'an" (Nihaya 1/341).
- The fatra (pause with traces) is hayd by agreement (Tuhfa 1/412).
- No maximum of purity (Tuhfa 1/385, by consensus).
- 'Ada of 5, blood returns in the first five and continues: these five are hayd by agreement (FK 1/111).
