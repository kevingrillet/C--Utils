using FuzzySharp;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using FuzzySharp.SimilarityRatio;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using FuzzySharp.SimilarityRatio.Scorer;
using CSharp_Utils.D4Companion.Entities.D4Companion;

namespace CSharp_Utils.Tests.D4Companion;

[TestFixture, Parallelizable, Category("NotOnGitHub")]
internal partial class D4CompanionFuzzySharpTests
{
    private List<Tuple<string, string>> _affixes;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _affixes = JsonSerializer.Deserialize<List<AffixInfo>>(File.ReadAllText("D4Companion/Ressources/D4Companion/Affixes.enUS.json"))
            .Select(ai => Tuple.Create(ai.Description, RemoveSpecialCharacters(ai.Description)))
            .ToList();
    }

    [Test, TestCaseSource(nameof(GetTestAffixesD4Builds))]
    public void Test_D4Builds_FuzzyDescription(string affix, string expected = null)
    {
        affix = RemoveSpecialCharacters(affix).ToLower();
        expected ??= affix;

        var result = Tuple.Create(
            "WithoutScorer",
            Process.ExtractOne(affix, _affixes.Select(a => a.Item1.ToLower())).Value,
            Process.ExtractOne(affix, _affixes.Select(a => a.Item2.ToLower())).Value
        );

        Assert.Multiple(() =>
        {
            Assert.That(RemoveSpecialCharacters(result.Item2).ToLower(),
                Is.EqualTo(RemoveSpecialCharacters(expected).ToLower()),
                $"Error [{result.Item1}] Affixes: \n{string.Join("\n", Process.ExtractTop(affix, _affixes.Select(a => a.Item1.ToLower())).Select(r => $"[{r.Score}] {r.Value}"))}");
            Assert.That(result.Item3,
                Is.EqualTo(RemoveSpecialCharacters(expected).ToLower()),
                $"Error [{result.Item1}] Affixes clean: \n{string.Join("\n", Process.ExtractTop(affix, _affixes.Select(a => a.Item2.ToLower())).Select(r => $"[{r.Score}] {r.Value}"))}");
        });
    }

    [Test, TestCaseSource(nameof(GetTestAffixesD4Builds))]
    public void Test_D4Builds_FuzzyDescriptionScorer(string affix, string expected = null)
    {
        affix = RemoveSpecialCharacters(affix).ToLower();
        expected ??= affix;

        var scorers = new List<IRatioScorer>() {
            ScorerCache.Get<DefaultRatioScorer>(),
            //ScorerCache.Get<PartialRatioScorer>(),
            //ScorerCache.Get<TokenSetScorer>(),
            //ScorerCache.Get<PartialTokenSetScorer>(),
            //ScorerCache.Get<TokenSortScorer>(),
            //ScorerCache.Get<PartialTokenSortScorer>(),
            //ScorerCache.Get<TokenAbbreviationScorer>(),
            //ScorerCache.Get<PartialTokenAbbreviationScorer>(),
            //ScorerCache.Get<WeightedRatioScorer>()
            };

        var results = new List<Tuple<string, string, string>>();

        foreach (var scorer in scorers)
        {
            results.Add(Tuple.Create(
                scorer.GetType().Name,
                Process.ExtractOne(affix, _affixes.Select(a => a.Item1.ToLower()), scorer: scorer).Value,
                Process.ExtractOne(affix, _affixes.Select(a => a.Item2.ToLower()), scorer: scorer).Value
            ));
        }

        Assert.Multiple(() =>
        {
            foreach (var result in results)
            {
                Assert.That(RemoveSpecialCharacters(result.Item2).ToLower(),
                    Is.EqualTo(RemoveSpecialCharacters(expected).ToLower()),
                    $"Error [{result.Item1}] Affixes: \n{string.Join("\n", Process.ExtractTop(affix, _affixes.Select(a => a.Item1.ToLower())).Select(r => $"[{r.Score}] {r.Value}"))}");
                Assert.That(result.Item3,
                    Is.EqualTo(RemoveSpecialCharacters(expected).ToLower()),
                    $"Error [{result.Item1}] Affixes clean: \n{string.Join("\n", Process.ExtractTop(affix, _affixes.Select(a => a.Item2.ToLower())).Select(r => $"[{r.Score}] {r.Value}"))}");
            }
        });
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out", Justification = "<En attente>")]
    private static IEnumerable<TestCaseData> GetTestAffixesD4Builds()
    {
        /*
         * Every affixes withoutimplicit
         * Exec script: https://d4builds.gg/database/gear-affixes/

function GetTextFromPrevious(el, tagName){
let prev = el.previousElementSibling;
if (el.tagName == tagName){
    return el.innerText;
} else {
    return GetTextFromPrevious(prev, tagName);
}
}

var arr = [];
document.querySelectorAll('.stats__slot__all__value').forEach(s => {
	if (GetTextFromPrevious(s, 'DIV') != 'Implicit') arr.push(s.innerText.replace(/\[.*\]/,'#'))
});
arr.sort();
arr = arr.filter((item, index) => arr.indexOf(item) === index);
console.log(JSON.stringify(arr, null, 2));

        */

        var affixes = JsonSerializer.Deserialize<List<string>>(File.ReadAllText("D4Companion/Ressources/D4Builds.affixes.json"));

        // Fix d4builds typos / errors.
        foreach (var affix in affixes)
        {
            switch (affix)
            {
                case "# Rank of All Agility Skills":
                    yield return new TestCaseData(affix, "# Ranks of All Agility Skills");
                    break;

                case "# Rank of All Imbuement Skills":
                    yield return new TestCaseData(affix, "# Ranks of All Imbuement Skills");
                    break;

                case "#% Damage Reduction from Affected By Shadow Damage Over Time Enemies":
                    yield return new TestCaseData(affix, "#% Damage Reduction from Shadow Damage Over Time-Affected Enemies");
                    break;

                case "#% Damage to Affected by Shadow Damage Over Time Enemies":
                    yield return new TestCaseData(affix, "#% Damage to Shadow Damage Over Time-Affected Enemies");
                    break;

                case "Lucky Hit: Up to #% Chance to Execute Injured Non-Elites":
                    yield return new TestCaseData(affix, "Lucky Hit: Up to a +#% Chance to Execute Injured Non-Elites");
                    break;

                case "Reduces the Arm Time of your Trap Skills by # Seconds":
                    yield return new TestCaseData(affix, "Trap Skill Arm Time Reduced by # Seconds");
                    break;

                default:
                    yield return new TestCaseData(affix, null);
                    break;
            }
        }
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex RegexMultipleSpaces();

    private static string RemoveSpecialCharacters(string str)
    {
        StringBuilder sb = new();
        foreach (char c in str)
        {
            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ')
            {
                sb.Append(c);
            }
        }
        return RegexMultipleSpaces().Replace(sb.ToString().Trim(), " ");
    }
}
