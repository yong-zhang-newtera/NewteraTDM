
namespace Newtera.MLStudio.Execution
{
    using System;
    using System.Activities.Presentation.Validation;
    using System.Collections.Generic;

    public class ValidationErrorService : IValidationErrorService
    {
        private IList<ValidationErrorInfo> errorList;

        public ValidationErrorService(IList<ValidationErrorInfo> errorList)
        {
            this.errorList = errorList;
        }

        public delegate void ErrorsChangedHandler(object sender, EventArgs e);

        public event ErrorsChangedHandler ErrorsChangedEvent;

        public void ShowValidationErrors(IList<ValidationErrorInfo> errors)
        {
            this.errorList.Clear();

            foreach (var error in errors)
            {
                this.errorList.Add(error);
            }

            if (this.ErrorsChangedEvent != null)
            {
                this.ErrorsChangedEvent(this, null);
            }
        }
    }
}