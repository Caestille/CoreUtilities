using System.ComponentModel;

namespace CoreUtilities.HelperClasses.Enums
{
	public enum AvailableOperation
	{
		[Description("=")]
		EqualTo,

		[Description("=/=")]
		NotEqualTo,

		[Description(">")]
		GreaterThan,

		[Description("<")]
		LessThan,

		[Description("> X <")]
		InBetween,

		[Description("< X >")]
		OutsideOf,

		[Description("Contains")]
		Contains,

		[Description("Does not contain")]
		DoesNotContain,
	}
}
