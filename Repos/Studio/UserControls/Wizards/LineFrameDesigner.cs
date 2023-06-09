using System;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace Newtera.Studio.UserControls.Wizards
{
  public class LineFrameDesigner : ParentControlDesigner
  {
    public override System.Windows.Forms.Design.SelectionRules SelectionRules
    {
      get
      {
        SelectionRules sel = SelectionRules.LeftSizeable | 
          SelectionRules.RightSizeable | 
          SelectionRules.Moveable | 
          SelectionRules.Visible;
        
        return sel;
      }
    }
  }
}
