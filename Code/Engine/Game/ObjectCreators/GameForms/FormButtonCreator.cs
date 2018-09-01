using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class FormButtonCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(FormButton);
            this.Catagory = "Form";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new FormButton();
        }
    }
}
