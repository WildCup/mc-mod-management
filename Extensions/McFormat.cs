using System.Text;

namespace McHelper.Extensions;

/// <summary>
/// Renders Minecraft server console text as safe colored HTML: each line is tinted by its log level
/// (ERROR/WARN/…) and any inline "§" formatting codes are translated too. Text is HTML-escaped.
/// See https://minecraft.wiki/w/Formatting_codes
/// </summary>
public static class McFormat
{
	private static readonly Dictionary<char, string> Colors = new()
	{
		['0'] = "#000000", ['1'] = "#0000aa", ['2'] = "#00aa00", ['3'] = "#00aaaa",
		['4'] = "#aa0000", ['5'] = "#aa00aa", ['6'] = "#ffaa00", ['7'] = "#aaaaaa",
		['8'] = "#555555", ['9'] = "#5555ff", ['a'] = "#55ff55", ['b'] = "#55ffff",
		['c'] = "#ff5555", ['d'] = "#ff55ff", ['e'] = "#ffff55", ['f'] = "#ffffff",
	};

	private const string Error = "#e05555", Warn = "#d98c2b", Debug = "#7f8896", Trace = "#9b7d6b", Info = "#cdd6e0";

	public static string ToHtml(string? text)
	{
		if (string.IsNullOrEmpty(text))
			return "";

		// color each line by its log level, then translate any inline §codes within it
		var sb = new StringBuilder(text.Length + 128);
		foreach (var line in text.Split('\n'))
		{
			sb.Append("<span style=\"color:").Append(LevelColor(line)).Append("\">");
			Inline(line, sb);
			sb.Append("</span>\n");
		}
		return sb.ToString();
	}

	//pick a base color from the log level tag, e.g. "[main/ERROR]"; stack-trace lines get a dim tint
	private static string LevelColor(string line)
	{
		if (line.Contains("/ERROR]") || line.Contains("/FATAL]")) return Error;
		if (line.Contains("/WARN]")) return Warn;
		if (line.Contains("/DEBUG]") || line.Contains("/TRACE]")) return Debug;
		var t = line.TrimStart();
		if (t.StartsWith("at ") || t.StartsWith("Caused by") || t.StartsWith("... ") || t.StartsWith("Exception"))
			return Trace;
		return Info;
	}

	//translate one line's §codes into spans (overriding the line color); inner spans win in CSS
	private static void Inline(string text, StringBuilder sb)
	{
		string? color = null;
		bool bold = false, italic = false, underline = false, strike = false;
		var open = false;

		void Close()
		{
			if (open) { sb.Append("</span>"); open = false; }
		}

		void Open()
		{
			sb.Append("<span style=\"");
			if (color != null) sb.Append("color:").Append(color).Append(';');
			if (bold) sb.Append("font-weight:bold;");
			if (italic) sb.Append("font-style:italic;");
			if (underline || strike)
				sb.Append("text-decoration:")
				  .Append(underline ? "underline " : "")
				  .Append(strike ? "line-through" : "")
				  .Append(';');
			sb.Append("\">");
			open = true;
		}

		for (var i = 0; i < text.Length; i++)
		{
			var c = text[i];
			if (c == '§' && i + 1 < text.Length)
			{
				var code = char.ToLowerInvariant(text[++i]);
				if (Colors.TryGetValue(code, out var hex))
				{
					// a color code also resets active styles, matching the game
					Close(); color = hex; bold = italic = underline = strike = false; Open();
				}
				else
				{
					switch (code)
					{
						case 'l': Close(); bold = true; Open(); break;
						case 'o': Close(); italic = true; Open(); break;
						case 'n': Close(); underline = true; Open(); break;
						case 'm': Close(); strike = true; Open(); break;
						case 'r': Close(); color = null; bold = italic = underline = strike = false; break;
						case 'k': break; // obfuscated — rendered as plain text
						default: break;  // unknown code — drop it
					}
				}
				continue;
			}

			switch (c)
			{
				case '&': sb.Append("&amp;"); break;
				case '<': sb.Append("&lt;"); break;
				case '>': sb.Append("&gt;"); break;
				default: sb.Append(c); break;
			}
		}

		Close();
	}
}
