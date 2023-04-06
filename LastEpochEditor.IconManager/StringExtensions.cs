namespace LastEpochEditor.IconManager;

internal static class StringExtensions
{
	public static string ToPlannerURL(this string self) => self.ToLowerInvariant().Replace(" ", "_");
}
