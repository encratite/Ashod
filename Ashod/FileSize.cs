using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ashod
{
	class FileSizeUnit
	{
		public readonly Regex Pattern;
		public readonly long Factor;

		public FileSizeUnit(string pattern, long factor)
		{
			Pattern = new Regex(pattern, RegexOptions.IgnoreCase);
			Factor = factor;
		}

		public bool Match(string input, ref double? output)
		{
			var match = Pattern.Match(input);
			if (match.Success)
			{
				output = Convert.ToDouble(match.Groups[1]);
				return true;
			}
			else
				return false;
		}
	}

	public static class FileSize
	{
		private readonly static FileSizeUnit[] _Units =
		{
			new FileSizeUnit("^\\s*(\\d+)(?:\\s+bytes?)?\\s*$", Power(0)),
			GetUnit("K", 1),
			GetUnit("M", 2),
			GetUnit("G", 3),
			GetUnit("T", 4),
		};

		public static double FromString(string fileSizeString)
		{
			foreach (var unit in _Units)
			{
				double? output = null;
				if (unit.Match(fileSizeString, ref output))
					return output.Value;
			}
			throw new ArgumentException("Invalid file size string");
		}

		private static long Power(int power)
		{
			const int factor = 1024;
			long output = 1;
			for (int i = 0; i < power; i++)
				output *= factor;
			return output;
		}

		private static FileSizeUnit GetUnit(string prefix, int power)
		{
			string pattern = string.Format("^\\s*(\\d+(?:\\.\\d+)?)\\s+{0}i?B\\s*$", prefix);
			return new FileSizeUnit(pattern, Power(power));
		}
	}
}
