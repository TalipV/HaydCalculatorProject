# Zero to hero: calculating hayd days in the Shafi'i school

This guide teaches the fiqh behind the calculator from the ground up. You do not need any prior
knowledge of fiqh. When you finish it, you should be able to take any course of bleeding
(e.g. "3 days red, 12 days pure, 3 days red") and work out which days are hayd, which are tuhr,
and which are neither. You should also know why, and which book says so.

This guide does not add any new rules. It collects and orders what the other files in this
folder say, and links back to them. Those files hold the full Arabic quotations, the page links and
the reasoning. If this guide and a topic file disagree, the topic file is right; correct the
guide. No view is preferred here except the one the sources and the calculator's defaults state.
Open questions are labelled as open.

---

## Contents

- [How to read this guide](#how-to-read-this-guide)
- **Part I: Foundations**
  - [1. The four kinds of blood](#1-the-four-kinds-of-blood)
  - [2. Where the rules come from](#2-where-the-rules-come-from)
  - [3. Measuring time and blood](#3-measuring-time-and-blood)
  - [4. The three numbers (and the age)](#4-the-three-numbers-and-the-age)
- **Part II: Cases within the limits**
  - [5. Blood within the limits is all hayd](#5-blood-within-the-limits-is-all-hayd)
  - [6. Intermittent blood: sahb and laqt](#6-intermittent-blood-sahb-and-laqt)
  - [7. Yellow and brownish discharge](#7-yellow-and-brownish-discharge)
  - [8. The rest of the minimal purity (baqiyyat tuhr)](#8-the-rest-of-the-minimal-purity-baqiyyat-tuhr)
- **Part III: The mustahada (blood beyond 15 days)**
  - [9. When she becomes a mustahada, and the seven kinds](#9-when-she-becomes-a-mustahada-and-the-seven-kinds)
  - [10. Strong and weak blood](#10-strong-and-weak-blood)
  - [11. Tamyiz: the mubtada'a mumayyiza](#11-tamyiz-the-mubtadaa-mumayyiza)
  - [12. The mubtada'a ghayr mumayyiza](#12-the-mubtadaa-ghayr-mumayyiza)
  - [13. The mu'tada: a woman with a habit ('ada)](#13-the-mutada-a-woman-with-a-habit-ada)
  - [14. The mutahayyira](#14-the-mutahayyira)
- **Part IV: Around the core**
  - [15. Nifas and pregnancy](#15-nifas-and-pregnancy)
  - [16. Map of the khilaf and the settings](#16-map-of-the-khilaf-and-the-settings)
  - [17. Open questions](#17-open-questions)
  - [18. From fiqh to code](#18-from-fiqh-to-code)
- **Appendix**
  - [A. The whole procedure in one flow](#a-the-whole-procedure-in-one-flow)
  - [B. Exercises with solutions](#b-exercises-with-solutions)
  - [C. Glossary](#c-glossary)

---

## How to read this guide

**Notation.** A course of bleeding is written as blocks of days separated by slashes:

| Notation | Meaning |
|---|---|
| `3/12/3` | 3 days blood, 12 days pure, 3 days blood (blood and purity alternate, starting with blood) |
| `3 red, 12 pure, 3 red, stop` | the same, with the colour named and with "stop" = nothing comes after |
| `... continues` / `then red continues` | the last blood goes on (for a long time, at least beyond 15 days) |
| `7/7/7` (in tamyiz cases) | 7 black, 7 red, 7 black: here the blocks are qualities of blood, not blood / purity |

**Day numbers** count from day 1 = the first day of the first blood (the same as in the other
files of this folder). The tests use day ranges starting at 0: `(0.0, 5.0)` = days 1-5.

**Timelines** look like this (R = red, B = black, Y = yellow, . = pure):

```
day     1  2  3  4  5  6  7  8  9 10 11 12 13 14 15 16 17 18
blood   R  R  R  .  .  .  .  .  .  .  .  .  .  .  .  R  R  R
ruling  H  H  H  T  T  T  T  T  T  T  T  T  T  T  T  F  F  F     H = hayd, T = tuhr, F = dam fasad
```

**Source references** are short: "Tuhfa 1/399" = Ibn Hajar, Tuhfat al-Muhtaj, vol. 1, printed page
399. The topic files give the Shamela link for each one. Abbreviations:

| Short | Work | Author |
|---|---|---|
| Tuhfa | Tuhfat al-Muhtaj (with the hawashi of ash-Sharwani and Ibn Qasim) | Ibn Hajar al-Haytami (d. 974) |
| Nihaya | Nihayat al-Muhtaj (with the hawashi of ash-Shabramallisi and ar-Rashidi) | Shams ad-Din ar-Ramli (d. 1004) |
| Mughni | Mughni al-Muhtaj | al-Khatib ash-Shirbini (d. 977) |
| FK | al-Fatawa al-Kubra al-Fiqhiyya | Ibn Hajar al-Haytami |
| Bughya | Bughyat al-Mustarshidin (Dar al-Minhaj ed.) | Ba 'Alawi (approved exception) |
| al-Ibana | al-Ibana wa-l-Ifada (3rd ed.) | as-Saqqaf (user's source) |

---

# Part I: Foundations

## 1. The four kinds of blood

Everything the calculator does is sorting days into categories. The sources name these:

| Term | Definition in the sources |
|---|---|
| **hayd** (menstruation) | Mughni 1/277: «دَمُ جِبِلَّةٍ أَيْ تَقْتَضِيهِ الطِّبَاعُ السَّلِيمَةُ يَخْرُجُ مِنْ أَقْصَى رَحِمِ الْمَرْأَةِ بَعْدَ بُلُوغِهَا عَلَى سَبِيلِ الصِّحَّةِ مِنْ غَيْرِ سَبَبٍ فِي أَوْقَاتٍ مَعْلُومَةٍ»: a natural blood that comes out of the far end of the womb after puberty, in health, without a cause, at known times. Tuhfa 1/383: «دَمُ جِبِلَّةٍ يَخْرُجُ فِي وَقْتٍ مَخْصُوصٍ». |
| **istihada** | Mughni 1/277: «دَمُ عِلَّةٍ يَسِيلُ مِنْ عِرْقٍ مِنْ أَدْنَى الرَّحِمِ يُقَالُ لَهُ الْعَاذِلُ»: a blood of illness from a vein near the opening of the womb. Tuhfa 1/383: istihada is everything that is neither hayd nor nifas («مَا عَدَاهُمَا عَلَى الْأَصَحِّ»). |
| **nifas** | Mughni 1/277: «الدَّمُ الْخَارِجُ بَعْدَ فَرَاغِ الرَّحِمِ مِنْ الْحَمْلِ»: blood after the womb is empty of the pregnancy. Not implemented yet (section 15). |
| **dam fasad** | "corrupt blood": blood that cannot be hayd, e.g. a blood under 24 hours in total, or blood inside the rest of the minimal purity (sections 4 and 8). |

Two words for the time **between** hayds:

- **tuhr**: purity in the legal sense. A mustahada can be in tuhr although she bleeds.
- **naqa'**: actual cleanness. The cotton comes out clean (Tuhfa 1/412).

**Istihada and dam fasad.** The definitions above overlap. Mughni 1/277 mentions a khilaf about
the blood of a girl too young for hayd: the asahh is that it is called both istihada and dam
fasad. At-Tarmasi (via al-Kurdi) calls the difference between the two names a «خلاف لفظي», a
dispute about words only (sources.md, log of material). For the calculator, both mean "not hayd".
It names a day "dam fasad" when the sources do.

**Why this matters.** Whether a day is hayd decides whether she prays and fasts on it. The
sources show this throughout the chapter. If blood stops before 24 h, it was dam fasad, and the
prayers she left are made up (Mughni 1/289). If it turns out that she had no valid tamyiz, she makes up
the prayers and fasts of the days that were not hayd after all (Tuhfa 1/402f., Mughni 1/287). The
calculator only does the sorting; what is forbidden in hayd is not part of this guide.

## 2. Where the rules come from

### The mu'tamad of the later Shafi'is

The Shafi'i school contains many views. The standard here is the view used for fatwa (the
**mu'tamad**) as the later Shafi'is (muta'akhkhirun) settled it ([sources.md](sources.md)). Its three
main works are the Tuhfa, the Nihaya and the Mughni. This project uses **only** these works and their
circle:

- Tuhfa, Nihaya, Mughni and their **hawashi** (marginal commentaries: ash-Sharwani, Ibn Qasim
  "سم", ash-Shabramallisi "ع ش", ar-Rashidi);
- the other later hawashi (al-Bajuri, al-Bujayrimi, ash-Sharqawi, al-Kurdi, at-Tarmasi), I'anat
  at-Talibin, Ibn Hajar's al-Fatawa al-Kubra;
- the user's own sources: the lecture series "Rulings of Haydh and Nifas in the Shafi'i school"
  and al-Ibana wa-l-Ifada;
- approved exceptions: so far only one passage of the Bughya (sources.md).

Not used as evidence: a **matn** (the base text, e.g. al-Minhaj); early works from before the mu'tamad
was fixed; comparative works. Earlier works such as ar-Rawda and al-Majmu' count only when an allowed
work quotes them. Each point needs at least 2 sources from different authors, ideally 4. All the
details are in [sources.md](sources.md).

**Who counts as an author.** Two books by one author count once: the Tuhfa and FK are both Ibn
Hajar. Each hashiya author counts as his own author.

### How strong is the other view?

When a shafi'i book calls a view "asahh" or "azhar", that word tells you how strong the
**opposite** view is. Learn this table; it decides what goes into the calculator's settings:

| The book calls the chosen view | The other view is called | Strength of the other view | In the settings? |
|---|---|---|---|
| al-asahh ("the most correct") | muqabil al-asahh / as-sahih | strong | yes |
| al-azhar ("the most apparent") | muqabil al-azhar / az-zahir | strong | yes |
| as-sahih | muqabil as-sahih | weak | no |
| al-mashhur | muqabil al-mashhur | weak | no |
| "wa qila ..." ("and it was said") | - | weak | no |

These words come from the terminology of al-Minhaj, which the three shuruh explain in their
introductions (Tuhfa 1/50, Nihaya 1/48, Mughni 1/105). **azhar / mashhur** mark the stronger of two
or more **sayings (aqwal) of ash-Shafi'i**; **asahh / sahih** mark the stronger of two or more **views
(awjuh) of his followers (ashab)**, which they derive from his words. In both pairs, the first word
(azhar, asahh) is used when the khilaf is strong, the second (mashhur, sahih) when the other view is
weak. Mughni 1/105: «فَإِنْ قَوِيَ الْخِلَافُ قُلْت الْأَظْهَرُ وَإِلَّا فَالْمَشْهُورُ، وَحَيْثُ أَقُولُ الْأَصَحُّ أَوْ الصَّحِيحُ فَمِنْ الْوَجْهَيْنِ أَوْ الْأَوْجُهِ، فَإِنْ قَوِيَ الْخِلَافُ قُلْت: الْأَصَحُّ وَإِلَّا فَالصَّحِيحُ».

There is a second kind of disagreement: when **Ibn Hajar** differs from **ar-Ramli and
al-Khatib**. Both sides are mu'tamad. The calculator's default follows ar-Ramli / al-Khatib, and
the preset `IbnHajar` switches to the Tuhfa (README.md, "Which view the calculator follows").

## 3. Measuring time and blood

### A day and a night = 24 hours

"A day and a night" (يوم وليلة) means 24 hours, counted from any moment to the same moment the next
day (Tuhfa 1/385: «وَهُوَ أَرْبَعٌ وَعِشْرُونَ سَاعَةً»; Nihaya 1/325; Mughni 1/278). It does not have
to start at sunrise or sunset.

The maximum of "15 days with their nights" is likewise counted in real time: 15 × 24 = **360
hours** from the start of the blood. If the blood starts in the morning, the 15th night is the
night of the 16th day (Nihaya 1/326 with ash-Shabramallisi, at-Tarmasi p. 269; basics.md). This is
why the calculator works with NodaTime `Instant` and `Duration` and never with day counts in
floating point (section 18).

### When is blood "present"?

Blood counts as present when a piece of cotton put in would come out stained, even if nothing
flows to the outside (al-Kurdi via at-Tarmasi p. 267; basics.md). From this come two words that are
easy to mix up:

| Word | What happens | Ruling |
|---|---|---|
| **fatra** | a pause between gushes; the cotton still comes out stained | hayd **by agreement** (Tuhfa 1/412) |
| **naqa'** | real purity; the cotton comes out white and clean | the khilaf of sahb / laqt (section 6) |

So a "pure day" in this guide always means naqa', not just a day with less blood.

## 4. The three numbers (and the age)

Almost all of the calculation follows from three numbers. Learn them first:

| | Value | Sources |
|---|---|---|
| **Minimum of hayd** | a day and a night = 24 h | Tuhfa 1/385, Nihaya 1/325f., Mughni 1/278 |
| **Maximum of hayd** | 15 days with their nights = 360 h | Tuhfa 1/385, Nihaya 1/326, Mughni 1/278 |
| **Minimum of tuhr between two hayds** | 15 days | Tuhfa 1/385, Nihaya 1/326, Mughni 1/278 |
| Maximum of tuhr | none, by consensus | Tuhfa 1/385 |
| Usual hayd (ghalib) | 6 or 7 days | Tuhfa 1/385 |

Ar-Ramli (Nihaya 1/326) gives the reason for the 15-day purity: a month usually contains a
hayd and a tuhr, so if the maximum of hayd is 15, the minimum of tuhr is 15 as well. The 3 months
of 'idda of a woman past menopause stand for three periods. He also calls the report
"the minimum of hayd is three days and the maximum ten" weak, as in al-Majmu'.

What follows directly from the numbers:

- Blood under 24 h in total is **not hayd** (dam fasad). If it stops before 24 h, the prayers she
  left are made up (Mughni 1/289).
- No hayd can last more than 15 days. If blood goes on beyond that, she is a **mustahada**
  (Part III).
- Between the end of one hayd and the start of the next there must be at least 15 days. Blood
  that comes earlier falls into the **rest of the minimal purity** (section 8).
- Since tuhr has no maximum, a woman can be pure for years, even with weak blood flowing all the
  time (Tuhfa 1/401, Nihaya 1/341: a day and a night black, then red for years: the red is all tuhr).
  An 'ada with a cycle of ten years still counts (FK 1/110).

**Age.** The earliest age for hayd is **nine lunar years** (Mughni 1/278: «تِسْعُ سِنِينَ قَمَرِيَّةٍ»),
approximately, not exactly: if she is short of nine by less than the time of a hayd plus a tuhr
(16 days, ash-Shabramallisi after Ibn Hajar), the blood is still hayd (Nihaya 1/325:
«وَالْمُعْتَبَرُ فِي التِّسْعِ التَّقْرِيبُ لَا التَّحْدِيدُ»). There is no upper age: hayd is possible as long as
she lives (Nihaya 1/325, after al-Mawardi). The 62 years of menopause in the chapter on 'idda are
"the usual case" only. The calculator does not check the age yet.

### Check yourself

> Blood for 20 hours, then nothing. Hayd?
> No: under 24 h, dam fasad.
>
> Blood for 15 days, then it stops. Hayd?
> Yes, all 15 days (the maximum is not exceeded; Ibn Qasim on Tuhfa 1/399: "before 15" means
> "before exceeding 15").

---

# Part II: Cases within the limits

As long as the blood **does not exceed 15 days**, the rules are short. These are the cases the
calculator calls "basic" (not a mustahada case).

## 5. Blood within the limits is all hayd

The core rule (Mughni 1/284, Nihaya 1/339, Tuhfa 1/399; basics.md):

> If she sees blood at an age when hayd is possible, at least for the minimum and not beyond the
> maximum, **all of it is hayd**, whatever its colour, whether she is a beginner or has a habit,
> whether it matches her habit or not, **unless she still has a rest of purity** to complete.

Mughni 1/284: «فَكُلُّهُ حَيْضٌ سَوَاءٌ أَكَانَ أَسْوَدَ أَمْ لَا، وَسَوَاءٌ أَكَانَتْ مُبْتَدَأَةً أَمْ مُعْتَادَةً، تَغَيَّرَتْ عَادَتُهَا أَمْ لَا، إلَّا أَنْ يَكُونَ عَلَيْهَا بَقِيَّةُ طُهْرٍ»

Consequences:

- **Colours don't matter within 15 days.** 5 black, then red, stop before day 15: all hayd (Tuhfa
  1/399). Even if the weak blood comes before the strong one (Mughni 1/289, FK 1/100). 5 red, 5
  black, 5 red, stop: all hayd according to the madhhab (FK 1/86).
- **Her habit does not matter within 15 days.** An 'ada of 5 and blood for 9 days, stop: all 9
  hayd. Habits can change (Nihaya 1/339: «وَاحْتِمَالُ تَغَيُّرِ الْعَادَةِ مُمْكِنٌ»).
- **Strength only matters once 15 days are exceeded.** Then tamyiz (Part III) decides, e.g.
  5 black then red continuing: only the black is hayd (Tuhfa 1/399).

**What she does while it is still unclear.** When the blood starts, she keeps away from what is
forbidden in hayd. In the first cycle she waits up to 15 days, because only when the blood goes
beyond 15 days does it turn out that she is a mustahada (Mughni 1/289, Tuhfa 1/402). The calculator
does not model this waiting. It looks at the whole course afterwards and gives the final result.

## 6. Intermittent blood: sahb and laqt

Many women do not bleed without a break: blood, a pure day, blood again. What is the pure day?

```
day     1  2  3  4  5  6  7  8  9 10 11
blood   R  R  R  .  .  .  .  .  R  R  R    (3 red, 5 pure, 3 red, stop)
sahb    H  H  H  H  H  H  H  H  H  H  H    -> 11 days hayd
laqt    H  H  H  T  T  T  T  T  H  H  H    -> 6 days hayd, the pure days are tuhr
```

Two views (Tuhfa 1/412, Mughni 1/293f., Nihaya 1/356):

| View | Name | Strength | Ruling of the pure days |
|---|---|---|---|
| **qawl as-sahb** ("drawing over") | the azhar, **default** | chosen | hayd: the ruling of hayd is "drawn" over the purity, like a fatra |
| qawl al-laqt / at-talfiq ("picking up / patching together") | muqabil al-azhar | strong | tuhr: only the bloods are hayd |

Setting: `PurityBetweenBloods` (`Hayd` = sahb, default; `Tuhr` = laqt).

**The three conditions for sahb** (Mughni 1/293, Nihaya 1/356):

1. Bloods and purity together do **not exceed 15 days**.
2. The bloods **together reach 24 hours** (each one may be shorter).
3. The purity is **enclosed by blood** on both sides. Purity after the last blood is tuhr by
   agreement.

If condition 2 fails, all of it is dam fasad. If condition 1 fails, it is "istihada": the
intermittent blood is treated as a mustahada case (Mughni 1/294, Nihaya 1/356; but see section 8
for the restriction).

Fine points:

- Sahb means the bloods only have to reach 24 h **together**. They do not have to be in one piece
  (Mughni 1/278). Under laqt, a hayd under 24 h in one piece is possible (ash-Sharwani on Tuhfa 1/385).
- The khilaf concerns prayer, fasting and intercourse. For the end of the 'idda the pure days never
  count as a tuhr (Tuhfa 1/412, Nihaya 1/356).

## 7. Yellow and brownish discharge

**Sufra** (yellow) and **kudra** (brownish, turbid) are not "real" blood colours. Are they hayd?

- Hayd **according to the asahh** (Mughni 1/284, Nihaya 1/340, Tuhfa 1/400), because what a woman
  sees at a time when hayd is possible is in principle hayd.
- Muqabil al-asahh (strong): not hayd, because of the report of Umm 'Atiyya «كُنَّا لَا نَعُدُّ الصُّفْرَةَ وَالْكُدْرَةَ شَيْئًا».
- Inside the days of her 'ada, yellow is hayd without khilaf (Mughni 1/285).

Setting: `YellowAndTurbidAreHayd` (default `true`). With `false`, 5 days of only yellow are no
hayd at all.

## 8. The rest of the minimal purity (baqiyyat tuhr)

This is the hardest part of the basic rules. Spend time on it.

**Idea:** after a hayd, the next 15 days must be tuhr (section 4). If blood comes back before these
15 days are over, then at least the part of the blood that falls into those 15 days cannot be hayd.
All of the following is in [rest-of-tuhr.md](rest-of-tuhr.md).

### Which rule applies: the 15-day window

Call the first blood **A**, the purity after it **P**, and the returning blood **B**.

```
case 1:  |--A--|--P--|--B--|                    all inside the first 15 days     -> one hayd (sahb, section 6)
case 2:  |--A--|-------P-------|--B-->          B starts after day 15 of A        -> rest of tuhr (this section)
case 3:  |--A--|--P--|--B-----------> day 15    B starts inside, runs past day 15 -> mustahada case
```

1. **A + P + B within 15 days**: one hayd, the purity included (sahb).
2. **B starts after day 15 counted from A** (so A + P are at least 15 days), but before the 15 days of
   purity after A are complete: the rules below. (The Bughya's own condition says A + P "exceed"
   15 days; for A + P of exactly 15 see open question 7.)
3. **B starts inside the 15 days and runs past day 15**: intermittent blood that exceeds 15 days,
   a mustahada case (Mughni 1/294, Nihaya 1/356). al-Ibana p. 27 states the line: blood that crosses
   day 15 **connected to the blood before it** makes her a mustahada; if not connected, she has a
   rest of tuhr. (One shape of this case, 13/1/5, is an open question; section 17.)

### Case 2a: the returning blood stops inside the rest: dam fasad (3/12/3)

The classic example, in all three shuruh (Mughni 1/284, Nihaya 1/339f., Tuhfa 1/399):

```
day     1  2  3 | 4  5  6  7  8  9 10 11 12 13 14 15 | 16 17 18 | 19 ...
blood   R  R  R | .  .  .  .  .  .  .  .  .  .  .  . | R  R  R  | (stops)
ruling  H  H  H | T  T  T  T  T  T  T  T  T  T  T  T | F  F  F  |
                  \-------- tuhr must last until the end of day 18 --------/
```

The first 3 days are hayd. The last 3 days are **dam fasad**: they are not hayd together with the
first blood, because that would exceed 15 days, and not hayd on their own, because no minimal purity
comes before them (FK 1/108 after al-Majmu'). Ibn Hajar: «وَلَا خِلَافَ فِي شَيْءٍ مِنْ هَذَا» (FK 1/86).
Ash-Sharwani: this includes the mubtada'a.

### Case 2b: the returning blood goes beyond the rest and stops: the rest is hayd (Bughya rule)

What if the returning blood is longer than the remaining purity? Bughya 1/277 (after the Fatawa of
Ibn Yahya), al-Ibana p. 29-31, Ibn Qasim «لَا يَبْعُدُ»:

> The first blood is hayd. **The part of the returning blood that completes the purity is dam
> fasad. What goes beyond it is hayd**, if it reaches a day and a night and does not exceed 15 days.

Worked example from al-Ibana p. 29, **10/8/12**:

```
day     1 .. 10 | 11 .. 18 | 19 .. 25 | 26 .. 30
blood   R .. R  | .  ..  . | R  ..  R | R  ..  R
ruling  H .. H  | T  ..  T | F  ..  F | H  ..  H
                  \--- 15 days tuhr: days 11-25 ---/
```

10 days hayd; tuhr from day 11 to day 25; the returning blood starts on day 19, so days 19-25 (7
days) complete the purity and are dam fasad; days 26-30 (5 days) are a **new hayd**.

**How to compute it:** end of the purity = last day of the hayd + 15. Every day of blood up to that
day is dam fasad; the blood after it is a new candidate for hayd, and the normal rules apply to it.

| Course | Purity complete at end of day | Dam fasad | New hayd |
|---|---|---|---|
| 3 red, 14 pure, 4 red | 18 | day 18 | days 19-21 |
| 10 red, 8 pure, 12 red | 25 | days 19-25 | days 26-30 |
| 3 red, 14 pure, 16 red | 18 | day 18 | days 19-33 (exactly 15) |
| 3 red, 14 pure, 17 red | 18 | day 18 | the rest is 16 days: beyond 15, a mustahada case |

### Case 2c: the returning blood continues: a mustahada case

If the rest after the completed purity goes beyond 15 days, the new cycle is a mustahada case
(Part III). Bughya: the mubtada'a ghayr mumayyiza takes a day and a night from the start of the
rest and 29 days purity; the mu'tada takes her 'ada.

The textbook case for a woman with an 'ada (Tuhfa 1/399, FK 1/111): 'ada of 5 days, then 14 days
pure, then the blood comes back and continues:

```
day     1 .. 5 | 6 .. 19 | 20 | 21 .. 25 | 26 .. 40 | 41 .. 45 | ...
blood   R .. R | .  ..  .| R  | R  ..  R | R  ..  R | R  ..  R |
ruling  H .. H | T  ..  T| T* | H  ..  H | T  ..  T | H  ..  H |    T* = istihada that completes the tuhr
```

Day 20 completes the purity (istihada), then 5 days hayd, 15 days tuhr; her cycle becomes 20 days.

For the **mubtada'a** in this situation (e.g. 3 red, 12 pure, then continuing) the sources are read
in two ways; this is open question 7 (section 17).

---

# Part III: The mustahada (blood beyond 15 days)

## 9. When she becomes a mustahada, and the seven kinds

**Definition:** a woman whose blood goes beyond 15 days is a **mustahada** (Mughni 1/285,
Nihaya 1/340). Once the blood has gone beyond 15 days, she is a mustahada even if it stops
later (FK 1/102). From now on "all of it is hayd" no longer holds; we have to find **which part**
is hayd.

To do that we ask two questions:

1. **Does she have an 'ada?** A **mubtada'a** is at the start of her bleeding, with no previous
   hayd and tuhr that she knows. A **mu'tada** has a hayd and a following tuhr she knows ('ada,
   habit).
2. **Does her blood show tamyiz?** She is **mumayyiza** if her blood is partly strong and partly
   weak and the conditions of tamyiz hold (section 11).

This gives the **seven kinds** (Mughni 1/285, Nihaya 1/340f., Tuhfa 1/401):

| # | Kind | Rule (default view) | Section |
|---|---|---|---|
| 1 | mubtada'a mumayyiza | strong blood = hayd, weak blood = tuhr | 11 |
| 2 | mubtada'a ghayr mumayyiza | a day and a night hayd, 29 days tuhr | 12 |
| 3 | mu'tada ghayr mumayyiza | back to her 'ada in amount and time | 13 |
| 4 | mu'tada mumayyiza | tamyiz decides (asahh) | 13 |
| 5-7 | mutahayyira (forgot the amount and time of her 'ada / only the amount / only the time) | ihtiyat (precaution); not implemented | 14 |

A woman whose blood shows strong and weak blood but **misses a condition** of tamyiz is treated
like kind 2 (or 3 if she has an 'ada). Only the name differs (Mughni 1/287, Tuhfa 1/404, Nihaya 1/342).

**One course, several cycles.** The calculator decides per hayd cycle whether it is basic
(Part II) or a mustahada case. Example: 3 red, 12 pure, 20 red: cycle 1 (the 3 days) is basic; the
returning blood completes the purity and then exceeds 15 days, so cycle 2 is a mustahada case
(mustahada.md, "Classification"). The docs record that no source uses the word "cycle" for this;
it is derived from Tuhfa 1/399 and the Bughya (review-notes.md B4).

## 10. Strong and weak blood

Strength comes from three qualities (Tuhfa 1/402, Nihaya 1/341, Mughni 1/285):

1. **Colour**, from strong to weak:

   ```
   black  >  red  >  ashqar (reddish-yellow)  >  yellow  >  brownish (kudra)
   ```

   Black includes blood with black streaks.
2. **Thickness**: thick is stronger than thin.
3. **Smell**: a bad smell is stronger than none.

Rules for comparing:

- More qualities = stronger. Thick, foul-smelling black is stronger than thick black.
- **Strength is relative.** Red is weak next to black and strong next to yellow (Nihaya 1/341).
- **Equal strength: the earlier one counts.** Thick black against foul-smelling black, or thick red
  against plain black (one quality each): the earlier blood is the stronger (Tuhfa 1/402).
- "Weak" means purely weak. Weak blood that still has streaks of the stronger one belongs to the
  stronger (Mughni 1/285).

The calculator so far only knows colours (`EFlowAppearanceColor`), not thickness or smell.

## 11. Tamyiz: the mubtada'a mumayyiza

Basis: the hadith «دَمُ الْحَيْضِ أَسْوَدُ يُعْرَفُ» (quoted in Mughni 1/288). If the blood goes beyond 15
days and shows strong and weak blood, **the strong blood is hayd and the weak is istihada**, under
four conditions. Everything here is in [tamyiz.md](tamyiz.md).

### The four conditions

The matn, quoted in all three shuruh: «فَالضَّعِيفُ اسْتِحَاضَةٌ، وَالْقَوِيُّ حَيْضٌ إنْ لَمْ يَنْقُصْ عَنْ أَقَلِّهِ، وَلَا عَبَرَ أَكْثَرَهُ، وَلَا نَقَصَ الضَّعِيفُ عَنْ أَقَلِّ الطُّهْرِ»

| # | Condition | Fails, for example (Mughni 1/285) |
|---|---|---|
| 1 | The strong blood lasts **at least 24 h** | black for less than a day and a night, then red continues |
| 2 | The strong blood **does not exceed 15 days** | 16 days black, then red |
| 3 | The weak blood lasts **at least 15 days in a row** (only needed if the blood continues) | 1 black, 14 red, then black again |
| 4 | The weak blood is **not interrupted by strong blood** (purity in between does not matter) | 1 day black, 1 day red, alternating without end |

If a condition fails, she is a mumayyiza "whose tamyiz does not count" and is treated like a
ghayr mumayyiza (section 12).

**Example that works:** 10 black, then red for a long time:

```
day     1 .. 10 | 11 ........................ (continues)
blood   B .. B  | R  ........................
ruling  H .. H  | T  ........................
```

Black is 10 days (1 ✓, 2 ✓), the red continues beyond 15 days without black (3 ✓, 4 ✓). Hayd = the
10 black days. The red is all tuhr, however long it lasts (Tuhfa 1/401, Nihaya 1/341).

### Condition 3: only if the blood continues

The weak blood only has to reach 15 days if the blood **continues** (after al-Mutawalli; Tuhfa 1/401,
Mughni 1/286). Example: **10 black, 10 red, stop**. Together 20 days, so beyond 15, but the red
is only 10. She still follows her tamyiz: the black is hayd, the red istihada.

Why the condition exists: weak blood is to be counted as the **tuhr between two hayds**, so if strong
blood follows it, the weak blood must be a full minimal purity. Al-Isnawi's example (via ash-Sharwani
on Tuhfa 1/401): 1 black, 14 red, then black again: taking the tamyiz would make the 14 red days a tuhr
that is too short. So no valid tamyiz. With 16 red days in between, the reason no longer applies.

### Condition 4: weak blood between strong bloods

If strong and weak alternate **within the first 15 days** and after that only weak blood follows,
the alternation counts as one hayd (the weak days in between are treated like naqa' under sahb),
and the weak blood after it is tuhr (FK 1/100, 1/106, 1/86 «عَلَى الْأَصَحِّ»; al-Ibana p. 71-72):

```
day     1  2  3  4  5  6  7  8  9 10 11 | 12 ... (continues)
blood   B  R  B  R  B  R  B  R  B  R  B | R  ...
ruling  H  H  H  H  H  H  H  H  H  H  H | T  ...
```

If the alternation goes on beyond 15 days (1 black / 1 red until the end of the month), the
strong blood exceeds 15 days and she is not mumayyiza (FK 1/100).

### The strong blood need not come first

5 red, then 5 black, then red continues: the black is hayd (Tuhfa 1/402; Nihaya 1/341f. «عَلَى الْأَصَحِّ»;
FK 1/102). Strong blood that comes first is hayd «قَطْعًا» (without khilaf); strong blood that comes later
or in the middle is hayd according to the asahh. Its counter-view is not named in the works checked
(open question 2).

Example: 15 red, then 15 black, then it stops: the black is hayd, the red before it istihada (FK 1/105
quoting al-Majmu'; Tuhfa 1/402, Mughni 1/287).

### Three strengths: strong, weak, weaker

When three qualities appear (e.g. black, red, yellow), the stronger weak blood may be joined to the
strong blood (Nihaya 1/342, Mughni 1/286), under three conditions:

1. the strong comes first;
2. the stronger weak follows it directly;
3. both together do not exceed 15 days.

| Course | ar-Ramli / al-Khatib (default) | Ibn Hajar (Tuhfa 1/403f.) |
|---|---|---|
| 5 black, 5 red, then yellow continues | black + red = 10 days hayd | same |
| 10 black, 6 red, then yellow continues | only the black (together over 15) | same |
| 5 red, 5 black, then yellow continues | only the black (the weak comes first) | (not discussed) |
| 5 black, 5 yellow, then red continues | only the black (the red does not follow the black directly) | **black + yellow** = 10 days |

Setting: `StrongerWeakMustFollowStrongDirectly` (default `true`; `false` = Ibn Hajar). Ibn Qasim
notes that on Ibn Hajar's view black + yellow count only if the red **continues**. 5 black, 5 yellow,
6 red, stop: only the black, on both views.

### Strong, weak, strong (7/7/7)

7 black, 7 red, 7 black, then it stops (beyond 15 days in total):

| View | Hayd | Holders |
|---|---|---|
| **default** | the first black + the red (days 1-14) | ar-Ramli, al-Khatib, after Ibn Surayj, al-Majmu' / ar-Rawda |
| `IbnHajar` | only the first black (days 1-7) | Ibn Hajar, after at-Tahqiq and ar-Ruyani, "most of the later scholars" (Tuhfa 1/401) |

Setting: `WeakBetweenStrongBeyondMaximum`. **8/8/8** has no khilaf: only the first black is hayd
(FK 1/86 «بِالِاتِّفَاقِ», al-Ibana p. 71), because black + red would already be 16 days. Ibn Hajar
(FK 1/105) adds that both cases have to be pictured with the blood stopping after the third block;
otherwise condition 3 would fail.

## 12. The mubtada'a ghayr mumayyiza

A beginner whose blood exceeds 15 days and shows no valid tamyiz (one quality only, or a condition
missing):

> **Her hayd is a day and a night from the start of the blood, and her tuhr 29 days**, and so on
> in every month, as long as the blood continues.

(Tuhfa 1/404, Mughni 1/286, Nihaya 1/342, FK 1/104.) Reason: hayd is certain only in the minimum;
what is beyond it is doubtful (Mughni: «لِأَنَّ ذَلِكَ هُوَ الْمُتَيَقَّنُ، وَمَا زَادَ مَشْكُوكٌ فِيهِ»). Why 29 and not
"the rest of the month": the cycle of a mustahada is always **30 days** (Tuhfa 1/404). This only
applies if she knows when the blood started; if not, she is a mutahayyira.

```
day     1 | 2 .......... 30 | 31 | 32 .......... 60 | ...
blood   R | R ..........  R | R  | R  ..........  R |      (red continues)
ruling  H | T ..........  T | H  | T  ..........  T |
```

KHILAF: a day and a night is the **azhar**. Muqabil al-azhar (strong): the usual **6 or 7 days**,
the rest of the month tuhr (Mughni 1/286). Setting `MubtadaaGhayrMumayyizaHayd`.

**Example to think through:** 15 red, then 15 black, then the black continues (Tuhfa 1/402f., Mughni
1/287). While it happens, she stops praying for the whole first month: when the black comes on day
16, the red before it turns out to be istihada and the black looks like hayd. When the black goes
beyond 15 days, it turns out she has no valid tamyiz (condition 2 fails). So her hayd is a day and a
night from the start of each month, and she makes up the prayers and fasts.

## 13. The mu'tada: a woman with a habit ('ada)

Everything here is in [ada.md](ada.md).

### Back to her 'ada in amount and time

A mu'tada ghayr mumayyiza whose blood exceeds 15 days follows her habit: **as many days as her
hayd used to last, at the same place in her cycle** (Tuhfa 1/404, Mughni 1/287, with the hadith of
Umm Salama). 'Ada of 5 days at the start of the month, then the blood continues: days 1-5 of each
month are hayd, the rest tuhr. An 'ada counts whatever the length of her cycle, even ten years
(FK 1/110).

### How an 'ada is established

- **One occurrence** is enough (asahh; FK 1/110: the asahh of four views, by agreement; Nihaya 1/345,
  Mughni 1/288). One hayd of 5 days with a following tuhr gives her an 'ada of 5.
- Muqabil al-asahh (strong): two occurrences, or three.
- Setting `OccurrencesToEstablishAda` (1 = default, 2, 3). Example from Mughni 1/288: hayd of 5, then
  6, then 7, then istihada: back to **7** (one occurrence), 6 (two), 5 (three).
- An 'ada is also established by a tamyiz (asahh, «بَلْ الصَّوَابُ», FK 1/110).

**Changing 'ada:** an ordered rotation (3, 5, 7, 3, 5, 7) counts once it has repeated (two rounds).
If the 'ada changes without order, she goes back to the month before the istihada (Nihaya 1/345,
Mughni 1/288). If that last 'ada is shorter than an earlier one, there is a khilaf about **precaution
days** up to the longest 'ada. The calculator has no category for precaution days (ada.md).

### Shifted cycle

The cycle is hayd + tuhr, and a single occurrence changes it (FK 1/111):

- 'Ada 5 at the start of the month, then 20 days pure, then blood in the last five days of the month
  that continues: her hayd came early, her cycle becomes 25: 5 hayd from the start of the blood, then
  20 tuhr, and so on.
- 'Ada 5, then only 14 days pure, then continuous blood: the first day of the returning blood
  completes the purity, then 5 hayd, 15 tuhr, cycle 20 (the timeline in section 8, case 2c).

### Mu'tada mumayyiza: tamyiz against 'ada

If she has an 'ada **and** her blood shows a valid tamyiz that contradicts it:

- **Tamyiz decides** (asahh; Nihaya 1/345, Mughni 1/288): tamyiz is a sign in the blood itself,
  the 'ada a sign in the woman («التَّمْيِيزُ عَلَامَةٌ فِي الدَّمِ، وَالْعَادَةُ عَلَامَةٌ فِي صَاحِبَتِهِ»).
- Muqabil al-asahh (strong): the 'ada decides.
- Example: 'ada 5 days at the start of the month, then 10 black from the start of the month and then
  red: hayd = 10 days (tamyiz) or the first 5 (by 'ada). Setting `MutadaMumayyizaFollows`.
- If a full minimal purity lies between the 'ada and the strong blood, **both** are hayd (asahh,
  Mughni 1/288f.): 'ada of 5, then 20 red, then 5 black, then red: days 1-5 and the 5 black days.

**A long case to test yourself** (Mughni 1/287 after al-Barizi, Tuhfa 1/403): 'ada 15 days, then 15
red, then continuous black. She stops praying for 45 days: the first 15 as her 'ada, the next 15
because the black is strong and might be a tamyiz, the third 15 because it is still unclear. When the
black goes on, it turns out her reference is the 'ada.

## 14. The mutahayyira

A mustahada who has **forgotten** her 'ada (its amount, its time, or both) is a mutahayyira, "the
confused one". The mashhur is that she follows precaution (ihtiyat): like a woman in hayd for what
hayd forbids (e.g. intercourse), like a pure woman for worship (Mughni 1/289; Tuhfa 1/405 for a woman
who forgot the order of a changing 'ada). The calculator does not handle her yet: it would need a new
input, because she does not know her own history (mustahada.md).

---

# Part IV: Around the core

## 15. Nifas and pregnancy

Not implemented yet; [nifas.md](nifas.md) collects the material. The points most relevant for
calculating:

- Between **hayd and nifas** the purity can be shorter than 15 days (Tuhfa 1/385, Mughni 1/278,
  Nihaya 1/326). A day and a night of blood shortly before labour is hayd.
- Nifas for 60 days (its maximum), then a pause of even a moment, then blood: hayd. If nifas stops
  before 60 days, the returning blood is hayd only after 15 days (Tuhfa 1/385).
- Blood during pregnancy is hayd (azhar; Mughni 1/293, Nihaya 1/356).

## 16. Map of the khilaf and the settings

All in [khilaf.md](khilaf.md). The calculator's settings live in `HaydCalculatorSettings`
(`HaydCalculator.Core/Calculator/Models/HaydCalculatorSettings.cs`):

| Point | Default | Other view | Setting | Section |
|---|---|---|---|---|
| Purity between bloods | hayd (sahb, azhar) | tuhr (laqt) | `PurityBetweenBloods` | 6 |
| Yellow / brownish | hayd (asahh) | not hayd | `YellowAndTurbidAreHayd` | 7 |
| Mubtada'a ghayr mumayyiza | a day and a night (azhar) | 6 or 7 days | `MubtadaaGhayrMumayyizaHayd` | 12 |
| Occurrences for an 'ada | one (asahh) | two; three | `OccurrencesToEstablishAda` | 13 |
| Mu'tada mumayyiza | tamyiz (asahh) | 'ada | `MutadaMumayyizaFollows` | 13 |
| 7/7/7 | first strong + weak (ar-Ramli, al-Khatib) | only first strong (Ibn Hajar) | `WeakBetweenStrongBeyondMaximum` | 11 |
| 5 black, 5 yellow, red | only black (ar-Ramli, al-Khatib) | black + yellow (Ibn Hajar) | `StrongerWeakMustFollowStrongDirectly` | 11 |

Presets: `Default` = `Ramli` = `Shirbini`; `IbnHajar` changes the last two rows.

Things that are **not** in the settings: weak views (sahih, mashhur, "wa qila"); views whose holder or
strength has not been found in the allowed works; points that only affect what she does while waiting
(khilaf.md, "Known, but not in the settings"). No-khilaf points are listed there as well.

## 17. Open questions

What the sources checked so far do not settle is **not decided** in the code or the tests; it goes
into [open-questions.md](open-questions.md), and tests for it throw `OpenFiqhQuestionException`.
Summary (read the file for the passages):

| # | Question | Status |
|---|---|---|
| 1 | 13 red, 1 pure, 5 red, stop: the returning blood starts inside the 15 days and stops after day 15. Mustahada case (Mughni 1/294, Nihaya 1/356, al-Ibana p. 27) or 13 hayd + 5 dam fasad (the broad wording of FK 1/86)? | open by the user's decision |
| 2 | Who holds the strong counter-view for strong blood that comes later? | not found |
| 3 | 8/8/8: the last day of the second black | not discussed in the sources |
| 4 | Condition 4 with a long weak blood at the end | rule found (FK 1/100, al-Ibana p. 71f.); counter-view not named |
| 5 | Mutahayyira | not implemented |
| 6 | Exactly 24 h as first blood in the Bughya case | taken over as [IMPLICIT] |
| 7 | Mubtada'a, 3 red, 12 pure, then continuing: is the first blood a basic hayd (Bughya), or is she a mustahada from day 1 (al-Bujayrimi and ash-Shabramallisi on Tuhfa 1/399)? | open; the tests follow the Bughya reading |

## 18. From fiqh to code

How the fiqh becomes the calculator (details in CLAUDE.md):

- **Time.** Every blood entry is an `Instant` range (`From` / `To`), lengths are `Duration`.
  24 h and 360 h are exact durations, without daylight-saving shifts or rounding. The app maps
  calendar days to midnight UTC.
- **One run = one `HaydCalculation`.** `HaydCalculatorService` is stateless; each `Calculate` call
  works through the course of blood cycle by cycle: basic rules (Part II) first, and the mustahada
  rules (Part III) for a cycle whose blood exceeds 15 days. Today a mustahada case still ends in
  `InfoException(TextUtil.ComplexIstihadaNotImplemented)` (mustahada.md).
- **Settings.** Each khilaf point from section 16 is one property; tests for another view pass
  `HaydCalculatorSettings.Default with { ... }` or a preset.
- **Tests as fiqh documentation.** Each expected result has a source comment
  (`// [Author, Work, vol. X, p. Y; link] summary`), a `// KHILAF:` line, and `[FiqhTodo]` until
  the user has checked it. A wrong result stays a red test. The test files by topic are listed in
  README.md ("Tests").

---

# Appendix

## A. The whole procedure in one flow

```
Start of a blood
│
├─ Too young for hayd (under about 9 lunar years, section 4)?    → not hayd
│
├─ Is she still inside the 15 days of tuhr after her last hayd?
│     yes → the part up to the end of that tuhr is dam fasad / istihada (section 8);
│           continue with the rest of the blood as a new start
│
├─ Do the bloods (together) reach 24 h?
│     no  → dam fasad, no hayd
│
├─ Does the course (bloods + purity in between) stay within 15 days?
│     yes → all of it is hayd (section 5)
│           pure days in between: hayd (sahb, default) or tuhr (laqt)       (section 6)
│           yellow / brownish: hayd (default)                                (section 7)
│
└─ no: it exceeds 15 days → mustahada (section 9)
      │
      ├─ valid tamyiz (4 conditions, section 11)?
      │     ├─ mubtada'a → strong = hayd, weak = tuhr
      │     └─ mu'tada   → tamyiz decides (default) / 'ada (setting)
      │
      └─ no valid tamyiz
            ├─ mubtada'a  → 1 day and night hayd, 29 tuhr (default) / 6-7 days (setting)
            ├─ mu'tada    → her 'ada in amount and time
            └─ forgot her 'ada → mutahayyira (not implemented)
```

## B. Exercises with solutions

Try each one before you open the solution. Day 1 = first day of blood; default settings unless
stated.

**1.** 6 days red, then it stops.

<details><summary>Solution</summary>

All 6 days hayd: between 24 h and 15 days, so the colour does not matter (section 5).
</details>

**2.** 3 red, 5 pure, 3 red, then it stops. Default, and with `PurityBetweenBloods = Tuhr`.

<details><summary>Solution</summary>

11 days in total, within 15, the bloods reach 24 h, the purity is enclosed: sahb applies, days 1-11
are hayd. With laqt: days 1-3 and 9-11 are hayd, days 4-8 tuhr (section 6).
</details>

**3.** 3 red, 12 pure, 3 red, then it stops.

<details><summary>Solution</summary>

Days 1-3 hayd. Tuhr must last until the end of day 18. Days 16-18 lie inside it: dam fasad, no
khilaf (FK 1/86). Not a mustahada case (section 8, case 2a).
</details>

**4.** 10 red, 8 pure, 12 red, then it stops.

<details><summary>Solution</summary>

Days 1-10 hayd; tuhr days 11-25; days 19-25 dam fasad; days 26-30 a new hayd (al-Ibana p. 29;
Bughya). Not a mustahada case (section 8, case 2b).
</details>

**5.** A beginner: red for 40 days, all the same.

<details><summary>Solution</summary>

Exceeds 15 days → mustahada. One quality → no tamyiz → mubtada'a ghayr mumayyiza: day 1 hayd,
days 2-30 tuhr, day 31 hayd, days 32-40 tuhr. With `UsualSixOrSevenDays`: 6 or 7 days hayd, the
rest of the month tuhr (Mughni 1/286; section 12).
</details>

**6.** A beginner: 10 black, then red for a long time.

<details><summary>Solution</summary>

Mustahada; tamyiz conditions all met → days 1-10 hayd, the red is tuhr however long it lasts
(section 11).
</details>

**7.** A beginner: 1 day black, then 1 day red, alternating like this for 40 days.

<details><summary>Solution</summary>

Condition 4 fails (strong blood keeps interrupting the weak, and the strong blood runs beyond 15
days): no valid tamyiz → like exercise 5: a day and a night hayd, 29 tuhr (Nihaya 1/341, Mughni 1/285).
</details>

**8.** A beginner: 7 black, 7 red, 7 black, then it stops. Default and `IbnHajar`.

<details><summary>Solution</summary>

Default: days 1-14 (black + red) hayd. Ibn Hajar: days 1-7 only (section 11).
</details>

**9.** A beginner: 5 black, 5 yellow, then red continues. Default and `IbnHajar`.

<details><summary>Solution</summary>

Default: only days 1-5. Ibn Hajar: days 1-10 (black + yellow), because the red continues (section 11).
</details>

**10.** 'Ada of 5 days at the start of each month. Now: 10 black from the start of the month, then
red continuing. Default and `MutadaMumayyizaFollows = Ada`.

<details><summary>Solution</summary>

Default (tamyiz, asahh): days 1-10 hayd. By 'ada: days 1-5 (section 13).
</details>

**11.** Hayd 5, then 6, then 7 days in three months, then continuous blood without tamyiz.

<details><summary>Solution</summary>

One occurrence (default): 7 days. Two: 6. Three: 5 (Mughni 1/288).
</details>

**12.** 'Ada of 5, then 14 days pure, then blood continues.

<details><summary>Solution</summary>

Days 1-5 hayd, 6-19 pure, day 20 completes the tuhr (istihada), days 21-25 hayd, 26-40 tuhr, cycle 20
(Tuhfa 1/399, FK 1/111; section 8, case 2c).
</details>

**13.** 13 red, 1 pure, 5 red, then it stops.

<details><summary>Solution</summary>

Trick question: this is **open question 1**. The returning blood starts inside the 15 days and
stops after day 15. The sources read it as a mustahada case or as 13 hayd + 5 dam fasad; the user
has left it open (section 17).
</details>

## C. Glossary

| Term | Meaning |
|---|---|
| 'ada | her habit: length and position of hayd and tuhr |
| ashab | the jurists of the school after ash-Shafi'i |
| ashqar | reddish-yellow blood, between red and yellow |
| asahh | the chosen view among the awjuh of the ashab when the other view is strong (its counterpart: sahih); section 2 |
| azhar | the chosen saying of ash-Shafi'i when the other saying is strong (its counterpart: zahir); section 2 |
| mashhur | the chosen saying of ash-Shafi'i when the other saying is weak |
| sahih | the chosen wajh when the other view is weak; also the name of the counterpart of the asahh |
| baqiyyat tuhr | the rest of the 15 days of minimal purity (section 8) |
| dam fasad | blood that cannot be hayd |
| fatra | a pause with traces of blood; hayd by agreement |
| hashiya, pl. hawashi | a gloss on a sharh |
| hayd | menstruation |
| ihtiyat | precaution |
| [IMPLICIT] | a source states the rule but not the numbers of the case |
| istihada | blood of illness; everything that is not hayd or nifas |
| khilaf | difference of views |
| kudra | brownish, turbid discharge |
| laqt / talfiq | the view that purity between bloods is tuhr |
| matn | base text that a sharh explains (e.g. al-Minhaj); not used as evidence here |
| mubtada'a | beginner, without a known previous hayd and tuhr |
| mu'tada | woman with an 'ada |
| mu'tamad | the view of the school used for fatwa |
| mumayyiza | woman whose blood shows valid tamyiz |
| muqabil | "the counterpart": the other view, e.g. muqabil al-asahh |
| mustahada | woman whose blood exceeds 15 days |
| mutahayyira | mustahada who forgot her 'ada |
| naqa' | real purity: the cotton comes out clean |
| nifas | blood after childbirth |
| sahb | the view that purity between bloods is hayd (azhar) |
| sharh, pl. shuruh | commentary on a matn (Tuhfa, Nihaya and Mughni are shuruh on al-Minhaj) |
| sufra | yellow discharge |
| tamyiz | distinguishing strong from weak blood |
| tuhr | purity in the legal sense |
| wajh, pl. awjuh | a view of the ashab derived from ash-Shafi'i's principles |
