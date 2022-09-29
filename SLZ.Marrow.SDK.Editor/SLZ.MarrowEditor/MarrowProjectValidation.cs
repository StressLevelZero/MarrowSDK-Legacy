using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SLZ.MarrowEditor
{
    public static class MarrowProjectValidation
    {
        [Serializable]
        public class MarrowValidationRule
        {
            public int order = 10;
            public string message;
            public Func<bool> Validate;
            public Action FixRule;
            public string fixMessage;
        }





        private static List<MarrowValidationRule> _validationRules = new List<MarrowValidationRule>();
        public static List<MarrowValidationRule> ValidationRules
        {
            get => _validationRules;
        }

        public static bool ValidateProject()
        {
            bool valid = true;
            foreach (var rule in ValidationRules)
            {
                valid &= rule.Validate();
            }
            return valid;
        }

        public static void GetIssues(List<MarrowValidationRule> issues)
        {
            issues ??= new List<MarrowValidationRule>();
            issues.Clear();

            foreach (var rule in ValidationRules)
            {
                if (!rule.Validate())
                    issues.Add(rule);
            }

            var sortedIssues = issues.OrderBy((rule) => rule.order).ToList();
            issues.Clear();
            foreach (var sortedIssue in sortedIssues)
            {
                issues.Add(sortedIssue);
            }
        }

        public static void FixIssues(List<MarrowValidationRule> issues)
        {
            if (issues != null)
            {
                foreach (var issue in issues)
                {
                    issue.FixRule();
                }
            }
        }
    }
}