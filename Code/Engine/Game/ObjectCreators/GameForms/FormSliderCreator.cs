using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class FormSliderCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(FormSlider);
            this.Catagory = "Form";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new FormSlider();
        }
    }
}
