using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class FormTextCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(FormText);
            this.Catagory = "Form";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new FormText();
        }
    }
}
