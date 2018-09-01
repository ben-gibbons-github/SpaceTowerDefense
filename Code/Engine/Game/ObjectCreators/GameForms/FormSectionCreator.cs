using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class FormSectionCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(FormSection);
            this.Catagory = "Form";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new FormSection();
        }
    }
}
