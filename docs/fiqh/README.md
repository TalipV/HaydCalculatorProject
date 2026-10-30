# Fiqh reference for the hayd calculator

This folder is the reference for the Shafi'i fiqh behind the calculator and its tests. It is
written first of all for Claude sessions: read the file that matches the question instead of
re-deriving the rules, and add every new finding here, so that the next session starts with it.

Precision matters more than brevity. Every statement carries its source (author, work, volume,
page, Shamela link) and, where it was looked up, the Arabic wording. When a statement here and a
test disagree, find out which one is wrong; do not silently follow either.

## Files

| File | Contents |
|---|---|
| [zero-to-hero.md](zero-to-hero.md) | Beginner's guide through all of the following, in teaching order, with timelines and exercises; adds no rules of its own |
| [sources.md](sources.md) | Allowed works, Shamela ids and page offsets, citation format, approved exceptions (Bughyat al-Mustarshidin), lecture series |
| [basics.md](basics.md) | Definitions, age of hayd, terminology of strength, minimum and maximum of hayd, minimal purity, a day and a night = 24 h, intermittent blood (sahb / laqt), yellow and brownish blood, "all hayd within 15 days" |
| [rest-of-tuhr.md](rest-of-tuhr.md) | Blood that falls into the remainder of the minimal purity: 3/12/3, blood that continues, the Bughya rule for the part after the purity is complete, the 15-day window |
| [mustahada.md](mustahada.md) | When a case is a mustahada case (classification per hayd cycle), the seven kinds, mubtada'a ghayr mumayyiza |
| [tamyiz.md](tamyiz.md) | Strength of blood, the four conditions of tamyiz, order of strong and weak, strong / weak / weaker, red between two blacks |
| [ada.md](ada.md) | Mu'tada: 'ada by one occurrence, changing 'ada, shifted cycle, mu'tada mumayyiza |
| [khilaf.md](khilaf.md) | Every known khilaf with strength, holders, sources and the setting in `HaydCalculatorSettings` |
| [nifas.md](nifas.md) | Nifas and blood in pregnancy (not implemented yet): purity between hayd and nifas, collected quotations |
| [open-questions.md](open-questions.md) | Points the sources checked so far do not settle, with the conflicting passages |
| [review-notes.md](review-notes.md) | Interpretations, single-author points and past mistakes an independent review should check first; review log |

## Rules for every fiqh statement (summary; full text in sources.md)

- Only the mu'tamad of the later Shafi'is: Tuhfat al-Muhtaj, Nihayat al-Muhtaj, Mughni al-Muhtaj,
  their hawashi, the other hawashi of the later Shafi'is (al-Bajuri, al-Bujayrimi, ash-Sharqawi,
  al-Kurdi, at-Tarmasi, ...; user, 2026-10-29), I'anat at-Talibin, Ibn Hajar's al-Fatawa al-Kubra,
  plus the user's sources. Never a matn (al-Minhaj) as evidence, no early or comparative works. Anything
  else only after the user approved it (see "Approved exceptions" in sources.md).
- At least 2, ideally 4 sources from different authors per point. Two books of one author count once.
  A hashiya author (ash-Sharwani, Ibn Qasim, ash-Shabramallisi, ar-Rashidi) counts as his own author.
- Look the passage up and check the wording. Mark a source `[IMPLICIT]` when it states the rule
  but not the numbers of the case.
- State every known khilaf with its strength (see below) and say which view is followed.
- No own opinion. If the sources do not settle a point, it goes into open-questions.md.

## Which view the calculator follows

- Default: the asahh / azhar.
- Where Ibn Hajar (Tuhfa) differs from ar-Ramli (Nihaya) and al-Khatib (Mughni): ar-Ramli / al-Khatib.
- Other strong views are selectable in `HaydCalculatorSettings`
  (`HaydCalculator.Core/Calculator/Models/HaydCalculatorSettings.cs`), presets `IbnHajar`, `Ramli`, `Shirbini`.

Strength of the other view:

| Wording in the source | The other view is | Goes into settings and tests? |
|---|---|---|
| al-asahh (its opposite: as-sahih) | muqabil al-asahh: strong | yes |
| al-azhar (its opposite: az-zahir) | muqabil al-azhar: strong | yes |
| as-sahih | muqabil as-sahih: weak | no, at most a note |
| al-mashhur | muqabil al-mashhur: weak | no, at most a note |
| "wa qila ..." | weak | no, at most a note |

## Terms

| Term | Meaning here |
|---|---|
| hayd | menstruation in the legal sense |
| tuhr | legal purity between two hayds (also while blood flows, e.g. for a mustahada) |
| naqa' | actual purity: the cotton comes out clean (Tuhfa 1/412). Different from fatra (a pause with traces), which is hayd by agreement |
| dam fasad | blood that cannot be hayd (e.g. under 24 h in total, or inside the remaining minimal purity) |
| istihada | blood beyond the limits of hayd; for the calculator "not hayd" |
| mustahada | woman whose blood exceeds 15 days (Mughni 1/285, Nihaya 1/340) |
| mubtada'a | woman at the start of her bleeding, without a previous hayd and tuhr she knows |
| mu'tada | woman with a previous hayd and tuhr she knows ('ada) |
| 'ada | her habit in length and time of hayd and tuhr |
| tamyiz / mumayyiza | distinguishing strong and weak blood / a woman whose blood shows that |
| mutahayyira | mustahada who forgot her 'ada (amount, time or both); not implemented yet |
| strong / weak blood | by colour (black > red > ashqar > yellow > brownish), thickness and bad smell |
| kudra | brownish (turbid) discharge; with sufra (yellow) hayd according to the asahh |
| qawl as-sahb | purity between bloods within 15 days counts as hayd (azhar) |
| qawl al-laqt / at-talfiq | only the bloods count as hayd (muqabil al-azhar) |
| [IMPLICIT] | the source states the rule clearly, but not with the numbers of the case |

## Tests

| Test file (`tests/HaydCalculator.Core.Tests.Unit/Calculator/`) | Topic |
|---|---|
| `HaydCalculatorServiceTests_BasicCalculations.cs` | limits, intermittent blood, rest of tuhr, yellow |
| `HaydCalculatorServiceTests_IstihadaClassification.cs` | mustahada case or basic rules, per hayd cycle |
| `HaydCalculatorServiceTests_ComplexIstihada_Mustahada.cs` | results for each kind of mustahada |
| `HaydCalculatorServiceTests_ComplexIstihada_Tamyiz.cs` | conditions of tamyiz (`HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows`) |
| `HaydCalculatorServiceTests_ComplexIstihada_Ada.cs` | 'ada |
| `HaydCalculatorServiceTests_OpenQuestions.cs` | open questions, fail with `OpenFiqhQuestionException` |

Test conventions (from CLAUDE.md): a source comment above every expected result, a `// KHILAF:`
line, `[FiqhTodo]` until the user checked the result, red tests stay red (TDD). Day numbers in
this folder count from day 1 = the first day of the first blood; the tests use day ranges from 0
(`(0.0, 5.0)` = days 1-5).

## Keeping this folder up to date

- New source checked, new case decided by the user, khilaf found: add it to the matching file,
  with the Arabic wording and the link.
- Correct wrong statements instead of adding contradicting ones.
- A point the user decided against a source: write down the decision and who made it.
- Write English (repository rule); Arabic only for quotations.
