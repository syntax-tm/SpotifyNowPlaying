using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using JetBrains.Annotations;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SpotifyNowPlaying.Output;

public static class OutputFormatter
{
    public const string TOKEN_REGEX = @"\{(?<token>[\w\_\-\.]+)(?:\:)?(?<format>(?:[\w\,]+)+)?\}";
    public const string TOKEN_FORMAT = @"\{{{0}\:?[\w\,]*\}}";
    public const string TOKEN_GROUP_NAME = "token";
    public const string FORMAT_GROUP_NAME = "format";
    public const string TOKEN_FORMAT_SEPARATOR = ",";

    private static readonly ILog log = LogManager.GetLogger(typeof(OutputFormatter));

    private static IDictionary<string, Func<string>> _staticTokens;

    private static IDictionary<string, Func<string, string>> _formatters;
    public static IDictionary<string, Func<string, string>> GetFormatters()
    {
        if (_formatters != null) return _formatters;
        _formatters = new Dictionary<string, Func<string, string>>
        {
            ["ucase"] = (input) => input.ToUpper(),
            ["lcase"] = (input) => input.ToLower(),
            ["no_space"] = (input) => Regex.Replace(input, @"\s", @"_"),
            ["tcase"] = (input) =>
            {
                var locale = System.Globalization.CultureInfo.CurrentCulture;
                var textInfo = locale.TextInfo;
                return textInfo.ToTitleCase(input);
            },
            ["unescape"] = Regex.Unescape,
            ["escape"] = Regex.Escape,
            ["encode"] = HttpUtility.HtmlEncode,
            ["decode"] = HttpUtility.HtmlDecode
        };
        return _formatters;
    }
    
    public static string Format([NotNull] OutputContext context, [NotNull] string input)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (string.IsNullOrWhiteSpace(input)) throw new ArgumentNullException(nameof(input));

        var variables = GetVariables(input);
        var formattedText = input;

        var contextJson = JsonConvert.SerializeObject(context);
        var jContext = JObject.Parse(contextJson);

        foreach (var variable in variables)
        {
            var jsonPathString = variable.GetJsonPath();

            var token = jContext.SelectToken(jsonPathString, false);

            if (token is null)
            {
                log.Warn($"Unable to evaluate json path query '{jsonPathString}'. Value not replaced.");

                continue;
            }

            var tokenValue = token.Value<string>();

            Debug.Assert(tokenValue != null, nameof(tokenValue) + " != null");

            var formattedValue = variable.GetFormattedValue(tokenValue);
            
            var variableFormat = string.Format(TOKEN_FORMAT, variable.Token);
            var variableRegex = new Regex(variableFormat, RegexOptions.IgnoreCase);

            formattedText = variableRegex.Replace(formattedText, tokenValue);

            //if (!tokens.ContainsKey(variable.Token))
            //{
            //    throw new ArgumentOutOfRangeException($"{variable.Token} is not a valid token.");
            //}
            //
            //var variableValue = tokens[variable.Token]();
            //var formattedValue = variable.GetFormattedValue(variableValue);
            //
            //formattedText = variableRegex.Replace(formattedText, formattedValue);
        }

        return formattedText;
    }

    public static string Format([NotNull] IDictionary<string, Func<string>> tokens, [NotNull] string input)
    {
        if (tokens == null) throw new ArgumentNullException(nameof(tokens));
        if (input == null) throw new ArgumentNullException(nameof(input));

        var variables = GetVariables(input);
        var formattedText = input;
        
        foreach (var variable in variables)
        {
            var variableFormat = string.Format(TOKEN_FORMAT, variable.Token);
            var variableRegex = new Regex(variableFormat, RegexOptions.IgnoreCase);

            if (!tokens.ContainsKey(variable.Token))
            {
                throw new ArgumentOutOfRangeException($"{variable.Token} is not a valid token.");
            }

            var variableValue = tokens[variable.Token]();
            var formattedValue = variable.GetFormattedValue(variableValue);

            formattedText = variableRegex.Replace(formattedText, formattedValue);
        }

        return formattedText;
    }

    public static List<VariableText> GetVariables(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) throw new ArgumentNullException(nameof(input));

        var variables = new List<VariableText>();
        var variableRegex = new Regex(TOKEN_REGEX, RegexOptions.IgnoreCase);
        
        var matches = variableRegex.Matches(input);

        if (!matches.Any()) return variables;

        foreach (Match match in matches)
        {
            var variable = new VariableText
            {
                Token = match.Groups[TOKEN_GROUP_NAME].Value
            };

            var hasFormat = match.Groups.ContainsKey(FORMAT_GROUP_NAME);
            if (hasFormat)
            {
                var formatValue = match.Groups[FORMAT_GROUP_NAME].Value;
                variable.Format = formatValue;
            }

            variables.Add(variable);
        }

        return variables;
    }
    
    private static IDictionary<string, Func<string>> GetStaticTokens()
    {
        if (_staticTokens != null) return _staticTokens;

        _staticTokens = new Dictionary<string, Func<string>>
                {
                    { @"newline", () => Environment.NewLine },
                    { @"nl", () => Environment.NewLine },
                    { @"tab", () => "\t" },
                    { @"t", () => "\t" },
                    { @"space", () => " " },
                    { @"s", () => " " },
                    { @"cr", () => "\r" },
                    { @"lf", () => "\n" },
                    { @"crlf", () => "\r\n" },
                    { @"quote", () => "'" },
                    { @"q", () => "'" },
                    { @"dquote", () => "\"" },
                    { @"dq", () => "\"" },
                    { @"unicode", () => "\0" },
                    { @"u", () => "\0" },
                    { @"alert", () => "\a" },
                    { @"a", () => "\a" },
                    { @"form", () => "\f" },
                    { @"f", () => "\f" },
                    { @"vtab", () => "\v" },
                    { @"bs", () => "\\" }
                };

        return _staticTokens;
    }

    private static IDictionary<string, Func<string>> GetDateTokens()
    {
        return new Dictionary<string, Func<string>>
            {
                { @"longdate", () => DateTime.Now.ToLongDateString() },
                { @"longtime", () => DateTime.Now.ToLongTimeString() },
                { @"date", () => DateTime.Now.ToShortDateString() },
                { @"time", () => DateTime.Now.ToShortTimeString() },
                { @"dayofweek", () => DateTime.Now.DayOfWeek.ToString() },
                { @"hour", () => DateTime.Now.Hour.ToString() },
                { @"minutes", () => DateTime.Now.Minute.ToString() },
                { @"month", () => DateTime.Now.Month.ToString() },
                { @"year", () => DateTime.Now.Year.ToString() }
            };
    }
    
}
