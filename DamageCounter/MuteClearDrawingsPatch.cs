namespace BetterSpire2;

public class MuteClearDrawingsPatch
{
	public static bool Prefix(ulong senderId)
	{
		return !PartyManager.IsDrawingMuted(senderId);
	}
}
