
namespace Docker.Registry.DotNet.Application.Authentication;

internal static class AuthenticateParser
{
    public static IDictionary<string, string?> Parse(string value)
    {
        //https://stackoverflow.com/questions/45516717/extracting-and-parsing-the-www-authenticate-header-from-httpresponsemessage-in/45516809#45516809            
        return SplitWWWAuthenticateHeader(value).ToDictionary(GetKey, GetValue);
    }

    private static IEnumerable<string> SplitWWWAuthenticateHeader(string value)
    {
        var builder = new StringBuilder();
        var inQuotes = false;
        
        foreach (var charI in value)
        {
            switch (charI)
            {
                case '\"':
                    if (inQuotes)
                    {
                        yield return builder.ToString();
                        builder.Clear();
                        inQuotes = false;
                    }
                    else
                    {
                        inQuotes = true;
                    }

                    break;

                case ',':
                    if (inQuotes)
                    {
                        builder.Append(charI);
                    }
                    else
                    {
                        if (builder.Length > 0)
                        {
                            yield return builder.ToString();
                            builder.Clear();
                        }
                    }

                    break;

                default:
                    builder.Append(charI);
                    break;
            }
        }

        if (builder.Length > 0) yield return builder.ToString();
    }

    public static ParsedAuthentication ParseTyped(string value)
    {
        var parsed = Parse(value);

        return new ParsedAuthentication(
            parsed.GetValueOrDefault("realm"),
            parsed.GetValueOrDefault("service"),
            parsed.GetValueOrDefault("scope"));
    }

    private static string GetKey(string pair)
    {
        var equalPos = pair.IndexOf("=", StringComparison.Ordinal);

        if (equalPos < 1)
            throw new FormatException("No '=' found.");

        return pair.Substring(0, equalPos);
    }

    private static string GetValue(string pair)
    {
        var equalPos = pair.IndexOf("=", StringComparison.Ordinal);

        if (equalPos < 1)
            throw new FormatException("No '=' found.");

        var value = pair.Substring(equalPos + 1).Trim();

        //Trim quotes
        if (value.StartsWith("\"") && value.EndsWith("\""))
            value = value.Substring(1, value.Length - 2);

        return value;
    }
}