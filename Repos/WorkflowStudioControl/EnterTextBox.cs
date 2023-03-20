using System;
using System.Drawing;
using System.Windows.Forms;


namespace Newtera.WorkflowStudioControl
{
	/// <summary>
	/// A customized textbox that generates an EnterKey event when enter
	/// key is pressed.
	/// </summary>
	public class EnterTextBox : System.Windows.Forms.TextBox
	{
		/// <summary>
		/// Overrided method to accept enter key as input key
		/// </summary>
		/// <param name="keyData"></param>
		/// <returns>true if it is an input key, false, otherwise.</returns>
		protected override bool IsInputKey(Keys keyData)
		{
			if (keyData == Keys.Enter)
			{
				return true;
			}

			return base.IsInputKey (keyData);
		}
	}
}
