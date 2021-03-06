// Copyright 2015, Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.Api.Ads.AdWords.Lib;
using Google.Api.Ads.AdWords.v201506;

using System;
using System.Collections.Generic;
using System.IO;

namespace Google.Api.Ads.AdWords.Examples.CSharp.v201506 {
  /// <summary>
  /// This code example gets all keywords in an ad group. To add keywords, run
  /// AddKeywords.cs.
  /// </summary>
  public class GetKeywords : ExampleBase {
    /// <summary>
    /// Main method, to run this code example as a standalone application.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    public static void Main(string[] args) {
      GetKeywords codeExample = new GetKeywords();
      Console.WriteLine(codeExample.Description);
      try {
        long adGroupId = long.Parse("INSERT_ADGROUP_ID_HERE");
        codeExample.Run(new AdWordsUser(), adGroupId);
      } catch (Exception e) {
        Console.WriteLine("An exception occurred while running this code example. {0}",
            ExampleUtilities.FormatException(e));
      }
    }

    /// <summary>
    /// Returns a description about the code example.
    /// </summary>
    public override string Description {
      get {
        return "This code example gets all keywords in an ad group. To add keywords, run " +
            "AddKeywords.cs.";
      }
    }

    /// <summary>
    /// Runs the code example.
    /// </summary>
    /// <param name="user">The AdWords user.</param>
    /// <param name="adGroupId">ID of the ad group from which keywords are
    /// retrieved.</param>
    public void Run(AdWordsUser user, long adGroupId) {
      // Get the AdGroupCriterionService.
      AdGroupCriterionService adGroupCriterionService =
          (AdGroupCriterionService) user.GetService(
              AdWordsService.v201506.AdGroupCriterionService);

      // Create a selector.
      Selector selector = new Selector();
      selector.fields = new string[] { "Id", "KeywordMatchType", "KeywordText", "CriteriaType" };

      // Select only keywords.
      Predicate criteriaPredicate = new Predicate();
      criteriaPredicate.field = "CriteriaType";
      criteriaPredicate.@operator = PredicateOperator.IN;
      criteriaPredicate.values = new string[] {"KEYWORD"};

      // Restrict search to an ad group.
      Predicate adGroupPredicate = new Predicate();
      adGroupPredicate.field = "AdGroupId";
      adGroupPredicate.@operator = PredicateOperator.EQUALS;
      adGroupPredicate.values = new string[] { adGroupId.ToString() };

      selector.predicates = new Predicate[] {adGroupPredicate, criteriaPredicate};

      // Set the selector paging.
      selector.paging = new Paging();

      int offset = 0;
      int pageSize = 500;

      AdGroupCriterionPage page = new AdGroupCriterionPage();

      try {
        do {
          selector.paging.startIndex = offset;
          selector.paging.numberResults = pageSize;

          // Get the keywords.
          page = adGroupCriterionService.get(selector);

          // Display the results.
          if (page != null && page.entries != null) {
            int i = offset;

            foreach (AdGroupCriterion adGroupCriterion in page.entries) {
              bool isNegative = (adGroupCriterion is NegativeAdGroupCriterion);

              // If you are retrieving multiple type of criteria, then you may
              // need to check for
              //
              // if (adGroupCriterion is Keyword) { ... }
              //
              // to identify the criterion type.
              Keyword keyword = (Keyword) adGroupCriterion.criterion;
              string keywordType = isNegative ? "Negative keyword" : "Keyword";

              Console.WriteLine("{0}) {1} with text = '{2}', matchtype = '{3}', ID = '{4}' and " +
                  "criteria type = '{5}' was found.", i + 1, keywordType, keyword.text,
                  keyword.matchType, keyword.id, keyword.CriterionType);
              i++;
            }
          }
          offset += pageSize;
        } while (offset < page.totalNumEntries);
        Console.WriteLine("Number of keywords found: {0}", page.totalNumEntries);
      } catch (Exception e) {
        throw new System.ApplicationException("Failed to retrieve keywords.", e);
      }
    }
  }
}
