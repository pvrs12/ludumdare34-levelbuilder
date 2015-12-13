
using System.Windows.Forms;

namespace GridCtrl
{
	/// <summary>
	/// HScrollBarEx: prevents the scroll bar being selected (no UI cues, ie Flashing scrollbar)
	/// </summary>
	public class HScrollBarEx : HScrollBar
	{
		/// <summary>
		/// Overriden to prevent scoll bar from being selected
		/// </summary>
		/// <param name="directed"></param>
		/// <param name="forward"></param>
		protected override void Select(bool directed, bool forward)
		{
		}
	}

	/// <summary>
	/// VScrollBarEx: prevents the scroll bar being selected (no UI cues, ie Flashing scrollbar)
	/// </summary>
	public class VScrollBarEx : VScrollBar
	{
		/// <summary>
		/// Overriden to prevent scoll bar from being selected
		/// </summary>
		/// <param name="directed"></param>
		/// <param name="forward"></param>
		protected override void Select(bool directed, bool forward)
		{
		}
	}
}